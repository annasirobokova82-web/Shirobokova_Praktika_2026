using Microsoft.EntityFrameworkCore;
using ShirobokovaPartnerLib.Models;

namespace ShirobokovaPartnerLib.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Partner> Partners { get; set; }
        public DbSet<PartnerType> PartnerTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("app");

            // Partner
            modelBuilder.Entity<Partner>(entity =>
            {
                entity.ToTable("shirobokova_partners");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.PartnerTypeId).HasColumnName("partner_type_id");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Rating).HasColumnName("rating");
                entity.Property(e => e.Address).HasColumnName("address");
                entity.Property(e => e.DirectorName).HasColumnName("director_name");
                entity.Property(e => e.Phone).HasColumnName("phone");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasOne(p => p.PartnerType)
                    .WithMany(pt => pt.Partners)
                    .HasForeignKey(p => p.PartnerTypeId);
            });

            // PartnerType
            modelBuilder.Entity<PartnerType>(entity =>
            {
                entity.ToTable("shirobokova_partner_types");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name");
            });

            // Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("shirobokova_products");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Price).HasColumnName("price");
            });

            // Sale
            modelBuilder.Entity<Sale>(entity =>
            {
                entity.ToTable("shirobokova_sales");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.PartnerId).HasColumnName("partner_id");
                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.Quantity).HasColumnName("quantity");
                entity.Property(e => e.SaleDate).HasColumnName("sale_date");
                entity.Property(e => e.TotalAmount).HasColumnName("total_amount");
            });
        }
    }
}