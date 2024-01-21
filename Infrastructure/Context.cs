using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class Context : DbContext
    {
        public Context()
        {

        }

        public Context(string connectionString) : base(GetOptions(connectionString)) { }

        public Context(DbContextOptions<Context> options) : base(options) { }

        public static DbContextOptions GetOptions(string connectionString)
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connectionString).Options;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            optionsBuilder.EnableSensitiveDataLogging();

            optionsBuilder.UseSqlServer("conn string");

            // For using localDb
            //optionsBuilder.UseSqlServer("conn string");
            // also optional to add this to make the migrations assembly to "ASP NET template"
            //, b => b.MigrationsAssembly("ASP NET Template");
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Security> Securities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasColumnName("id")
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
                        
            modelBuilder.Entity<Security>()
                .HasOne(s => s.User);
        }
    }
}