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
                .HasColumnName("book_id")
                .ValueGeneratedOnAdd();

            builder.Property(b => b.PublisherId)
                .HasColumnName("publisher_id")
                .IsRequired();

            builder.Property(b => b.Title)
                .HasColumnName("title")
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(b => b.ISBN)
                .HasColumnName("isbn")
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(b => b.ISBN)
                .IsUnique();

            builder.Property(b => b.Language)
                .HasColumnName("language")
                .HasMaxLength(50);

            builder.Property(b => b.Description)
                .HasColumnName("description")
                .HasColumnType("nvarchar(max)");

            builder.Property(b => b.CoverImage)
                .HasColumnName("cover_image")
                .HasMaxLength(500);

            builder.Property(b => b.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(b => b.UpdatedAt)
                .HasColumnName("updated_at");

            // Relationships

            builder.HasOne(b => b.Publisher)
                .WithMany(p => p.Books)
                .HasForeignKey(b => b.PublisherId)
                .OnDelete(DeleteBehavior.Restrict);

            // Book -> BookAuthor (explicit junction)
            builder.HasMany(b => b.BookAuthors)
                .WithOne(ba => ba.Book)
                .HasForeignKey(ba => ba.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            // Book -> BookCategory (explicit junction)
            builder.HasMany(b => b.BookCategories)
                .WithOne(bc => bc.Book)
                .HasForeignKey(bc => bc.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(b => b.BookCopies)
                .WithOne(bc => bc.Book)
                .HasForeignKey(bc => bc.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b => b.Reservations)
                .WithOne(r => r.Book)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}