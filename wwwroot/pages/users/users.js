let currentPage = 1;
const pageSize = 10;

$(document).ready(function () {
    if (!checkAuth('STAFF')) return;
    initLayout('Quản lý độc giả');
    if (window.location.pathname.includes('detail.html')) initUserDetail();
    else initUserList();
});

function initUserList() {
    loadUsers();
    $('#searchForm').on('submit', function (e) { e.preventDefault(); currentPage = 1; loadUsers(); });
    $('#btnReset').on('click', function () { $('#filterName').val(''); $('#filterEmail').val(''); currentPage = 1; loadUsers(); });
}

function loadUsers() {
    const params = { page: currentPage, pageSize: pageSize, roleName: 'READER' };
    const name = $('#filterName').val().trim();
    const email = $('#filterEmail').val().trim();
    if (name) params.fullName = name;
    if (email) params.email = email;

    UserAPI.getList(params, function (err, res) {
        if (err) { showToast('error', err.message); return; }
        const data = res.data;
        let html = '';
        (data.items || []).forEach(function (u) {
            html += '<tr><td>' + escapeHtml(u.fullName) + '</td><td>' + escapeHtml(u.email) + '</td><td>' + escapeHtml(u.phone) + '</td>';
            html += '<td>' + getBadgeStatus(u.status) + '</td><td>' + getBadgeStatus(u.cardStatus) + '</td>';
            html += '<td class="text-center"><a href="detail.html?userId=' + u.id + '" class="btn btn-sm btn-outline-info"><i class="bi bi-eye"></i></a></td></tr>';
        });
        $('#tableBody').html(html || '<tr><td colspan="6" class="text-center text-muted">Không có dữ liệu</td></tr>');
        updateShowingCount((data.items || []).length, data.totalRecords || 0);
        renderPagination('pagination', data.totalRecords || 0, currentPage, pageSize, function (p) { currentPage = p; loadUsers(); });
    });
}

function initUserDetail() {
    const userId = getQueryParam('userId');
    if (!userId) { window.location.href = 'index.html'; return; }

    UserAPI.getById(userId, function (err, res) {
        if (err) { showToast('error', err.message); return; }
        const u = res.data;
        $('#userName').text(u.fullName);
        $('#userEmail').text(u.email);
        $('#userPhone').text(u.phone);
        $('#userAddress').text(u.address);
        $('#userStatus').html(getBadgeStatus(u.status));
        $('#userCard').text(u.libraryCardCode || '—');
        $('#userCardStatus').html(getBadgeStatus(u.cardStatus));

        if (isAdmin()) {
            $('#adminActions').show();
            $('#btnLock').on('click', function () {
                UserAPI.updateStatus(userId, { status: 'LOCKED' }, function (err2) {
                    if (err2) showToast('error', err2.message); else { showToast('success', 'Đã khóa'); location.reload(); }
                });
            });
            $('#btnUnlock').on('click', function () {
                UserAPI.updateStatus(userId, { status: 'ACTIVE' }, function (err2) {
                    if (err2) showToast('error', err2.message); else { showToast('success', 'Đã mở khóa'); location.reload(); }
                });
            });
        }
    });

    UserAPI.getBorrowRecords(userId, { page: 1, pageSize: 5 }, function (err, res) {
        if (err) return;
        let html = '';
        (res.data.items || []).forEach(function (r) {
            html += '<tr><td>' + escapeHtml(r.borrowCode) + '</td><td>' + formatDate(r.borrowDate) + '</td><td>' + getBadgeStatus(r.status) + '</td></tr>';
        });
        $('#borrowHistory').html(html || '<tr><td colspan="3" class="text-muted">Chưa có</td></tr>');
    });
}
