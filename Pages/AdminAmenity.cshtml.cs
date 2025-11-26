using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace HOMEnitor.Pages
{
    public class AdminAmenityModel : PageModel
    {
        public List<AmenityInfo> Amenities { get; set; } = new();

        private readonly string connectionString = "server=127.0.0.1;database=homenitor_db;uid=root;pwd=;";

        // Hardcoded amenity prices
        private readonly Dictionary<string, double> AmenityPrices = new()
        {
            { "Pool", 100 },
            { "Clubhouse", 150 },
            { "Basketball Court", 50 }
        };

        public void OnGet()
        {
            LoadAmenities();
        }

        public void OnPostApprove(int AccessID)
        {
            UpdateStatus(AccessID, "Approved");
            AddTransactionForAmenity(AccessID);
            LoadAmenities();
        }

        public void OnPostDeny(int AccessID)
        {
            UpdateStatus(AccessID, "Denied");
            LoadAmenities();
        }

        private void UpdateStatus(int accessId, string status)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            string query = "UPDATE amenity SET Status=@Status WHERE AccessID=@AccessID";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@AccessID", accessId);
            cmd.ExecuteNonQuery();
        }

        // MAIN LOGIC – Add to transactions table
        private void AddTransactionForAmenity(int accessId)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            // Fetch amenity details
            string selectQuery = "SELECT UserID, AmenityName FROM amenity WHERE AccessID=@AccessID";
            using var cmd = new MySqlCommand(selectQuery, conn);
            cmd.Parameters.AddWithValue("@AccessID", accessId);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return;

            int userId = reader.GetInt32("UserID");
            string amenity = reader.GetString("AmenityName");
            reader.Close();

            // Determine price
            double price = AmenityPrices.ContainsKey(amenity) ? AmenityPrices[amenity] : 0;

            // Prevent duplicate transaction
            string checkQuery = @"SELECT COUNT(*) FROM transaction 
                                  WHERE UserID=@UserID 
                                  AND ItemDescription=@Desc 
                                  AND SaleType='Amenity Fee'";
            using var checkCmd = new MySqlCommand(checkQuery, conn);
            checkCmd.Parameters.AddWithValue("@UserID", userId);
            checkCmd.Parameters.AddWithValue("@Desc", amenity);

            long count = (long)checkCmd.ExecuteScalar();
            if (count > 0) return; // Already added → skip

            // Insert new transaction
            string insertQuery = @"INSERT INTO transaction 
                (UserID, UnitPrice, Quantity, AmountPaid, TotalAmount, SaleType, PaymentMethod, ItemDescription)
                VALUES (@UserID, @UnitPrice, @Quantity, @AmountPaid, @TotalAmount, 'Amenity Fee', 'Cash', @Desc)";

            using var insertCmd = new MySqlCommand(insertQuery, conn);
            insertCmd.Parameters.AddWithValue("@UserID", userId);
            insertCmd.Parameters.AddWithValue("@UnitPrice", price);
            insertCmd.Parameters.AddWithValue("@Quantity", 1);
            insertCmd.Parameters.AddWithValue("@AmountPaid", 0);
            insertCmd.Parameters.AddWithValue("@TotalAmount", price);
            insertCmd.Parameters.AddWithValue("@Desc", amenity);

            insertCmd.ExecuteNonQuery();
        }

        private void LoadAmenities()
        {
            Amenities.Clear();
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            string query = "SELECT AccessID, UserID, AmenityName, AccessDate, Status FROM amenity";
            using var cmd = new MySqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Amenities.Add(new AmenityInfo
                {
                    AccessID = reader.GetInt32("AccessID"),
                    UserID = reader.GetInt32("UserID"),
                    AmenityName = reader.GetString("AmenityName"),
                    AccessDate = reader.GetDateTime("AccessDate"),
                    Status = reader.GetString("Status")
                });
            }
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }

        public class AmenityInfo
        {
            public int AccessID { get; set; }
            public int UserID { get; set; }
            public string AmenityName { get; set; }
            public DateTime AccessDate { get; set; }
            public string Status { get; set; }
        }
    }
}
