using Microsoft.EntityFrameworkCore;
using JobIntelPro_API.Models;

namespace JobIntelPro_API
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Jobs> Jobs { get; set; }
        public DbSet<Subscriptions> Subscriptions { get; set; }
    }
}
