let bookId = null, deleteId = null;

$(document).ready(function () {
    if (!checkAuth('STAFF')) return;
    initLayout('Quản lý bản sao');
    bookId = getQueryParam('bookId');
    if (!bookId) { showToast('error', 'Thiếu bookId'); return; }

    BookAPI.getById(bookId, function (err, res) {
        if (!err) $('#bookTitle').text(res.data.title);
    });

    loadCopies();
    $('#btnCreate').on('click', function () { $('#copyBarcode').val(''); $('#copyShelf').val(''); new bootstrap.Modal('#copyModal').show(); });
    $('#btnSaveCopy').on('click', saveCopy);
    $('#btnSaveStatus').on('click', function () {
        const id = $('#statusCopyId').val();
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
        let html = '';
        (res.data.items || []).forEach(function (c, i) {
            html += '<tr><td>' + (i+1) + '</td><td>' + escapeHtml(c.barcode) + '</td><td>' + escapeHtml(c.shelfLocation) + '</td>';
            html += '<td>' + getBadgeStatus(c.condition) + '</td><td>' + getBadgeStatus(c.status) + '</td>';
            html += '<td class="text-center">';
            html += '<button class="btn btn-sm btn-outline-warning btn-status" data-id="' + c.copyId + '"><i class="bi bi-arrow-repeat"></i></button>';
            if (isAdmin()) html += ' <button class="btn btn-sm btn-outline-danger btn-delete" data-id="' + c.copyId + '"><i class="bi bi-trash"></i></button>';
            html += '</td></tr>';
        });
        $('#tableBody').html(html || '<tr><td colspan="6" class="text-center text-muted">Chưa có bản sao</td></tr>');
        $('.btn-delete').on('click', function () { deleteId = $(this).data('id'); $('#deleteItemName').text('bản sao #' + deleteId); new bootstrap.Modal('#deleteModal').show(); });
        $('.btn-status').on('click', function () {
            const id = $(this).data('id');
            $('#statusCopyId').val(id);
            new bootstrap.Modal('#statusModal').show();
        });
    });
}

function saveCopy() {
    BookCopyAPI.create(bookId, {
        barcode: $('#copyBarcode').val().trim(),
        shelfLocation: $('#copyShelf').val().trim(),
        condition: 'New'
    }, function (err) {
        if (err) { showToast('error', err.message); return; }
        showToast('success', 'Thêm bản sao thành công');
        bootstrap.Modal.getInstance('#copyModal').hide();
        loadCopies();
    });
}
