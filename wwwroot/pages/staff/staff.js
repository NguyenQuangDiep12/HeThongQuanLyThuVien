let currentPage = 1;
const pageSize = 10;

$(document).ready(function () {
    if (!checkAuth('ADMIN')) return;
    initLayout('Quản lý nhân viên');
    loadStaff();
    $('#btnCreate').on('click', function () { $('#staffForm')[0].reset(); new bootstrap.Modal('#staffModal').show(); });
    $('#btnSaveStaff').on('click', saveStaff);
});

function loadStaff() {
    UserAPI.getStaffs({ page: currentPage, pageSize: pageSize }, function (err, res) {
        if (err) { showToast('error', err.message); return; }
        const data = res.data;
        let html = '';
        (data.items || []).forEach(function (u) {
            html += '<tr><td>' + escapeHtml(u.fullName) + '</td><td>' + escapeHtml(u.email) + '</td><td>' + escapeHtml(u.phone) + '</td>';
            html += '<td>' + escapeHtml(u.role) + '</td><td>' + getBadgeStatus(u.status) + '</td></tr>';
        });
        $('#tableBody').html(html || '<tr><td colspan="5" class="text-center text-muted">Không có dữ liệu</td></tr>');
        updateShowingCount((data.items || []).length, data.totalRecords || 0);
        renderPagination('pagination', data.totalRecords || 0, currentPage, pageSize, function (p) { currentPage = p; loadStaff(); });
    });
}

function saveStaff() {
    showLoading('btnSaveStaff');
    UserAPI.createStaff({
        fullName: $('#staffName').val().trim(),
        email: $('#staffEmail').val().trim(),
        phone: $('#staffPhone').val().trim(),
        address: $('#staffAddress').val().trim(),
        password: $('#staffPassword').val()
    }, function (err) {
        hideLoading('btnSaveStaff');
        if (err) { showToast('error', err.message); return; }
        showToast('success', 'Tạo nhân viên thành công');
        bootstrap.Modal.getInstance('#staffModal').hide();
        loadStaff();
    });
}
