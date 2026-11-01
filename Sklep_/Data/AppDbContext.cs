using Microsoft.EntityFrameworkCore;
using Sklep_.Models.Stock;
using Sklep_.Models;
using Sklep_.Models.Descriptions;
using Sklep_.Models.Sales;
using Sklep_.Models.Vinted;

namespace Sklep_.Data;

public class AppDbContext : DbContext
{
    public DbSet<Design> Designs { get; set; }
    public DbSet<Longsleeve> Longsleeves { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<VintedListing> VintedListings { get; set; }
    public DbSet<GeneratedDescription> GeneratedDescriptions { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Longsleeve>()
            .HasIndex(l => new { l.Color, l.Size })
            .IsUnique();
        modelBuilder.Entity<Sale>()
            .HasOne(s => s.Design)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Sale>()
            .HasOne(s => s.Longsleeve)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<VintedListing>()
            .HasIndex(l => l.VintedItemId)
            .IsUnique();
    }
}