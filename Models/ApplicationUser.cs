using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Autsim.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string? ProfilePictureUrl { get; set; }

    }
}
