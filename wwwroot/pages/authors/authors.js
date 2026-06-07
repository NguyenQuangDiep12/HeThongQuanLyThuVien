let currentPage = 1, editId = null, deleteId = null;
const pageSize = 20;

$(document).ready(function () {
    if (!checkAuth('STAFF')) return;
    initLayout('Tác giả');
    loadAuthors();
    $('#searchForm').on('submit', function (e) { e.preventDefault(); currentPage = 1; loadAuthors(); });
    $('#btnCreate').on('click', function () { editId = null; $('#authorName').val(''); $('#authorBio').val(''); $('#authorModalTitle').text('Thêm tác giả'); new bootstrap.Modal('#authorModal').show(); });
    $('#btnSaveAuthor').on('click', saveAuthor);
    $('#btnConfirmDelete').on('click', function () {
        AuthorAPI.delete(deleteId, function (err) {
            if (err) showToast('error', err.message);
            else { showToast('success', 'Đã xóa'); bootstrap.Modal.getInstance('#deleteModal').hide(); loadAuthors(); }
        });
    });
});

function loadAuthors() {
    const params = { page: currentPage, pageSize: pageSize };
    const name = $('#filterName').val().trim();
    if (name) params.name = name;
    AuthorAPI.getList(params, function (err, res) {
        if (err) { showToast('error', err.message); return; }
        const data = res.data;
        let html = '';
        (data.items || []).forEach(function (a, i) {
            html += '<tr><td>' + ((currentPage-1)*pageSize+i+1) + '</td><td>' + escapeHtml(a.authorName) + '</td><td>' + escapeHtml((a.biography || '').substring(0, 80)) + '</td>';
            html += '<td class="text-center"><button class="btn btn-sm btn-outline-warning btn-edit" data-id="' + a.authorId + '" data-name="' + escapeHtml(a.authorName) + '" data-bio="' + escapeHtml(a.biography || '') + '"><i class="bi bi-pencil"></i></button>';
            if (isAdmin()) html += ' <button class="btn btn-sm btn-outline-danger btn-delete" data-id="' + a.authorId + '" data-name="' + escapeHtml(a.authorName) + '"><i class="bi bi-trash"></i></button>';
            html += '</td></tr>';
        });
        $('#tableBody').html(html || '<tr><td colspan="4" class="text-center text-muted">Không có dữ liệu</td></tr>');
        updateShowingCount((data.items||[]).length, data.totalRecords||0);
        renderPagination('pagination', data.totalRecords||0, currentPage, pageSize, function(p){currentPage=p;loadAuthors();});
        bindAuthorEvents();
    });
}

function bindAuthorEvents() {
    $('.btn-edit').on('click', function () {
        editId = $(this).data('id'); $('#authorName').val($(this).data('name')); $('#authorBio').val($(this).data('bio'));
        $('#authorModalTitle').text('Sửa tác giả'); new bootstrap.Modal('#authorModal').show();
    });
    $('.btn-delete').on('click', function () {
        deleteId = $(this).data('id'); $('#deleteItemName').text($(this).data('name')); new bootstrap.Modal('#deleteModal').show();
    });
}

function saveAuthor() {
    const data = {
        authorName: $('#authorName').val().trim(),
        biography: $('#authorBio').val().trim(),
        authorUrl: 'https://example.com'
    };
    const cb = function (err) {
        if (err) showToast('error', err.message);
        else { showToast('success', 'Lưu thành công'); bootstrap.Modal.getInstance('#authorModal').hide(); loadAuthors(); }
    };
    if (editId) AuthorAPI.update(editId, data, cb); else AuthorAPI.create(data, cb);
}
