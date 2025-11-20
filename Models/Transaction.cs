using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HOMEnitor.Models
{
    [Table("transaction")]
    public class Transaction
    {
        public Transaction()
        {
            DatePaid = DateTime.Now;
        }

        [Key]
        [Column("TransactionID")]
        public int TransactionID { get; set; }

        [Required]
        [Column("UserID")]
        public int UserID { get; set; }

        [Required]
        [Column("UnitPrice")]
        public int UnitPrice { get; set; }

        [Required]
        [Column("Quantity")]
        public int Quantity { get; set; }

        [Required]
        [Column("AmountPaid")]
        public int AmountPaid { get; set; }

        [Required]
        [Column("TotalAmount")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        [Required]
        [Column("SaleType")]
        [StringLength(50)]
        public string SaleType { get; set; } = string.Empty;

        [Required]
        [Column("DatePaid")]
        [DataType(DataType.Date)]
        public DateTime DatePaid { get; set; }

        [Required]
        [Column("PaymentMethod")]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        [Required]
        [Column("ItemDescription")]
        [StringLength(100)]
        public string ItemDescription { get; set; } = string.Empty;
    }
}
