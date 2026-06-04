using HeThongQuanLyThuVien.Models;
using Microsoft.EntityFrameworkCore;
using HeThongQuanLyThuVien.Models.Enums;

namespace HeThongQuanLyThuVien.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
            
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Publisher> Publisher { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Fine> Fines { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; }
        public DbSet<BorrowDetail> BorrowDetails { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<LibraryCard> LibraryCards { get; set; }
        public DbSet<BookCopy> BookCopies { get; set; }
        public DbSet<BookAuthor> BookAuthors { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Tu dong quet va ap dung tat ca cac cau hinh IEntityTypeConfiguration trong assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            base.OnModelCreating(modelBuilder);

            // Role Data SEED
            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    RoleId = 1,
                    RoleName = RoleName.READER,
                    Description = "Nguoi doc"
                },
                new Role
                {
                    RoleId = 2,
                    RoleName = RoleName.STAFF,
                    Description = "Thu thu"
                },
                new Role
                {
                    RoleId = 3,
                    RoleName = RoleName.ADMIN,
                    Description = "Quan tri vien"
                });

            // Author DATA SEED 
            modelBuilder.Entity<Author>().HasData(
                new Author
                {
                    AuthorId = 1,
                    AuthorName = "Frederick Forsyth",
                    Biography = "English novelist and journalist.",
                    AuthorUrl = "https://en.wikipedia.org/wiki/Frederick_Forsyth"
                },
                new Author
                {
                    AuthorId = 2,
                    AuthorName = "Nguyễn Nhật Ánh",
                    Biography = "Nhà văn Việt Nam nổi tiếng.",
                    AuthorUrl = "https://vi.wikipedia.org/wiki/Nguyễn_Nhật_Ánh"
                },
                new Author
                {
                    AuthorId = 3,
                    AuthorName = "Tô Hoài",
                    Biography = "Tác giả Dế Mèn Phiêu Lưu Ký.",
                    AuthorUrl = "https://vi.wikipedia.org/wiki/Tô_Hoài"
                },
                new Author
                {
                    AuthorId = 4,
                    AuthorName = "Nam Cao",
                    Biography = "Nhà văn hiện thực Việt Nam.",
                    AuthorUrl = "https://vi.wikipedia.org/wiki/Nam_Cao"
                },
                new Author
                {
                    AuthorId = 5,
                    AuthorName = "J. K. Rowling",
                    Biography = "Tác giả Harry Potter.",
                    AuthorUrl = "https://www.jkrowling.com"
                },
                new Author
                {
                    AuthorId = 6,
                    AuthorName = "Stephen King",
                    Biography = "Nhà văn kinh dị nổi tiếng.",
                    AuthorUrl = "https://stephenking.com"
                }
            );
            // Publisher Data SEED
            modelBuilder.Entity<Publisher>().HasData(
                new Publisher
                {
                    PublisherId = 1,
                    PublisherName = "NXB Trẻ",
                    LogoUrl = "https://www.nxbtre.com.vn"
                },
                new Publisher
                {
                    PublisherId = 2,
                    PublisherName = "NXB Kim Đồng",
                    LogoUrl = "https://www.nxbkimdong.com.vn"
                },
                new Publisher
                {
                    PublisherId = 3,
                    PublisherName = "Bloomsbury Publishing",
                    LogoUrl = "https://www.bloomsbury.com"
                },
                new Publisher
                {
                    PublisherId = 4,
                    PublisherName = "Penguin Random House",
                    LogoUrl = "https://www.penguinrandomhouse.com"
                },
                new Publisher
                {
                    PublisherId = 5,
                    PublisherName = "Scribner",
                    LogoUrl = "https://www.simonandschusterpublishing.com/scribner"
                }
            );
            // Category Data SEED
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    CategoryId = 1,
                    CategoryName = "Tin học",
                    Description = "Khoa học máy tính"
                },
                new Category
                {
                    CategoryId = 2,
                    CategoryName = "Văn học Việt Nam",
                    Description = "Tiểu thuyết, truyện ngắn"
                },
                new Category
                {
                    CategoryId = 3,
                    CategoryName = "Văn học nước ngoài",
                    Description = "Tiểu thuyết nước ngoài"
                },
                new Category
                {
                    CategoryId = 4,
                    CategoryName = "Thiếu nhi",
                    Description = "Sách cho thiếu nhi"
                },
                new Category
                {
                    CategoryId= 5,
                    CategoryName = "Kinh dị",
                    Description = "Truyện kinh dị"
                },
                new Category
                {
                    CategoryId = 6,
                    CategoryName = "Trinh thám",
                    Description = "Tiểu thuyết trinh thám"
                }
            );

            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    BookId = 1,
                    PublisherId = 4,
                    Title = "The Day of the Jackal",
                    ISBN = "9780099559855",
                    Language = "EN",
                    Description = "Political thriller novel by Frederick Forsyth.",
                    CoverImage = "https://example.com/jackal.jpg",
                    CreatedAt = new DateTime(2025, 1, 1)
                },
                new Book
                {
                    BookId = 2,
                    PublisherId = 1,
                    Title = "Mắt Biếc",
                    ISBN = "9786042041234",
                    Language = "VI",
                    Description = "Tiểu thuyết nổi tiếng của Nguyễn Nhật Ánh.",
                    CoverImage = "https://example.com/matbiec.jpg",
                    CreatedAt = new DateTime(2025, 1, 1)
                },
                new Book
                {
                    BookId = 3,
                    PublisherId = 2,
                    Title = "Dế Mèn Phiêu Lưu Ký",
                    ISBN = "9786042088888",
                    Language = "VI",
                    Description = "Tác phẩm thiếu nhi kinh điển.",
                    CoverImage = "https://example.com/demen.jpg",
                    CreatedAt = new DateTime(2025, 1, 1)
                },
                new Book
                {
                    BookId = 4,
                    PublisherId = 1,
                    Title = "Chí Phèo",
                    ISBN = "9786042099999",
                    Language = "VI",
                    Description = "Tác phẩm hiện thực phê phán nổi tiếng.",
                    CoverImage = "https://example.com/chipheo.jpg",
                    CreatedAt = new DateTime(2025, 1, 1)
                },
                new Book
                {
                    BookId = 5,
                    PublisherId = 3,
                    Title = "Harry Potter and the Philosopher's Stone",
                    ISBN = "9780747532699",
                    Language = "EN",
                    Description = "The first Harry Potter novel.",
                    CoverImage = "https://example.com/hp1.jpg",
                    CreatedAt = new DateTime(2025, 1, 1)
                }
            );

            modelBuilder.Entity<BookAuthor>().HasData(
                new BookAuthor
                {
                    BookId = 1,
                    AuthorId = 1
                },
                new BookAuthor
                {
                    BookId = 2,
                    AuthorId = 2
                },
                new BookAuthor
                {
                    BookId = 3,
                    AuthorId = 3
                },
                new BookAuthor
                {
                    BookId = 4,
                    AuthorId = 4
                },
                new BookAuthor
                {
                    BookId = 5,
                    AuthorId = 5
                }
            );

            modelBuilder.Entity<BookCategory>().HasData(
                new BookCategory
                {
                    BookId = 1,
                    CategoryId = 3
                },
                new BookCategory
                {
                    BookId = 1,
                    CategoryId = 6
                },
                new BookCategory
                {
                    BookId = 2,
                    CategoryId = 2
                },
                new BookCategory
                {
                    BookId = 3,
                    CategoryId = 4
                },
                new BookCategory
                {
                    BookId = 4,
                    CategoryId = 2
                },
                new BookCategory
                {
                    BookId = 5,
                    CategoryId = 3
                },
                new BookCategory
                {
                    BookId = 5,
                    CategoryId = 4
                }
            );
        }

    }
}
