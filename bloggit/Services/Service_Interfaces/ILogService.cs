using System.Threading.Tasks;
using bloggit.DTOs;
using bloggit.Models;

namespace bloggit.Services.Service_Interfaces
{
    public interface ILogService
    {
        Task LogBlogActionAsync(int blogId, string actionType, string description, string userId);
        Task LogCommentActionAsync(int commentId, string actionType, string description);
    }
}