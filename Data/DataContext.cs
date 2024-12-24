using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Autsim.Models
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<PecsCard> PecsCards { get; set; }
        public DbSet<PecsImage> PecsImages { get; set; }
        public DbSet<PecsCardExpectedEntry> PecsCardExpectedEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().ToTable("Users");
            modelBuilder.Entity<PecsCard>().ToTable("PecsCards");
            modelBuilder.Entity<PecsImage>().ToTable("PecsImages");
            modelBuilder.Entity<PecsCardExpectedEntry>().ToTable("PecsCardExpectedEntries");

            modelBuilder.Entity<PecsCard>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CreationTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
                
                entity.HasOne(pc => pc.Image)
                    .WithOne(pi => pi.PecsCard)
                    .HasForeignKey<PecsImage>(pi => pi.PecsCardID);

                entity.HasMany(pc => pc.ExpectedEntries)
                    .WithOne(e => e.PecsCard)
                    .HasForeignKey(e => e.PecsCardId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
