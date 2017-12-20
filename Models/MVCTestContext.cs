using Microsoft.EntityFrameworkCore;

namespace mvcproj.Models
{
    public class MVCTestContext : DbContext
    {
        public MVCTestContext(DbContextOptions<MVCTestContext> options) : base(options) { }
        //
        // Database's tables
        //
        public DbSet<mvcproj.Models.Users> User { get; set; }
    }
}