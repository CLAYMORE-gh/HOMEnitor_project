using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;


namespace HOMEnitor.Pages
{
    public class AdminUsersModel : PageModel
    {
        private readonly string connectionString = "server=localhost;database=homenitor_db;uid=root;pwd=;";

        public List<UserData> Users { get; set; } = new();

        [BindProperty] public UserData NewUser { get; set; }
        [BindProperty] public UserData EditUser { get; set; }
        [BindProperty] public int DeleteUserID { get; set; }

        public void OnGet() => LoadUsers();

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
        // ADD USER
        public void OnPostAdd()
        {
            using var con = new MySqlConnection(connectionString);
            con.Open();

            string hashedPassword = HashPassword(NewUser.Password);

            string query = @"INSERT INTO user (UserType, Username, Password, DateCreated)
                     VALUES (@UserType, @Username, @Password, @DateCreated)";
            using var cmd = new MySqlCommand(query, con);

            cmd.Parameters.AddWithValue("@UserType", NewUser.UserType);
            cmd.Parameters.AddWithValue("@Username", NewUser.Username);
            cmd.Parameters.AddWithValue("@Password", hashedPassword);
            cmd.Parameters.AddWithValue("@DateCreated", DateTime.Now);

            cmd.ExecuteNonQuery();

            LoadUsers();
        }


        // EDIT USER
        public void OnPostEdit()
        {
            using var con = new MySqlConnection(connectionString);
            con.Open();

            string hashedPassword = HashPassword(EditUser.Password);

            string query = @"UPDATE user 
                     SET UserType=@UserType, Username=@Username, Password=@Password 
                     WHERE UserID=@UserID";

            using var cmd = new MySqlCommand(query, con);

            cmd.Parameters.AddWithValue("@UserID", EditUser.UserID);
            cmd.Parameters.AddWithValue("@UserType", EditUser.UserType);
            cmd.Parameters.AddWithValue("@Username", EditUser.Username);
            cmd.Parameters.AddWithValue("@Password", hashedPassword);

            cmd.ExecuteNonQuery();

            LoadUsers();
        }


        // DELETE USER
        public void OnPostDelete()
        {
            using var con = new MySqlConnection(connectionString);
            con.Open();

            string query = "DELETE FROM user WHERE UserID=@UserID";

            using var cmd = new MySqlCommand(query, con);
            cmd.Parameters.AddWithValue("@UserID", DeleteUserID);
            cmd.ExecuteNonQuery();

            LoadUsers();
        }

        // LOGOUT
        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }

        // LOAD USERS
        private void LoadUsers()
        {
            Users.Clear();
            using var con = new MySqlConnection(connectionString);
            con.Open();

            string query = "SELECT * FROM user ORDER BY UserID ASC";
            using var cmd = new MySqlCommand(query, con);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Users.Add(new UserData
                {
                    UserID = reader.GetInt32("UserID"),
                    UserType = reader.GetString("UserType"),
                    Username = reader.GetString("Username"),
                    Password = reader.GetString("Password"),
                    DateCreated = reader.GetDateTime("DateCreated")
                });
            }
        }

        // USER MODEL
        public class UserData
        {
            public int UserID { get; set; }
            public string UserType { get; set; }
            public string Username { get; set; }   // ✔ Username added
            public string Password { get; set; }
            public DateTime DateCreated { get; set; }
        }
    }
}
