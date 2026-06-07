let currentPage = 1;
const pageSize = 10;

$(document).ready(function () {
    if (!checkAuth('STAFF')) return;
    initLayout('Vi phạm & Phạt');
    loadFines();
    $('#searchForm').on('submit', function (e) { e.preventDefault(); currentPage = 1; loadFines(); });
    $('#btnCreate').on('click', function () { new bootstrap.Modal('#fineModal').show(); });
    $('#btnSaveFine').on('click', saveFine);
});

function loadFines() {
    FineAPI.getList({ page: currentPage, pageSize: pageSize }, function (err, res) {
        if (err) { showToast('error', err.message); return; }
        const data = res.data;
        let html = '';
        (data.items || []).forEach(function (f) {
            html += '<tr><td>' + escapeHtml(f.readerName) + '</td><td>' + escapeHtml(f.borrowCode) + '</td>';
            html += '<td>' + escapeHtml(f.fineType) + '</td><td>' + formatCurrency(f.amount) + '</td>';
            html += '<td>' + getBadgeStatus(f.paymentStatus) + '</td><td>' + formatDate(f.createdAt) + '</td>';
            html += '<td class="text-center">';
            if (f.paymentStatus === 'UNPAID') {
                html += '<button class="btn btn-sm btn-success btn-pay" data-id="' + f.fineId + '"><i class="bi bi-cash"></i></button>';
            }
            html += '</td></tr>';
        });
        $('#tableBody').html(html || '<tr><td colspan="7" class="text-center text-muted">Không có dữ liệu</td></tr>');
        updateShowingCount((data.items || []).length, data.totalRecords || 0);
        renderPagination('pagination', data.totalRecords || 0, currentPage, pageSize, function (p) { currentPage = p; loadFines(); });
        $('.btn-pay').on('click', function () {
            const id = $(this).data('id');
            confirmAction('Thanh toán', 'Xác nhận độc giả đã thanh toán?', function () {
                FineAPI.pay(id, function (err2) {
                    if (err2) showToast('error', err2.message);
                    else { showToast('success', 'Thanh toán thành công'); loadFines(); }
                });
            });
        });
    });
}

function saveFine() {
    FineAPI.create({
        borrowDetailId: parseInt($('#fineBorrowDetailId').val()),
        fineType: $('#fineType').val(),
        amount: parseFloat($('#fineAmount').val()),
        reason: $('#fineReason').val().trim() || null
    }, function (err) {
        if (err) { showToast('error', err.message); return; }
        showToast('success', 'Tạo phiếu phạt thành công');
        bootstrap.Modal.getInstance('#fineModal').hide();
        loadFines();
    });
}
