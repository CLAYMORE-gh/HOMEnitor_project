using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;

namespace HOMEnitor.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        private readonly string connectionString =
            "server=127.0.0.1;user=root;password=;database=homenitor_db;";

        public void OnGet()
        {
            HttpContext.Session.Clear();
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter both Username and Password.";
                return Page();
            }

            using var con = new MySqlConnection(connectionString);
            con.Open();

            // Username-based login
            var cmd = new MySqlCommand(
                "SELECT UserID, UserType, Password FROM user WHERE Username = @uname",
                con
            );
            cmd.Parameters.AddWithValue("@uname", Username);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                string dbPasswordHash = reader.GetString("Password");
                string userType = reader.GetString("UserType");
                int dbUserID = reader.GetInt32("UserID");

                // Hash entered password
                string hashedInputPassword = HashPassword(Password);

                if (dbPasswordHash == hashedInputPassword)
                {
                    HttpContext.Session.SetInt32("UserID", dbUserID);
                    HttpContext.Session.SetString("UserType", userType);
                    HttpContext.Session.SetString("Username", Username);

                    // Redirect based on user role
                    return userType switch
                    {
                        "Admin" => RedirectToPage("/AdminDashboard"),
                        "Cashier" => RedirectToPage("/CashierDashboard"),
                        "Homeowner" => RedirectToPage("/UserDashboard"),
                        "Visitor" => RedirectToPage("/VisitorDashboard"),
                        _ => Page()
                    };
                }
            }

            ErrorMessage = "Invalid Username or Password.";
            return Page();
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
