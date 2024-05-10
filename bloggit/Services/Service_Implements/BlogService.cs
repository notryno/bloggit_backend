using bloggit.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bloggit.DTOs;
using bloggit.Models;
using bloggit.Services.Service_Interfaces;
using Org.BouncyCastle.Asn1.Cms;

namespace bloggit.Services.Service_Implements
{
    public class BlogService : IBlogService
    {
        private readonly AppDbContext _context;

        public BlogService(AppDbContext context)
        {
            _context = context;
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
                isLatest = true
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

            return blogDto;
        }

        public async Task<BlogDto> UpdateBlogAsync(BlogDto model)
        {
            var existingBlog = await _context.Blogs
                .Where(b => b.Id == model.Id && !b.isDeleted && b.isLatest)
                .FirstOrDefaultAsync();
    
            if (existingBlog == null)
            {
                throw new Exception("Blog not found");
            }

            // Set the existing blog's isLatest to 0
            existingBlog.isLatest = false;

            // Create a new blog entry with updated data
            var newBlog = new Blogs
            {
                Title = model.Title,
                Summary = model.Summary,
                Content = model.Content,
                Author = model.Author,
                Image = model.Image,
                CreatedOn = existingBlog.CreatedOn,
                ModifiedOn = DateTime.Now,
                isLatest = true
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
                    newBlog.Tags.Add(tag);
                }
            }

            _context.Blogs.Add(newBlog);

            await _context.SaveChangesAsync();

            var updatedBlogDto = new BlogDto
            {
                Id = newBlog.Id,
                Title = newBlog.Title,
                Summary = newBlog.Summary,
                Content = newBlog.Content,
                Author = newBlog.Author,
                Image = newBlog.Image,
            };

            return updatedBlogDto;
        }


        public async Task<bool> DeleteBlogAsync(int id)
        {
            var blog = await _context.Blogs
                .Where(b => b.Id == id && !b.isDeleted && b.isLatest)
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
                .Where(b => b.Id == id && !b.isDeleted && b.isLatest)
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
                .Where(blog => !blog.isDeleted && blog.isLatest)
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
