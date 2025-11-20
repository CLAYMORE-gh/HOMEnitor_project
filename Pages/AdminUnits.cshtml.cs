using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace HOMEnitor.Pages
{
    public class AdminUnitsModel : PageModel
    {
        private readonly string connectionString = "server=localhost;user id=root;password=;database=homenitor_db;";

        [BindProperty] public int UnitID { get; set; }
        [BindProperty] public int HomeownerID { get; set; }
        [BindProperty] public string Village { get; set; } = string.Empty;
        [BindProperty] public string PaymentStatus { get; set; } = string.Empty;

        public List<Unit> Units { get; set; } = new();

        public void OnGet() => LoadUnits();

        private void LoadUnits()
        {
            Units.Clear();
            using var con = new MySqlConnection(connectionString);
            con.Open();

            string query = "SELECT * FROM units ORDER BY UnitID DESC";
            using var cmd = new MySqlCommand(query, con);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Units.Add(new Unit
                {
                    UnitID = reader.GetInt32("UnitID"),
                    HomeownerID = reader.GetInt32("HomeownerID"),
                    Village = reader.GetString("Village"),
                    PaymentStatus = reader.GetString("PaymentStatus")
                });
            }
        }

        public IActionResult OnPostAdd()
        {
            using var con = new MySqlConnection(connectionString);
            con.Open();

            string sql = "INSERT INTO units (HomeownerID, Village, PaymentStatus) VALUES (@hid, @village, @ps)";
            using var cmd = new MySqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@hid", HomeownerID);
            cmd.Parameters.AddWithValue("@village", Village);
            cmd.Parameters.AddWithValue("@ps", PaymentStatus);
            cmd.ExecuteNonQuery();

            return RedirectToPage();
        }
        public IActionResult OnPostUpdate()
        {
            using var con = new MySqlConnection(connectionString);
            con.Open();

            string sql = "UPDATE units SET HomeownerID=@hid, PaymentStatus=@ps WHERE UnitID=@id";
            using var cmd = new MySqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@hid", HomeownerID);
            cmd.Parameters.AddWithValue("@ps", PaymentStatus);
            cmd.Parameters.AddWithValue("@id", UnitID);
            cmd.ExecuteNonQuery();

            return RedirectToPage();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }

        public class Unit
        {
            public int UnitID { get; set; }
            public int HomeownerID { get; set; }
            public string Village { get; set; } = string.Empty;
            public string PaymentStatus { get; set; } = string.Empty;
        }
    }
}
