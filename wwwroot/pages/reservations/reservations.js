let currentPage = 1;
const pageSize = 10;

$(document).ready(function () {
    if (!checkAuth('STAFF')) return;
    initLayout('Đặt trước');
    loadReservations();
    $('#btnCreate').on('click', function () {
        $('#resUserId').val('');
        $('#resBookId').val('');
        new bootstrap.Modal('#resModal').show();
    });
    $('#btnSaveRes').on('click', saveReservation);
});

function loadReservations() {
    ReservationAPI.getList({ page: currentPage, pageSize: pageSize }, function (err, res) {
        if (err) { showToast('error', err.message); return; }
        const data = res.data;
        let html = '';
        (data.items || []).forEach(function (r) {
            html += '<tr>';
            html += '<td>' + escapeHtml(r.fullName) + '</td>';
            html += '<td>' + escapeHtml(r.title) + '</td>';
            html += '<td>' + formatDate(r.createdAt) + '</td>';
            html += '<td>' + getReservationStatusBadge(r.status) + '</td>';
            html += '<td class="text-center">';
            // FIX: backend ReservationStatus dùng WAITING (không phải PENDING hoặc READY)
            if (r.status === 'WAITING') {
                html += '<button class="btn btn-sm btn-success btn-complete me-1" data-id="' + r.reservationId + '" title="Chuyển thành phiếu mượn"><i class="bi bi-check2-circle me-1"></i>Chuyển mượn</button>';
                html += '<button class="btn btn-sm btn-outline-danger btn-cancel" data-id="' + r.reservationId + '" title="Hủy đặt trước"><i class="bi bi-x"></i></button>';
            } else if (r.status === 'COMPLETED' || r.status === 'CANCELLED' || r.status === 'EXPIRED') {
                html += '<span class="text-muted small">—</span>';
            }
            html += '</td></tr>';
        });
        $('#tableBody').html(html || '<tr><td colspan="5" class="text-center text-muted py-4">Không có đặt trước nào</td></tr>');
        updateShowingCount((data.items || []).length, data.totalRecords || 0);
        renderPagination('pagination', data.totalRecords || 0, currentPage, pageSize, function (p) { currentPage = p; loadReservations(); });

        $('.btn-complete').on('click', function () {
            const resId = $(this).data('id');
            confirmAction('Chuyển thành phiếu mượn', 'Xác nhận chuyển đặt trước thành phiếu mượn? Sách sẽ được giao cho độc giả.', function () {
                ReservationAPI.complete(resId, function (err2, res2) {
                    if (err2) { showToast('error', err2.message); return; }
                    const borrowId = res2 && res2.data && res2.data.BorrowId;
                    showToast('success', 'Chuyển mượn thành công' + (borrowId ? ' — Phiếu #' + borrowId : ''));
                    loadReservations();
                });
            });
        });

        $('.btn-cancel').on('click', function () {
            const resId = $(this).data('id');
            confirmAction('Hủy đặt trước', 'Xác nhận hủy phiếu đặt trước này?', function () {
                ReservationAPI.cancel(resId, function (err2) {
                    if (err2) { showToast('error', err2.message); return; }
                    showToast('success', 'Đã hủy đặt trước');
                    loadReservations();
                });
            });
        });
    });
}

function getReservationStatusBadge(status) {
    // FIX: map đúng theo ReservationStatus enum: WAITING / COMPLETED / CANCELLED / EXPIRED
    const map = {
        'WAITING': '<span class="badge bg-warning text-dark">Đang chờ</span>',
        'COMPLETED': '<span class="badge bg-success">Đã chuyển mượn</span>',
        'CANCELLED': '<span class="badge bg-secondary">Đã hủy</span>',
        'EXPIRED': '<span class="badge bg-danger">Hết hạn</span>'
    };
    return map[status] || '<span class="badge bg-secondary">' + (status || '—') + '</span>';
}

function saveReservation() {
    const userId = parseInt($('#resUserId').val());
    const bookId = parseInt($('#resBookId').val());

    if (!userId || userId < 1) { showToast('error', 'Vui lòng nhập User ID hợp lệ'); return; }
    if (!bookId || bookId < 1) { showToast('error', 'Vui lòng nhập Book ID hợp lệ'); return; }

    showLoading('btnSaveRes');
    // FIX: CreateReservationRequest nhận userId + bookId (đúng rồi)
    ReservationAPI.create({ userId: userId, bookId: bookId }, function (err) {
        hideLoading('btnSaveRes');
        if (err) { showToast('error', err.message); return; }
        showToast('success', 'Tạo đặt trước thành công');
        bootstrap.Modal.getInstance('#resModal').hide();
        loadReservations();
    });
}