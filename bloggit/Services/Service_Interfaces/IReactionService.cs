using bloggit.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bloggit.Services.Service_Interfaces
{
    public interface IReactionService
    {
        Task<IEnumerable<ReactionDto>> GetAllReactionsAsync();
        Task<ReactionDto> GetReactionByIdAsync(int id);
        Task<ReactionDto> CreateReactionAsync(CreateReactionDto model);
        Task<ReactionDto> UpdateReactionAsync(int id, UpdateReactionDto model);
        Task<bool> DeleteReactionAsync(int id);
    }
}