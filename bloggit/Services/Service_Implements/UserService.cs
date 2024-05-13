using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using bloggit.DTOs;
using bloggit.Models;
using bloggit.Services.Service_Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace bloggit.Services.Service_Implements
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> GetUserByIdAsync(string id)
        {
            var currentUser = await GetCurrentUserAsync();
            var requestedUser = await _userManager.FindByIdAsync(id);

            if (currentUser == null || requestedUser == null)
            {
                return new NotFoundResult();
            }

            if (!IsAuthorized(currentUser, requestedUser))
            {
                return new ForbidResult();
            }

            return new OkObjectResult(requestedUser);
        }

        public async Task<IActionResult> UpdateUserAsync(string id, UpdateUserRequest request)
        {
            var userToUpdate = await _userManager.FindByIdAsync(id);
            if (userToUpdate == null)
            {
                return new NotFoundResult();
            }

            var currentUser = await GetCurrentUserAsync();
            if (!IsAuthorized(currentUser, userToUpdate))
            {
                return new ForbidResult();
            }

            UpdateUserFields(userToUpdate, request);
            var result = await _userManager.UpdateAsync(userToUpdate);

            return result.Succeeded
                ? new OkObjectResult(new { message = "User updated successfully" })
                : new BadRequestObjectResult(new { errors = result.Errors });
        }


        private void UpdateUserFields(ApplicationUser userToUpdate, UpdateUserRequest request)
        {
            if (!string.IsNullOrEmpty(request.FirstName))
            {
                userToUpdate.FirstName = request.FirstName;
            }

            if (!string.IsNullOrEmpty(request.LastName))
            {
                userToUpdate.LastName = request.LastName;
            }

            if (!string.IsNullOrEmpty(request.Username))
            {
                userToUpdate.UserName = request.Username;
            }

            if (!string.IsNullOrEmpty(request.Country))
            {
                userToUpdate.Country = request.Country;
            }

            if (!string.IsNullOrEmpty(request.Gender))
            {
                userToUpdate.Gender = request.Gender;
            }

            if (!string.IsNullOrEmpty(request.ProfilePicture))
            {
                userToUpdate.ProfilePicture = request.ProfilePicture;
            }
        }

        private async Task<ApplicationUser?> GetCurrentUserAsync()
        {
            var user = _httpContextAccessor.HttpContext.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                return null;
            }

            var userIdClaim = user.FindFirst("userId");
            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            {
                return null;
            }
            var foundUser = await _userManager.FindByIdAsync(userIdClaim.Value);
            if (foundUser == null || foundUser.isDeleted)
            {
                return null;
            }

            return foundUser;
        }

        private bool IsAuthorized(ApplicationUser currentUser, ApplicationUser targetUser)
        {
            var isAdmin = _userManager.IsInRoleAsync(currentUser, "Admin").Result;
            return currentUser.Id == targetUser.Id || isAdmin;
        }


        public async Task<IActionResult> DeleteUserAsync(string id)
        {
            var currentUser = await GetCurrentUserAsync();
            var userToDelete = await _userManager.FindByIdAsync(id);

            if (currentUser == null || userToDelete == null || userToDelete.isDeleted)
            {
                return new NotFoundResult();
            }

            if (!IsAuthorized(currentUser, userToDelete))
            {
                return new ForbidResult();
            }

            userToDelete.isDeleted = true;
            var result = await _userManager.UpdateAsync(userToDelete);

            return result.Succeeded
                ? new OkObjectResult(new { message = "User deleted successfully" })
                : new BadRequestObjectResult(new { errors = result.Errors });
        }

        public Task<IActionResult> GetAllUsersAsync()
        {
            var users = _userManager.Users;
            return Task.FromResult<IActionResult>(new OkObjectResult(users));
        }
        
        public async Task<IActionResult> GetPublicUser(string id)
        {
            var requestedUser = await _userManager.FindByIdAsync(id);
            if (requestedUser == null || requestedUser.isDeleted)
            {
                return new NotFoundResult();
            }

            var userDto = new PublicUserDto
            {
                Id = requestedUser.Id,
                Username = requestedUser.UserName,
                FirstName = requestedUser.FirstName,
                LastName = requestedUser.LastName,
                Country = requestedUser.Country,
                Gender = requestedUser.Gender,
                ProfilePicture = requestedUser.ProfilePicture,
                CreatedOn = requestedUser.CreatedOn
            };

            return new OkObjectResult(userDto);
        }
    }
}