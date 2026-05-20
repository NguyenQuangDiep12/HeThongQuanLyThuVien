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
                .HasColumnName("notification_id");

            builder.Property(n => n.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.Property(n => n.Title)
                .HasColumnName("title")
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(n => n.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}