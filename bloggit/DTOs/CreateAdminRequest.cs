namespace bloggit.DTOs;

public class CreateAdminRequest
{
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string Country { get; set; }
    public string Gender { get; set; }
    public string? ProfilePicture { get; set; }
}