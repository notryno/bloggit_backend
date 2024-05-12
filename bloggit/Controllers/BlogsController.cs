using Microsoft.AspNetCore.Authorization;

namespace bloggit.Controllers;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bloggit.Services.Service_Interfaces;
using bloggit.DTOs;



    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BlogsController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogsController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBlogAsync([FromBody] BlogCreateRequest model)
        {
            var result = await _blogService.CreateBlogAsync(model);
            return Ok(result);
        }

        [HttpPut("{id}")]
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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllBlogsAsync()
        {
            return await _blogService.GetAllBlogsAsync();
        }
    }
