using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace HOMEnitor.Pages
{
    public class CashierAmenityModel : PageModel
    {
        public List<AmenityInfo> ApprovedAmenities { get; set; } = new();

        private readonly string connectionString = "server=127.0.0.1;database=homenitor_db;uid=root;pwd=;";

        public void OnGet()
        {
            LoadApprovedAmenities();
        }

        private void LoadApprovedAmenities()
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            string query = "SELECT AccessID, UserID, AmenityName, AccessDate, Status FROM amenity WHERE Status = 'Approved'";
            using var cmd = new MySqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ApprovedAmenities.Add(new AmenityInfo
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
