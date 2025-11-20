using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace HOMEnitor.Pages
{
    public class AdminTransactionModel : PageModel
    {
        public List<TransactionRecord> Transactions { get; set; } = new List<TransactionRecord>();

        public void OnGet()
        {
            string connectionString = "server=localhost;database=homenitor_db;user=root;password=;";
            using var con = new MySqlConnection(connectionString);
            con.Open();

            string query = @"SELECT TransactionID, UserID, SaleType, PaymentMethod, ItemDescription,
                             Quantity, UnitPrice, AmountPaid, TotalAmount, DatePaid
                             FROM transaction";

            using var cmd = new MySqlCommand(query, con);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Transactions.Add(new TransactionRecord
                {
                    TransactionID = reader.GetInt32("TransactionID"),
                    UserID = reader.GetInt32("UserID"),
                    SaleType = reader.GetString("SaleType"),
                    PaymentMethod = reader.GetString("PaymentMethod"),
                    ItemDescription = reader.GetString("ItemDescription"),
                    Quantity = reader.GetInt32("Quantity"),
                    UnitPrice = reader.GetDecimal("UnitPrice"),
                    AmountPaid = reader.GetDecimal("AmountPaid"),
                    TotalAmount = reader.GetDecimal("TotalAmount"),
                    DatePaid = reader.GetDateTime("DatePaid")
                });
            }
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }

        public class TransactionRecord
        {
            public int TransactionID { get; set; }
            public int UserID { get; set; }
            public string SaleType { get; set; } = string.Empty;
            public string PaymentMethod { get; set; } = string.Empty;
            public string ItemDescription { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal AmountPaid { get; set; }
            public decimal TotalAmount { get; set; }
            public DateTime DatePaid { get; set; }
        }
    }
}
