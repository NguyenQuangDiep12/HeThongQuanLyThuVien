### Reader Use Cases

1. Đăng ký tài khoản
2. Đăng nhập hệ thống
3. Quên mật khẩu
4. Đổi mật khẩu
5. Xem và cập nhật hồ sơ cá nhân
6. Tìm kiếm sách
7. Xem chi tiết sách
8. Xem lịch sử mượn sách
9. Gửi yêu cầu gia hạn sách
10. Xem thông báo
11. Đánh dấu thông báo đã đọc
13. Hủy phiếu mượn (khi chưa được duyệt)

Usecase chung
- Quan ly xac thuc (1,2,3,4) / vai tro (Reader/Staff/Admin)
- Quan ly ho so ca nhan (5) / (Reader)
- Tra cuu va quan ly muon sach (6,7,8,9,11)
- Quan ly thong bao (10,11)

### Staff Use Cases

1. Đăng nhập hệ thống
2. Đổi mật khẩu

--- Quản lý người dùng ---
3. Xem danh sách người dùng
4. Xem chi tiết người dùng
5. Theo dõi lịch sử mượn sách
6. Theo dõi vi phạm và tiền phạt

--- Quản lý đầu sách ---
7. Thêm đầu sách mới
8. Cập nhật thông tin đầu sách
9. Tìm kiếm đầu sách
10. Xem chi tiết đầu sách

--- Quản lý bản sao sách ---
11. Thêm bản sao sách
12. Cập nhật tình trạng sách
13. Xem danh sách bản sao
14. Xem chi tiết bản sao sách

--- Quản lý danh mục ---
15. Thêm danh mục
16. Cập nhật danh mục

--- Quản lý tác giả ---
17. Thêm tác giả
18. Cập nhật tác giả

--- Quản lý nhà xuất bản ---
19. Thêm nhà xuất bản
20. Cập nhật nhà xuất bản

--- Nghiệp vụ mượn trả ---
21. Tạo phiếu mượn
22. Xem danh sách phiếu mượn
23. Xem chi tiết phiếu mượn
24. Xác nhận trả sách
25. Gia hạn sách
26. Duyệt yêu cầu gia hạn
27. Hủy phiếu mượn

--- Quản lý đặt trước ---
28. Tạo phiếu đặt trước
29. Hủy đặt trước
30. Chuyển đặt trước thành phiếu mượn

--- Quản lý vi phạm ---
31. Tạo phiếu phạt
32. Xác nhận thanh toán tiền phạt
33. Xử lý sách mất / hỏng / quá hạn


### Admin Use Cases

1. Đăng nhập hệ thống
2. Đổi mật khẩu

--- Quản lý nhân viên ---
3. Thêm nhân viên
4. Xem danh sách nhân viên
5. Cập nhật thông tin nhân viên

--- Quản lý người dùng ---
6. Xem danh sách người dùng
7. Xem chi tiết người dùng
8. Khóa / mở tài khoản
9. Khóa / mở thẻ thư viện

--- Quản lý đầu sách ---
10. Thêm đầu sách
11. Cập nhật đầu sách
12. Xóa đầu sách

--- Quản lý bản sao sách ---
13. Xóa bản sao sách

--- Quản lý danh mục ---
14. Xóa danh mục

--- Quản lý tác giả ---
15. Xóa tác giả

--- Quản lý nhà xuất bản ---
16. Xóa nhà xuất bản

--- Quản lý phiếu mượn ---
17. Xem danh sách phiếu mượn
18. Xem chi tiết phiếu mượn
19. Xác nhận trả sách
20. Gia hạn sách
21. Hủy phiếu mượn

--- Quản lý vi phạm ---
22. Xem danh sách phiếu phạt
23. Xác nhận thanh toán tiền phạt

--- Thống kê hệ thống ---
24. Xem thống kê tổng quan
25. Xem danh sách quá hạn
26. Xem sách được mượn nhiều nhất


Ti viet ham check thoi gian ReservationStatus trang thai Expiry

-- UserService: Done
-- StatisticsService: Done
-- ReservationService: Done
-- PublisherService: Done
-- MailService: Done
-- JwtService: Done
-- CategoryService: Done
-- AuthorService: Done
-- AuthService: Done

-- BookCopyService: PENDING
== BookService: PENDING
-- FineService: PENDING
-- Notification: PENDING
-- BorrowRecordService: PEDING