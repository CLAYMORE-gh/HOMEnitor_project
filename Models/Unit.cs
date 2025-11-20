using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HOMEnitor.Models
{
    [Table("unit")] // match your MySQL table name
    public class Unit
    {
        [Key]
        [Column("UnitID")]
        public int UnitID { get; set; }

        [Column("HomeownerID")]
        public int HomeownerID { get; set; }

        [Column("Village")]
        [StringLength(50)]
        public string Village { get; set; } = string.Empty;

        [Column("PaymentStatus")]
        [StringLength(50)]
        public string PaymentStatus { get; set; } = string.Empty;

        // Optional navigation property
        [ForeignKey("HomeownerID")]
        public Homeowner? Homeowner { get; set; }
    }
}
