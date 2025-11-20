using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HOMEnitor.Pages
{
    public class CashierDashboardModel : PageModel
    {
        public decimal TotalPayments { get; set; }
        public decimal TotalAmenities { get; set; }
        public decimal TotalPenalties { get; set; }
        public int TransactionsThisMonth { get; set; }
        public List<Transaction> RecentTransactions { get; set; } = new List<Transaction>();

        public void OnGet()
        {
            var transactions = new List<Transaction>
            {
                new Transaction { TransactionId = 1, TransactionType = "Housing Payment", HomeownerName = "John Doe", Amount = 2500m, Date = DateTime.Now.AddDays(-3) },
                new Transaction { TransactionId = 2, TransactionType = "Amenity Payment", HomeownerName = "Jane Smith", Amount = 300m, Date = DateTime.Now.AddDays(-5) },
                new Transaction { TransactionId = 3, TransactionType = "Penalty Payment", HomeownerName = "Carlos Reyes", Amount = 150m, Date = DateTime.Now.AddDays(-10) },
                new Transaction { TransactionId = 4, TransactionType = "Housing Payment", HomeownerName = "Emily Cruz", Amount = 2800m, Date = DateTime.Now.AddDays(-12) },
                new Transaction { TransactionId = 5, TransactionType = "Amenity Payment", HomeownerName = "Mark Velasquez", Amount = 400m, Date = DateTime.Now.AddDays(-1) }
            };

            // Compute summary
            TotalPayments = transactions.Where(t => t.TransactionType == "Housing Payment").Sum(t => t.Amount);
            TotalAmenities = transactions.Where(t => t.TransactionType == "Amenity Payment").Sum(t => t.Amount);
            TotalPenalties = transactions.Where(t => t.TransactionType == "Penalty Payment").Sum(t => t.Amount);
            TransactionsThisMonth = transactions.Count(t => t.Date.Month == DateTime.Now.Month);

            // Show last 5 transactions
            RecentTransactions = transactions
                .OrderByDescending(t => t.Date)
                .Take(5)
                .ToList();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }
    }


    public class Transaction
    {
        public int TransactionId { get; set; }
        public string TransactionType { get; set; }
        public string HomeownerName { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
