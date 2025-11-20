using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace HOMEnitor.Pages
{
    public class UserComplaintModel : PageModel
    {
        public List<Complaint> Complaints { get; set; } = new();
        public int? UserID { get; set; }

        private readonly string connectionString = "server=localhost;user=root;password=;database=homenitor_db;";

        public void OnGet()
        {
            UserID = HttpContext.Session.GetInt32("UserID");
            if (UserID == null)
            {
                RedirectToPage("/Login");
            }

            Complaints.Clear();
            using var con = new MySqlConnection(connectionString);
            con.Open();
            var cmd = new MySqlCommand("SELECT * FROM complaint WHERE UserID=@uid ORDER BY ComplaintID DESC", con);
            cmd.Parameters.AddWithValue("@uid", UserID);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Complaints.Add(new Complaint
                {
                    ComplaintID = reader.GetInt32("ComplaintID"),
                    Description = reader.GetString("Description"),
                    Date = reader.GetDateTime("Date").ToString("yyyy-MM-dd"),
                    Status = reader.GetString("Status")
                });
            }
        }


        public IActionResult OnPostSave(int? ComplaintID, string Description)
        {
            // Always pull UserID from session
            int? uid = HttpContext.Session.GetInt32("UserID");
            if (uid == null)
                return RedirectToPage("/Login");

            if (string.IsNullOrWhiteSpace(Description))
            {
                TempData["Error"] = "Description cannot be empty.";
                return RedirectToPage();
            }

            using var con = new MySqlConnection(connectionString);
            con.Open();

            MySqlCommand cmd;

            if (ComplaintID == null)
            {
                // Automatically use the logged-in UserID, and Status = Pending
                cmd = new MySqlCommand(
                    "INSERT INTO complaint (UserID, Description, Date, Status) VALUES (@uid, @desc, NOW(), 'Pending')",
                    con);
            }
            else
            {
                cmd = new MySqlCommand(
                    "UPDATE complaint SET Description=@desc WHERE ComplaintID=@id AND UserID=@uid",
                    con);
                cmd.Parameters.AddWithValue("@id", ComplaintID);
            }

            cmd.Parameters.AddWithValue("@uid", uid);
            cmd.Parameters.AddWithValue("@desc", Description);
            cmd.ExecuteNonQuery();

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int ComplaintID)
        {
            int? uid = HttpContext.Session.GetInt32("UserID");
            if (uid == null) return RedirectToPage("/Login");

            using var con = new MySqlConnection(connectionString);
            con.Open();

            var cmd = new MySqlCommand("DELETE FROM complaint WHERE ComplaintID=@id AND UserID=@uid", con);
            cmd.Parameters.AddWithValue("@id", ComplaintID);
            cmd.Parameters.AddWithValue("@uid", uid);
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
            public string Description { get; set; }
            public string Date { get; set; }
            public string Status { get; set; }
        }
    }
}
