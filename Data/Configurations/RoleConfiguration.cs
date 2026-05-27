using HeThongQuanLyThuVien.Models;
using HeThongQuanLyThuVien.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongQuanLyThuVien.Data.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("roles");

            builder.HasKey(r => r.RoleId);

            builder.Property(r => r.RoleId)
                .HasColumnName("role_id")
                .ValueGeneratedOnAdd();

            builder.Property(r => r.RoleName)
                .HasColumnName("role_name")
                .IsRequired()
                .HasMaxLength(50)
                .HasConversion<string>();

            builder.HasIndex(r => r.RoleName)
                .IsUnique();

            builder.Property(r => r.Description)
                .HasColumnName("description")
                .HasMaxLength(255);
        }
    }
}