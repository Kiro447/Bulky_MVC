using BulkyWeb_Razor_temp.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb_Razor_temp.DbCotnext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Category> Categories { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, DisplayOrder = 1, Name = "Action" },
                new Category { Id = 2, DisplayOrder = 2, Name = "SciFi" },
                new Category { Id = 3, DisplayOrder = 3, Name = "History" }
                );
        }
    }
}
