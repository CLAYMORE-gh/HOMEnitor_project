using Microsoft.EntityFrameworkCore;
using HOMEnitor.Models;

namespace HOMEnitor.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Homeowner> Homeowners { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
    }
}
