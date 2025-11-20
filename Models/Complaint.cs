using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HOMEnitor.Models
{
    [Table("complaint")] // name of your table in MySQL

    public class Complaint
    {
        [Key]
        [Column("ComplaintID")]
        public int ComplaintID { get; set; }

        [Column("UserID")]
        public int UserID { get; set; }

        [Column("Description")]
        [StringLength(250)]
        public string Description { get; set; } = string.Empty;

        [Column("Date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        [Column("Status")]
        [StringLength(50)]
        public string Status { get; set; } = "Pending";
    }
}
