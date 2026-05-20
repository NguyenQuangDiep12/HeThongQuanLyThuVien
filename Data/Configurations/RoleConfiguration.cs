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
                .HasColumnName("role_id");

            builder.Property(r => r.RoleName)
                .HasColumnName("role_name")
                .IsRequired()
                .HasConversion<string>();

            builder.Property(r => r.Description)
                .HasColumnName("description")
                .IsRequired()
                .HasMaxLength(255);

            // Relationships
            builder.HasMany(r => r.Users)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}