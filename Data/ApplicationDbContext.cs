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
        }

    }
}
