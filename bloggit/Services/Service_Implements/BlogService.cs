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
            // Find the blog in the database based on the provided ID
            var blog = await _context.Blogs
                .Include(blog => blog.Tags)
                .Include(blog => blog.User)
                .Include(blog => blog.Reaction)
                .FirstOrDefaultAsync(blog => blog.Id == id && !blog.isDeleted);

            if (blog == null)
            {
                return new NotFoundResult(); // Return a 404 response if the blog is not found
            }

            // Map the blog entity to a DTO (Data Transfer Object) for serialization
            var blogDto = new BlogDto
            {
                Id = blog.Id,
                Title = blog.Title,
                Summary = blog.Summary,
                Content = blog.Content,
                Author = blog.Author,
                AuthorFirstName = blog.User?.FirstName,
                AuthorLastName = blog.User?.LastName,
                AuthorUserName = blog.User?.UserName,
                Image = blog.Image,
                CreatedOn = blog.CreatedOn,
                Tags = blog.Tags?.Select(t => t.Name).ToList(),
                ReactionCount = blog.CalculateTotalReactions(),
                Reactions = blog.Reaction.Select(r => new ReactionDto
                {
                    Id = r.Id,
                    Type = r.Type,
                    UserId = r.UserId,
                }).ToList()
            };

            return new OkObjectResult(blogDto); // Return a 200 OK response with the blog DTO
        }


        public async Task<IActionResult> GetAllBlogsAsync()
        {
            var blogs = await _context.Blogs
                .Include(blog => blog.Tags)
                .Include(blog => blog.User)
                .Include(blog => blog.Reaction)
                .Where(blog => !blog.isDeleted)
                .ToListAsync();

            var blogDtos = blogs.Select(blog => new BlogDto
            {
                Id = blog.Id,
                Title = blog.Title,
                Summary = blog.Summary,
                Content = blog.Content,
                Author = blog.Author,
                AuthorFirstName = blog.User?.FirstName,
                AuthorLastName = blog.User?.LastName,
                AuthorUserName = blog.User?.UserName,
                Image = blog.Image,
                CreatedOn = blog.CreatedOn,
                Tags = blog.Tags?.Select(t => t.Name).ToList(),
                ReactionCount = blog.CalculateTotalReactions(),
                Reactions = blog.Reaction.Select(r => new ReactionDto
                {
                    Id = r.Id,
                    Type = r.Type,
                    UserId = r.UserId,
                }).ToList()
            });

            return new OkObjectResult(blogDtos);
        }

        public async Task<IActionResult> GetBlogsPaginate(int pageNumber, int pageSize, string sortingOption)
        {
            IQueryable<Blogs> query = _context.Blogs
                .Include(blog => blog.Tags)
                .Include(blog => blog.User)
                .Include(blog => blog.Reaction)
                .Where(blog => !blog.isDeleted);

            switch (sortingOption)
            {
                case "popularity":
                    query = query.OrderByDescending(blog => blog.CalculateTotalReactions());
                    break;
                case "recency":
                    query = query.OrderByDescending(blog => blog.CreatedOn);
                    break;
                case "random":
                    var blogs = await query
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();
        
                    // Shuffle the blogs using Guid.NewGuid() for random order
                    blogs = blogs.OrderBy(blog => Guid.NewGuid()).ToList();
        
                    // Return the shuffled blogs
                    return new OkObjectResult(blogs.Select(blog => new BlogDto
                    {
                        Id = blog.Id,
                        Title = blog.Title,
                        Summary = blog.Summary,
                        Content = blog.Content,
                        Author = blog.Author,
                        AuthorFirstName = blog.User?.FirstName,
                        AuthorLastName = blog.User?.LastName,
                        AuthorUserName = blog.User?.UserName,
                        Image = blog.Image,
                        CreatedOn = blog.CreatedOn,
                        Tags = blog.Tags?.Select(t => t.Name).ToList(),
                        ReactionCount = blog.CalculateTotalReactions(),
                        Reactions = blog.Reaction.Select(r => new ReactionDto
                        {
                            Id = r.Id,
                            Type = r.Type,
                            UserId = r.UserId,
                        }).ToList()
                    }));
            }

            var pagedBlogs = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new OkObjectResult(pagedBlogs.Select(blog => new BlogDto
            {
                Id = blog.Id,
                Title = blog.Title,
                Summary = blog.Summary,
                Content = blog.Content,
                Author = blog.Author,
                AuthorFirstName = blog.User?.FirstName,
                AuthorLastName = blog.User?.LastName,
                AuthorUserName = blog.User?.UserName,
                Image = blog.Image,
                CreatedOn = blog.CreatedOn,
                Tags = blog.Tags?.Select(t => t.Name).ToList(),
                ReactionCount = blog.CalculateTotalReactions(),
                Reactions = blog.Reaction.Select(r => new ReactionDto
                {
                    Id = r.Id,
                    Type = r.Type,
                    UserId = r.UserId,
                }).ToList()
            }));
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