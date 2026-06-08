let currentPage = 1;
const pageSize = 10;

$(document).ready(function () {
    if (!checkAuth('STAFF')) return;
    initLayout('Vi phạm & Phạt');
    loadFines();
    $('#searchForm').on('submit', function (e) { e.preventDefault(); currentPage = 1; loadFines(); });
    $('#btnCreate').on('click', function () {
        $('#fineBorrowDetailId').val('');
        $('#fineType').val('OVERDUE');
        $('#fineAmount').val('');
        $('#fineReason').val('');
        new bootstrap.Modal('#fineModal').show();
    });
    $('#btnSaveFine').on('click', saveFine);
});

function loadFines() {
    FineAPI.getList({ page: currentPage, pageSize: pageSize }, function (err, res) {
        if (err) { showToast('error', err.message); return; }
        const data = res.data;
        let html = '';
        (data.items || []).forEach(function (f) {
            html += '<tr>';
            html += '<td>' + escapeHtml(f.readerName) + '</td>';
            html += '<td><code>' + escapeHtml(f.borrowCode) + '</code></td>';
            html += '<td>' + getFineTypeBadge(f.fineType) + '</td>';
            html += '<td class="fw-semibold">' + formatCurrency(f.amount) + '</td>';
            // FIX: backend trả về PaymentStatus.PENDING (không phải UNPAID)
            html += '<td>' + getBadgeStatus(f.paymentStatus) + '</td>';
            html += '<td>' + formatDate(f.createdAt) + '</td>';
            html += '<td class="text-center">';
            // FIX: check f.paymentStatus === 'PENDING' (không phải 'UNPAID')
            if (f.paymentStatus === 'PENDING') {
                html += '<button class="btn btn-sm btn-success btn-pay" data-id="' + f.fineId + '" title="Xác nhận thanh toán"><i class="bi bi-cash-coin me-1"></i>Thanh toán</button>';
            }
            html += '</td></tr>';
        });
        $('#tableBody').html(html || '<tr><td colspan="7" class="text-center text-muted py-4">Không có phiếu phạt nào</td></tr>');
        updateShowingCount((data.items || []).length, data.totalRecords || 0);
        renderPagination('pagination', data.totalRecords || 0, currentPage, pageSize, function (p) { currentPage = p; loadFines(); });

        $('.btn-pay').on('click', function () {
            const id = $(this).data('id');
            confirmAction('Thanh toán phiếu phạt', 'Xác nhận độc giả đã thanh toán phiếu phạt này?', function () {
                FineAPI.pay(id, function (err2) {
                    if (err2) showToast('error', err2.message);
                    else { showToast('success', 'Xác nhận thanh toán thành công'); loadFines(); }
                });
            });
        });
    });
}

function getFineTypeBadge(fineType) {
    const map = {
        'OVERDUE': '<span class="badge bg-warning text-dark">Quá hạn</span>',
        'DAMAGED': '<span class="badge bg-orange text-dark" style="background:#fd7e14">Hư hỏng</span>',
        'LOST': '<span class="badge bg-danger">Mất sách</span>'
    };
    return map[fineType] || '<span class="badge bg-secondary">' + (fineType || '—') + '</span>';
}

function saveFine() {
    const borrowDetailId = parseInt($('#fineBorrowDetailId').val());
    const fineType = $('#fineType').val();
    const amount = parseFloat($('#fineAmount').val());
    const reason = $('#fineReason').val().trim() || null;

    if (!borrowDetailId || borrowDetailId < 1) { showToast('error', 'Vui lòng nhập Borrow Detail ID hợp lệ'); return; }
    if (!fineType) { showToast('error', 'Vui lòng chọn loại vi phạm'); return; }
    if (!amount || amount < 1000) { showToast('error', 'Số tiền phạt tối thiểu 1,000 VNĐ'); return; }

    showLoading('btnSaveFine');
    FineAPI.create({
        borrowDetailId: borrowDetailId,
        // FIX: fineType phải là OVERDUE / DAMAGED / LOST (uppercase) - không phải Overdue/Damaged/Lost
        fineType: fineType,
        amount: amount,
        reason: reason
    }, function (err) {
        hideLoading('btnSaveFine');
        if (err) { showToast('error', err.message); return; }
        showToast('success', 'Tạo phiếu phạt thành công');
        bootstrap.Modal.getInstance('#fineModal').hide();
        loadFines();
    });
}