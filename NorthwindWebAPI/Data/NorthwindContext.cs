using Microsoft.EntityFrameworkCore;
using NorthwindWebAPI.Models;

namespace NorthwindWebAPI.Data
{
    public class NorthwindContext : DbContext
    {
        public NorthwindContext(DbContextOptions<NorthwindContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; }
    }
}

