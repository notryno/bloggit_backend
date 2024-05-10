using bloggit.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bloggit.DTOs;
using bloggit.Models;
using bloggit.Services.Service_Interfaces;

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
                Content = model.Content,
                Author = model.Author,
                Image = model.Image,
                CreatedOn = DateTime.Now
            };

            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();

            var blogDto = new BlogDto
            {
                Id = blog.Id,
                Title = blog.Title,
                Content = blog.Content,
                Author = blog.Author,
                Image = blog.Image
            };

            return blogDto;
        }

        public async Task<BlogDto> UpdateBlogAsync(BlogDto model)
        {
            var blog = await _context.Blogs
                .Where(b => b.Id == model.Id && !b.isDeleted)
                .FirstOrDefaultAsync();
            if (blog == null)
            {
                throw new Exception("Blog not found");
            }

            // Perform manual mapping
            blog.Title = model.Title;
            blog.Content = model.Content;
            blog.Author = model.Author;
            blog.Image = model.Image;

            await _context.SaveChangesAsync();

            var updatedBlogDto = new BlogDto
            {
                Id = blog.Id,
                Title = blog.Title,
                Content = blog.Content,
                Author = blog.Author,
                Image = blog.Image
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

            blog.isDeleted = true; // Mark the blog as deleted
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
                Content = blog.Content,
                Author = blog.Author,
                Image = blog.Image
            });

            return blogDtos;
        }
    }
}
