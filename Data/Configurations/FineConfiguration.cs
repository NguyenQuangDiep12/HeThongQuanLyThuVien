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
                .HasColumnName("fine_id")
                .ValueGeneratedOnAdd();

            // Tham chieu den chi tiet muon bi vi pham
            builder.Property(f => f.BorrowDetailId)
                .HasColumnName("borrow_detail_id")
                .IsRequired();

            builder.Property(f => f.Amount)
                .HasColumnName("amount")
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(f => f.Reason)
                .HasColumnName("reason")
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(f => f.FineType)
                .HasColumnName("fine_type")
                .IsRequired()
                .HasMaxLength(30)
                .HasConversion<string>();

            builder.Property(f => f.PaymentStatus)
                .HasColumnName("payment_status")
                .IsRequired()
                .HasMaxLength(30)
                .HasConversion<string>()
                .HasDefaultValue(PaymentStatus.Pending);

            builder.Property(f => f.PaidAt)
                .HasColumnName("paid_at");

            builder.Property(f => f.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships

            // Fine thuoc ve BorrowDetail (Cascade: xoa chi tiet muon thi xoa phieu phat)
            builder.HasOne(f => f.BorrowDetail)
                .WithMany(bd => bd.Fines)
                .HasForeignKey(f => f.BorrowDetailId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}