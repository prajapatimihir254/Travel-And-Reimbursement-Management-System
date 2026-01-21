using Microsoft.EntityFrameworkCore;
using BizTravel.Models;

namespace BizTravel.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions <ApplicationDbContext> options) : base(options) { }
        
        //To join sql User to C#
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<TravelRequest> TravelRequest { get; set; }
    }
}
