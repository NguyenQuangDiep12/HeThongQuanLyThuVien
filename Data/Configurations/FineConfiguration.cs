using HeThongQuanLyThuVien.Models;
using HeThongQuanLyThuVien.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongQuanLyThuVien.Data.Configurations
{
    public class FineConfiguration : IEntityTypeConfiguration<Fine>
    {
        public void Configure(EntityTypeBuilder<Fine> builder)
        {
            builder.ToTable("fines");

            builder.HasKey(f => f.FineId);

            builder.Property(f => f.FineId)
                .HasColumnName("fine_id");

            builder.Property(f => f.BorrowingDetailId)
                .HasColumnName("borrowing_detail_id")
                .IsRequired();

            builder.Property(f => f.Amount)
                .HasColumnName("amount")
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(f => f.Reason)
                .HasColumnName("reason")
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(f => f.PaymentMethod)
                .HasColumnName("payment_method")
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(PaymentMethod.CASH);

            builder.Property(f => f.PaidAt)
                .HasColumnName("paid_at")
                .IsRequired();

            // Relationships
            builder.HasOne(f => f.BorrowDetail)
                .WithMany(bd => bd.Fines)
                .HasForeignKey(f => f.BorrowingDetailId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}