# Library Management System

## Used Package
- Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
- Microsoft.EntityFrameworkCore.Design --version 8.0.0
- Microsoft.EntityFrameworkCore.Tools --version 8.0.0

## endpoint xay dung
**Auth**
| Method	| Endpoint		|  Description	|
| --------- | :-----------:	| -----------:	|
|	Post	| /auth/register | Dang ky tai khoan |
|	Post	| /auth/login	| Dang nhap tai khoan |
|	Post	| /auth/logout	|	Dang xuat	|

**User**
| Method	| Endpoint		|  Description	|
| --------- | :-----------:	| -----------:	|
|	Get		| /users | Danh sach nguoi dung (Staff, Admin) |
|	Get		| /users/:id	| Xem chi tiet nguoi dung |
|	Put		| /users/:id	|	Cap nhat thong tin	|
|	Patch	| /users/:id/card-status |	khoa / mo the thu vien |
|	Get		| /users/:id/borrow-history | Lich su muon cua user	|
|	Post	| /users/staff	|	Them Staff (Admin) |

**Book**
| Method	| Endpoint		|  Description	|
| --------- | :-----------:	| -----------:	|
|	Get		| /books		| Tim kiem sach |
|	Get		| /books/:id	| Chi tiet sach |
|	Post	| /books		|	Them sach moi (Admin)	|
|	PUT		| /books/:id	| Cap nhat thong tin sach (Admin)	|
|	Delete	| /Books/:id	| Xoa chi tiet sach			|

**Categories / Author / Publisher**
| Method | Endpoint        | Description                      |
| ------ | :------------: | ---------------: |
| GET    | /categories     | Danh sach danh muc           |
| GET    | /categories/:id | Chi tiet danh muc                |
| POST   | /categories     | Them danh muc (Admin, Staff)     |
| PUT    | /categories/:id | Cap nhat danh muc (Admin, Staff) |
| DELETE | /categories/:id | Xoa danh muc (Admin)             |

| Method | Endpoint     | Description                     |
| ------ | :------------: | --------------------: |
| GET    | /authors     | Danh sach tac gia               |
| GET    | /authors/:id | Chi tiet tac gia                |
| POST   | /authors     | Them tac gia (Admin, Staff)     |
| PUT    | /authors/:id | Cap nhat tac gia (Admin, Staff) |
| DELETE | /authors/:id | Xoa tac gia (Admin)             |

| Method | Endpoint        | Description      |
| ------ | :-------------: | ---------------:	 |
| GET    | /publishers     | Danh sach NXB       |
| GET    | /publishers/:id | Chi tiet NXB        |
| POST   | /publishers     | Them NXB (Admin, Staff)     |
| PUT    | /publishers/:id | Cap nhat NXB (Admin, Staff) |
| DELETE | /publishers/:id | Xoa NXB (Admin)             |


**Borrow / Return**
| Method	| Endpoint		|  Description	|
| --------- | :-----------:	| -----------:	|
|	Get		| /borrow-records | Danh sach phieu muon (Admin/ Staff) |
|	Get	| /borrow-records/:id	| Chi tiet phieu muon (Admin/Staff) |
|	Post	| /borrow-record	|	Tao phieu muon moi	(Admin/Staff) |
|	Patch	| /borrow-records/:id/approve	|	Staff duyet phieu muon (Admin/Staff)	|
|	Patch	| /borrow-records/:id/return	|	Xac nhan tra sach	(Admin/Staff)	|
|	Patch	| /borrow-records/:id/cancel	|	Huy phieu muon	(Admin/Staff)	|
|	Patch	| /borrow-records/:id/extend	|   Gia han sach	|

**Fine**
| Method	| Endpoint		|  Description	|
| --------- | :-----------:	| -----------:	|
|	Get		| /fines		| Danh sach phieu phat (Admin/Staff) |
|	Get		| /fines/:id	|  Chi tiet phieu phat	(Admin/Staff) |
|	Patch	| /fines/:id/pay	|	Xac nhan da thanh toan (Admin/Staff)	|

**Reservation**
| Method	| Endpoint		|  Description	|
| --------- | :-----------:	| -----------:	|
|	Get		| /reservations		| Danh sach dat truoc (User) |
|	Post	| /reservations	|  Dat truoc sach (User) |
|	Patch	| /reservations/:id/cancel	|	Huy dat truoc (User)	|
|	Patch	| /reservations/:id/complete|	Dat thanh cong => chuyen sang phieu muon |

**Notifications & Logs**
| Method	| Endpoint		|  Description	|
| --------- | :-----------:	| -----------:	|
|	Get		| /notifications		| Thong bao cua User |
|	Patch	| /notifications/read-all	|	Doc tat ca	|

**statistics**
| Method	| Endpoint		|  Description	|
| --------- | :-----------:	| -----------:	|
|	Get		| /stats/overview			| Tong quan sach, phieu muon, user (Admin) |
|	Get		| /stats/overdue			|  Danh sach phieu muon qua han (Admin) |
|	Get		| /stats/top-books	|	Sach duoc muon nhieu nhat (Admin)	|

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
