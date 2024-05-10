using System.Collections.Generic;
using System.Threading.Tasks;
using bloggit.DTOs;

namespace bloggit.Services.Service_Interfaces
{
    public interface ICommentService
    {
        Task<CommentDto> CreateCommentAsync(CreateCommentDto model);
        Task<CommentDto> UpdateCommentAsync(UpdateCommentDto model);
        Task<bool> DeleteCommentAsync(int id);
        Task<CommentDto> GetCommentByIdAsync(int id);
        Task<IEnumerable<CommentDto>> GetAllCommentsAsync();
    }
}