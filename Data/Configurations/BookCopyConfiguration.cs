using HeThongQuanLyThuVien.Models;
using HeThongQuanLyThuVien.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongQuanLyThuVien.Data.Configurations
{
    public class BookCopyConfiguration : IEntityTypeConfiguration<BookCopy>
    {
        public void Configure(EntityTypeBuilder<BookCopy> builder)
        {
            builder.ToTable("book_copies");

            builder.HasKey(bc => bc.CopyId);

            builder.Property(bc => bc.CopyId)
                .HasColumnName("copy_id")
                .ValueGeneratedOnAdd();

            builder.Property(bc => bc.BookId)
                .HasColumnName("book_id")
                .IsRequired();

            builder.Property(bc => bc.Barcode)
                .HasColumnName("barcode")
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(bc => bc.Barcode)
                .IsUnique();

            builder.Property(bc => bc.ShelfLocation)
                .HasColumnName("shelf_location")
                .HasMaxLength(100);

            builder.Property(bc => bc.Status)
                .HasColumnName("status")
                .IsRequired()
                .HasMaxLength(30)
                .HasConversion<string>()
                .HasDefaultValue(BookCopyStatus.AVAILABLE);

            builder.Property(bc => bc.Condition)
                .HasColumnName("condition")
                .IsRequired()
                .HasMaxLength(30)
                .HasConversion<string>()
                .HasDefaultValue(BookCondition.NORMAL);

            builder.Property(bc => bc.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships

            // BookCopy belongs to Book (dinh nghia tren BookConfiguration)

            // BookCopy has many BorrowDetails
            builder.HasMany(bc => bc.BorrowDetails)
                .WithOne(bd => bd.BookCopy)
                .HasForeignKey(bd => bd.CopyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}