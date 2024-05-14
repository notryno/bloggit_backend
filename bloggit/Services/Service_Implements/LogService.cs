using System;
using System.Threading.Tasks;
using bloggit.Data;
using bloggit.Hubs;
using bloggit.Models;
using bloggit.Services.Service_Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace bloggit.Services.Service_Implements
{
    public class LogService : ILogService
    {
        private readonly AppDbContext _context;
        private IHubContext<NotificationHub> _hub;

        public LogService(AppDbContext context, IHubContext<NotificationHub> hub)
        {
            _context = context;
            _hub = hub;
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
            
            var result = _hub.Clients.All.SendAsync("ReceiveNotification", "Log", "New log added");
            await _context.SaveChangesAsync();
        }
    }
}