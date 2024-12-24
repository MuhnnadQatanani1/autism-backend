using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Autsim.Models;

public class PecsImage
{
    [Key]
    public int Id { get; set; }

    [Required]
    public byte[] ImageData { get; set; } // Removed nullable indicator

    public int PecsCardID { get; set; }

    [ForeignKey("PecsCardID")] // Corrected ForeignKey attribute
    public PecsCard PecsCard { get; set; }
}
