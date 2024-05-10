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
    public class CommentService : ICommentService
    {
        private readonly AppDbContext _context;

        public CommentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CommentDto> CreateCommentAsync(CreateCommentDto model)
        {
            var comment = new Comments
            {
                Content = model.Content,
                BlogId = model.BlogId,
                UserId = model.UserId,
                ReplyId = model.ReplyId,
                CreatedOn = DateTime.Now,
                isLatest = true
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            var commentDto = new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                BlogId = comment.BlogId,
                UserId = comment.UserId,
                ReplyId = comment.ReplyId
            };

            return commentDto;
        }

        public async Task<CommentDto> UpdateCommentAsync(UpdateCommentDto model)
        {
            var existingComment = await _context.Comments
                .Where(c => c.Id == model.Id && !c.isDeleted && c.isLatest)
                .FirstOrDefaultAsync();

            if (existingComment == null)
            {
                throw new Exception("Comment not found");
            }

            // Set the existing comment's isLatest to 0
            existingComment.isLatest = false;

            // Create a new comment entry with updated content
            var newComment = new Comments
            {
                Content = model.Content,
                BlogId = existingComment.BlogId,
                UserId = existingComment.UserId,
                ReplyId = existingComment.ReplyId,
                isLatest = true,
                ModifiedOn = DateTime.Now
            };

            _context.Comments.Add(newComment);
            await _context.SaveChangesAsync();

            var updatedCommentDto = new CommentDto
            {
                Id = newComment.Id, // Use the new comment's ID
                Content = newComment.Content,
                BlogId = newComment.BlogId,
                UserId = newComment.UserId,
                ReplyId = newComment.ReplyId
            };

            return updatedCommentDto;
        }


        public async Task<bool> DeleteCommentAsync(int id)
        {
            var comment = await _context.Comments
                .Where(c => c.Id == id && !c.isDeleted && c.isLatest)
                .FirstOrDefaultAsync();
            if (comment == null)
                throw new Exception("Comment not found");

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<CommentDto> GetCommentByIdAsync(int id)
        {
            var comment = await _context.Comments
                .Include(c => c.Reaction)
                .Where(c => c.Id == id && !c.isDeleted && c.isLatest)
                .FirstOrDefaultAsync();
            if (comment == null)
                throw new Exception("Comment not found");

            var commentDto = new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                BlogId = comment.BlogId,
                UserId = comment.UserId,
                ReplyId = comment.ReplyId,
                CreatedOn = comment.CreatedOn,
                ModifiedOn = comment.ModifiedOn,
                Reactions = comment.Reaction.Select(r => new ReactionDto
                {
                    Id = r.Id,
                    Type = r.Type,
                    BlogId = r.BlogId,
                    CommentId = r.CommentId,
                    UserId = r.UserId
                }).ToList()
            };

            return commentDto;
        }

        public async Task<IEnumerable<CommentDto>> GetAllCommentsAsync()
        {
            var comments = await _context.Comments
                .Include(c => c.Reaction)
                .Where(commment => !commment.isDeleted && commment.isLatest)
                .ToListAsync();

            var commentDtos = comments.Select(comment => new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                BlogId = comment.BlogId,
                UserId = comment.UserId,
                ReplyId = comment.ReplyId,
                Reactions = comment.Reaction.Select(r => new ReactionDto
                {
                    Id = r.Id,
                    Type = r.Type,
                    BlogId = r.BlogId,
                    CommentId = r.CommentId,
                    UserId = r.UserId
                }).ToList()
            });

            return commentDtos;
        }
    }
}
