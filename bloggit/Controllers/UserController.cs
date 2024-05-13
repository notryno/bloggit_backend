using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using bloggit.DTOs;
using bloggit.Services.Service_Interfaces;

namespace bloggit.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserByIdAsync(string id)
        {
            return await _userService.GetUserByIdAsync(id);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUserAsync(string id, [FromBody] UpdateUserRequest request)
        {
            return await _userService.UpdateUserAsync(id, request);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUserAsync(string id)
        {
            return await _userService.DeleteUserAsync(id);
        }
        
        [HttpGet("/api/users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            return await _userService.GetAllUsersAsync();
        }
        
        [HttpGet("/api/user/public/{id}")]
        public async Task<IActionResult> GetPublicUser(string id)
        {
            return await _userService.GetPublicUser(id);
        }
    }
}
