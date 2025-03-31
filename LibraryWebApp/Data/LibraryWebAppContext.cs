using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LibraryWebApp.Models;
using Microsoft.AspNetCore.Identity;

namespace LibraryWebApp.Data
{
    // If you're using the default IdentityUser and IdentityRole classes:
    public class LibraryWebAppContext : IdentityDbContext<IdentityUser>
    {
        public LibraryWebAppContext(DbContextOptions<LibraryWebAppContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Book { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // This needs to be called first

            // Configure the precision for the Price property of the Book entity
            modelBuilder.Entity<Book>()
                .Property(b => b.Price)
                .HasPrecision(18, 2); // Precision and scale set for the Price property

            // The rest of your model configuration goes here
        }
    }
}
