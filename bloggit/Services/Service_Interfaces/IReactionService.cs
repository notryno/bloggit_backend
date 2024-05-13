using bloggit.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace bloggit.Services.Service_Interfaces
{
    public interface IReactionService
    {
        Task<IActionResult> AddReaction(int blogId, CreateReactionDto model);
        Task<IActionResult> RemoveReaction(int reactionId, int blogId);
    }
}