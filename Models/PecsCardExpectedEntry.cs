using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Autsim.Models
{
    public class PecsCardExpectedEntry
    {
        [Key]
        public int Id { get; set; }

        // Foreign key to associate with a specific PecsCard
        [ForeignKey("PecsCard")]
        public int PecsCardId { get; set; }
        public PecsCard PecsCard { get; set; }

        public int Key { get; set; }  // Key of the dictionary entry
        public int Value { get; set; } // Value associated with the key
    }
}
