using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Autsim.Enums;

namespace Autsim.Models.AuthenticationModels
{
    public class RegisterRequestModel
    {
        
		[Required(ErrorMessage = "first name is required.")]
		public string Firstname { get; set; }
		[Required(ErrorMessage = "last name is required.")]
		public string Lastname { get; set; }
        [Required(ErrorMessage = "phone is required.")]
		public string phone { get; set; }

		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress(ErrorMessage = "Invalid email format.")]
		public string Email { get; set; }

		public UserRole Role { get; set; }

		[Required(ErrorMessage = "Password is required.")]
		[StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Confirm password is required.")]
		[Compare("Password", ErrorMessage = "Passwords do not match.")]
		public string CoPassword { get; set; }
    }
}