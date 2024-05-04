using System;

namespace bloggit.Models
{
    public class CustomRegisterRequest
	{
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        }
}

