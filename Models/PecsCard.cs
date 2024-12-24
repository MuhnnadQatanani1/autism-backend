using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Autsim.Models
{
    public class PecsCard
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public PecsImage Image { get; set; }
        public byte[] AudioData { get; set; } // البيانات الصوتية

        public List<PecsCardExpectedEntry> ExpectedEntries { get; set; } = new List<PecsCardExpectedEntry>();

        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
    }
}
