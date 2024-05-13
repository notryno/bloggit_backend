using System;
using System.Threading.Tasks;
using bloggit.Data;
using bloggit.Models;
using bloggit.Services.Service_Interfaces;

namespace bloggit.Services.Service_Implements
{
    public class LogService : ILogService
    {
        private readonly AppDbContext _context;

        public LogService(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogBlogActionAsync(int blogId, string actionType, string description, string userId)
        {
            var log = new Logs
            {
                BlogId = blogId,
                Type = actionType,
                Timestamp = DateTime.Now,
                Description = description,
                UserId = userId
            };

            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task LogCommentActionAsync(int commentId, string actionType, string description, string userId)
        {
            var log = new Logs
            {
                CommentId = commentId,
                Type = actionType,
                Timestamp = DateTime.Now,
                Description = description,
                UserId = userId
            };

            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task LogReactionActionAsync(string actionType, string description, string userId)
        {
            var log = new Logs
            {
                Type = actionType,
                Timestamp = DateTime.Now,
                Description = description,
                UserId = userId
            };

            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}