using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HOMEnitor.Models
{
    [Table("user")]
    public class User
    {
        [Key]
        [Column("UserID")]
        public int UserID { get; set; }

        [Required]
        [Column("UserType")]
        [StringLength(20)]
        public string UserType { get; set; } = string.Empty;

        [Required]
        [Column("Username")]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [Column("Password")]
        [StringLength(255)] 
        public string Password { get; set; } = string.Empty;

        [Column("DateCreated")]
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
