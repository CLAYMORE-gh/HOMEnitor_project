using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HOMEnitor.Models
{
    [Table("homeowner")] // match your MySQL table name
    public class Homeowner
    {
        [Key]
        [Column("HomeownerID")]
        public int HomeownerID { get; set; }

        [Column("FirstName")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Column("MiddleName")]
        [StringLength(50)]
        public string MiddleName { get; set; } = string.Empty;

        [Column("LastName")]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Column("ContactInfo")]
        public long ContactInfo { get; set; }

        [Column("UserType")]
        [StringLength(50)]
        public string UserType { get; set; } = "Homeowner";

        // Computed property for displaying full name
        [NotMapped]
        public string Name => $"{FirstName} {MiddleName} {LastName}".Trim();
    }
}
