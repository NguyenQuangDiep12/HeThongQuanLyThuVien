using HeThongQuanLyThuVien.Models;
using HeThongQuanLyThuVien.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongQuanLyThuVien.Data.Configurations
{
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.ToTable("reservations");

            builder.HasKey(r => r.ReservationId);

            builder.Property(r => r.ReservationId)
                .HasColumnName("reservation_id");

            builder.Property(r => r.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.Property(r => r.BookId)
                .HasColumnName("book_id")
                .IsRequired();

            builder.Property(r => r.ExpiryDate)
                .HasColumnName("expiry_date")
                .IsRequired();

            builder.Property(r => r.Status)
                .HasColumnName("status")
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(ReservationStatus.WAITING);

            builder.Property(r => r.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            builder.Property(r => r.UpdatedAt)
                .HasColumnName("updated_at");

            // Relationships
            builder.HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Book)
                .WithMany(b => b.Reservations)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}