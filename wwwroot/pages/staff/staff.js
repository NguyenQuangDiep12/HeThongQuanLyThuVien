let currentPage = 1;
const pageSize = 10;

$(document).ready(function () {
    if (!checkAuth('ADMIN')) return;
    initLayout('Quản lý nhân viên');
    loadStaffs();

    $('#btnCreate').on('click', function () {
        $('#staffForm')[0].reset();
        new bootstrap.Modal('#staffModal').show();
    });
    $('#btnSaveStaff').on('click', saveStaff);
});

function loadStaffs() {
    UserAPI.getStaffs({ page: currentPage, pageSize: pageSize }, function (err, res) {
        if (err) { showToast('error', err.message); return; }
        const data = res.data;
        let html = '';
        (data.items || []).forEach(function (u) {
            html += '<tr>';
            html += '<td><div class="fw-semibold">' + escapeHtml(u.fullName) + '</div></td>';
            html += '<td>' + escapeHtml(u.email) + '</td>';
            html += '<td>' + escapeHtml(u.phone) + '</td>';
            html += '<td><span class="badge bg-info text-dark">STAFF</span></td>';
            html += '<td>' + getBadgeStatus(u.status) + '</td>';
            html += '</tr>';
        });
        $('#tableBody').html(html || '<tr><td colspan="5" class="text-center text-muted py-4">Chưa có nhân viên nào</td></tr>');
        updateShowingCount((data.items || []).length, data.totalRecords || 0);
        renderPagination('pagination', data.totalRecords || 0, currentPage, pageSize, function (p) { currentPage = p; loadStaffs(); });
    });
}

function saveStaff() {
    const fullName = $('#staffName').val().trim();
    const email = $('#staffEmail').val().trim();
    const phone = $('#staffPhone').val().trim();
    const address = $('#staffAddress').val().trim();
    const password = $('#staffPassword').val();

    if (!fullName) { showToast('error', 'Vui lòng nhập họ tên'); return; }
    if (!email) { showToast('error', 'Vui lòng nhập email'); return; }
    if (!phone) { showToast('error', 'Vui lòng nhập số điện thoại'); return; }
    if (!address) { showToast('error', 'Vui lòng nhập địa chỉ'); return; }
    if (!password || password.length < 6) { showToast('error', 'Mật khẩu tối thiểu 6 ký tự'); return; }

    showLoading('btnSaveStaff');
    UserAPI.createStaff({
        fullName: fullName,
        email: email,
        phone: phone,
        address: address,
        password: password
    }, function (err) {
        hideLoading('btnSaveStaff');
        if (err) { showToast('error', err.message); return; }
        showToast('success', 'Tạo tài khoản nhân viên thành công');
        bootstrap.Modal.getInstance('#staffModal').hide();
        loadStaffs();
    });
}