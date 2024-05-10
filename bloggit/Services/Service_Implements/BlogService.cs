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
using Microsoft.AspNetCore.SignalR;
using Org.BouncyCastle.Asn1.Cms;

namespace bloggit.Services.Service_Implements
{
    public class BlogService : IBlogService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogService _logService;

        public BlogService(AppDbContext context, IHubContext<NotificationHub> hubContext, ILogService logService)
        {
            _context = context;
            _hubContext = hubContext;
            _logService = logService;
        }

        public async Task<BlogDto> CreateBlogAsync(BlogCreateRequest model)
        {
            var blog = new Blogs
            {
                Title = model.Title,
                Summary = model.Summary,
                Content = model.Content,
                Author = model.Author,
                Image = model.Image,
                CreatedOn = DateTime.Now,
                // isLatest = true,
                Tags = new List<Tags>() // Initialize the Tags collection
            };

            if (model.Tags != null && model.Tags.Any())
            {
                foreach (var tagName in model.Tags)
                {
                    var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                    if (tag == null)
                    {
                        tag = new Tags { Name = tagName };
                        _context.Tags.Add(tag);
                    }

                    Console.WriteLine("Tag added!");
                    Console.WriteLine(tag);
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

            await _hubContext.Clients.All.SendAsync("ReceiveNotification", "Admin", "New blog post created!");

            return blogDto;
        }


        public async Task<BlogDto> UpdateBlogAsync(BlogDto model)
        {
            var existingBlog = await _context.Blogs
                .Where(b => b.Id == model.Id && !b.isDeleted)
                .FirstOrDefaultAsync();

            if (existingBlog == null)
            {
                throw new Exception("Blog not found");
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
                var changeLog = string.Join(", ", changes);
                _logService.LogBlogActionAsync(existingBlog.Id, "Update", $"Changes: {changeLog}");
            }

            await _context.SaveChangesAsync();

            var updatedBlogDto = new BlogDto
            {
                Id = existingBlog.Id,
                Title = existingBlog.Title,
                Summary = existingBlog.Summary,
                Content = existingBlog.Content,
                Author = existingBlog.Author,
                Image = existingBlog.Image,
            };

            return updatedBlogDto;
        }


        public async Task<bool> DeleteBlogAsync(int id)
        {
            var blog = await _context.Blogs
                .Where(b => b.Id == id && !b.isDeleted)
                .FirstOrDefaultAsync();
            if (blog == null)
            {
                throw new Exception("Blog not found");
            }

            blog.isDeleted = true;
            blog.ModifiedOn = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<BlogDto> GetBlogByIdAsync(int id)
        {
            var blog = await _context.Blogs
                .Where(b => b.Id == id && !b.isDeleted)
                .FirstOrDefaultAsync();
            if (blog == null)
            {
                throw new Exception("Blog not found");
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

            return blogDto;
        }

        public async Task<IEnumerable<BlogDto>> GetAllBlogsAsync()
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

            return blogDtos;
        }
    }
}