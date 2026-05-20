using HeThongQuanLyThuVien.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongQuanLyThuVien.Data.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("books");

            builder.HasKey(b => b.BookId);

            builder.Property(b => b.BookId)
                .HasColumnName("book_id");

            builder.Property(b => b.CategoryId)
                .HasColumnName("category_id")
                .IsRequired();

            builder.Property(b => b.PublisherId)
                .HasColumnName("publisher_id")
                .IsRequired();

            builder.Property(b => b.AuthorId)
                .HasColumnName("author_id")
                .IsRequired();

            builder.Property(b => b.Title)
                .HasColumnName("title")
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(b => b.ISBN)
                .HasColumnName("isbn")
                .IsRequired()
                .HasMaxLength(13);

            builder.HasIndex(b => b.ISBN)
                .IsUnique();

            builder.Property(b => b.Quantity)
                .HasColumnName("quantity")
                .IsRequired();

            builder.Property(b => b.AvailableQuantity)
                .HasColumnName("available_quantity")
                .IsRequired();

            builder.Property(b => b.Language)
                .HasColumnName("language")
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(b => b.Description)
                .HasColumnName("description")
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(b => b.CoverImage)
                .HasColumnName("cover_image")
                .HasMaxLength(500);

            builder.Property(b => b.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(b => b.UpdatedAt)
                .HasColumnName("updated_at");

            // Relationships
            builder.HasOne(b => b.Category)
                .WithMany(c => c.Books)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Publisher)
                .WithMany(p => p.Books)
                .HasForeignKey(b => b.PublisherId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b => b.Reservations)
                .WithOne(r => r.Book)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b => b.BorrowDetails)
                .WithOne(bd => bd.Book)
                .HasForeignKey(bd => bd.BookId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}