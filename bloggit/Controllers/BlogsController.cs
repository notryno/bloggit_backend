using Microsoft.AspNetCore.Authorization;

namespace bloggit.Controllers;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bloggit.Services.Service_Interfaces;
using bloggit.DTOs;



    [Route("api/[controller]")]
    [ApiController]
    // [Authorize]
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
        public async Task<IActionResult> UpdateBlogAsync(int id, [FromBody] BlogDto model)
        {
            model.Id = id;
            var result = await _blogService.UpdateBlogAsync(model);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogAsync(int id)
        {
            var result = await _blogService.DeleteBlogAsync(id);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogByIdAsync(int id)
        {
            var result = await _blogService.GetBlogByIdAsync(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBlogsAsync()
        {
            var result = await _blogService.GetAllBlogsAsync();
            return Ok(result);
        }
    }
