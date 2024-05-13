using bloggit.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using bloggit.Models;
using Microsoft.AspNetCore.Mvc;

namespace bloggit.Services.Service_Interfaces
{
    public interface IReactionService
    {
        Task<IActionResult> AddReaction(int blogId, CreateReactionDto model);
        Task<IActionResult> RemoveReaction(int reactionId, int blogId);
        Task<ReactionCountDto> GetReactionCount(int blogId);
        Task<List<Reactions>> GetAllReactionsByUserId(string userId);
    }
}