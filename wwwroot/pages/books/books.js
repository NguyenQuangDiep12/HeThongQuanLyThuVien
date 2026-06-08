let currentPage = 1;
const pageSize = 10;
let editBookId = null;
let categories = [], authors = [], publishers = [];

$(document).ready(function () {
    if (!checkAuth('STAFF')) return;
    initLayout('Quản lý sách');
    loadMeta();
    loadBooks();

    $('#searchForm').on('submit', function (e) { e.preventDefault(); currentPage = 1; loadBooks(); });
    $('#btnReset').on('click', function () { $('#filterKeyword').val(''); $('#filterCategory').val(''); currentPage = 1; loadBooks(); });
    $('#btnCreate').on('click', openCreateModal);
    $('#btnSaveBook').on('click', saveBook);
    $('#bookModal').on('hidden.bs.modal', function () { clearFormErrors('bookForm'); $('#bookForm')[0].reset(); editBookId = null; });
    $('#btnConfirmDelete').on('click', confirmDelete);
});

function loadMeta() {
    // GET /categories — trả về List<CategoryResponse> trực tiếp trong res.data
    CategoryAPI.getList(function (err, res) {
        if (!err && res.data) {
            categories = res.data;  // FIX: res.data là array trực tiếp, KHÔNG phải res.data.items
        }
        let filterOpts = '<option value="">-- Danh mục --</option>';
        let formOpts = '';
        categories.forEach(function (c) {
            filterOpts += '<option value="' + c.categoryId + '">' + escapeHtml(c.categoryName) + '</option>';
            formOpts += '<option value="' + c.categoryId + '">' + escapeHtml(c.categoryName) + '</option>';
        });
        $('#filterCategory').html(filterOpts);
        $('#bookCategories').html(formOpts);
    });

    // GET /authors — trả về List<AuthorResponse> trực tiếp trong res.data (không có pagination/items)
    AuthorAPI.getList(null, function (err, res) {
        if (!err && res.data) {
            // FIX: GET /authors trả về List trực tiếp, KHÔNG phải paginated object với .items
            authors = Array.isArray(res.data) ? res.data : (res.data.items || []);
        }
        $('#bookAuthors').html(authors.map(function (a) {
            return '<option value="' + a.authorId + '">' + escapeHtml(a.authorName) + '</option>';
        }).join(''));
    });

    // GET /publishers — trả về List<PublisherResponse> trực tiếp trong res.data (không có pagination/items)
    PublisherAPI.getList(null, function (err, res) {
        if (!err && res.data) {
            // FIX: GET /publishers trả về List trực tiếp, KHÔNG phải paginated object với .items
            publishers = Array.isArray(res.data) ? res.data : (res.data.items || []);
        }
        let opts = '<option value="">-- Chọn NXB --</option>';
        publishers.forEach(function (p) {
            opts += '<option value="' + p.publisherId + '">' + escapeHtml(p.publisherName) + '</option>';
        });
        $('#bookPublisher').html(opts);
    });
}

function loadBooks() {
    const params = { page: currentPage, pageSize: pageSize };
    const kw = $('#filterKeyword').val().trim();
    if (kw) params.keyword = kw;
    const cat = $('#filterCategory').val();
    if (cat) params.categoryId = cat;

    BookAPI.getList(params, function (err, res) {
        if (err) { showToast('error', err.message); return; }
        const data = res.data;
        renderTable(data.items || []);
        updateShowingCount((data.items || []).length, data.totalRecords || 0);
        renderPagination('pagination', data.totalRecords || 0, currentPage, pageSize, function (p) { currentPage = p; loadBooks(); });
    });
}

function renderTable(items) {
    let html = '';
    items.forEach(function (b, i) {
        html += '<tr>';
        html += '<td>' + ((currentPage - 1) * pageSize + i + 1) + '</td>';
        html += '<td><div class="fw-semibold">' + escapeHtml(b.title) + '</div><small class="text-muted">' + escapeHtml(b.isbn) + '</small></td>';
        html += '<td class="text-center"><span class="' + (b.availableCopies > 0 ? 'text-success' : 'text-danger') + ' fw-semibold">' + b.availableCopies + '</span><span class="text-muted">/' + b.totalCopies + '</span></td>';
        html += '<td class="text-center">';
        html += '<a href="/pages/copies/index.html?bookId=' + b.bookId + '" class="btn btn-sm btn-outline-secondary me-1" title="Quản lý bản sao"><i class="bi bi-layers"></i></a>';
        html += '<button class="btn btn-sm btn-outline-info me-1 btn-view" data-id="' + b.bookId + '" title="Xem chi tiết"><i class="bi bi-eye"></i></button>';
        html += '<button class="btn btn-sm btn-outline-warning me-1 btn-edit" data-id="' + b.bookId + '" title="Chỉnh sửa"><i class="bi bi-pencil"></i></button>';
        if (isAdmin()) html += '<button class="btn btn-sm btn-outline-danger btn-delete" data-id="' + b.bookId + '" data-name="' + escapeHtml(b.title) + '" title="Xóa"><i class="bi bi-trash"></i></button>';
        html += '</td></tr>';
    });
    $('#tableBody').html(html || '<tr><td colspan="4" class="text-center text-muted py-4">Không có dữ liệu</td></tr>');

    $('.btn-view').on('click', function () { window.location.href = '/pages/book-search/detail.html?bookId=' + $(this).data('id'); });
    $('.btn-edit').on('click', function () { openEditModal($(this).data('id')); });
    $('.btn-delete').on('click', function () { openDeleteConfirm($(this).data('id'), $(this).data('name')); });
}

function openCreateModal() {
    editBookId = null;
    $('#bookModalTitle').text('Thêm đầu sách');
    $('#bookForm')[0].reset();
    new bootstrap.Modal('#bookModal').show();
}

function openEditModal(id) {
    BookAPI.getById(id, function (err, res) {
        if (err) { showToast('error', err.message); return; }
        const b = res.data;
        editBookId = id;
        $('#bookModalTitle').text('Sửa đầu sách');
        $('#bookTitle').val(b.title);
        $('#bookIsbn').val(b.isbn);
        $('#bookLanguage').val(b.language);
        $('#bookDescription').val(b.description);
        $('#bookCover').val(b.coverImage);
        if (b.publisher) $('#bookPublisher').val(b.publisher.publisherId);
        const authorIds = (b.authors || []).map(function (a) { return String(a.authorId); });
        const catIds = (b.categories || []).map(function (c) { return String(c.categoryId); });
        $('#bookAuthors').val(authorIds);
        $('#bookCategories').val(catIds);
        new bootstrap.Modal('#bookModal').show();
    });
}

function saveBook() {
    clearFormErrors('bookForm');
    const title = $('#bookTitle').val().trim();
    const isbn = $('#bookIsbn').val().trim();
    const publisherId = parseInt($('#bookPublisher').val());
    const authorIds = $('#bookAuthors').val() ? $('#bookAuthors').val().map(Number) : [];
    const categoryIds = $('#bookCategories').val() ? $('#bookCategories').val().map(Number) : [];

    if (!title) { showFieldError('bookTitle', 'Vui lòng nhập tên sách'); return; }
    if (!isbn) { showFieldError('bookIsbn', 'Vui lòng nhập ISBN'); return; }
    if (!publisherId) { showToast('error', 'Vui lòng chọn nhà xuất bản'); return; }
    if (!authorIds.length) { showToast('error', 'Vui lòng chọn ít nhất 1 tác giả'); return; }
    if (!categoryIds.length) { showToast('error', 'Vui lòng chọn ít nhất 1 danh mục'); return; }

    const data = {
        title: title,
        isbn: isbn,
        language: $('#bookLanguage').val().trim() || 'vi',
        description: $('#bookDescription').val().trim() || '',
        coverImage: $('#bookCover').val().trim() || 'https://via.placeholder.com/200x300',
        publisherId: publisherId,
        authorIds: authorIds,
        categoryIds: categoryIds
    };

    showLoading('btnSaveBook');
    const cb = function (err) {
        hideLoading('btnSaveBook');
        if (err) {
            if (err.errors) showApiFieldErrors(err.errors);
            else showToast('error', err.message);
            return;
        }
        showToast('success', editBookId ? 'Cập nhật thành công' : 'Thêm sách thành công');
        bootstrap.Modal.getInstance('#bookModal').hide();
        loadBooks();
    };
    if (editBookId) BookAPI.update(editBookId, data, cb);
    else BookAPI.create(data, cb);
}

let deleteBookId = null;
function openDeleteConfirm(id, name) {
    deleteBookId = id;
    $('#deleteItemName').text(name);
    new bootstrap.Modal('#deleteModal').show();
}
function confirmDelete() {
    BookAPI.delete(deleteBookId, function (err) {
        if (err) { showToast('error', err.message); return; }
        showToast('success', 'Đã xóa sách');
        bootstrap.Modal.getInstance('#deleteModal').hide();
        loadBooks();
    });
}