using HeThongQuanLyThuVien.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongQuanLyThuVien.Data.Configurations
{
    public class PublisherConfiguration : IEntityTypeConfiguration<Publisher>
    {
        public void Configure(EntityTypeBuilder<Publisher> builder)
        {
            builder.ToTable("publishers");

            builder.HasKey(p => p.PublisherId);

            builder.Property(p => p.PublisherId)
                .HasColumnName("publisher_id");

            builder.Property(p => p.PublisherName)
                .HasColumnName("publisher_name")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.LogoUrl)
                .HasColumnName("logo_url")
                .HasMaxLength(500);

            // Relationships
            builder.HasMany(p => p.Books)
                .WithOne(b => b.Publisher)
                .HasForeignKey(b => b.PublisherId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}