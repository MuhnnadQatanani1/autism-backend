using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Autsim.Models.settingModel
{
    public class UpdatePhotoRequestModel
    {
        [Required]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Profile photo is required.")]
        public IFormFile Photo { get; set; }
    }
}