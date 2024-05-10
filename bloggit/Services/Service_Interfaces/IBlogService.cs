namespace bloggit.Services.Service_Interfaces;
using bloggit.DTOs;

public interface IBlogService
{
    Task<BlogDto> CreateBlogAsync(BlogCreateRequest model);
    Task<BlogDto> UpdateBlogAsync(BlogDto model);
    Task<bool> DeleteBlogAsync(int id);
    Task<BlogDto> GetBlogByIdAsync(int id);
    Task<IEnumerable<BlogDto>> GetAllBlogsAsync();
}