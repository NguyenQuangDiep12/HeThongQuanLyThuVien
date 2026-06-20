$(document).ready(function () {
    if (!checkAuth(null)) return;
    initLayout('Hồ sơ cá nhân');
    const user = getCurrentUser();
    loadProfile(user.userId);

    $('#btnUpdateProfile').on('click', updateProfile);
    $('#btnChangePassword').on('click', changePassword);
});

function loadProfile(userId) {
    UserAPI.getById(userId, function (err, res) {
        if (err) { showToast('error', err.message); return; }
        const u = res.data;
        $('#profFullName').val(u.fullName);
        $('#profEmail').val(u.email);
        $('#profPhone').val(u.phone);
        $('#profAddress').val(u.address);
        $('#profAvatar').val(u.avatarUrl || '');
        $('#profRole').text(u.role);
        $('#profStatus').html(getBadgeStatus(u.status));
        $('#profCard').text(u.libraryCardCode || 'Chưa có thẻ');
        $('#profCardStatus').html(getBadgeStatus(u.cardStatus));
        if (u.avatarUrl) $('#avatarPreview').attr('src', u.avatarUrl).show();
    });
}

function updateProfile() {
    const user = getCurrentUser();
    if (user.role !== 'READER') {
        showToast('warning', 'Chỉ độc giả có thể tự cập nhật hồ sơ qua API này');
        return;
    }
    showLoading('btnUpdateProfile');
    UserAPI.updateProfile({
        fullName: $('#profFullName').val().trim(),
        phone: $('#profPhone').val().trim(),
        address: $('#profAddress').val().trim(),
        avatarUrl: $('#profAvatar').val().trim() || null
    }, function (err) {
        hideLoading('btnUpdateProfile');
        if (err) { showToast('error', err.message); return; }
        user.fullName = $('#profFullName').val().trim();
        localStorage.setItem('currentUser', JSON.stringify(user));
        $('#userName').text(user.fullName);
        showToast('success', 'Cập nhật hồ sơ thành công');
    });
}

function changePassword() {
    const oldPwd = $('#oldPassword').val();
    const newPwd = $('#newPassword').val();
    const confirmPwd = $('#confirmPassword').val();
    if (newPwd !== confirmPwd) { showToast('error', 'Mật khẩu xác nhận không khớp'); return; }
    showLoading('btnChangePassword');
    AuthAPI.resetPassword({ oldPassword: oldPwd, password: newPwd, confirmPassword: confirmPwd }, function (err) {
        hideLoading('btnChangePassword');
        if (err) { showToast('error', err.message); return; }
        showToast('success', 'Đổi mật khẩu thành công');
        $('#passwordForm')[0].reset();
    });
}
