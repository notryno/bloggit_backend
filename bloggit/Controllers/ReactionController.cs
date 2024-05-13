using bloggit.DTOs;
using bloggit.Services.Service_Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace bloggit.Controllers
{
    [Route("api/blogs/{blogId}/reactions")]
    [ApiController]
    public class ReactionsController : ControllerBase
    {
        private readonly IReactionService _reactionService;

        public ReactionsController(IReactionService reactionService)
        {
            _reactionService = reactionService;
        }

        [HttpPost]
        public async Task<IActionResult> AddReaction(int blogId, [FromBody] CreateReactionDto model)
        {
            var reaction = await _reactionService.AddReaction(blogId, model);
            return Ok(reaction);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveReaction(int id, int blogId)
        {
            var result = await _reactionService.RemoveReaction(id, blogId);
            return Ok(result);
        }
        
        [Route("count")]
        [HttpGet]
        public async Task<IActionResult> GetReactionCount(int blogId)
        {
            var reactionCount = await _reactionService.GetReactionCount(blogId);
            return Ok(reactionCount);
        }
        
    }
}