# Library Management System

## Used Package
- Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
- Microsoft.EntityFrameworkCore.Design --version 8.0.0
- Microsoft.EntityFrameworkCore.Tools --version 8.0.0

---

## Cấu trúc dự án

```
HeThongQuanLyThuVien/
├── Controllers/                 # API Controllers (mỗi controller ứng với 1 module nghiệp vụ)
│   ├── AuthController.cs
│   ├── UsersController.cs
│   ├── BooksController.cs
│   ├── BookCopyController.cs
│   ├── AuthorsController.cs
│   ├── CategoriesController.cs
│   ├── PublishersController.cs
│   ├── BorrowRecordsController.cs
│   ├── FinesController.cs
│   ├── ReservationsController.cs
│   ├── NotificationsController.cs
│   └── StatisticsController.cs
│
├── DTOs/                        # Request/Response object, tách theo module nghiệp vụ
│   ├── Auth/
│   ├── Authors/
│   ├── BookCopies/
│   ├── Books/
│   ├── BorrowRecords/
│   ├── Categories/
│   ├── Fines/
│   ├── Notifications/
│   ├── Publishers/
│   ├── Reservations/
│   ├── Statistics/
│   ├── Users/
│   └── Shared/                  # ApiResponse<T>, PaginationRequest/Response, UserInfoResponse
│
├── Data/
│   ├── ApplicationDbContext.cs  # DbContext + seed data (Role, Author, Publisher, Category, Book...)
│   ├── Configurations/          # IEntityTypeConfiguration cho từng entity (Fluent API)
│   └── Migrations/              # EF Core Migrations (sinh tự động qua dotnet ef)
│
├── Exceptions/                  # Custom exception (BadRequest, Conflict, Forbidden, NotFound, Unauthorized, Validation)
│
├── Middlewares/
│   └── GlobalExceptionMiddleware.cs  # Bắt exception toàn cục, trả response JSON thống nhất
│
├── Models/                      # Entity classes ánh xạ tới bảng trong DB
│   ├── User.cs, Role.cs, LibraryCard.cs
│   ├── Book.cs, Author.cs, Publisher.cs, Category.cs, BookCopy.cs
│   ├── BookAuthor.cs, BookCategory.cs        # Bảng trung gian many-to-many
│   ├── BorrowRecord.cs, BorrowDetail.cs
│   ├── Fine.cs, Reservation.cs, Notification.cs
│   └── Enums/                   # Các enum nghiệp vụ (BorrowStatus, BookCopyStatus, FineType...)
│
├── Options/
│   └── LibrarySettings.cs       # Cấu hình nghiệp vụ: số sách tối đa/lượt, hạn mượn, số lần gia hạn, mức phạt/ngày
│
├── Services/
│   ├── Interfaces/              # Hợp đồng (contract) cho từng service
│   └── *.cs                     # Triển khai nghiệp vụ (AuthService, BookService, BorrowRecordService...)
│
├── Properties/
│   └── launchSettings.json
│
├── Program.cs                   # Cấu hình DI, JWT Bearer, CORS, Swagger, Middleware pipeline
├── appsettings.json
├── HeThongQuanLyThuVien.csproj
└── README.md
```

**Luồng xử lý 1 request** đi qua các tầng theo thứ tự:

`Controller` (nhận request, kiểm tra `[Authorize]`) → `Service` (xử lý nghiệp vụ, ném `Exception` tùy ngữ cảnh) → `ApplicationDbContext` (EF Core) → trả `ApiResponse<T>` về client. Nếu có lỗi, `GlobalExceptionMiddleware` bắt và format lại response JSON theo đúng status code.

---

## Sơ đồ thực thể quan hệ (ERD)

> *Ảnh sẽ được đính kèm sau.*

![ERD](./docs/images/erd.png)

Các thực thể chính: `User` — `Role` (1-N), `User` — `LibraryCard` (1-1), `Book` — `Publisher` (N-1), `Book` — `Author` qua `BookAuthor` (N-N), `Book` — `Category` qua `BookCategory` (N-N), `Book` — `BookCopy` (1-N), `BorrowRecord` — `BorrowDetail` (1-N), `BorrowDetail` — `BookCopy` (N-1), `BorrowDetail` — `Fine` (1-N), `User` — `Reservation` (1-N), `Book` — `Reservation` (1-N), `User` — `Notification` (1-N).

---

## Biểu đồ Sequence

> *Ảnh sẽ được đính kèm sau.*

![Sequence Diagram](./docs/images/sequence-diagram.png)

Gợi ý các luồng nên minh họa: đăng nhập (Auth), tạo phiếu mượn (Staff tạo `BorrowRecord` cho `Reader`), gửi & duyệt yêu cầu gia hạn (Reader → Staff → Notification), xác nhận trả sách kèm phát sinh phiếu phạt nếu hư hỏng/mất/quá hạn, đặt trước sách khi hết bản sao khả dụng.

---

## Biểu đồ Activity

> *Ảnh sẽ được đính kèm sau.*

![Activity Diagram](./docs/images/activity-diagram.png)

Gợi ý các luồng nên minh họa: quy trình mượn sách từ lúc Staff kiểm tra điều kiện Reader (thẻ thư viện, phiếu phạt chưa thanh toán, số sách đang mượn) đến khi tạo phiếu mượn thành công; quy trình gia hạn sách (Pending → Approved/Rejected); quy trình trả sách và xử lý tình trạng vật lý của bản sao.

---

---

## Endpoint xây dựng
> *Tất cả endpoint đều nằm dưới base path `/api`. Ký hiệu quyền truy cập: Reader(Owner), Staff, Admin*

<details>
<summary><b>Auth</b></summary>

| Method | Endpoint | Description | Access |
|---|---|---|---|
| POST | /api/auth/register | Đăng ký tài khoản | Public |
| POST | /api/auth/login | Đăng nhập tài khoản | Public |
| PUT | /api/auth/reset-password | Thay đổi mật khẩu | Owner (đã đăng nhập) |
| POST | /api/auth/forgot-password | Quên mật khẩu | Public |
| POST | /api/auth/verify-otp | Xác thực OTP và cấp mật khẩu mới | Public |

</details>
---

<details>
<summary><b>User</b></summary>

| Method | Endpoint | Description | Access |
|---|---|---|---|
| GET | /api/users | Danh sách người dùng | Staff/Admin |
| GET | /api/users/:id | Xem chi tiết người dùng | Owner/Staff/Admin |
| PUT | /api/users/:id | Cập nhật thông tin người dùng | Staff/Admin |
| PUT | /api/users/me/profile | Reader tự cập nhật hồ sơ cá nhân | Owner |
| PATCH | /api/users/:id/card-status | Khóa / mở thẻ thư viện | Admin |
| PATCH | /api/users/:id/status | Khóa / mở tài khoản | Admin |
| POST | /api/users/staff | Thêm nhân viên | Admin |
| GET | /api/users/staffs | Danh sách nhân viên | Admin |
| PUT | /api/users/staff/:id | Cập nhật thông tin nhân viên | Admin |

- Staff cần Admin cập nhật thông tin (qua `PUT /api/users/:id`, không phải tự sửa).
- STAFF không được tự sửa chính mình qua `PUT /api/users/:id` (bị chặn ở `UserService.UpdateUserAsync`).
</details>
---

<details>
<summary><b>Book</b></summary>

| Method | Endpoint | Description | Access |
|---|---|---|---|
| GET | /api/books | Tìm kiếm sách | Public |
| GET | /api/books/:id | Chi tiết sách | Public |
| POST | /api/books | Thêm sách mới | Staff/Admin |
| PUT | /api/books/:id | Cập nhật thông tin sách | Staff/Admin |
| DELETE | /api/books/:id | Xóa đầu sách | Admin |

</details>
---

<details>
<summary><b>BookCopy</b></summary>

| Method | Endpoint | Description | Access |
|---|---|---|---|
| GET | /api/book-copies | Danh sách bản sao sách | Staff/Admin |
| GET | /api/book-copies/:id | Chi tiết cuốn sách vật lý | Staff/Admin |
| POST | /api/book-copies/book/:id | Thêm bản sao sách | Staff/Admin |
| PUT | /api/book-copies/:id | Cập nhật tình trạng bản sao | Staff/Admin |
| PATCH | /api/book-copies/:id/status | Thay đổi trạng thái bản sao | Staff/Admin |
| DELETE | /api/book-copies/:id | Xóa bản sao sách | Admin |

</details>
---

<details>
<summary><b>Categories</b></summary>

| Method | Endpoint | Description | Access |
|---|---|---|---|
| GET | /api/categories | Danh sách danh mục | Public |
| GET | /api/categories/:id | Chi tiết danh mục | Public |
| POST | /api/categories | Thêm danh mục | Staff/Admin |
| PUT | /api/categories/:id | Cập nhật danh mục | Staff/Admin |
| DELETE | /api/categories/:id | Xóa danh mục | Admin |

</details>
---

<details>
<summary><b>Author</b></summary>

| Method | Endpoint | Description | Access |
|---|---|---|---|
| GET | /api/authors | Danh sách tác giả | Public |
| GET | /api/authors/:id | Chi tiết tác giả | Public |
| POST | /api/authors | Thêm tác giả | Staff/Admin |
| PUT | /api/authors/:id | Cập nhật tác giả | Staff/Admin |
| DELETE | /api/authors/:id | Xóa tác giả | Admin |

</details>
---

<details>
<summary><b>Publisher</b></summary>

| Method | Endpoint | Description | Access |
|---|---|---|---|
| GET | /api/publishers | Danh sách nhà xuất bản | Public |
| GET | /api/publishers/:id | Chi tiết nhà xuất bản | Public |
| POST | /api/publishers | Thêm nhà xuất bản | Staff/Admin |
| PUT | /api/publishers/:id | Cập nhật nhà xuất bản | Staff/Admin |
| DELETE | /api/publishers/:id | Xóa nhà xuất bản | Admin |

</details>
---

<details>
<summary><b>Borrow / Return</b></summary>

| Method | Endpoint | Description | Access |
|---|---|---|---|
| GET | /api/borrow-records | Danh sách phiếu mượn (hỗ trợ filter `extensionRequestStatus`) | Staff/Admin |
| GET | /api/users/:id/borrow-records | Lịch sử mượn sách của người dùng | Owner/Staff/Admin |
| GET | /api/borrow-records/:id | Chi tiết phiếu mượn | Owner/Staff/Admin |
| POST | /api/borrow-records/:id/extension-requests | Gửi yêu cầu gia hạn sách | Owner |
| POST | /api/borrow-records | Tạo phiếu mượn mới | Staff/Admin |
| PATCH | /api/borrow-records/:id/return | Xác nhận trả sách | Staff/Admin |
| PATCH | /api/borrow-records/:id/cancel | Hủy phiếu mượn | Staff/Admin |
| PATCH | /api/borrow-records/:id/extend | Duyệt **hoặc** từ chối gia hạn (body `IsApproved` + `Reason` nếu từ chối) | Staff/Admin |

- Yêu cầu gia hạn dùng `BorrowRecord.ExtensionRequestStatus` (`NONE/PENDING/APPROVED/REJECTED`) 
thay vì bảng Notification — Staff/Admin lọc danh sách chờ duyệt bằng `GET /api/borrow-records?extensionRequestStatus=PENDING`.
- Không có endpoint `reject-extension` riêng; việc duyệt và từ chối đều xử lý chung tại `PATCH /api/borrow-records/:id/extend` thông qua `ProcessExtensionRequest.IsApproved`.

</details>
---

<details>
<summary><b>Fine</b></summary>

| Method | Endpoint | Description | Access |
|---|---|---|---|
| GET | /api/fines | Danh sách phiếu phạt | Staff/Admin |
| POST | /api/fines | Tạo phiếu phạt | Staff/Admin |
| GET | /api/users/:id/fines | Theo dõi vi phạm của người dùng | Staff/Admin |
| GET | /api/fines/:id | Chi tiết phiếu phạt | Staff/Admin |
| PATCH | /api/fines/:id/pay | Xác nhận đã thanh toán | Staff/Admin |

</details>
---

<details>
<summary><b>Reservation</b></summary>

| Method | Endpoint | Description | Access |
|---|---|---|---|
| GET | /api/reservations | Danh sách đặt trước | Staff/Admin |
| POST | /api/reservations | Tạo phiếu đặt trước sách cho bạn đọc | Staff/Admin |
| PATCH | /api/reservations/:id/cancel | Hủy đặt trước | Staff/Admin |
| PATCH | /api/reservations/:id/complete | Chuyển phiếu đặt trước thành phiếu mượn | Staff/Admin |

</details>
---

<details>
<summary><b>Notifications</b></summary>

| Method | Endpoint | Description | Access |
|---|---|---|---|
| GET | /api/notifications | Danh sách thông báo | Owner |
| GET | /api/notifications/:id | Xem chi tiết thông báo | Owner |
| PATCH | /api/notifications/read-all | Đánh dấu tất cả thông báo là đã đọc | Owner |
| PATCH | /api/notifications/:id/read | Đánh dấu thông báo chi tiết đã đọc | Owner |

- Notification chỉ phục vụ inbox của Reader (`NotificationService`), không còn dùng làm queue duyệt gia hạn cho Staff.
</details>
---

<details>
<summary><b>Statistics</b></summary>

| Method | Endpoint | Description | Access |
|---|---|---|---|
| GET | /api/statistic/overviews | Thống kê tổng quan sách, phiếu mượn, người dùng | Admin |
| GET | /api/statistic/overdue | Danh sách phiếu mượn quá hạn | Admin |
| GET | /api/statistic/top-books | Danh sách sách được mượn nhiều nhất | Admin |

</details>
---

## Ý nghĩa các trường hệ thống

<details>
<summary>Users Table</summary>

| Field | Type | Description |
|---|---|---|
| UserId | int | Khoa chinh nguoi dung |
| RoleId | int | Vai tro cua nguoi dung |
| Email | varchar(255) | Email dang nhap |
| FullName | nvarchar(255) | Ho va ten |
| PasswordHash | varchar(255) | Mat khau da ma hoa |
| Phone | varchar(20) | So dien thoai |
| Address | nvarchar(500) | Dia chi |
| AvatarUrl | varchar(500) | Anh dai dien nguoi dung |
| Status | varchar(40) | Trang thai tai khoan |
| CreatedAt | datetime | Ngay tao |
| UpdatedAt | datetime | Ngay cap nhat |

Status: `ACTIVE`, `LOCKED`
</details>

<details>
<summary>Roles Table</summary>

| Field | Type | Description |
|---|---|---|
| RoleId | int | Khoa chinh Role |
| RoleName | varchar(50) | Ten vai tro |
| Description | nvarchar(255) | Mo ta vai tro |

RoleName: `READER`, `STAFF`, `ADMIN`
</details>

<details>
<summary>LibraryCard Table</summary>

| Field | Type | Description |
|---|---|---|
| CardId | int | Khoa chinh the |
| UserId | int | Chu so huu the |
| LibraryCardCode | varchar(50) | Ma the thu vien |
| Status | varchar(20) | Trang thai the |
| IssuedAt | datetime | Ngay phat hanh the |
| ExpiredAt | datetime | Ngay het han |

Status: `ACTIVE`, `EXPIRED`, `BLOCKED`
</details>

<details>
<summary>Books Table</summary>

| Field | Type | Description |
|---|---|---|
| BookId | int | Khoa chinh dau sach |
| PublisherId | int | Nha xuat ban cua sach |
| Title | nvarchar(255) | Ten sach |
| ISBN | varchar(50) | Ma ISBN cua sach |
| Language | varchar(50) | Ngon ngu sach |
| Description | nvarchar(max) | Mo ta sach |
| CoverImage | varchar(500) | Anh bia sach |
| CreatedAt | datetime | Ngay tao |
| UpdatedAt | datetime | Ngay cap nhat |
</details>

<details>
<summary>BookCopies Table</summary>

| Field | Type | Description |
|---|---|---|
| CopyId | int | Khoa chinh ban sao |
| BookId | int | Dau sach cua ban sao |
| Barcode | varchar(100) | Ma vach sach |
| ShelfLocation | varchar(100) | Vi tri ke sach |
| Status | varchar(30) | Trang thai sach |
| Condition | varchar(30) | Tinh trang vat ly cua sach |
| CreatedAt | datetime | Ngay tao ban sao |

Status: `AVAILABLE`, `BORROWED`, `RESERVED`, `UNAVAILABLE`

Condition: `NORMAL`, `TORN`, `DAMAGED`, `LOST`
</details>

<details>
<summary>Categories Table</summary>

| Field | Type | Description |
|---|---|---|
| CategoryId | int | Khoa chinh danh muc |
| CategoryName | nvarchar(255) | Ten danh muc |
| Description | nvarchar(500) | Mo ta danh muc |
</details>

<details>
<summary>BookCategories Table</summary>

| Field | Type | Description |
|---|---|---|
| BookId | int | Dau sach |
| CategoryId | int | Danh muc cua sach |
</details>

<details>
<summary>Authors Table</summary>

| Field | Type | Description |
|---|---|---|
| AuthorId | int | Khoa chinh tac gia |
| AuthorName | nvarchar(255) | Ten tac gia |
| Biography | nvarchar(max) | Tieu su tac gia |
| AuthorUrl | varchar(500) | Link thong tin tac gia |
</details>

<details>
<summary>BookAuthors Table</summary>

| Field | Type | Description |
|---|---|---|
| BookId | int | Dau sach |
| AuthorId | int | Tac gia cua sach |
</details>

<details>
<summary>Publishers Table</summary>

| Field | Type | Description |
|---|---|---|
| PublisherId | int | Khoa chinh nha xuat ban |
| PublisherName | nvarchar(255) | Ten nha xuat ban |
| LogoUrl | varchar(500) | Logo nha xuat ban |
</details>

<details>
<summary>BorrowRecords Table</summary>

| Field | Type | Description |
|---|---|---|
| BorrowId | int | Khoa chinh phieu muon |
| ReaderId | int | Nguoi muon sach |
| ApprovedBy | int | Staff duyet phieu |
| BorrowCode | varchar(50) | Ma phieu muon |
| BorrowDate | datetime | Ngay muon |
| DueDate | datetime | Han tra sach |
| ReturnedDate | datetime | Ngay tra sach |
| ExtensionCount | int | So lan gia han |
| ExtensionRequestStatus | varchar(30) | Trang thai yeu cau gia han hien tai |
| BorrowType | varchar(30) | Hinh thuc muon |
| Status | varchar(30) | Trang thai phieu muon |
| ApprovedAt | datetime | Ngay duyet phieu |
| CreatedAt | datetime | Ngay tao phieu |

BorrowType: `READINGONSITE`, `TAKEHOME`

Status: `BORROWING`, `RETURNED`, `OVERDUE`, `CANCELLED`

ExtensionRequestStatus: `NONE`, `PENDING`, `APPROVED`, `REJECTED`
</details>

<details>
<summary>BorrowDetails Table</summary>

| Field | Type | Description |
|---|---|---|
| BorrowDetailId | int | Khoa chinh chi tiet muon |
| BorrowId | int | Phieu muon |
| CopyId | int | Ban sao sach |
| ReturnedAt | datetime | Ngay tra thuc te |
| ItemCondition | varchar(30) | Tinh trang sach khi tra |
| Status | varchar(30) | Trang thai chi tiet muon |

Status: `BORROWING`, `RETURNED`, `LOST`, `DAMAGED`
</details>

<details>
<summary>Reservations Table</summary>

| Field | Type | Description |
|---|---|---|
| ReservationId | int | Khoa chinh dat truoc |
| UserId | int | Nguoi dat sach |
| BookId | int | Sach duoc dat |
| Status | varchar(30) | Trang thai dat truoc |
| CreatedAt | datetime | Ngay tao |
| UpdatedAt | datetime | Ngay cap nhat |

Status: `WAITING`, `COMPLETED`, `CANCELLED`, `EXPIRED`
</details>

<details>
<summary>Fines Table</summary>

| Field | Type | Description |
|---|---|---|
| FineId | int | Khoa chinh phieu phat |
| BorrowDetailId | int | Chi tiet muon bi vi pham |
| Amount | decimal(18,2) | So tien phat |
| Reason | nvarchar(500) | Ly do vi pham |
| FineType | varchar(30) | Loai vi pham |
| PaymentStatus | varchar(30) | Trang thai thanh toan |
| PaidAt | datetime | Ngay thanh toan |
| CreatedAt | datetime | Ngay tao phieu phat |

FineType: `OVERDUE`, `LOST`, `DAMAGED`

PaymentStatus: `PENDING`, `PAID`, `CANCELLED`
</details>

<details>
<summary>Notifications Table</summary>

| Field | Type | Description |
|---|---|---|
| NotificationId | int | Khoa chinh thong bao |
| UserId | int | Nguoi nhan thong bao |
| Type | varchar(30) | Loai thong bao |
| Title | nvarchar(255) | Tieu de thong bao |
| Content | nvarchar(max) | Noi dung thong bao |
| IsRead | bit | Da doc hay chua |
| ReadAt | datetime | Thoi gian doc thong bao |
| CreatedAt | datetime | Ngay tao thong bao |

Type: `EXTENSIONAPPROVED`, `EXTENSIONREJECTED`, `FINECREATED`, `FINEPAID`, `BORROWREMINDER`, `OVERDUEALERT`, `SYSTEMANNOUNCEMENT`
</details>

---

- Ký hiệu (Vai trò): vai trò của người dùng trong API đó là Reader/Staff/Admin.

- Giới thiệu về JWT (Json Web Token): hiện nay là một trong những cách phổ biến và an toàn nhất để bảo vệ API trong ASP.NET Core.
- Nó cho phép xây dựng các hệ thống có khả năng mở rộng, xác thực không trạng thái — tức là máy chủ không phải lưu trữ trong kho dữ liệu.
- Nói ngắn gọn JWT là gì? Nó là cách truyền tải thông tin một cách an toàn và nhỏ gọn giữa client và server.
- Một Token là thẻ mã xác thực của JWT: khi người dùng gửi request đăng nhập vào hệ thống, server sẽ tạo ra mã JWT token và gửi kèm với response về client của người dùng.
- Khi người dùng gửi request tiếp theo, token JWT sẽ được đính kèm vào request và gửi đến server để server xác thực.

- Một JWT token gồm 3 phần Header, Payload, Signature, trong đó:
  - Header: chứa thuật toán và kiểu token
  - Payload: chứa dữ liệu người dùng (claims)
  - Signature: được sử dụng để xác thực mã token
  - Cấu trúc cơ bản của JWT: `Header.Payload.Signature`

- Tại sao lại sử dụng JWT để xác thực hệ thống? JWT có những lợi ích khi sử dụng trong hệ thống bao gồm:
  - Tính không trạng thái (không yêu cầu lưu trữ dữ liệu ở server)
  - Tính an toàn do đã được ký và mã hóa thành token
  - Hoạt động tốt trên các hệ thống mobile và SPA applications

Sau khi đã cài xong package `Microsoft.AspNetCore.Authentication.JwtBearer`, thêm các settings cho JWT trong `appsettings.json`:

```json
{
  "JWT": {
    "Key": "day la khoa bi mat cho jwt token nen thay the bang mot chuoi ky tu khac va bao mat no",
    "Issuer": "Server cua minh",
    "Audience": "May khach cua nguoi dung",
    "ExpireMinutes": 60
  }
}
```

Giải thích chi tiết:
- `Key` => khóa bí mật dùng để ký token
- `Issuer` => người khởi tạo token, trường hợp này là server
- `Audience` => người sử dụng token, trường hợp này là máy khách
- `ExpireMinutes` => thời gian token hết hạn (phút) — đúng tên cấu hình đang được `JwtService` đọc (`_configuration["JWT:ExpireMinutes"]`)

Các lệnh sử dụng:
```
dotnet ef database drop --force   # xoa database da tao tren sqlserver
dotnet ef database update         # ap dung migration
dotnet ef migrations add <Name>   # tao migration moi
```

[Tài liệu tham khảo](https://www.c-sharpcorner.com/article/how-to-implement-jwt-authentication-in-asp-net-core-step-by-step/)