using bloggit.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace bloggit.Services.Service_Interfaces
{
    public interface IAuthenticationService
    {
        Task<string> TokenLoginAsync(string email, string password);

        Task Register(string firstName, string lastName, string email, string password, string userName, string country, string gender, string? profilePicture);
        
        Task<IActionResult> CreateAdmin(string firstName, string lastName, string email, string username, string password, string country, string gender, string? profilePicture);

    }
}
