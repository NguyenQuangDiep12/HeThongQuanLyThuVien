namespace HeThongQuanLyThuVien.Data.Configurations
{
    using global::HeThongQuanLyThuVien.Models;
    using global::HeThongQuanLyThuVien.Models.Enums;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    namespace HeThongQuanLyThuVien.Data.Configurations
    {
        public class LibraryCardConfiguration : IEntityTypeConfiguration<LibraryCard>
        {
            public void Configure(EntityTypeBuilder<LibraryCard> builder)
            {
                builder.ToTable("library_cards");

                builder.HasKey(lc => lc.CardId);

                builder.Property(lc => lc.CardId)
                    .HasColumnName("card_id")
                    .ValueGeneratedOnAdd();

                builder.Property(lc => lc.UserId)
                    .HasColumnName("user_id")
                    .IsRequired();

                builder.Property(lc => lc.LibraryCardCode)
                    .HasColumnName("library_card_code")
                    .IsRequired()
                    .HasMaxLength(50);

                builder.HasIndex(lc => lc.LibraryCardCode)
                    .IsUnique();

                builder.Property(lc => lc.Status)
                    .HasColumnName("status")
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasConversion<string>()
                    .HasDefaultValue(CardStatus.Active);

                builder.Property(lc => lc.IssuedAt)
                    .HasColumnName("issued_at")
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                builder.Property(lc => lc.ExpiredAt)
                    .HasColumnName("expired_at")
                    .IsRequired();

                // Relationships

                // Moi librarycard thuoc ve 1 user duy nhat da dinh nghia quan he 1-1 trong userconfig
            }
        }
    }
}
