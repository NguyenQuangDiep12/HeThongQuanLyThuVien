using HeThongQuanLyThuVien.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongQuanLyThuVien.Data.Configurations
{
    public class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.ToTable("authors");

            builder.HasKey(a => a.AuthorId);

            builder.Property(a => a.AuthorId)
                .HasColumnName("author_id");

            builder.Property(a => a.AuthorName)
                .HasColumnName("author_name")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Biography)
                .HasColumnName("biography")
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(a => a.AuthorUrl)
                .HasColumnName("author_url")
                .HasMaxLength(500);

            // Relationships
            builder.HasMany(a => a.Books)
                .WithOne(b => b.Author)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}