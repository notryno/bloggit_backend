using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace bloggit.DTOs
{
    public class RegisterRequest
    {
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Username { get; set; }
        public string Country { get; set; }
        public string Gender { get; set; }
        public string? ProfilePicture { get; set; }
    }
}

