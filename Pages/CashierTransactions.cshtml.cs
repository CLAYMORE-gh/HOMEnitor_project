using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace HOMEnitor.Pages
{
    public class CashierTransactionsModel : PageModel
    {
        private readonly string connectionString = "server=localhost;database=homenitor_db;uid=root;pwd=;";

        public List<Transaction> Transactions { get; set; } = new();

        public void OnGet()
        {
            Transactions.Clear();
            using var con = new MySqlConnection(connectionString);
            con.Open();

            string query = "SELECT TransactionID, UserID, UnitPrice, Quantity, AmountPaid, TotalAmount, SaleType, DatePaid, PaymentMethod, ItemDescription FROM transaction";
            using var cmd = new MySqlCommand(query, con);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Transactions.Add(new Transaction
                {
                    TransactionID = reader.GetInt32(reader.GetOrdinal("TransactionID")),
                    UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                    UnitPrice = reader.GetInt32(reader.GetOrdinal("UnitPrice")),
                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                    AmountPaid = reader.GetInt32(reader.GetOrdinal("AmountPaid")),
                    TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                    SaleType = reader.IsDBNull(reader.GetOrdinal("SaleType")) ? string.Empty : reader.GetString(reader.GetOrdinal("SaleType")),
                    DatePaid = reader.IsDBNull(reader.GetOrdinal("DatePaid")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("DatePaid")),
                    PaymentMethod = reader.IsDBNull(reader.GetOrdinal("PaymentMethod")) ? string.Empty : reader.GetString(reader.GetOrdinal("PaymentMethod")),
                    ItemDescription = reader.IsDBNull(reader.GetOrdinal("ItemDescription")) ? string.Empty : reader.GetString(reader.GetOrdinal("ItemDescription"))
                });
            }
        }

        public IActionResult OnPostAdd(
            int UserID,
            int UnitPrice,
            int Quantity,
            int AmountPaid,
            decimal TotalAmount,
            string SaleType,
            string PaymentMethod,
            string ItemDescription,
            DateTime? DatePaid)
        {
            using var con = new MySqlConnection(connectionString);
            con.Open();

            string query = @"INSERT INTO transaction 
                     (UserID, UnitPrice, Quantity, AmountPaid, TotalAmount, 
                      SaleType, DatePaid, PaymentMethod, ItemDescription)
                     VALUES (@UserID, @UnitPrice, @Quantity, @AmountPaid, 
                      @TotalAmount, @SaleType, @DatePaid, @PaymentMethod, 
                      @ItemDescription)";

            using var cmd = new MySqlCommand(query, con);
            cmd.Parameters.AddWithValue("@UserID", UserID);
            cmd.Parameters.AddWithValue("@UnitPrice", UnitPrice);
            cmd.Parameters.AddWithValue("@Quantity", Quantity);
            cmd.Parameters.AddWithValue("@AmountPaid", AmountPaid);
            cmd.Parameters.AddWithValue("@TotalAmount", TotalAmount);
            cmd.Parameters.AddWithValue("@SaleType", SaleType);
            cmd.Parameters.AddWithValue("@DatePaid", DatePaid ?? DateTime.Now);
            cmd.Parameters.AddWithValue("@PaymentMethod", PaymentMethod ?? string.Empty);
            cmd.Parameters.AddWithValue("@ItemDescription", ItemDescription ?? string.Empty);

            cmd.ExecuteNonQuery();

            return RedirectToPage();
        }

        public IActionResult OnPostEdit(
            int TransactionID,
            int UserID,
            int UnitPrice,
            int Quantity,
            int AmountPaid,
            decimal TotalAmount,
            string SaleType,
            string PaymentMethod,
            string ItemDescription,
            DateTime? DatePaid = null)
        {
            using var con = new MySqlConnection(connectionString);
            con.Open();

            string query = @"UPDATE transaction 
                     SET UserID=@UserID, UnitPrice=@UnitPrice, Quantity=@Quantity, 
                         AmountPaid=@AmountPaid, TotalAmount=@TotalAmount, SaleType=@SaleType, 
                         DatePaid=@DatePaid, PaymentMethod=@PaymentMethod, ItemDescription=@ItemDescription
                     WHERE TransactionID=@TransactionID";

            using var cmd = new MySqlCommand(query, con);
            cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
            cmd.Parameters.AddWithValue("@UserID", UserID);
            cmd.Parameters.AddWithValue("@UnitPrice", UnitPrice);
            cmd.Parameters.AddWithValue("@Quantity", Quantity);
            cmd.Parameters.AddWithValue("@AmountPaid", AmountPaid);
            cmd.Parameters.AddWithValue("@TotalAmount", TotalAmount);
            cmd.Parameters.AddWithValue("@SaleType", SaleType ?? string.Empty);
            cmd.Parameters.AddWithValue("@DatePaid", DatePaid ?? DateTime.Now);
            cmd.Parameters.AddWithValue("@PaymentMethod", PaymentMethod ?? string.Empty);
            cmd.Parameters.AddWithValue("@ItemDescription", ItemDescription ?? string.Empty);

            cmd.ExecuteNonQuery();
            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int TransactionID)
        {
            using var con = new MySqlConnection(connectionString);
            con.Open();

            string query = "DELETE FROM transaction WHERE TransactionID = @TransactionID";
            using var cmd = new MySqlCommand(query, con);
            cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
            cmd.ExecuteNonQuery();

            return RedirectToPage();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }

        public class Transaction
        {
            public int TransactionID { get; set; }
            public int UserID { get; set; }
            public int UnitPrice { get; set; }
            public int Quantity { get; set; }
            public int AmountPaid { get; set; }
            public decimal TotalAmount { get; set; }
            public string SaleType { get; set; } = string.Empty;
            public DateTime DatePaid { get; set; }
            public string PaymentMethod { get; set; } = string.Empty;
            public string ItemDescription { get; set; } = string.Empty;
        }
    }
}
