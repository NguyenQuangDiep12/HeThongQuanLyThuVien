let currentPage = 1;
const pageSize = 12;
let categories = [];

$(document).ready(function () {
    if (!checkAuth('READER')) return;
    initLayout('Tra cứu sách');

    CategoryAPI.getList(function (err, res) {
        if (!err && res.data) {
            categories = res.data;
            let opts = '<option value="">-- Tất cả danh mục --</option>';
            categories.forEach(function (c) {
                opts += '<option value="' + c.categoryId + '">' + escapeHtml(c.categoryName) + '</option>';
            });
            $('#filterCategory').html(opts);
        }
        loadBooks();
    });

    $('#searchForm').on('submit', function (e) {
        e.preventDefault();
        currentPage = 1;
        loadBooks();
    });

    $('#btnReset').on('click', function () {
        $('#filterKeyword').val('');
        $('#filterCategory').val('');
        $('#filterAuthor').val('');
        currentPage = 1;
        loadBooks();
    });
});

function loadBooks() {
    const params = { page: currentPage, pageSize: pageSize };
    const kw = $('#filterKeyword').val().trim();
    if (kw) params.keyword = kw;
    const cat = $('#filterCategory').val();
    if (cat) params.categoryId = cat;

    $('#bookGrid').html('<div class="col-12 text-center py-5"><div class="spinner-border text-primary"></div></div>');

    BookAPI.getList(params, function (err, res) {
        if (err) { showToast('error', err.message); return; }
        const data = res.data;
        const items = data.items || [];
        renderBooks(items);
        updateShowingCount(items.length, data.totalRecords || 0);
        renderPagination('pagination', data.totalRecords || 0, currentPage, pageSize, function (p) {
            currentPage = p;
            loadBooks();
        });
    });
}

function renderBooks(items) {
    if (!items.length) {
        $('#bookGrid').html('<div class="col-12 text-center text-muted py-5"><i class="bi bi-book" style="font-size:3rem"></i><p class="mt-2">Không tìm thấy sách</p></div>');
        return;
    }
    let html = '';
    items.forEach(function (book) {
        const cover = book.coverImage
            ? '<img src="' + escapeHtml(book.coverImage) + '" class="card-img-top" alt="">'
            : '<div class="placeholder-cover"><i class="bi bi-book"></i></div>';
        const avail = book.availableCopies > 0;
        html += '<div class="col-sm-6 col-md-4 col-lg-3"><div class="card book-card shadow-sm" onclick="location.href=\'detail.html?bookId=' + book.bookId + '\'">';
        html += cover + '<div class="card-body">';
        html += '<h6 class="card-title text-truncate" title="' + escapeHtml(book.title) + '">' + escapeHtml(book.title) + '</h6>';
        html += '<p class="card-text small text-muted text-truncate">' + escapeHtml(book.isbn) + '</p>';
        html += '<div class="d-flex justify-content-between align-items-center">';
        html += '<small class="text-muted">' + book.availableCopies + '/' + book.totalCopies + ' bản</small>';
        html += avail ? '<span class="badge bg-success">Còn sách</span>' : '<span class="badge bg-secondary">Hết sách</span>';
        html += '</div></div></div></div>';
    });
    $('#bookGrid').html(html);
}
