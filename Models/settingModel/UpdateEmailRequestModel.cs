using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Autsim.Models.settingModel
{
    public class UpdateEmailRequestModel
    {

        [Required]
        public string UserId { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string NewEmail { get; set; }
    }
}