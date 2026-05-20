using HeThongQuanLyThuVien.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongQuanLyThuVien.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("categories");

            builder.HasKey(c => c.CategoryId);

            builder.Property(c => c.CategoryId)
                .HasColumnName("category_id");

            builder.Property(c => c.CategoryName)
                .HasColumnName("category_name")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Description)
                .HasColumnName("description")
                .IsRequired()
                .HasMaxLength(255);

            // Relationships
            builder.HasMany(c => c.Books)
                .WithOne(b => b.Category)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}