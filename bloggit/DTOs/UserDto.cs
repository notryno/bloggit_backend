namespace bloggit.DTOs;

public class UpdateUserRequest
{
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? Country { get; set; }
        public string? Gender { get; set; }
        public string? ProfilePicture { get; set; }
}

public class PasswordUpdateRequest
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
}

public class PublicUserDto
{
    public string Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Username { get; set; }
    public string? Country { get; set; }
    public string? Gender { get; set; }
    public string? ProfilePicture { get; set; }
    public DateTime? CreatedOn { get; set; }
}
