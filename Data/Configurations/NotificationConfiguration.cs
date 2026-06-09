using HeThongQuanLyThuVien.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongQuanLyThuVien.Data.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("notifications");

            builder.HasKey(n => n.NotificationId);

            builder.Property(n => n.NotificationId)
                .HasColumnName("notification_id")
                .ValueGeneratedOnAdd();

            builder.Property(n => n.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.Property(n => n.Title)
                .HasColumnName("title")
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(n => n.Content)
                .HasColumnName("content")
                .HasColumnType("nvarchar(max)");

            builder.Property(n => n.Type)
                .HasColumnName("type")
                .IsRequired()
                .HasMaxLength(30)
                .HasConversion<string>();

            builder.Property(n => n.IsRead)
                .HasColumnName("is_read")
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(n => n.ReadAt)
                .HasColumnName("read_at");

            builder.Property(n => n.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships

            // Notification belongs to User
            builder.HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}