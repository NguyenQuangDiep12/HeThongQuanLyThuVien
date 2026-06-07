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
                .HasColumnName("user_id")
                .ValueGeneratedOnAdd();

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
                .HasColumnName("full_name")
                .IsRequired()
                .HasMaxLength(255)
                .UseCollation("Vietnamese_CI_AS"); // ho tro sap xep tieng Viet

            builder.Property(u => u.PasswordHash)
                .HasColumnName("password_hash")
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.Phone)
                .HasColumnName("phone")
                .HasMaxLength(20);

            builder.Property(u => u.Address)
                .HasColumnName("address")
                .HasMaxLength(500);

            builder.Property(u => u.AvatarUrl)
                .HasColumnName("avatar_url")
                .HasMaxLength(500);

            builder.Property(u => u.Status)
                .HasColumnName("status")
                .IsRequired()
                .HasMaxLength(40)
                .HasConversion<string>()
                .HasDefaultValue(UserStatus.ACTIVE);

            builder.Property(e => e.ResetOpt)
                .HasColumnName("reset_otp")
                .HasMaxLength(10)
                .IsRequired(false);

            builder.Property(e => e.ResetOtpExpiry)
                .HasColumnName("reset_otp_expiry")
                .HasColumnType("datetime2")
                .IsRequired(false);

            builder.Property(u => u.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(u => u.UpdatedAt)
                .HasColumnName("updated_at");

            // Relationships 

            // User belongs to one Role
            builder.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // User has one LibraryCard
            builder.HasOne(u => u.LibraryCard)
                .WithOne(lc => lc.User)
                .HasForeignKey<LibraryCard>(lc => lc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User has many BorrowRecords (as reader)
            builder.HasMany(u => u.BorrowRecords)
                .WithOne(br => br.Reader)
                .HasForeignKey(br => br.ReaderId)
                .OnDelete(DeleteBehavior.Restrict);

            // User has many Reservations
            builder.HasMany(u => u.Reservations)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User has many Notifications
            builder.HasMany(u => u.Notifications)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}