using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace HOMEnitor.Pages
{
    public class UserTransactionsModel : PageModel
    {
        private readonly string connectionString = "server=127.0.0.1;database=homenitor_db;uid=root;pwd=;";
        public List<TransactionInfo> Transactions { get; set; } = new();

        public void OnGet()
        {
            LoadTransactions();
        }

        private void LoadTransactions()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return;

            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string sql = @"SELECT TransactionID, DatePaid, SaleType, AmountPaid, TotalAmount
                           FROM transaction
                           WHERE UserID=@uid
                           ORDER BY DatePaid DESC";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@uid", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Transactions.Add(new TransactionInfo
                {
                    TransactionID = reader.GetInt32("TransactionID"),
                    DatePaid = reader.GetDateTime("DatePaid"),
                    SaleType = reader.GetString("SaleType"),
                    AmountPaid = reader.GetDecimal("AmountPaid"),
                    TotalAmount = reader.GetDecimal("TotalAmount")
                });
            }
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }

        public class TransactionInfo
        {
            public int TransactionID { get; set; }
            public DateTime DatePaid { get; set; }
            public string SaleType { get; set; } = string.Empty;
            public decimal AmountPaid { get; set; }
            public decimal TotalAmount { get; set; } 
        }
    }
}
