using bloggit.DTOs;
using bloggit.Models;
using bloggit.Services.Service_Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bloggit.Data;

namespace bloggit.Services.Service_Implements
{
    public class ReactionService : IReactionService
    {
        private readonly AppDbContext _context;

        public ReactionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReactionDto>> GetAllReactionsAsync()
        {
            var reactions = await _context.Reactions
                .Select(r => new ReactionDto
                {
                    Id = r.Id,
                    Type = r.Type,
                    BlogId = r.BlogId,
                    CommentId = r.CommentId,
                    UserId = r.UserId
                })
                .ToListAsync();

            return reactions;
        }

        public async Task<ReactionDto> GetReactionByIdAsync(int id)
        {
            var reaction = await _context.Reactions
                .Where(r => r.Id == id)
                .Select(r => new ReactionDto
                {
                    Id = r.Id,
                    Type = r.Type,
                    BlogId = r.BlogId,
                    CommentId = r.CommentId,
                    UserId = r.UserId
                })
                .FirstOrDefaultAsync();

            if (reaction == null)
                throw new Exception("Reaction not found");

            return reaction;
        }

        public async Task<ReactionDto> CreateReactionAsync(CreateReactionDto model)
        {
            var reaction = new Reactions
            {
                Type = model.Type,
                BlogId = model.BlogId,
                CommentId = model.CommentId,
                UserId = model.UserId,
                CreatedOn = DateTime.Now,
                // isLatest = true
            };

            _context.Reactions.Add(reaction);
            await _context.SaveChangesAsync();

            var reactionDto = new ReactionDto
            {
                Id = reaction.Id,
                Type = reaction.Type,
                BlogId = reaction.BlogId,
                CommentId = reaction.CommentId,
                UserId = reaction.UserId,
            };

            return reactionDto;
        }

        public async Task<ReactionDto> UpdateReactionAsync(int id, UpdateReactionDto model)
        {
            var existingReaction = await _context.Reactions.FindAsync(id);
            if (existingReaction == null)
                throw new Exception("Reaction not found");

            // Check if a reaction already exists for the specified blog or comment by the user
            var sameUserReaction = await _context.Reactions
                .FirstOrDefaultAsync(r => r.Id != id && r.BlogId == existingReaction.BlogId && r.CommentId == existingReaction.CommentId && r.UserId == existingReaction.UserId);

            if (sameUserReaction != null)
            {
                if (model.Type == existingReaction.Type)
                {
                    // User is trying to upvote/downvote again; handle it accordingly
                    throw new Exception("User already reacted with the same type");
                }
                else
                {
                    // User is un-voting; delete the existing reaction
                    _context.Reactions.Remove(existingReaction);
                    await _context.SaveChangesAsync();
                    return null; // Return null or a custom response indicating successful un-vote
                }
            }

            // Update the existing reaction
            existingReaction.Type = model.Type;
            await _context.SaveChangesAsync();

            var updatedReactionDto = new ReactionDto
            {
                Id = existingReaction.Id,
                Type = existingReaction.Type,
                BlogId = existingReaction.BlogId,
                CommentId = existingReaction.CommentId,
                UserId = existingReaction.UserId
            };

            return updatedReactionDto;
        }


        public async Task<bool> DeleteReactionAsync(int id)
        {
            var reaction = await _context.Reactions.FindAsync(id);
            if (reaction == null)
                throw new Exception("Reaction not found");

            _context.Reactions.Remove(reaction);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
