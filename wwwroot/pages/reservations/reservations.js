let currentPage = 1;
const pageSize = 10;

$(document).ready(function () {
    if (!checkAuth('STAFF')) return;
    initLayout('Đặt trước');
    loadReservations();
    $('#btnCreate').on('click', function () { new bootstrap.Modal('#resModal').show(); });
    $('#btnSaveRes').on('click', saveReservation);
});

function loadReservations() {
    ReservationAPI.getList({ page: currentPage, pageSize: pageSize }, function (err, res) {
        if (err) { showToast('error', err.message); return; }
        const data = res.data;
        let html = '';
        (data.items || []).forEach(function (r) {
            html += '<tr><td>' + escapeHtml(r.fullName) + '</td><td>' + escapeHtml(r.title) + '</td>';
            html += '<td>' + formatDate(r.createdAt) + '</td><td>' + getBadgeStatus(r.status) + '</td>';
            html += '<td class="text-center">';
            if (r.status === 'PENDING' || r.status === 'READY') {
                html += '<button class="btn btn-sm btn-success btn-complete" data-id="' + r.reservationId + '"><i class="bi bi-check2"></i></button> ';
            }
            html += '<button class="btn btn-sm btn-outline-danger btn-cancel" data-id="' + r.reservationId + '"><i class="bi bi-x"></i></button>';
            html += '</td></tr>';
        });
        $('#tableBody').html(html || '<tr><td colspan="5" class="text-center text-muted">Không có dữ liệu</td></tr>');
        updateShowingCount((data.items||[]).length, data.totalRecords||0);
        renderPagination('pagination', data.totalRecords||0, currentPage, pageSize, function(p){currentPage=p;loadReservations();});
        $('.btn-complete').on('click', function () {
            ReservationAPI.complete($(this).data('id'), function (err2) {
                if (err2) showToast('error', err2.message); else { showToast('success', 'Chuyển mượn thành công'); loadReservations(); }
            });
        });
        $('.btn-cancel').on('click', function () {
            const resId = $(this).data('id');
            confirmAction('Hủy đặt trước', 'Xác nhận hủy?', function () {
                ReservationAPI.cancel(resId, function (err2) {
                    if (err2) showToast('error', err2.message); else { showToast('success', 'Đã hủy'); loadReservations(); }
                });
            });
        });
    });
}

function saveReservation() {
    ReservationAPI.create({
        userId: parseInt($('#resUserId').val()),
        bookId: parseInt($('#resBookId').val())
    }, function (err) {
        if (err) { showToast('error', err.message); return; }
        showToast('success', 'Tạo đặt trước thành công');
        bootstrap.Modal.getInstance('#resModal').hide();
        loadReservations();
    });
}
