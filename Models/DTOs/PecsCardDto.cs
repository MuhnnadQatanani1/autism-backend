namespace Autsim.Models.DTOs
{
    public class PecsCardDto
    {
        public int Id { get; set; }  // Unique identifier of the card
        public string Name { get; set; }  // Name of the PECS card
        public string IFormFile { get; set; }  // Base64-encoded image string for frontend
    }
}
