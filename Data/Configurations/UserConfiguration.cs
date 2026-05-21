using HeThongQuanLyThuVien.Models;
using HeThongQuanLyThuVien.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongQuanLyThuVien.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(u => u.UserId);

            builder.Property(u => u.UserId)
                .HasColumnName("user_id");

            builder.Property(u => u.RoleId)
                .HasColumnName("role_id")
                .IsRequired();

            builder.Property(u => u.Email)
                .HasColumnName("email")
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.FullName)
                .HasColumnName("fullname")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.PasswordHash)
                .HasColumnName("password_hash")
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.Phone)
                .HasColumnName("phone")
                .IsRequired()
                .HasMaxLength(15);

            builder.Property(u => u.Address)
                .HasColumnName("address")
                .HasMaxLength(200);

            builder.Property(u => u.LibraryCardCode)
                .HasColumnName("library_card_code")
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(u => u.LibraryCardCode)
                .IsUnique();

            builder.Property(u => u.CardStatus)
                .HasColumnName("card_status")
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(CardStatus.PENDING);

            builder.Property(u => u.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("GETUTCDATE()");
            builder.Property(u => u.UpdatedAt)
                .HasColumnName("updated_at");

            // Relationships
            builder.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.BorrowRecords)
                .WithOne(br => br.Reader)
                .HasForeignKey(br => br.ReaderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Reservations)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Notifications)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}