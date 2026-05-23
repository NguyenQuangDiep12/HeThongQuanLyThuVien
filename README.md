# Library Management System

## Used Package
- Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
- Microsoft.EntityFrameworkCore.Design --version 8.0.0
- Microsoft.EntityFrameworkCore.Tools --version 8.0.0

## endpoint xay dung
**Auth**
| Method	| Endpoint		|  Description	|	Access	|
| --------- | :-----------:	| :-----------:	| ---------: |
|	Post	| /auth/register | Dang ky tai khoan  |		public		|
|	Post	| /auth/login	| Dang nhap tai khoan |		public		|

**User**
| Method	| Endpoint		|  Description	|	Access	|
| --------- | :-----------:	| :-----------:	| ---------:	|
|	Get		| /users		| Danh sach nguoi dung | (Staff, Admin) |
|	Get		| /users/:id	| Xem chi tiet nguoi dung | (Owner, STAFF, ADMIN) |
|	Put		| /users/:id	|	Cap nhat thong tin	|	Owner, STAFF		|
|	Patch	| /users/:id/card-status |	khoa / mo the thu vien |	Admin	|
|	Get		| /users/:id/borrow-history | Lich su muon cua user	|	Owner, STAFF, ADMIN		|
|	Post	| /users/staff	|	Them Staff	|		ADMIN		|

**Book**
| Method	| Endpoint		|  Description	|		Access		|	
| --------- | :-----------:	| :-----------:	|	--------------:	|
|	Get		| /books		| Tim kiem sach |	public	|
|	Get		| /books/:id	| Chi tiet sach |	public	|
|	Post	| /books		|	Them sach moi|		Staff,Admin		|
|	PUT		| /books/:id	| Cap nhat thong tin sach	|	 Staff,(Admin)	|
|	Delete	| /Books/:id	| Xoa chi tiet sach			|	Admin		|

**Categories / Author / Publisher**
| Method | Endpoint        | Description |			Access		|
| ------ | :------------: | :---------------: |	--------------:	|
| GET    | /categories     | Danh sach danh muc |	public	|
| GET    | /categories/:id | Chi tiet danh muc  |	public	|
| POST   | /categories     | Them danh muc		|	(Admin, Staff) |
| PUT    | /categories/:id | Cap nhat danh muc	|   (Admin, Staff) |
| DELETE | /categories/:id | Xoa danh muc		|	(Admin)        |

| Method | Endpoint     | Description		|		Access		  |
| ------ | :------------: | :--------------------: |	----------:	|
| GET    | /authors     | Danh sach tac gia |	public				|
| GET    | /authors/:id | Chi tiet tac gia  |	public				|
| POST   | /authors     | Them tac gia		|   (Admin, Staff)		|
| PUT    | /authors/:id | Cap nhat tac gia	|   (Admin, Staff)		|
| DELETE | /authors/:id | Xoa tac gia		|	(Admin)             |

| Method | Endpoint        | Description      |		Access		|
| ------ | :-------------: | :---------------:|	-----------:	|
| GET    | /publishers     | Danh sach NXB    |		public		|
| GET    | /publishers/:id | Chi tiet NXB     |		public		|
| POST   | /publishers     | Them NXB		  | (Admin, Staff)	|
| PUT    | /publishers/:id | Cap nhat NXB     | (Admin, Staff)	|
| DELETE | /publishers/:id | Xoa NXB		  |		 (Admin)	|


**Borrow / Return**
| Method	| Endpoint		|  Description	|		Access		|
| --------- | :-----------:	| :-----------:	|	-------------:	|
|	Get		| /borrow-records | Danh sach phieu muon	| (Admin/ Staff) |
|	Get	| /borrow-records/:id	| Chi tiet phieu muon	| (Owner/Admin/Staff) |
|	Post	| /borrow-record	|	Tao phieu muon moi	|	(Admin/Staff) |
|	Patch	| /borrow-records/:id/approve	|	Staff duyet phieu muon |	(Admin/Staff)	|
|	Patch	| /borrow-records/:id/return	|	Xac nhan tra sach |		(Admin/Staff)	|
|	Patch	| /borrow-records/:id/cancel	|	Huy phieu muon	|	(Owner/Admin/Staff)	|
|	Patch	| /borrow-records/:id/extend	|   Gia han sach	|	(Owner/Admin/Staff)	|

**Fine**
| Method	| Endpoint		|  Description	|		Access		|
| --------- | :-----------:	| :-----------:	| ----------------: |
|	Get		| /fines		| Danh sach phieu phat | (Admin/Staff) |
|	Get		| /fines/:id	|  Chi tiet phieu phat |	(Admin/Staff) |
|	Patch	| /fines/:id/pay	|	Xac nhan da thanh toan (Admin/Staff)	|

**Reservation**
| Method	| Endpoint		|  Description	|		Access		|
| --------- | :-----------:	| :-----------:	| ----------------:	|
|	Get		| /reservations		| Danh sach dat truoc (Staff/Admin) |
|	Post	| /reservations	|  Dat truoc sach|		 (Reader)	|
|	Patch	| /reservations/:id/cancel	|	Huy dat truoc	| (Owner)	|
|	Patch	| /reservations/:id/complete|	chuyen sang phieu muon |	(Staff/Admin)	|

**Notifications**
| Method	| Endpoint		|  Description	|		Access		|
| --------- | :-----------:	| :-----------:	| ----------------:	|
|	Get		| /notifications		| Thong bao |	 (Reader)	|
|	Patch	| /notifications/read-all	|	Danh dau da doc tat ca |	(Reader)	| // chua co truogn IsRead

**statistics**
| Method	| Endpoint		|  Description	|		Access		|
| --------- | :-----------:	| :-----------:	| ----------------:	|
|	Get		| /stats/overview			| Tong quan sach, phieu muon, user (Thong ke tong quan) | (Admin) |
|	Get		| /stats/overdue			|  Danh sach phieu muon qua han	| (Admin) |
|	Get		| /stats/top-books	|	Sach duoc muon nhieu nhat	| (Admin)	|

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

[!tai lieu tham khao](https://www.c-sharpcorner.com/article/how-to-implement-jwt-authentication-in-asp-net-core-step-by-step/)
