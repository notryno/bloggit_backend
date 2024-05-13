using bloggit.DTOs;
using bloggit.Models;
using bloggit.Services.Service_Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bloggit.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ConflictResult = System.Web.Http.Results.ConflictResult;

namespace bloggit.Services.Service_Implements
{
    public class ReactionService : IReactionService
    {
        private readonly AppDbContext _context;
        private readonly ILogService _logService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReactionService(AppDbContext context, ILogService logService, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logService = logService;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }


        public async Task<IActionResult> AddReaction(int blogId, CreateReactionDto reactionDto)
        {
            var currentUser = await GetCurrentUserAsync();
            var blog = await _context.Blogs.Include(b => b.Reaction).FirstOrDefaultAsync(b => b.Id == blogId);
            if (blog == null)
            {
                return new NotFoundObjectResult("Blog not found");
            }

            var existingReaction = blog.Reaction.FirstOrDefault(r => r.UserId == reactionDto.UserId);
            if (existingReaction != null)
            {
                return new ConflictObjectResult("User has already reacted.");
            }

            var reaction = new Reactions
            {
                Type = reactionDto.Type,
                BlogId = blogId,
                UserId = reactionDto.UserId,
                CreatedOn = DateTime.Now
            };

            _context.Reactions.Add(reaction);
            await _logService.LogReactionActionAsync(reactionDto.Type, $"User {currentUser.UserName} {reactionDto.Type.ToLower()} blog {blogId}", currentUser.Id);
            await _context.SaveChangesAsync();


            return new OkObjectResult("Reaction added successfully");
        }


        public async Task<IActionResult> RemoveReaction(int reactionId, int blogId)
        {
            var reaction = await _context.Reactions.FirstOrDefaultAsync(r => r.Id == reactionId && r.BlogId == blogId);
            if (reaction == null)
            {
                return new NotFoundObjectResult("Reaction not found");
            }
            
            var currentUser = await GetCurrentUserAsync();

            _context.Reactions.Remove(reaction);
            await _logService.LogReactionActionAsync("Un-React", $"User {currentUser.UserName} un-reacted blog {blogId}", currentUser.Id);
            await _context.SaveChangesAsync();


            return new OkObjectResult("Reaction removed successfully");
        }
        
        private async Task<ApplicationUser?> GetCurrentUserAsync()
        {
            var user = _httpContextAccessor.HttpContext.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                return null;
            }

            var userIdClaim = user.FindFirst("userId");
            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            {
                return null;
            }
            var foundUser = await _userManager.FindByIdAsync(userIdClaim.Value);
            if (foundUser == null || foundUser.isDeleted)
            {
                return null;
            }

            return foundUser;
        }
        
        public async Task<ReactionCountDto> GetReactionCount(int blogId)
        {
            var upvoteCount = await _context.Reactions
                .Where(r => r.BlogId == blogId && r.Type == "Upvote")
                .CountAsync();

            var downvoteCount = await _context.Reactions
                .Where(r => r.BlogId == blogId && r.Type == "Downvote")
                .CountAsync();

            var reactionCount = new ReactionCountDto
            {
                UpvoteCount = upvoteCount,
                DownvoteCount = downvoteCount
            };

            return reactionCount;
        }

    }
}
