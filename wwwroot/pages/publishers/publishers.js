let currentPage = 1, editId = null, deleteId = null;
const pageSize = 20;

$(document).ready(function () {
    if (!checkAuth('STAFF')) return;
    initLayout('Nhà xuất bản');
    loadPublishers();
    $('#searchForm').on('submit', function (e) { e.preventDefault(); currentPage = 1; loadPublishers(); });
    $('#btnCreate').on('click', function () { editId = null; $('#pubName').val(''); $('#pubModalTitle').text('Thêm NXB'); new bootstrap.Modal('#pubModal').show(); });
    $('#btnSavePub').on('click', savePublisher);
    $('#btnConfirmDelete').on('click', function () {
        PublisherAPI.delete(deleteId, function (err) {
            if (err) showToast('error', err.message);
            else { showToast('success', 'Đã xóa'); bootstrap.Modal.getInstance('#deleteModal').hide(); loadPublishers(); }
        });
    });
});

function loadPublishers() {
    const params = { page: currentPage, pageSize: pageSize };
    const name = $('#filterName').val().trim();
    if (name) params.name = name;
    PublisherAPI.getList(params, function (err, res) {
        if (err) { showToast('error', err.message); return; }
        const data = res.data;
        let html = '';
        (data.items || []).forEach(function (p, i) {
            html += '<tr><td>' + ((currentPage-1)*pageSize+i+1) + '</td><td>' + escapeHtml(p.publisherName) + '</td>';
            html += '<td class="text-center"><button class="btn btn-sm btn-outline-warning btn-edit" data-id="' + p.publisherId + '" data-name="' + escapeHtml(p.publisherName) + '"><i class="bi bi-pencil"></i></button>';
            if (isAdmin()) html += ' <button class="btn btn-sm btn-outline-danger btn-delete" data-id="' + p.publisherId + '" data-name="' + escapeHtml(p.publisherName) + '"><i class="bi bi-trash"></i></button>';
            html += '</td></tr>';
        });
        $('#tableBody').html(html || '<tr><td colspan="3" class="text-center text-muted">Không có dữ liệu</td></tr>');
        updateShowingCount((data.items||[]).length, data.totalRecords||0);
        renderPagination('pagination', data.totalRecords||0, currentPage, pageSize, function(p){currentPage=p;loadPublishers();});
        $('.btn-edit').on('click', function () { editId = $(this).data('id'); $('#pubName').val($(this).data('name')); $('#pubModalTitle').text('Sửa NXB'); new bootstrap.Modal('#pubModal').show(); });
        $('.btn-delete').on('click', function () { deleteId = $(this).data('id'); $('#deleteItemName').text($(this).data('name')); new bootstrap.Modal('#deleteModal').show(); });
    });
}

function savePublisher() {
    const data = { name: $('#pubName').val().trim(), address: '', phone: '' };
    const cb = function (err) {
        if (err) showToast('error', err.message);
        else { showToast('success', 'Lưu thành công'); bootstrap.Modal.getInstance('#pubModal').hide(); loadPublishers(); }
    };
    if (editId) PublisherAPI.update(editId, data, cb); else PublisherAPI.create(data, cb);
}
