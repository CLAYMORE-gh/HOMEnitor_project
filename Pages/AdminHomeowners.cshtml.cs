using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace HOMEnitor.Pages
{
    public class AdminHomeownersModel : PageModel
    {
        private readonly string connectionString = "server=localhost;user id=root;password=;database=homenitor_db;";

        [BindProperty] public int HomeownerID { get; set; }
        [BindProperty] public string FirstName { get; set; } = string.Empty;
        [BindProperty] public string MiddleName { get; set; } = string.Empty;
        [BindProperty] public string LastName { get; set; } = string.Empty;
        [BindProperty] public string ContactInfo { get; set; } = string.Empty;

        public List<Homeowner> Homeowners { get; set; } = new();

        public void OnGet() => LoadHomeowners();

        private void LoadHomeowners()
        {
            Homeowners.Clear();
            using var con = new MySqlConnection(connectionString);
            con.Open();

            var cmd = new MySqlCommand("SELECT * FROM homeowner ORDER BY HomeownerID DESC", con);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Homeowners.Add(new Homeowner
                {
                    HomeownerID = reader.GetInt32("HomeownerID"),
                    FirstName = reader.GetString("FirstName"),
                    MiddleName = reader["MiddleName"]?.ToString() ?? string.Empty,
                    LastName = reader.GetString("LastName"),
                    ContactInfo = reader["ContactInfo"]?.ToString() ?? string.Empty,
                });
            }
        }

        public IActionResult OnPostAdd()
        {
            using var con = new MySqlConnection(connectionString);
            con.Open();

            string sql = @"INSERT INTO homeowner (FirstName, MiddleName, LastName, ContactInfo)
                           VALUES (@fn, @mn, @ln, @ci)";
            using var cmd = new MySqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@fn", FirstName);
            cmd.Parameters.AddWithValue("@mn", MiddleName ?? string.Empty);
            cmd.Parameters.AddWithValue("@ln", LastName);
            cmd.Parameters.AddWithValue("@ci", ContactInfo);
            cmd.ExecuteNonQuery();

            return RedirectToPage();
        }

        public IActionResult OnPostUpdate()
        {
            using var con = new MySqlConnection(connectionString);
            con.Open();

            string sql = @"UPDATE homeowner
                           SET FirstName=@fn, MiddleName=@mn, LastName=@ln, ContactInfo=@ci
                           WHERE HomeownerID=@id";
            using var cmd = new MySqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@fn", FirstName);
            cmd.Parameters.AddWithValue("@mn", MiddleName ?? string.Empty);
            cmd.Parameters.AddWithValue("@ln", LastName);
            cmd.Parameters.AddWithValue("@ci", ContactInfo);
            cmd.Parameters.AddWithValue("@id", HomeownerID);
            cmd.ExecuteNonQuery();

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            using var con = new MySqlConnection(connectionString);
            con.Open();

            var cmd = new MySqlCommand("DELETE FROM homeowner WHERE HomeownerID=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

            return RedirectToPage();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }

        public class Homeowner
        {
            public int HomeownerID { get; set; }
            public string FirstName { get; set; } = string.Empty;
            public string MiddleName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string ContactInfo { get; set; } = string.Empty;
        }
    }
}
