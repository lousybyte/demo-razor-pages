using DemoRazor.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DemoRazor.Contexts
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<EmailQueueData>       EmailQueue { get; set; }
        public DbSet<ProductData>          Products { get; set; }
        public DbSet<ProductsBrandData>    ProductsBrands { get; set; }
        public DbSet<ProductsCategoryData> ProductsCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EmailQueueData>(entity =>
            {
                entity.ToTable("email_queue");

                entity.HasIndex(e => e.Account, "email_queue_account_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasIdentityOptions(null, null, null, 2147483647L, null, null);

                entity.Property(e => e.Account)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("account");

                entity.Property(e => e.ContentHtml)
                    .IsRequired()
                    .HasMaxLength(100000)
                    .HasColumnName("contentHtml")
                    .HasDefaultValueSql("'NO CONTENT'::character varying");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("email")
                    .HasDefaultValueSql("'NO EMAIL'::character varying");

                entity.Property(e => e.Ip)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("ip")
                    .HasDefaultValueSql("'0.0.0.0'::character varying");

                entity.Property(e => e.RequestedOn)
                    .HasColumnName("requested_on")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("subject")
                    .HasDefaultValueSql("'NO SUBJECT'::character varying");

                entity.Property(e => e.UpdatedOn)
                    .HasColumnName("updated_on")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<ProductData>(entity =>
            {
                entity.ToTable("products");

                entity.HasIndex(e => e.Id, "products_id_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.ProductName, "products_product_name_index");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BrandId).HasColumnName("brand_id");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.ListPrice)
                    .HasPrecision(10, 2)
                    .HasColumnName("list_price")
                    .HasDefaultValueSql("0.00");

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("product_name")
                    .HasDefaultValueSql("'PRODUCT_NAME'::character varying");

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("products_products_brands_id_fk");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("products_products_categories_id_fk");
            });

            modelBuilder.Entity<ProductsBrandData>(entity =>
            {
                entity.ToTable("products_brands");

                entity.HasIndex(e => e.Id, "products_brands_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BrandName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("brand_name")
                    .HasDefaultValueSql("'BRAND_NAME'::character varying");
            });

            modelBuilder.Entity<ProductsCategoryData>(entity =>
            {
                entity.ToTable("products_categories");

                entity.HasIndex(e => e.Id, "products_categories_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("category_name")
                    .HasDefaultValueSql("'CATEGORY_NAME'::character varying");
            });

            modelBuilder.Entity<EmailQueueData>().UseXminAsConcurrencyToken();
            modelBuilder.Entity<ProductData>().UseXminAsConcurrencyToken();
            modelBuilder.Entity<ProductsBrandData>().UseXminAsConcurrencyToken();
            modelBuilder.Entity<ProductsCategoryData>().UseXminAsConcurrencyToken();
        }
    }
}
