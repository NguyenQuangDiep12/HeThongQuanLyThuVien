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
                .HasColumnName("borrow_detail_id");

            builder.Property(bd => bd.BorrowId)
                .HasColumnName("borrow_id")
                .IsRequired();

            builder.Property(bd => bd.BookId)
                .HasColumnName("book_id")
                .IsRequired();

            builder.Property(bd => bd.Quantity)
                .HasColumnName("quantity")
                .IsRequired();

            builder.Property(bd => bd.Status)
                .HasColumnName("item_borrowed_status")
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(ItemBorrowedStatus.BORROWING);

            // Relationships
            builder.HasOne(bd => bd.BorrowRecord)
                .WithMany(br => br.BorrowDetails)
                .HasForeignKey(bd => bd.BorrowId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(bd => bd.Book)
                .WithMany(b => b.BorrowDetails)
                .HasForeignKey(bd => bd.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(bd => bd.Fines)
                .WithOne(f => f.BorrowDetail)
                .HasForeignKey(f => f.BorrowingDetailId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}