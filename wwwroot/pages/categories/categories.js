let editId = null, deleteId = null;

$(document).ready(function () {
    if (!checkAuth('STAFF')) return;
    initLayout('Danh mục');
    loadCategories();

    $('#btnCreate').on('click', function () {
        editId = null;
        $('#catName').val('');
        $('#catDesc').val('');
        $('#catModalTitle').text('Thêm danh mục');
        new bootstrap.Modal('#catModal').show();
    });
    $('#btnSaveCat').on('click', saveCategory);
    $('#btnConfirmDelete').on('click', confirmDelete);
});

function loadCategories() {
    // GET /categories trả về List<CategoryResponse> trực tiếp (không pagination)
    CategoryAPI.getList(function (err, res) {
        if (err) { showToast('error', err.message); return; }
        const items = res.data || [];
        let html = '';
        items.forEach(function (c, i) {
            html += '<tr><td>' + (i + 1) + '</td>';
            html += '<td>' + escapeHtml(c.categoryName) + '</td>';
            html += '<td>' + escapeHtml(c.description || '—') + '</td>';
            html += '<td class="text-center">';
            html += '<button class="btn btn-sm btn-outline-warning btn-edit" data-id="' + c.categoryId + '" data-name="' + escapeHtml(c.categoryName) + '" data-desc="' + escapeHtml(c.description || '') + '"><i class="bi bi-pencil"></i></button>';
            if (isAdmin()) html += ' <button class="btn btn-sm btn-outline-danger btn-delete" data-id="' + c.categoryId + '" data-name="' + escapeHtml(c.categoryName) + '"><i class="bi bi-trash"></i></button>';
            html += '</td></tr>';
        });
        $('#tableBody').html(html || '<tr><td colspan="4" class="text-center text-muted">Không có dữ liệu</td></tr>');

        $('.btn-edit').on('click', function () {
            editId = $(this).data('id');
            $('#catName').val($(this).data('name'));
            $('#catDesc').val($(this).data('desc'));
            $('#catModalTitle').text('Sửa danh mục');
            new bootstrap.Modal('#catModal').show();
        });
        $('.btn-delete').on('click', function () {
            deleteId = $(this).data('id');
            $('#deleteItemName').text($(this).data('name'));
            new bootstrap.Modal('#deleteModal').show();
        });
    });
}

function saveCategory() {
    const categoryName = $('#catName').val().trim();
    const description = $('#catDesc').val().trim();

    if (!categoryName) { showToast('error', 'Vui lòng nhập tên danh mục'); return; }

    // FIX: backend CreateCategoryRequest / UpdateCategoryRequest yêu cầu categoryName (không phải name)
    const data = {
        categoryName: categoryName,
        description: description
    };

    const cb = function (err) {
        if (err) { showToast('error', err.message); return; }
        showToast('success', 'Lưu thành công');
        bootstrap.Modal.getInstance('#catModal').hide();
        loadCategories();
    };

    if (editId) CategoryAPI.update(editId, data, cb);
    else CategoryAPI.create(data, cb);
}

function confirmDelete() {
    CategoryAPI.delete(deleteId, function (err) {
        if (err) { showToast('error', err.message); return; }
        showToast('success', 'Đã xóa');
        bootstrap.Modal.getInstance('#deleteModal').hide();
        loadCategories();
    });
}