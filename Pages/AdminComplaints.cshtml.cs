using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace HOMEnitor.Pages
{
    public class AdminComplaintsModel : PageModel
    {
        private readonly string connectionString = "server=localhost;user id=root;password=;database=homenitor_db;";
        public List<Complaint> Complaints { get; set; } = new();

        public void OnGet() => LoadComplaints();

        private void LoadComplaints()
        {
            Complaints.Clear();
            using var con = new MySqlConnection(connectionString);
            con.Open();
            string query = "SELECT * FROM complaint ORDER BY ComplaintID DESC";
            using var cmd = new MySqlCommand(query, con);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Complaints.Add(new Complaint
                {
                    ComplaintID = reader.GetInt32("ComplaintID"),
                    UserID = reader.GetInt32("UserID"),
                    Description = reader.GetString("Description"),
                    Date = reader.GetDateTime("Date").ToString("yyyy-MM-dd"),
                    Status = reader.GetString("Status")
                });
            }
        }

        public IActionResult OnPostUpdate(int ComplaintID, string Status)
        {
            using var con = new MySqlConnection(connectionString);
            con.Open();
            string sql = "UPDATE complaint SET Status=@status WHERE ComplaintID=@id";
            using var cmd = new MySqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@status", Status);
            cmd.Parameters.AddWithValue("@id", ComplaintID);
            cmd.ExecuteNonQuery();
            return RedirectToPage();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }

        public class Complaint
        {
            public int ComplaintID { get; set; }
            public int UserID { get; set; }
            public string Description { get; set; } = string.Empty;
            public string Date { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
        }
    }
}
