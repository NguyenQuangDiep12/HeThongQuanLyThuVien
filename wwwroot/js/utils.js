function showToast(type, message) {
    const toast = document.getElementById('appToast');
    if (!toast) return;
    const colors = {
        success: 'bg-success text-white',
        error: 'bg-danger text-white',
        warning: 'bg-warning text-dark',
        info: 'bg-info text-white'
    };
    toast.className = 'toast align-items-center border-0 ' + (colors[type] || colors.info);
    document.getElementById('toastMessage').textContent = message;
    bootstrap.Toast.getOrCreateInstance(toast, { delay: 3000 }).show();
}

function formatDate(dateStr) {
    if (!dateStr) return '—';
    return new Date(dateStr).toLocaleDateString('vi-VN');
}

function formatDateTime(dateStr) {
    if (!dateStr) return '—';
    return new Date(dateStr).toLocaleString('vi-VN');
}

function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount || 0);
}

function normalizeStatus(status) {
    if (!status) return '';
    return String(status).toUpperCase();
}

function getBadgeStatus(status) {
    const s = normalizeStatus(status);
    const map = {
        ACTIVE: '<span class="badge bg-success">Hoạt động</span>',
        LOCKED: '<span class="badge bg-danger">Bị khóa</span>',
        INACTIVE: '<span class="badge bg-secondary">Không hoạt động</span>',
        EXPIRED: '<span class="badge bg-warning text-dark">Hết hạn</span>',
        BORROWING: '<span class="badge bg-primary">Đang mượn</span>',
        RETURNED: '<span class="badge bg-success">Đã trả</span>',
        OVERDUE: '<span class="badge bg-danger">Quá hạn</span>',
        CANCELLED: '<span class="badge bg-secondary">Đã hủy</span>',
        PENDING: '<span class="badge bg-warning text-dark">Đang chờ</span>',
        AVAILABLE: '<span class="badge bg-success">Sẵn sàng</span>',
        BORROWED: '<span class="badge bg-primary">Đang mượn</span>',
        RESERVED: '<span class="badge bg-info">Đặt trước</span>',
        UNAVAILABLE: '<span class="badge bg-warning text-dark">Không khả dụng</span>',
        DAMAGED: '<span class="badge bg-warning text-dark">Hư hỏng</span>',
        LOST: '<span class="badge bg-danger">Đã mất</span>',
        UNPAID: '<span class="badge bg-danger">Chưa thanh toán</span>',
        PAID: '<span class="badge bg-success">Đã thanh toán</span>',
        READY: '<span class="badge bg-info">Sẵn sàng nhận</span>',
        CONVERTED: '<span class="badge bg-success">Đã chuyển mượn</span>',
        NORMAL: '<span class="badge bg-success">Bình thường</span>',
        TORN: '<span class="badge bg-warning text-dark">Rách</span>'
    };
    return map[s] || '<span class="badge bg-secondary">' + status + '</span>';
}

function renderPagination(containerId, totalRecords, currentPage, pageSize, onPageChange) {
    const totalPages = Math.ceil(totalRecords / pageSize) || 1;
    const container = $('#' + containerId);
    container.empty();

    if (totalPages <= 1) return;

    let html = '';
    html += '<li class="page-item ' + (currentPage === 1 ? 'disabled' : '') + '">';
    html += '<a class="page-link" href="#" data-page="' + (currentPage - 1) + '"><i class="bi bi-chevron-left"></i></a></li>';

    for (let i = 1; i <= totalPages; i++) {
        if (i === 1 || i === totalPages || Math.abs(i - currentPage) <= 2) {
            html += '<li class="page-item ' + (i === currentPage ? 'active' : '') + '">';
            html += '<a class="page-link" href="#" data-page="' + i + '">' + i + '</a></li>';
        } else if (Math.abs(i - currentPage) === 3) {
            html += '<li class="page-item disabled"><span class="page-link">...</span></li>';
        }
    }

    html += '<li class="page-item ' + (currentPage === totalPages ? 'disabled' : '') + '">';
    html += '<a class="page-link" href="#" data-page="' + (currentPage + 1) + '"><i class="bi bi-chevron-right"></i></a></li>';

    container.html(html);
    container.find('.page-link[data-page]').on('click', function (e) {
        e.preventDefault();
        const page = parseInt($(this).data('page'));
        if (page >= 1 && page <= totalPages) onPageChange(page);
    });
}

function updateShowingCount(showing, total) {
    $('#showingCount').text(showing);
    $('#totalCount').text(total);
}

function showFieldError(fieldId, message) {
    $('#' + fieldId + 'Error').text(message).show();
    $('#' + fieldId).addClass('is-invalid');
}

function clearFormErrors(formId) {
    $('#' + formId + ' .form-error').hide().text('');
    $('#' + formId + ' .is-invalid').removeClass('is-invalid');
}

function showApiFieldErrors(errors) {
    $.each(errors, function (field, messages) {
        const fieldMap = {
            Title: 'bookTitle', ISBN: 'bookIsbn', Email: 'email',
            Password: 'password', FullName: 'fullName', Phone: 'phone'
        };
        const id = fieldMap[field] || field.charAt(0).toLowerCase() + field.slice(1);
        if ($('#' + id + 'Error').length) {
            showFieldError(id, messages[0]);
        }
    });
}

function showLoading(btnId) {
    const btn = $('#' + btnId);
    btn.data('original-text', btn.html());
    btn.html('<span class="spinner-border spinner-border-sm"></span> Đang xử lý...');
    btn.prop('disabled', true);
}

function hideLoading(btnId) {
    const btn = $('#' + btnId);
    btn.html(btn.data('original-text'));
    btn.prop('disabled', false);
}

function getQueryParam(name) {
    return new URLSearchParams(window.location.search).get(name);
}

function confirmAction(title, message, onConfirm) {
    $('#confirmModalTitle').text(title);
    $('#confirmModalBody').html(message);
    const modal = bootstrap.Modal.getOrCreateInstance(document.getElementById('confirmModal'));
    $('#btnConfirmAction').off('click').on('click', function () {
        modal.hide();
        onConfirm();
    });
    modal.show();
}

function escapeHtml(text) {
    if (!text) return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

function getAuthorsText(authors) {
    if (!authors || !authors.length) return '—';
    return authors.map(function (a) { return a.authorName || a.name; }).join(', ');
}

function getCategoriesText(categories) {
    if (!categories || !categories.length) return '—';
    return categories.map(function (c) { return c.categoryName || c.name; }).join(', ');
}
