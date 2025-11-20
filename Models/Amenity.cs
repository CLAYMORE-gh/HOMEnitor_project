using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HOMEnitor.Models
{
    [Table("amenity")] // match the table name in MySQL
    public class Amenity
    {
        [Key]
        [Column("AccessID")]
        public int AccessID { get; set; }
        
        [Column("UserID")]
        public int UserID { get; set; }

        [Required]
        [Column("AmenityName")]
        [StringLength(50)]
        public string AmenityName { get; set; } = string.Empty;

        [Required]
        [Column("AccessDate")]
        public DateTime AccessDate { get; set; } = DateTime.Now;
    }
}
