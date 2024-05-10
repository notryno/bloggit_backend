using bloggit.DTOs;

namespace bloggit.Services.Service_Interfaces
{
    public interface IAuthenticationService
    {
        Task<string> TokenLoginAsync(string email, string password);

        Task Register(string firstName, string lastName, string email, string password, string userName, string country, string gender, string? profilePicture);
    }
}
