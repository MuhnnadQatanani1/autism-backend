using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Autsim.Models.DTOs
{
    public class PecsCardCreateDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "An image is required.")]
        public IFormFile Images { get; set; }

    }
}
