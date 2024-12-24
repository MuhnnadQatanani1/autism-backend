using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Autsim.Models.AuthenticationModels
{
    public class LoginRequestModel
    {
        [Required(ErrorMessage = "Email is required.")]
[EmailAddress(ErrorMessage = "Invalid email format.")]
public string Email { get; set; }

[Required(ErrorMessage = "Password is required.")]
[StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
public string Password { get; set; }
    }
}