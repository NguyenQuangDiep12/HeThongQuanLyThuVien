let currentPage = 1;
const pageSize = 10;

$(document).ready(function () {
    if (!checkAuth('STAFF')) return;
    initLayout('Mượn / Trả');
    if (window.location.pathname.includes('create.html')) initCreatePage();
    else if (window.location.pathname.includes('detail.html')) initDetailPage();
    else initListPage();
});

function initListPage() {
    loadRecords();
    $('#searchForm').on('submit', function (e) { e.preventDefault(); currentPage = 1; loadRecords(); });
    $('#btnReset').on('click', function () { $('#filterCode').val(''); $('#filterStatus').val(''); currentPage = 1; loadRecords(); });
}

function loadRecords() {
    const params = { page: currentPage, pageSize: pageSize };
    const code = $('#filterCode').val().trim();
    const status = $('#filterStatus').val();
    if (code) params.borrowCode = code;
    if (status) params.status = status;

    BorrowAPI.getList(params, function (err, res) {
        if (err) { showToast('error', err.message); return; }
        const data = res.data;
        let html = '';
        (data.items || []).forEach(function (r) {
            html += '<tr><td>' + escapeHtml(r.borrowCode) + '</td><td>' + escapeHtml(r.readerName) + '</td>';
            html += '<td>' + formatDate(r.borrowDate) + '</td><td>' + formatDate(r.dueDate) + '</td>';
            html += '<td>' + getBadgeStatus(r.status) + '</td>';
            html += '<td class="text-center"><a href="detail.html?borrowId=' + r.borrowId + '" class="btn btn-sm btn-outline-info"><i class="bi bi-eye"></i></a></td></tr>';
        });
        $('#tableBody').html(html || '<tr><td colspan="6" class="text-center text-muted">Không có dữ liệu</td></tr>');
        updateShowingCount((data.items || []).length, data.totalRecords || 0);
        renderPagination('pagination', data.totalRecords || 0, currentPage, pageSize, function (p) { currentPage = p; loadRecords(); });
    });
}

function initDetailPage() {
    const borrowId = getQueryParam('borrowId');
    if (!borrowId) { window.location.href = 'index.html'; return; }

    BorrowAPI.getById(borrowId, function (err, res) {
        if (err) { showToast('error', err.message); return; }
        const r = res.data;
        $('#borrowCode').text(r.borrowCode);
        $('#readerName').text(r.readerName);
        $('#borrowDate').text(formatDate(r.borrowDate));
        $('#dueDate').text(formatDate(r.dueDate));
        $('#status').html(getBadgeStatus(r.status));
        $('#extensionCount').text(r.extensionCount + '/2');

        let html = '';
        (r.borrowDetails || []).forEach(function (d) {
            html += '<tr><td>' + escapeHtml(d.bookTitle) + '</td><td>' + escapeHtml(d.barcode) + '</td><td>' + getBadgeStatus(d.status) + '</td></tr>';
        });
        $('#detailsBody').html(html);

        if (r.status === 'BORROWING' || r.status === 'OVERDUE') {
            $('#btnReturn').show().on('click', function () { openReturnModal(r); });
            $('#btnExtend').show().on('click', function () {
                confirmAction('Gia hạn', 'Xác nhận gia hạn phiếu mượn này?', function () {
                    BorrowAPI.extend(borrowId, function (err2) {
                        if (err2) showToast('error', err2.message);
                        else { showToast('success', 'Gia hạn thành công'); location.reload(); }
                    });
                });
            });
        }
    });
}

function openReturnModal(record) {
    let html = '';
    (record.borrowDetails || []).forEach(function (d) {
        if (d.returnedAt) return;
        html += '<div class="border rounded p-3 mb-2"><strong>' + escapeHtml(d.bookTitle) + '</strong> (' + escapeHtml(d.barcode) + ')';
        html += '<input type="hidden" class="copy-id" value="' + d.copyId + '">';
        html += '<div class="mt-2"><label class="form-label small">Tình trạng</label><select class="form-select form-select-sm item-condition">';
        html += '<option value="NORMAL">Bình thường</option><option value="TORN">Rách</option><option value="DAMAGED">Hư hỏng</option><option value="LOST">Mất</option></select></div>';
        html += '<div class="mt-1"><input type="text" class="form-control form-control-sm item-note" placeholder="Ghi chú"></div></div>';
    });
    $('#returnItems').html(html);
    new bootstrap.Modal('#returnModal').show();
    $('#btnConfirmReturn').off('click').on('click', function () {
        const items = [];
        $('#returnItems .border').each(function () {
            items.push({
                copyId: parseInt($(this).find('.copy-id').val()),
                condition: $(this).find('.item-condition').val(),
                copyStatus: $(this).find('.item-condition').val() === 'LOST' ? 'UNAVAILABLE' : 'AVAILABLE',
                fineReason: $(this).find('.item-note').val() || null
            });
        });
        BorrowAPI.return(record.borrowId, { returnItems: items }, function (err) {
            if (err) { showToast('error', err.message); return; }
            showToast('success', 'Trả sách thành công');
            bootstrap.Modal.getInstance('#returnModal').hide();
            location.reload();
        });
    });
}

let createStep = 1, selectedReader = null, selectedCopies = [];

function initCreatePage() {
    $('#btnNext').on('click', nextStep);
    $('#btnPrev').on('click', prevStep);
    $('#btnSearchReader').on('click', searchReader);
    $('#btnSearchBook').on('click', searchBooks);
    $('#btnSubmitBorrow').on('click', submitBorrow);
}

function nextStep() {
    if (createStep === 1 && !selectedReader) { showToast('error', 'Vui lòng tìm và chọn độc giả'); return; }
    if (createStep === 2 && selectedCopies.length === 0) { showToast('error', 'Vui lòng chọn ít nhất 1 bản sao'); return; }
    if (createStep === 2 && selectedCopies.length > 3) { showToast('error', 'Tối đa 3 sách/lượt'); return; }
    createStep++;
    updateCreateSteps();
}

function prevStep() { if (createStep > 1) { createStep--; updateCreateSteps(); } }

function updateCreateSteps() {
    $('.step').removeClass('active done');
    for (let i = 1; i < createStep; i++) $('.step[data-step="' + i + '"]').addClass('done');
    $('.step[data-step="' + createStep + '"]').addClass('active');
    $('.create-panel').hide();
    $('#step' + createStep).show();
    $('#btnPrev').toggle(createStep > 1);
    $('#btnNext').toggle(createStep < 3);
    $('#btnSubmitBorrow').toggle(createStep === 3);
    if (createStep === 3) renderConfirm();
}

function searchReader() {
    const name = $('#readerSearch').val().trim();
    UserAPI.getList({ fullName: name, roleName: 'READER', page: 1, pageSize: 10 }, function (err, res) {
        if (err) { showToast('error', err.message); return; }
        const items = res.data.items || [];
        if (!items.length) { $('#readerResult').html('<div class="alert alert-warning">Không tìm thấy</div>'); return; }
        const u = items[0];
        selectedReader = u;
        UserAPI.getById(u.id, function (err2, res2) {
            const detail = res2 ? res2.data : u;
            let warn = '';
            if (detail.status !== 'ACTIVE') warn = '<div class="alert alert-danger">Tài khoản không hoạt động</div>';
            else if (detail.cardStatus !== 'ACTIVE') warn = '<div class="alert alert-danger">Thẻ thư viện không hợp lệ</div>';
            $('#readerResult').html(warn + '<div class="card"><div class="card-body"><h6>' + escapeHtml(detail.fullName) + '</h6><p class="mb-0 small">Email: ' + escapeHtml(detail.email) + '<br>Thẻ: ' + escapeHtml(detail.libraryCardCode || '—') + ' ' + getBadgeStatus(detail.cardStatus) + '</p></div></div>');
        });
    });
}

function searchBooks() {
    BookAPI.getList({ keyword: $('#bookSearch').val().trim(), page: 1, pageSize: 20 }, function (err, res) {
        if (err) return;
        const books = (res.data.items || []).filter(function (b) { return b.availableCopies > 0; });
        let html = '';
        books.forEach(function (b) {
            html += '<div class="list-group-item"><div class="d-flex justify-content-between"><span>' + escapeHtml(b.title) + ' (' + b.availableCopies + ' có sẵn)</span>';
            html += '<button class="btn btn-sm btn-outline-primary btn-pick-book" data-id="' + b.bookId + '">Chọn bản sao</button></div></div>';
        });
        $('#bookResults').html(html || '<div class="text-muted p-3">Không có sách khả dụng</div>');
        $('.btn-pick-book').on('click', function () { loadCopiesForBook($(this).data('id')); });
    });
}

function loadCopiesForBook(bookId) {
    BookCopyAPI.getList({ bookId: bookId, status: 'AVAILABLE', page: 1, pageSize: 20 }, function (err, res) {
        if (err) return;
        let html = '<div class="mt-2"><strong>Chọn bản sao:</strong>';
        (res.data.items || []).forEach(function (c) {
            const checked = selectedCopies.find(function (x) { return x.copyId === c.copyId; });
            html += '<div class="form-check"><input class="form-check-input copy-check" type="checkbox" value="' + c.copyId + '" data-title="' + escapeHtml(c.bookTitle) + '" data-barcode="' + escapeHtml(c.barcode) + '"' + (checked ? ' checked' : '') + '>';
            html += '<label class="form-check-label">' + escapeHtml(c.barcode) + ' - ' + escapeHtml(c.shelfLocation) + '</label></div>';
        });
        html += '</div>';
        $('#copyPicker').html(html);
        $('.copy-check').on('change', function () {
            const id = parseInt($(this).val());
            if ($(this).is(':checked')) {
                if (selectedCopies.length >= 3) { $(this).prop('checked', false); showToast('warning', 'Tối đa 3 sách'); return; }
                selectedCopies.push({ copyId: id, title: $(this).data('title'), barcode: $(this).data('barcode') });
            } else {
                selectedCopies = selectedCopies.filter(function (x) { return x.copyId !== id; });
            }
            $('#selectedCount').text(selectedCopies.length);
        });
    });
}

function renderConfirm() {
    $('#confirmReader').text(selectedReader ? selectedReader.fullName : '—');
    let html = '<ul class="mb-0">';
    selectedCopies.forEach(function (c) { html += '<li>' + escapeHtml(c.title) + ' (' + escapeHtml(c.barcode) + ')</li>'; });
    html += '</ul>';
    $('#confirmBooks').html(html);
}

function submitBorrow() {
    if (!selectedReader || !selectedCopies.length) return;
    showLoading('btnSubmitBorrow');
    BorrowAPI.create({
        readerId: selectedReader.id,
        borrowType: 'TakeHome',
        copyIds: selectedCopies.map(function (c) { return c.copyId; })
    }, function (err, res) {
        hideLoading('btnSubmitBorrow');
        if (err) { showToast('error', err.message); return; }
        showToast('success', 'Tạo phiếu ' + res.data.borrowCode + ' thành công');
        setTimeout(function () { window.location.href = 'detail.html?borrowId=' + res.data.borrowId; }, 1000);
    });
}
