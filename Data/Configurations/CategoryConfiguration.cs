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
                .HasColumnName("category_id")
                .ValueGeneratedOnAdd();

            builder.Property(c => c.CategoryName)
                .HasColumnName("category_name")
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(c => c.Description)
                .HasColumnName("description")
                .HasMaxLength(500);

            //  Relationships
            // many-to-many Books <-> Categories duoc cau hinh trong BookConfiguration
        }
    }
}