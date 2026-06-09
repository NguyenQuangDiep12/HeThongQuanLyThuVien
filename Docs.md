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


# Nhiệm vụ Refactor chức năng Gia hạn sách và Notification

## Bối cảnh dự án

Dự án là hệ thống quản lý thư viện ASP.NET Core + EF Core.

Hiện tại hệ thống đang sử dụng bảng Notification để lưu cả:

* Thông báo cho Reader
* Yêu cầu gia hạn sách của Reader

Cách làm này gây lẫn lộn giữa dữ liệu nghiệp vụ và dữ liệu thông báo.

Đã thống nhất thay đổi thiết kế.

---

# Thiết kế mới

## BorrowRecord

Thêm thuộc tính:

```csharp
public ExtensionRequestStatus ExtensionRequestStatus { get; set; }
```

Enum đã tồn tại:

```csharp
public enum ExtensionRequestStatus
{
    None = 0,
    Pending = 1,
    Approved = 2,
    Rejected = 3
}
```

Ý nghĩa:

* None: chưa từng gửi yêu cầu gia hạn
* Pending: đang chờ Staff/Admin xử lý
* Approved: đã được duyệt
* Rejected: đã bị từ chối

---

## Notification

Notification chỉ dùng để gửi thông báo cho Reader.

KHÔNG sử dụng Notification để lưu workflow gia hạn.

Enum đã tồn tại:

```csharp
public enum NotificationType
{
    ExtensionApproved = 1,
    ExtensionRejected = 2,

    FineCreated = 3,
    FinePaid = 4,

    BorrowReminder = 5,
    OverdueAlert = 6,

    SystemAnnouncement = 7
}
```

Notification vẫn giữ chức năng:

* Reader xem danh sách thông báo
* Reader xem chi tiết thông báo
* Reader đánh dấu đã đọc

---

# Yêu cầu thực hiện

## 1. Entity

Cập nhật BorrowRecord entity:

* Thêm ExtensionRequestStatus
* Mapping EF Core đầy đủ
* Migration database

Nếu BorrowRecordConfiguration tồn tại thì cập nhật configuration.

---

## 2. SubmitExtensionRequestAsync

Refactor phương thức:

```csharp
SubmitExtensionRequestAsync
```

Yêu cầu:

* Kiểm tra BorrowRecord tồn tại
* Kiểm tra Reader sở hữu BorrowRecord
* Gọi ValidateExtensionEligibility()
* Nếu ExtensionRequestStatus == Pending thì throw BadRequestException
* Không tạo Notification
* Không gọi SendToStaffAsync
* Cập nhật:

```csharp
record.ExtensionRequestStatus = ExtensionRequestStatus.Pending;
```

* SaveChangesAsync

---

## 3. ConfirmExtensionAsync

Refactor:

```csharp
ConfirmExtensionAsync
```

Yêu cầu:

* BorrowRecord phải tồn tại
* ExtensionRequestStatus phải là Pending
* ValidateExtensionEligibility()

Gia hạn:

```csharp
record.ExtensionCount += 1;
record.DueDate = record.DueDate.AddDays(_librarySettings.ExtensionDays);
```

Cập nhật:

```csharp
record.ExtensionRequestStatus = ExtensionRequestStatus.Approved;
```

Sau đó tạo Notification cho Reader:

```csharp
Type = NotificationType.ExtensionApproved
```

Title:

```text
Gia hạn sách thành công
```

Content:

```text
Phiếu mượn {BorrowCode} đã được gia hạn đến {DueDate}
```

Lưu notification.

SaveChangesAsync.

---

## 4. Tạo chức năng từ chối gia hạn

Thêm service:

```csharp
RejectExtensionAsync(
    int borrowId,
    string reason,
    int staffId,
    CancellationToken ct)
```

Yêu cầu:

* BorrowRecord phải tồn tại
* ExtensionRequestStatus phải là Pending

Cập nhật:

```csharp
record.ExtensionRequestStatus = ExtensionRequestStatus.Rejected;
```

Tạo Notification:

```csharp
Type = NotificationType.ExtensionRejected
```

Title:

```text
Yêu cầu gia hạn bị từ chối
```

Content:

```text
reason
```

SaveChangesAsync.

---

## 5. API từ chối gia hạn

Thêm endpoint:

```http
PATCH /api/borrow-records/{id}/reject-extension
```

Role:

```text
STAFF, ADMIN
```

Request:

```csharp
public class RejectExtensionRequest
{
    public string Reason { get; set; }
}
```

Controller gọi:

```csharp
RejectExtensionAsync(...)
```

---

## 6. Danh sách yêu cầu gia hạn đang chờ

Không sử dụng Notification.

Cho phép Staff/Admin lọc BorrowRecord theo:

```csharp
ExtensionRequestStatus.Pending
```

Nếu hệ thống đã có BorrowRecordQueryRequest thì bổ sung filter:

```csharp
ExtensionRequestStatus?
```

Để Staff/Admin xem các yêu cầu đang chờ xử lý.

---

## 7. NotificationService

Xóa hoàn toàn logic:

```csharp
SendToStaffAsync(...)
```

nếu nó chỉ phục vụ workflow gia hạn.

Không dùng Notification làm queue cho Staff.

Notification chỉ dùng gửi cho Reader.

---

# Kết quả mong muốn

Workflow cuối cùng:

Reader
→ Gửi yêu cầu gia hạn
→ BorrowRecord.ExtensionRequestStatus = Pending

Staff/Admin
→ Xem danh sách Pending
→ Duyệt hoặc từ chối

Nếu duyệt
→ ExtensionRequestStatus = Approved
→ Notification ExtensionApproved gửi cho Reader

Nếu từ chối
→ ExtensionRequestStatus = Rejected
→ Notification ExtensionRejected gửi cho Reader

Notification không còn lưu trạng thái yêu cầu gia hạn.
Notification chỉ là inbox của Reader.
