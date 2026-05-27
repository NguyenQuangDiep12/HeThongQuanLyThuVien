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
                .HasColumnName("author_id")
                .ValueGeneratedOnAdd();

            builder.Property(a => a.AuthorName)
                .HasColumnName("author_name")
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(a => a.Biography)
                .HasColumnName("biography")
                .HasColumnType("nvarchar(max)");

            builder.Property(a => a.AuthorUrl)
                .HasColumnName("author_url")
                .HasMaxLength(500);

            // Relationships 
            // many-to-many Books <-> Authors duoc cau hinh trong BookConfiguration
        }
    }
}