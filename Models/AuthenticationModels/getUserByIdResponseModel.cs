using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Autsim.Models.AuthenticationModels
{
    public class getUserByIdResponseModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
         public string LastName { get; set; }

        public string ProfilePictureUrl { get; set; }
        public ICollection<string> Roles { get; set; }
    }
}