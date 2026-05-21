using HeThongQuanLyThuVien.Models;
using HeThongQuanLyThuVien.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongQuanLyThuVien.Data.Configurations
{
    public class BorrowRecordConfiguration : IEntityTypeConfiguration<BorrowRecord>
    {
        public void Configure(EntityTypeBuilder<BorrowRecord> builder)
        {
            builder.ToTable("borrow_records");

            builder.HasKey(br => br.BorrowId);

            builder.Property(br => br.BorrowId)
                .HasColumnName("borrow_id");

            builder.Property(br => br.ReaderId)
                .HasColumnName("reader_id")
                .IsRequired();

            builder.Property(br => br.ApprovedBy)
                .HasColumnName("approved_by");

            builder.Property(br => br.BorrowCode)
                .HasColumnName("borrow_code")
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(br => br.BorrowCode)
                .IsUnique();

            builder.Property(br => br.DueDate)
                .HasColumnName("due_date")
                .IsRequired();

            builder.Property(br => br.Status)
                .HasColumnName("status")
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(BorrowStatus.PENDING);

            // Relationships
            builder.HasOne(br => br.Reader)
                .WithMany(u => u.BorrowRecords)
                .HasForeignKey(br => br.ReaderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(br => br.Approver)
                .WithMany()
                .HasForeignKey(br => br.ApprovedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(br => br.BorrowDetails)
                .WithOne(bd => bd.BorrowRecord)
                .HasForeignKey(bd => bd.BorrowId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}