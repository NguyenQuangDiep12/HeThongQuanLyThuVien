let bookId = null, deleteId = null;

$(document).ready(function () {
    if (!checkAuth('STAFF')) return;
    initLayout('Quản lý bản sao');
    bookId = getQueryParam('bookId');
    if (!bookId) { showToast('error', 'Thiếu bookId'); return; }

    BookAPI.getById(bookId, function (err, res) {
        if (!err && res.data) $('#bookTitle').text(res.data.title);
    });

    loadCopies();

    $('#btnCreate').on('click', function () {
        $('#copyQuantity').val(1);
        $('#copyShelf').val('');
        new bootstrap.Modal('#copyModal').show();
    });

    $('#btnSaveCopy').on('click', saveCopy);

    // Cập nhật trạng thái bản sao (PATCH /book-copies/:id/status)
    $('#btnSaveStatus').on('click', function () {
        const id = $('#statusCopyId').val();
        // FIX: updateStatus gọi PATCH với query params, status phải là enum uppercase
        BookCopyAPI.updateStatus(id, { status: $('#copyStatus').val() }, function (err2) {
            if (err2) showToast('error', err2.message);
            else { showToast('success', 'Cập nhật thành công'); bootstrap.Modal.getInstance('#statusModal').hide(); loadCopies(); }
        });
    });

    $('#btnConfirmDelete').on('click', function () {
        BookCopyAPI.delete(deleteId, function (err) {
            if (err) showToast('error', err.message);
            else { showToast('success', 'Đã xóa'); bootstrap.Modal.getInstance('#deleteModal').hide(); loadCopies(); }
        });
    });
});

function loadCopies() {
    BookCopyAPI.getList({ bookId: bookId, page: 1, pageSize: 50 }, function (err, res) {
        if (err) { showToast('error', err.message); return; }
        const items = (res.data && res.data.items) || [];
        let html = '';
        items.forEach(function (c, i) {
            html += '<tr>';
            html += '<td>' + (i + 1) + '</td>';
            html += '<td><code>' + escapeHtml(c.barcode) + '</code></td>';
            html += '<td>' + escapeHtml(c.shelfLocation || '—') + '</td>';
            html += '<td>' + getBadgeStatus(c.condition) + '</td>';
            html += '<td>' + getBadgeStatus(c.status) + '</td>';
            html += '<td class="text-center">';
            html += '<button class="btn btn-sm btn-outline-warning btn-status" data-id="' + c.copyId + '" title="Đổi trạng thái"><i class="bi bi-arrow-repeat"></i></button>';
            if (isAdmin()) html += ' <button class="btn btn-sm btn-outline-danger btn-delete" data-id="' + c.copyId + '"><i class="bi bi-trash"></i></button>';
            html += '</td></tr>';
        });
        $('#tableBody').html(html || '<tr><td colspan="6" class="text-center text-muted">Chưa có bản sao nào</td></tr>');
        $('#copyCount').text(items.length);

        $('.btn-delete').on('click', function () {
            deleteId = $(this).data('id');
            $('#deleteItemName').text('bản sao #' + deleteId);
            new bootstrap.Modal('#deleteModal').show();
        });
        $('.btn-status').on('click', function () {
            $('#statusCopyId').val($(this).data('id'));
            new bootstrap.Modal('#statusModal').show();
        });
    });
}

function saveCopy() {
    const quantity = parseInt($('#copyQuantity').val()) || 1;
    const shelfLocation = $('#copyShelf').val().trim() || null;

    if (quantity < 1 || quantity > 100) {
        showToast('error', 'Số lượng phải từ 1 đến 100');
        return;
    }

    // FIX: backend CreateBookCopyRequest chỉ nhận quantity (int, required) + shelfLocation?
    // KHÔNG nhận barcode và condition (backend tự tạo barcode)
    BookCopyAPI.create(bookId, {
        quantity: quantity,
        shelfLocation: shelfLocation
    }, function (err, res) {
        if (err) { showToast('error', err.message); return; }
        const created = res.data && res.data.totalCreated ? res.data.totalCreated : quantity;
        showToast('success', 'Đã thêm ' + created + ' bản sao thành công');
        bootstrap.Modal.getInstance('#copyModal').hide();
        loadCopies();
    });
}