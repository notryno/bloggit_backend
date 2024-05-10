using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bloggit.DTOs;
using bloggit.Services.Service_Interfaces;

namespace bloggit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCommentAsync([FromBody] CreateCommentDto model)
        {
            var result = await _commentService.CreateCommentAsync(model);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCommentAsync(int id, [FromBody] UpdateCommentDto model)
        {
            model.Id = id;
            var result = await _commentService.UpdateCommentAsync(model);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommentAsync(int id)
        {
            var result = await _commentService.DeleteCommentAsync(id);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommentByIdAsync(int id)
        {
            var result = await _commentService.GetCommentByIdAsync(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCommentsAsync()
        {
            var result = await _commentService.GetAllCommentsAsync();
            return Ok(result);
        }
    }
}