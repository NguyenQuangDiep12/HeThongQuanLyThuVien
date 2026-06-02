# Library Management System

## Used Package
- Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
- Microsoft.EntityFrameworkCore.Design --version 8.0.0
- Microsoft.EntityFrameworkCore.Tools --version 8.0.0

---

## endpoint xay dung
> *Ky hieu quyen truy cap: Reader(Owner), Staff, Admin*

<details>
<summary><b>Auth</b></summary>

| Method | Endpoint | Description | Access |
|---|---|---|---|
| POST | /auth/register | Đăng ký tài khoản | Public |
| POST | /auth/login | Đăng nhập tài khoản | Public |
| POST | /auth/reset-password | Thay đổi mật khẩu | Owner |
| POST | /auth/forgot-password | Quên mật khẩu | Public | 
</details>
---

<details>
<summary><b>User</b></summary>

| Method | Endpoint | Description | Access |
|---|---|---|---|
| GET | /users | Danh sách người dùng | Staff/Admin |
| GET | /users/:id | Xem chi tiết người dùng | Owner/Staff/Admin |
| PUT | /users/:id | Cap nhat thong tin nguoi dung | Staff/Admin |
| PUT | /users/me/profile | Reader tự cập nhật hồ sơ cá nhân | Owner |
| PATCH | /users/:id/card-status | Khóa / mở thẻ thư viện | Admin |
| PATCH | /users/:id/status | Khóa / mở tài khoản | Admin |
| POST | /staff | Thêm nhân viên | Admin |
| GET | /staffs | Danh sách nhân viên | Admin |
| PUT | /staff/:id | Cập nhật thông tin nhân viên | Admin |

- staff can admin cap nhat thong tin
</details>
---

<details>
<summary><b>Book</b></summary>

| Method | Endpoint | Description | Access |
|---|---|---|---|
| GET | /books | Tìm kiếm sách | Public |
| GET | /books/:id | Chi tiết sách | Public |
| POST | /book | Thêm sách mới | Staff/Admin |
| PUT | /books/:id | Cập nhật thông tin sách | Staff/Admin |
| DELETE | /books/:id | Xóa đầu sách | Admin |
</details>
---

<details>
<summary><b>BookCopy</b></summary>

| Method | Endpoint | Description | Access |
|---|---|---|---|
| GET | /book-copies | Danh sách bản sao sách | Staff/Admin |
| GET | /book-copies/:id | Chi tiết cuốn sách vật lý | Staff/Admin |
| POST | /book-copies/book/:id | Thêm bản sao sách | Staff/Admin |
| PUT | /book-copies/:id | Cập nhật tình trạng bản sao | Staff/Admin |
| PATCH | /book-copies/:id/status | Thay đổi trạng thái bản sao | Staff/Admin |
| DELETE | /book-copies/:id | Xóa bản sao sách | Admin |
</details>
---

<details>
<summary><b>Categories</b></summary>

| Method | Endpoint | Description | Access |
|---|---|---|---|
| GET | /categories | Danh sách danh mục | Public |
| GET | /categories/:id | Chi tiết danh mục | Public |
| POST | /categories | Thêm danh mục | Staff/Admin |
| PUT | /categories/:id | Cập nhật danh mục | Staff/Admin |
| DELETE | /categories/:id | Xóa danh mục | Admin |
</details>
---

<details>
<summary><b>Author</b></summary>

| Method | Endpoint | Description | Access |
|---|---|---|---|
| GET | /authors | Danh sách tác giả | Public |
| GET | /authors/:id | Chi tiết tác giả | Public |
| POST | /authors | Thêm tác giả | Staff/Admin |
| PUT | /authors/:id | Cập nhật tác giả | Staff/Admin |
| DELETE | /authors/:id | Xóa tác giả | Admin |
</details>
---

<details>
<summary><b>Publisher</b></summary>

| Method | Endpoint | Description | Access |
|---|---|---|---|
| GET | /publishers | Danh sách nhà xuất bản | Public |
| GET | /publishers/:id | Chi tiết nhà xuất bản | Public |
| POST | /publishers | Thêm nhà xuất bản | Staff/Admin |
| PUT | /publishers/:id | Cập nhật nhà xuất bản | Staff/Admin |
| DELETE | /publishers/:id | Xóa nhà xuất bản | Admin |
</details>
---


<details>
<summary><b>Borrow / Return</b></summary>

| Method | Endpoint | Description | Access |
|---|---|---|---|
| GET | /borrow-records | Danh sách phiếu mượn | Staff/Admin |
| GET | /users/:id/borrow-records | Lịch sử mượn sách của người dùng | Owner/Staff/Admin |
| GET | /borrow-records/:id | Chi tiết phiếu mượn | Owner/Staff/Admin |
| POST | /borrow-records/:id/extension-requests | Gửi yêu cầu gia hạn sách | Owner |
| POST | /borrow-records | Tạo phiếu mượn mới | Staff/Admin |
| PATCH | /borrow-records/:id/return | Xác nhận trả sách | Staff/Admin |
| PATCH | /borrow-records/:id/cancel | Hủy phiếu mượn | Owner/Staff/Admin |
| PATCH | /borrow-records/:id/extend | Gia hạn sách | Staff/Admin |
</details>
---


<details>
<summary><b>Fine</b></summary>

| Method | Endpoint | Description | Access |
|---|---|---|---|
| GET | /fines | Danh sách phiếu phạt | Staff/Admin |
| POST | /fines | Tạo phiếu phạt | Staff/Admin |
| GET | /users/:id/fines | Theo dõi vi phạm của người dùng | Staff/Admin |
| GET | /fines/:id | Chi tiết phiếu phạt | Staff/Admin |
| PATCH | /fines/:id/pay | Xác nhận đã thanh toán | Staff/Admin |
</details>
---

<details>
<summary><b>Reservation</b></summary>

| Method | Endpoint | Description | Access |
|---|---|---|---|
| GET | /reservations | Danh sách đặt trước | Staff/Admin |
| POST | /reservations | Tạo phiếu đặt trước sách cho bạn đọc | Staff/Admin |
| PATCH | /reservations/:id/cancel | Hủy đặt trước | Staff/Admin |
| PATCH | /reservations/:id/complete | Chuyển phiếu đặt trước thành phiếu mượn | Staff/Admin |
</details>
---

<details>
<summary><b>Notifications</b></summary>

| Method | Endpoint | Description | Access |
|---|---|---|---|
| GET | /notifications | Danh sách thông báo | Owner |
| GET | /notifications/:id | xem chi tiết thông báo | Owner | 
| PATCH | /notifications/read-all | Đánh dấu tất cả thông báo là đã đọc | Owner |
</details>
---


<details>
<summary><b>Statistics</b></summary>

| Method | Endpoint | Description | Access |
|---|---|---|---|
| GET | /stats/overview | Thống kê tổng quan sách, phiếu mượn, người dùng | Admin |
| GET | /stats/overdue | Danh sách phiếu mượn quá hạn | Admin |
| GET | /stats/top-books | Danh sách sách được mượn nhiều nhất | Admin |
</details>
---


## Y nghia cac truong he thong
```
<details>

<summary> Users Table </summary>
| Field | Type | Description |
|---|---|---|
| UserId | int | Khoa chinh nguoi dung |
| RoleId | int | Vai tro cua nguoi dung |
| Email | varchar(255) | Email dang nhap |
| FullName | nvarchar(255) | Ho va ten |
| Password_Hash | varchar(255) | Mat khau da ma hoa |
| Phone | varchar(20) | So dien thoai |
| Address | nvarchar(500) | Dia chi |
| AvatarUrl | varchar(500) | Anh dai dien nguoi dung |
| Status | varchar(40) | Trang thai tai khoan |
| CreatedAt | datetime | Ngay tao |
| UpdatedAt | datetime | Ngay cap nhat |

Status: Active, Locked

<details>
```

```
<details>
<summary> Roles Table </summary>
| Field | Type | Description |
|---|---|---|
| RoleId | int | Khoa chinh Role |
| RoleName | varchar(50) | Ten vai tro |
| Description | nvarchar(255) | Mo ta vai tro |

RoleName: READER, STAFF, ADMIN
</details>
```

```
<details>

<summary> LibraryCard Table </summary>
| Field | Type | Description |
|---|---|---|
| CardId | int | Khoa chinh the |
| UserId | int | Chu so huu the |
| LibraryCardCode | varchar(50) | Ma the thu vien |
| Status | varchar(20) | Trang thai the |
| IssuedAt | datetime | Ngay phat hanh the |
| ExpiredAt | datetime | Ngay het han |

Status: Active, Expired, Blocked

</details>
```

```
<details>

<summary> Books Table </summary>
| Field | Type | Description |
|---|---|---|
| BookId | int | Khoa chinh dau sach |
| PublisherId | int | Nha xuat ban cua sach |
| Title | nvarchar(255) | Ten sach |
| ISBN | varchar(50) | Ma ISBN cua sach |
| Language | varchar(50) | Ngon ngu sach |
| Description | nvarchar(max) | Mo ta sach |
| CoverImage | varchar(500) | Anh bia sach |
| AvailabilityCopies | int | So luong ban sao co san |
| CreatedAt | datetime | Ngay tao |
| UpdatedAt | datetime | Ngay cap nhat |

</details>
```

```
<details>

<summary> BookCopies Table </summary>
| Field | Type | Description |
|---|---|---|
| CopyId | int | Khoa chinh ban sao |
| BookId | int | Dau sach cua ban sao |
| Barcode | varchar(100) | Ma vach sach |
| ShelfLocation | varchar(100) | Vi tri ke sach |
| Status | varchar(30) | Trang thai sach |
| Condition | varchar(30) | Tinh trang vat ly cua sach |
| IsReferenceOnly | bit | Chi duoc doc tai cho |
| CreatedAt | datetime | Ngay tao ban sao |

Status: Available, Borrowed, Reserved, Lost, Damaged

Condition: Normal, Torn, Damaged

</details>
```

```
<details>

<summary> Categories Table </summary>
| Field | Type | Description |
|---|---|---|
| CategoryId | int | Khoa chinh danh muc |
| CategoryName | nvarchar(255) | Ten danh muc |
| Description | nvarchar(500) | Mo ta danh muc |

</details>
```

```
### BookCategories Table
| Field | Type | Description |
|---|---|---|
| BookId | int | Dau sach |
| CategoryId | int | Danh muc cua sach |
```

```
### Authors Table
| Field | Type | Description |
|---|---|---|
| AuthorId | int | Khoa chinh tac gia |
| AuthorName | nvarchar(255) | Ten tac gia |
| Biography | nvarchar(max) | Tieu su tac gia |
| AuthorUrl | varchar(500) | Link thong tin tac gia |

```

```
### BookAuthors Table
| Field | Type | Description |
|---|---|---|
| BookId | int | Dau sach |
| AuthorId | int | Tac gia cua sach |

```

```
### Publishers Table
| Field | Type | Description |
|---|---|---|
| PublisherId | int | Khoa chinh nha xuat ban |
| PublisherName | nvarchar(255) | Ten nha xuat ban |
| LogoURL | varchar(500) | Logo nha xuat ban |

```

```
### BorrowRecords Table
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
| BorrowType | varchar(30) | Hinh thuc muon |
| Status | varchar(30) | Trang thai phieu muon |
| ApprovedAt | datetime | Ngay duyet phieu |
| CreatedAt | datetime | Ngay tao phieu |

BorrowType: ReadingOnSite, TakeHome

Status: Pending, Borrowing, Returned, Overdue, Cancelled

```

```
### BorrowDetails Table
| Field | Type | Description |
|---|---|---|
| BorrowDetailId | int | Khoa chinh chi tiet muon |
| BorrowId | int | Phieu muon |
| CopyId | int | Ban sao sach |
| ReturnedAt | datetime | Ngay tra thuc te |
| ItemCondition | varchar(30) | Tinh trang sach khi tra |
| Status | varchar(30) | Trang thai chi tiet muon |

Status: Borrowing, Returned, Lost, Damaged

```

```
### Reservations Table
| Field | Type | Description |
|---|---|---|
| ReservationId | int | Khoa chinh dat truoc |
| UserId | int | Nguoi dat sach |
| BookId | int | Sach duoc dat |
| ExpiryDate | datetime | Han giu sach |
| Status | varchar(30) | Trang thai dat truoc |
| CreatedAt | datetime | Ngay tao |
| UpdatedAt | datetime | Ngay cap nhat |

Status: Waiting, Ready, Completed, Cancelled, Expired

```

```
### Fines Table
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

FineType: Overdue, Lost, Damaged

PaymentStatus: Pending, Paid, Cancelled

```

```
### Notifications Table
| Field | Type | Description |
|---|---|---|
| NotificationId | int | Khoa chinh thong bao |
| UserId | int | Nguoi nhan thong bao |
| Title | nvarchar(255) | Tieu de thong bao |
| Content | nvarchar(max) | Noi dung thong bao |
| IsRead | bit | Da doc hay chua |
| ReadAt | datetime | Thoi gian doc thong bao |

```


- Ky hieu (Vai tro): vai tro cua nguoi dung trong api do Reader/Staff/Admin

- gioi thieu ve Jwt (Json web token): hien nay la mot trong nhung cach pho bien va an toan nhat de bao ve api trong asp.net core.
- no cho phep xay dung cac he thong co kha nang mo rong, xac thuc khong trang thai tuc la may chu khong phai luu tru trong kho du lieu
- noi ngan gon Jwt la gi?, no la truyen tai thong tin mot cach an toan va nho gon giua client va server
- mot Token la the ma xac thuc cua jwt, khi nguoi dung tao mot request dang nhap vao he thong se tao ra ma jwt token va gui kem voi response ve client cua nguoi dung
- khi nguoi dung gui request tiep theo token jwt se duoc dinh kem vao request va gui den sever de server xac thuc.

- mot jwt token gom 3 phan Header, Payload, Signature trong do: 
- Header:  Chua thuat toan va kieu token
- Payload: Chua du lieu nguoi dung (claims)
- Signature: duoc su dung de xac thuc ma token
- Cau truc co ban cua JWT: Header.Payload.Signature

- Tai sao lai su dung Jwt de xac thuc he thong? Jwt co nhung loi ich khi su dung trong he thong bao gom:
- Tinh khong trang thai (khong yeu cau luu tru du lieu o server)
- Tinh an toan do da duoc ky va ma hoa thanh token
- no hoat dong tot tren cac he thong mobile va spa applications

Sau khi da cai xong Package Microsoft.AspNetCore.JwtBearer 
- them cac settings cho jwt trong application.json
{
	"JWT":{
		"key": "day la khoa bi mat cho jwt token nen thay the bang mot chuoi ky tu khac va bao mat no",
		"Issuer": "Server cua minh",
		"Audience": "May khach cua nguoi dung",
		"DurationInMinutes": 60
	}
}

- Giai thich chi tiet
- Key => khoa bi mat dung de ky token
- Issuer => Nguoi khoi tao token, truong hop nay la server
- Audience => Nguoi su dung token, truong hop nay la may khach
- DurationInMinutes => Thoi gian token het han


- Hinh minh qua quy trinh jwt hoat dong trong api

- Cac lenh su dung:
dotnet ef database drop --force: xoa database da tao tren sqlserver




[!tai lieu tham khao](https://www.c-sharpcorner.com/article/how-to-implement-jwt-authentication-in-asp-net-core-step-by-step/)
