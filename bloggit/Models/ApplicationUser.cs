using System;
using Microsoft.AspNetCore.Identity;

namespace bloggit.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfilePicture { get; set; }
        public string Country { get; set; }
        public string Gender { get; set; }
        public bool isDeleted { get; set; }

        public ICollection<Blogs> Blogs { get; set; }
        public ICollection<Comments> Comments { get; set; }
        public ICollection<Logs> Logs { get; set; }
        
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

    }

}

