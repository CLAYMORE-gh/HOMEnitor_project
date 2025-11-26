using HOMEnitor.Models;
using HOMEnitor.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;

namespace HOMEnitor.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public RegisterModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegisterVM Input { get; set; } = new RegisterVM();

        public string ErrorMessage { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please fill all fields.";
                return Page();
            }

            // Username must be unique
            if (_context.Users.Any(u => u.Username == Input.Username))
            {
                ErrorMessage = "Username already exists.";
                return Page();
            }

            // Hash password
            string hashedPassword = HashPassword(Input.Password);

            var newUser = new User
            {
                UserType = Input.UserType,
                Username = Input.Username,
                Password = hashedPassword,
                DateCreated = Input.DateCreated
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            SuccessMessage = "Registration successful!";
            return RedirectToPage("/Login");
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
