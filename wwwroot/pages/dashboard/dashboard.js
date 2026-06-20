$(document).ready(function () {
    if (!checkAuth('ADMIN')) return;
    initLayout('Dashboard');

    StatAPI.overview(function (err, res) {
        if (err) { showToast('error', err.message); return; }
        const d = res.data;
        $('#statBooks').text(d.totalBooks || 0);
        $('#statUsers').text(d.totalUsers || 0);
        $('#statBorrowing').text((d.totalBorrowRecords || 0) - (d.totalOverdueRecords || 0));
        $('#statOverdue').text(d.totalOverdueRecords || 0);
    });

    StatAPI.topBooks(10, function (err, res) {
        if (err) return;
        const items = res.data || [];
        let html = '';
        items.forEach(function (b, i) {
            html += '<tr><td>' + (i + 1) + '</td><td>' + escapeHtml(b.title) + '</td>';
            html += '<td class="text-center"><span class="badge bg-primary">' + b.borrowCount + '</span></td></tr>';
        });
        $('#topBooksBody').html(html || '<tr><td colspan="3" class="text-center text-muted">Chưa có dữ liệu</td></tr>');
    });

    StatAPI.overdue(function (err, res) {
        if (err) return;
        const items = res.data || [];
        let html = '';
        items.forEach(function (r) {
            html += '<tr><td>' + escapeHtml(r.readerName) + '</td>';
            html += '<td>' + formatDate(r.dueDate) + '</td>';
            html += '<td><span class="badge bg-danger">' + r.overdueDays + ' ngày</span></td></tr>';
        });
        $('#overdueBody').html(html || '<tr><td colspan="3" class="text-center text-muted">Không có phiếu quá hạn</td></tr>');
    });
});
