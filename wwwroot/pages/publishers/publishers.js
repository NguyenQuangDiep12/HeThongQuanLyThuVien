let currentPage = 1, editId = null, deleteId = null;
const pageSize = 20;

$(document).ready(function () {
    if (!checkAuth('STAFF')) return;
    initLayout('Nhà xuất bản');
    loadPublishers();
    $('#searchForm').on('submit', function (e) { e.preventDefault(); currentPage = 1; loadPublishers(); });
    $('#btnCreate').on('click', function () {
        editId = null;
        $('#pubName').val('');
        $('#pubLogoUrl').val('');
        $('#pubModalTitle').text('Thêm NXB');
        new bootstrap.Modal('#pubModal').show();
    });
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

    // GET /publishers trả về List trực tiếp (không pagination)
    PublisherAPI.getList(null, function (err, res) {
        if (err) { showToast('error', err.message); return; }
        // Backend trả về List<PublisherResponse> trực tiếp trong res.data
        const items = res.data || [];
        let html = '';
        items.forEach(function (p, i) {
            html += '<tr><td>' + ((currentPage - 1) * pageSize + i + 1) + '</td>';
            html += '<td>' + escapeHtml(p.publisherName) + '</td>';
            html += '<td>' + (p.logoUrl ? '<img src="' + escapeHtml(p.logoUrl) + '" height="32" style="object-fit:contain">' : '—') + '</td>';
            html += '<td class="text-center">';
            html += '<button class="btn btn-sm btn-outline-warning btn-edit" data-id="' + p.publisherId + '" data-name="' + escapeHtml(p.publisherName) + '" data-logo="' + escapeHtml(p.logoUrl || '') + '"><i class="bi bi-pencil"></i></button>';
            if (isAdmin()) html += ' <button class="btn btn-sm btn-outline-danger btn-delete" data-id="' + p.publisherId + '" data-name="' + escapeHtml(p.publisherName) + '"><i class="bi bi-trash"></i></button>';
            html += '</td></tr>';
        });
        $('#tableBody').html(html || '<tr><td colspan="4" class="text-center text-muted">Không có dữ liệu</td></tr>');
        updateShowingCount(items.length, items.length);

        $('.btn-edit').on('click', function () {
            editId = $(this).data('id');
            $('#pubName').val($(this).data('name'));
            $('#pubLogoUrl').val($(this).data('logo'));
            $('#pubModalTitle').text('Sửa NXB');
            new bootstrap.Modal('#pubModal').show();
        });
        $('.btn-delete').on('click', function () {
            deleteId = $(this).data('id');
            $('#deleteItemName').text($(this).data('name'));
            new bootstrap.Modal('#deleteModal').show();
        });
    });
}

function savePublisher() {
    const name = $('#pubName').val().trim();
    const logoUrl = $('#pubLogoUrl').val().trim();

    if (!name) { showToast('error', 'Vui lòng nhập tên NXB'); return; }
    if (!logoUrl) { showToast('error', 'Vui lòng nhập URL logo'); return; }

    // FIX: backend PublisherRequest yêu cầu publisherName + logoUrl (không phải name/address/phone)
    const data = {
        publisherName: name,
        logoUrl: logoUrl
    };

    const cb = function (err) {
        if (err) { showToast('error', err.message); return; }
        showToast('success', 'Lưu thành công');
        bootstrap.Modal.getInstance('#pubModal').hide();
        loadPublishers();
    };
    if (editId) PublisherAPI.update(editId, data, cb);
    else PublisherAPI.create(data, cb);
}