using Microsoft.AspNetCore.Mvc;

namespace bloggit.Services.Service_Interfaces;
using bloggit.DTOs;

public interface IBlogService
{
    Task<IActionResult> CreateBlogAsync(BlogCreateRequest model);
    Task<IActionResult> UpdateBlogAsync(int id, BlogUpdateRequest model);
    Task<IActionResult> DeleteBlogAsync(int id);
    Task<IActionResult> GetBlogByIdAsync(int id);
    Task<IActionResult> GetAllBlogsAsync();
    Task<IActionResult> GetBlogsPaginate(int pageNumber, int pageSize, string sortingOption);
}