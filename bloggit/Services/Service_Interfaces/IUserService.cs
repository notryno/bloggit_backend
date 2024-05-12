using bloggit.DTOs;

namespace bloggit.Services.Service_Interfaces;

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public interface IUserService
{
    Task<IActionResult> GetUserByIdAsync(string id);
    Task<IActionResult> UpdateUserAsync(string id, UpdateUserRequest request);
    Task<IActionResult> DeleteUserAsync(string id);
    Task<IActionResult> GetAllUsersAsync();
}
