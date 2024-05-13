using bloggit.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bloggit.DTOs;
using bloggit.Hubs;
using bloggit.Models;
using bloggit.Services.Service_Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Org.BouncyCastle.Asn1.Cms;

namespace bloggit.Services.Service_Implements
{
    public class BlogService : IBlogService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogService _logService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BlogService(AppDbContext context,UserManager<ApplicationUser> userManager, IHubContext<NotificationHub> hubContext, ILogService logService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
            _logService = logService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> CreateBlogAsync(BlogCreateRequest model)
        {
            var blog = new Blogs
            {
                Title = model.Title,
                Summary = model.Summary,
                Content = model.Content,
                Author = model.Author,
                Image = model.Image,
                CreatedOn = DateTime.Now,
                Tags = new List<Tags>()
            };
            
            if (model.Tags != null && model.Tags.Any())
            {
                foreach (var tagName in model.Tags)
                {
                    var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName && !t.isDeleted);
                    if (tag == null)
                    {
                        tag = new Tags { Name = tagName };
                        _context.Tags.Add(tag);
                    }
                    blog.Tags.Add(tag);
                }
            }

            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();

            var blogDto = new BlogDto
            {
                Id = blog.Id,
                Title = blog.Title,
                Summary = model.Summary,
                Content = blog.Content,
                Author = blog.Author,
                Image = blog.Image
            };

            Console.WriteLine("Blog created!");

            var currentUser = await GetCurrentUserAsync();

            
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", "Admin", "New blog post created!");
            await _logService.LogBlogActionAsync(blogDto.Id, "Create", $"User {currentUser.UserName} created blog {blogDto.Id}", currentUser.Id);

            return new OkObjectResult(new { message = "Blog created successfully" });
        }


        public async Task<IActionResult> UpdateBlogAsync(int id, BlogUpdateRequest model)
        {
            var existingBlog = await _context.Blogs
                .Where(b => b.Id == id && !b.isDeleted)
                .FirstOrDefaultAsync();
            
            if (existingBlog == null)
            {
                throw new Exception("Blog not found");
            }
            
            var currentUser = await GetCurrentUserAsync();
            if (!IsAuthorized(currentUser, existingBlog))
            {
                return new ForbidResult();
            }


            // Track changes for logging
            var changes = new List<string>();

            // Update properties if they are not null or empty and different from existing values
            if (!string.IsNullOrEmpty(model.Title) && model.Title != existingBlog.Title)
            {
                changes.Add($"Title changed from '{existingBlog.Title}' to '{model.Title}'");
                existingBlog.Title = model.Title;
            }

            if (!string.IsNullOrEmpty(model.Summary) && model.Summary != existingBlog.Summary)
            {
                changes.Add($"Summary changed from '{existingBlog.Summary}' to '{model.Summary}'");
                existingBlog.Summary = model.Summary;
            }

            if (!string.IsNullOrEmpty(model.Content) && model.Content != existingBlog.Content)
            {
                changes.Add($"Content changed from '{existingBlog.Content}' to '{model.Content}'");
                existingBlog.Content = model.Content;
            }

            if (!string.IsNullOrEmpty(model.Image) && model.Image != existingBlog.Image)
            {
                changes.Add($"Image changed from '{existingBlog.Image}' to '{model.Image}'");
                existingBlog.Image = model.Image;
            }

            if (!string.IsNullOrEmpty(model.Author) && model.Author != existingBlog.Author)
            {
                changes.Add($"Author changed from '{existingBlog.Author}' to '{model.Author}'");
                existingBlog.Author = model.Author;
            }

            // Update Tags
            if (model.Tags != null && model.Tags.Any())
            {
                existingBlog.Tags.Clear(); // Clear existing tags
                foreach (var tagName in model.Tags)
                {
                    var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                    if (tag == null)
                    {
                        tag = new Tags { Name = tagName };
                        _context.Tags.Add(tag);
                    }

                    existingBlog.Tags.Add(tag);
                    changes.Add($"Tag '{tagName}' added");
                }
            }

            // Log changes
            if (changes.Any())
            {
                foreach (var change in changes)
                {
                    await _logService.LogBlogActionAsync(existingBlog.Id, "Update", change, currentUser.Id);
                }
            }

            var result = await _context.SaveChangesAsync();

            return new OkObjectResult(new { message = "Blog updated successfully" });
        }


        public async Task<IActionResult> DeleteBlogAsync(int id)
        {
            var blog = await _context.Blogs
                .Where(b => b.Id == id && !b.isDeleted)
                .FirstOrDefaultAsync();
            if (blog == null)
            {
                throw new Exception("Blog not found");
            }
            
            var currentUser = await GetCurrentUserAsync();

            blog.isDeleted = true;
            blog.ModifiedOn = DateTime.Now;
            await _logService.LogBlogActionAsync(id, "Delete", $"User {currentUser.UserName} deleted blog {id}", currentUser.Id);
            await _context.SaveChangesAsync();
            return new OkObjectResult(new { message = "Blog deleted successfully" });
        }


        public async Task<IActionResult> GetBlogByIdAsync(int id)
        {
            var blog = await _context.Blogs
                .Where(b => b.Id == id && !b.isDeleted)
                .FirstOrDefaultAsync();
            if (blog == null)
            {
                throw new Exception("Blog not found");
            }
            
            var currentUser = await GetCurrentUserAsync();
            if (!IsAuthorized(currentUser, blog))
            {
                return new ForbidResult();
            }

            var blogDto = new BlogDto
            {
                Id = blog.Id,
                Title = blog.Title,
                Summary = blog.Summary,
                Content = blog.Content,
                Author = blog.Author,
                Image = blog.Image
            };

            return new OkObjectResult(blogDto);
        }

        public async Task<IActionResult> GetAllBlogsAsync()
        {
            var blogs = await _context.Blogs
                .Where(blog => !blog.isDeleted)
                .ToListAsync();

            var blogDtos = blogs.Select(blog => new BlogDto
            {
                Id = blog.Id,
                Title = blog.Title,
                Summary = blog.Summary,
                Content = blog.Content,
                Author = blog.Author,
                Image = blog.Image
            });

            return new OkObjectResult(blogDtos);
        }
        
        private bool IsAuthorized(ApplicationUser currentUser, Blogs blog)
        {
            return currentUser.Id == blog.User.Id || _userManager.IsInRoleAsync(currentUser, "Admin").Result;
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
    }
}