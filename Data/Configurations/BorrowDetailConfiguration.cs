using HeThongQuanLyThuVien.Models;
using HeThongQuanLyThuVien.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongQuanLyThuVien.Data.Configurations
{
    public class BorrowDetailConfiguration : IEntityTypeConfiguration<BorrowDetail>
    {
        public void Configure(EntityTypeBuilder<BorrowDetail> builder)
        {
            builder.ToTable("borrow_details");

            builder.HasKey(bd => bd.BorrowDetailId);

            builder.Property(bd => bd.BorrowDetailId)
                .HasColumnName("borrow_detail_id")
                .ValueGeneratedOnAdd();

            builder.Property(bd => bd.BorrowId)
                .HasColumnName("borrow_id")
                .IsRequired();

            // Tham chieu den ban sao vat ly cu the (BookCopy), khong phai dau sach (Book)
            builder.Property(bd => bd.CopyId)
                .HasColumnName("copy_id")
                .IsRequired();

            builder.Property(bd => bd.ReturnedAt)
                .HasColumnName("returned_at");

            builder.Property(bd => bd.ItemCondition)
                .HasColumnName("item_condition")
                .HasMaxLength(30)
                .HasConversion<string>();

            builder.Property(bd => bd.Status)
                .HasColumnName("status")
                .IsRequired()
                .HasMaxLength(30)
                .HasConversion<string>()
                .HasDefaultValue(BorrowDetailStatus.BORROWING);

            // Relationships

            // BorrowDetail belongs to BorrowRecord (Cascade: xoa phieu muon thi xoa chi tiet)
            builder.HasOne(bd => bd.BorrowRecord)
                .WithMany(br => br.BorrowDetails)
                .HasForeignKey(bd => bd.BorrowId)
                .OnDelete(DeleteBehavior.Cascade);

            // BorrowDetail tham chieu den BookCopy (Restrict: khong xoa ban sao khi con phieu muon)
            builder.HasOne(bd => bd.BookCopy)
                .WithMany(bc => bc.BorrowDetails)
                .HasForeignKey(bd => bd.CopyId)
                .OnDelete(DeleteBehavior.Restrict);

            // BorrowDetail has many Fines
            builder.HasMany(bd => bd.Fines)
                .WithOne(f => f.BorrowDetail)
                .HasForeignKey(f => f.BorrowDetailId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}