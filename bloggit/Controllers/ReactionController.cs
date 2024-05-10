using bloggit.DTOs;
using bloggit.Services.Service_Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace bloggit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReactionsController : ControllerBase
    {
        private readonly IReactionService _reactionService;

        public ReactionsController(IReactionService reactionService)
        {
            _reactionService = reactionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReactionsAsync()
        {
            var reactions = await _reactionService.GetAllReactionsAsync();
            return Ok(reactions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReactionByIdAsync(int id)
        {
            var reaction = await _reactionService.GetReactionByIdAsync(id);
            return Ok(reaction);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReactionAsync([FromBody] CreateReactionDto model)
        {
            var reaction = await _reactionService.CreateReactionAsync(model);
            return Ok(reaction);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReactionAsync(int id, [FromBody] UpdateReactionDto model)
        {
            var updatedReaction = await _reactionService.UpdateReactionAsync(id, model);
            return Ok(updatedReaction);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReactionAsync(int id)
        {
            var result = await _reactionService.DeleteReactionAsync(id);
            return Ok(result);
        }
    }
}