using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HOMEnitor.Pages
{
    public class UserDashboardModel : PageModel
    {
        public string UserID { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Login");
            }

            UserID = userId;
            return Page();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }
    }
}
