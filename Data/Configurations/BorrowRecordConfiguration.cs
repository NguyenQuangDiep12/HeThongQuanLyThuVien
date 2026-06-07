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
                .HasColumnName("borrow_id")
                .ValueGeneratedOnAdd();

            builder.Property(br => br.ReaderId)
                .HasColumnName("reader_id")
                .IsRequired();

            // Nullable: phieu chua duoc duyet thi chua co staff
            builder.Property(br => br.ApprovedBy)
                .HasColumnName("approved_by");

            builder.Property(br => br.BorrowCode)
                .HasColumnName("borrow_code")
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(br => br.BorrowCode)
                .IsUnique();

            builder.Property(br => br.BorrowDate)
                .HasColumnName("borrow_date");

            builder.Property(br => br.DueDate)
                .HasColumnName("due_date")
                .IsRequired();

            builder.Property(br => br.ReturnedDate)
                .HasColumnName("returned_date");

            builder.Property(br => br.ExtensionCount)
                .HasColumnName("extension_count")
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(br => br.BorrowType)
                .HasColumnName("borrow_type")
                .IsRequired()
                .HasMaxLength(30)
                .HasConversion<string>();

            builder.Property(br => br.Status)
                .HasColumnName("status")
                .IsRequired()
                .HasMaxLength(30)
                .HasConversion<string>();

            builder.Property(br => br.ApprovedAt)
                .HasColumnName("approved_at");

            builder.Property(br => br.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships

            // BorrowRecord belongs to Reader (User)
            builder.HasOne(br => br.Reader)
                .WithMany(u => u.BorrowRecords)
                .HasForeignKey(br => br.ReaderId)
                .OnDelete(DeleteBehavior.Restrict);

            // BorrowRecord approved by Staff (User) — no inverse nav de tranh vong lap
            builder.HasOne(br => br.Approver)
                .WithMany()
                .HasForeignKey(br => br.ApprovedBy)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // BorrowRecord has many BorrowDetails
            builder.HasMany(br => br.BorrowDetails)
                .WithOne(bd => bd.BorrowRecord)
                .HasForeignKey(bd => bd.BorrowId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}