using Microsoft.AspNetCore.Authorization;

namespace bloggit.Controllers;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bloggit.Services.Service_Interfaces;
using bloggit.DTOs;



    [Route("api/blog")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogsController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBlogAsync([FromBody] BlogCreateRequest model)
        {
            return await _blogService.CreateBlogAsync(model);;
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBlogAsync(int id, [FromBody] BlogUpdateRequest model)
        {
            return await _blogService.UpdateBlogAsync(id, model);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogAsync(int id)
        {
            return await _blogService.DeleteBlogAsync(id);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogByIdAsync(int id)
        {
            return await _blogService.GetBlogByIdAsync(id);
        }

        [HttpGet("/api/blogs")]
        public async Task<IActionResult> GetAllBlogsAsync()
        {
            return await _blogService.GetAllBlogsAsync();
        }
        
        [HttpGet("/api/blogs/paginate")]
        public async Task<IActionResult> GetBlogsPaginate([FromQuery] PaginateFilter filter)
        {
            return await _blogService.GetBlogsPaginate(filter.PageNumber, filter.PageSize, filter.SortingOption);
        }
    }
