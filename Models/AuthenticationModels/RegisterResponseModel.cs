using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Autsim.Models.AuthenticationModels
{
    public class RegisterResponseModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public string Message { get; set; }
        public int Status { get; set; }
    }
}