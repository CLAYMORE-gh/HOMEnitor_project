using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HOMEnitor.Data;
using System.Linq;

namespace HOMEnitor.Pages
{
    public class AdminDashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public int UserID { get; set; }

        public int TotalHomeowners { get; set; }
        public int TotalUnits { get; set; }
        public int TotalTransactions { get; set; }
        public int PendingComplaints { get; set; }
        public int TotalAmenities { get; set; }

        public void OnGet()
        {
            var userType = HttpContext.Session.GetString("UserType");
            if (string.IsNullOrEmpty(userType))
            {
                Response.Redirect("/Login");
                return;
            }

            if (userType != "Admin" && userType == "Homeowner")
            {
                Response.Redirect("/UserDashboard");
                return;
            }
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }
    }
}
