using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace HOMEnitor.Pages
{
    public class UserAmenityModel : PageModel
    {
        private readonly string connectionString = "server=127.0.0.1;database=homenitor_db;uid=root;pwd=;";
        public List<AmenityInfo> Amenities { get; set; } = new();

        public void OnGet()
        {
            LoadAmenities();
        }

        public IActionResult OnPostAdd(string AmenityName, DateTime AccessDate)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return RedirectToPage("/Login");

            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string sql = @"INSERT INTO amenity (UserID, AmenityName, AccessDate, Status)
                           VALUES (@uid, @name, @date, 'Pending')";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@uid", userId);
            cmd.Parameters.AddWithValue("@name", AmenityName);
            cmd.Parameters.AddWithValue("@date", AccessDate);
            cmd.ExecuteNonQuery();

            return RedirectToPage();
        }

        public IActionResult OnPostEdit(int AccessID, string AmenityName, DateTime AccessDate)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string sql = @"UPDATE amenity 
                           SET AmenityName=@name, AccessDate=@date 
                           WHERE AccessID=@id AND Status='Pending'";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@name", AmenityName);
            cmd.Parameters.AddWithValue("@date", AccessDate);
            cmd.Parameters.AddWithValue("@id", AccessID);
            cmd.ExecuteNonQuery();

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int AccessID)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string sql = "DELETE FROM amenity WHERE AccessID=@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", AccessID);
            cmd.ExecuteNonQuery();

            return RedirectToPage();
        }


        private void LoadAmenities()
        {
            Amenities.Clear();
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return;

            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string sql = "SELECT AccessID, AmenityName, AccessDate, Status FROM amenity WHERE UserID=@uid ORDER BY AccessID DESC";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@uid", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Amenities.Add(new AmenityInfo
                {
                    AccessID = reader.GetInt32("AccessID"),
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
            public string AmenityName { get; set; } = string.Empty;
            public DateTime AccessDate { get; set; }
            public string Status { get; set; } = string.Empty;
        }
    }
}
