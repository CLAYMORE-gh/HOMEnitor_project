using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace HOMEnitor.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        private readonly string connectionString = "server=127.0.0.1;user=root;password=;database=homenitor_db;";

        public void OnGet()
        {
            // Clear previous sessions when visiting login page
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

            // UPDATED QUERY: Use Username instead of UserID
            var cmd = new MySqlCommand("SELECT UserID, UserType, Password FROM user WHERE Username = @uname", con);
            cmd.Parameters.AddWithValue("@uname", Username);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                string dbPassword = reader.GetString("Password");
                string userType = reader.GetString("UserType");
                int dbUserID = reader.GetInt32("UserID");

                // Compare password
                if (dbPassword == Password)
                {
                    // Store session
                    HttpContext.Session.SetInt32("UserID", dbUserID);
                    HttpContext.Session.SetString("UserType", userType);
                    HttpContext.Session.SetString("Username", Username);

                    // Redirect based on user type
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

            // If not found or password mismatch
            ErrorMessage = "Invalid Username or Password.";
            return Page();
        }
    }
}
