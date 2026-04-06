using Microsoft.EntityFrameworkCore;
using DemoProductsWebAPI.Domain.Entities;

namespace DemoProductsWebAPI.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<DemoProductsWebAPI.Domain.Entities.ProductCart> ProductCarts { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(b =>
            {
                b.ToTable("Product");
                b.HasKey(p => p.Id);
                b.Property(p => p.ProductName).HasMaxLength(255).IsRequired();
                b.Property(p => p.CreatedBy).HasMaxLength(100).IsRequired();
                b.Property(p => p.CreatedOn).IsRequired();
                b.Property(p => p.ModifiedBy).HasMaxLength(100);
                b.HasIndex(p => p.ProductName).HasDatabaseName("IX_Product_ProductName");
            });

            // Map ProductCart CLR type to legacy Item table for compatibility
            modelBuilder.Entity<DemoProductsWebAPI.Domain.Entities.ProductCart>(b =>
            {
                b.ToTable("Item");
                b.HasKey(pc => pc.Id);
                b.Property(pc => pc.Quantity).IsRequired();
                b.HasOne(pc => pc.Product)
                    .WithMany()
                    .HasForeignKey(pc => pc.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RefreshToken>(b =>
            {
                b.ToTable("RefreshToken");
                b.HasKey(r => r.Id);
                b.Property(r => r.Token).IsRequired();
                b.Property(r => r.UserId).IsRequired();
                b.Property(r => r.ExpiresOn).IsRequired();
            });
        }
    }
}
