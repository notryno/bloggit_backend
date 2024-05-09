namespace bloggit.Services.Service_Interfaces
{
    public interface IAuthenticationService
    {
        Task<string> TokenLoginAsync(string email, string password);

        Task Register(string firstName, string lastName, string email, string password);
    }
}
