let currentPage = 1;
const pageSize = 10;

$(document).ready(function () {
    if (!checkAuth('READER')) return;
    initLayout('Lịch sử mượn');
    if (window.location.pathname.includes('detail.html')) initMyBorrowDetail();
    else initMyBorrowList();
});

function initMyBorrowList() {
    loadMyBorrows();
    $('#filterStatus').on('change', function () { currentPage = 1; loadMyBorrows(); });
}

function loadMyBorrows() {
    const user = getCurrentUser();
    const params = { page: currentPage, pageSize: pageSize };
    const status = $('#filterStatus').val();
    if (status) params.status = status;

    UserAPI.getBorrowRecords(user.userId, params, function (err, res) {
        if (err) { showToast('error', err.message); return; }
        const data = res.data;
        let html = '';
        (data.items || []).forEach(function (r) {
            html += '<tr><td>' + escapeHtml(r.borrowCode) + '</td><td>' + (r.totalBooks || '—') + ' cuốn</td>';
            html += '<td>' + formatDate(r.borrowDate) + '</td><td>' + formatDate(r.dueDate) + '</td>';
            html += '<td>' + getBadgeStatus(r.status) + '</td>';
            html += '<td class="text-center"><a href="detail.html?borrowId=' + r.borrowId + '" class="btn btn-sm btn-outline-info"><i class="bi bi-eye"></i></a></td></tr>';
        });
        $('#tableBody').html(html || '<tr><td colspan="6" class="text-center text-muted">Chưa có lịch sử mượn</td></tr>');
        updateShowingCount((data.items || []).length, data.totalRecords || 0);
        renderPagination('pagination', data.totalRecords || 0, currentPage, pageSize, function (p) { currentPage = p; loadMyBorrows(); });
    });
}

function initMyBorrowDetail() {
    const borrowId = getQueryParam('borrowId');
    if (!borrowId) { window.location.href = 'index.html'; return; }

    BorrowAPI.getById(borrowId, function (err, res) {
        if (err) { showToast('error', err.message); return; }
        const r = res.data;
        $('#borrowCode').text(r.borrowCode);
        $('#borrowDate').text(formatDate(r.borrowDate));
        $('#dueDate').text(formatDate(r.dueDate));
        $('#returnedDate').text(formatDate(r.returnedDate));
        $('#status').html(getBadgeStatus(r.status));
        $('#extensionCount').text(r.extensionCount + '/2');

        let html = '';
        (r.borrowDetails || []).forEach(function (d) {
            html += '<tr><td>' + escapeHtml(d.bookTitle) + '</td><td>' + escapeHtml(d.barcode) + '</td><td>' + getBadgeStatus(d.status) + '</td></tr>';
        });
        $('#detailsBody').html(html);

        if (r.status === 'BORROWING' && r.extensionCount < 2) {
            const due = new Date(r.dueDate);
            if (due >= new Date()) {
                $('#btnRequestExtend').show().on('click', function () {
                    confirmAction('Gia hạn', 'Gửi yêu cầu gia hạn phiếu mượn này?', function () {
                        BorrowAPI.requestExtension(borrowId, function (err2) {
                            if (err2) showToast('error', err2.message);
                            else showToast('success', 'Đã gửi yêu cầu gia hạn');
                        });
                    });
                });
            }
        }
    });
}
