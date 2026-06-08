let currentPage = 1;
const pageSize = 10;

$(document).ready(function () {
    if (!checkAuth('STAFF')) return;
    initLayout('Mượn / Trả');
    if (window.location.pathname.includes('create.html')) initCreatePage();
    else if (window.location.pathname.includes('detail.html')) initDetailPage();
    else initListPage();
});

/* ========== LIST PAGE ========== */
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
            html += '<tr>';
            html += '<td><code>' + escapeHtml(r.borrowCode) + '</code></td>';
            html += '<td>' + escapeHtml(r.readerName) + '</td>';
            html += '<td>' + formatDate(r.borrowDate) + '</td>';
            html += '<td>' + formatDate(r.dueDate) + '</td>';
            html += '<td>' + getBadgeStatus(r.status) + '</td>';
            html += '<td>' + getBadgeStatus(r.borrowType) + '</td>';
            html += '<td class="text-center"><a href="detail.html?borrowId=' + r.borrowId + '" class="btn btn-sm btn-outline-info" title="Chi tiết"><i class="bi bi-eye"></i></a></td>';
            html += '</tr>';
        });
        $('#tableBody').html(html || '<tr><td colspan="7" class="text-center text-muted">Không có dữ liệu</td></tr>');
        updateShowingCount((data.items || []).length, data.totalRecords || 0);
        renderPagination('pagination', data.totalRecords || 0, currentPage, pageSize, function (p) { currentPage = p; loadRecords(); });
    });
}

/* ========== DETAIL PAGE ========== */
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
        $('#borrowType').html(getBadgeStatus(r.borrowType));
        $('#extensionCount').text(r.extensionCount + '/2');
        if (r.approverName) $('#approverName').text(r.approverName);

        let html = '';
        (r.borrowDetails || []).forEach(function (d) {
            html += '<tr>';
            html += '<td>' + escapeHtml(d.bookTitle) + '</td>';
            html += '<td><code>' + escapeHtml(d.barcode) + '</code></td>';
            html += '<td>' + getBadgeStatus(d.status) + '</td>';
            html += '<td>' + (d.returnedAt ? formatDate(d.returnedAt) : '—') + '</td>';
            html += '</tr>';
        });
        $('#detailsBody').html(html);

        if (r.status === 'BORROWING' || r.status === 'OVERDUE') {
            $('#btnReturn').show().on('click', function () { openReturnModal(r); });
            $('#btnCancel').show().on('click', function () {
                confirmAction('Hủy phiếu mượn', 'Xác nhận hủy phiếu mượn <strong>' + r.borrowCode + '</strong>?', function () {
                    BorrowAPI.cancel(borrowId, function (err2) {
                        if (err2) showToast('error', err2.message);
                        else { showToast('success', 'Đã hủy phiếu mượn'); location.reload(); }
                    });
                });
            });
        }
        if (r.status === 'BORROWING' && r.extensionCount < 2) {
            $('#btnExtend').show().on('click', function () {
                confirmAction('Gia hạn', 'Xác nhận gia hạn phiếu mượn <strong>' + r.borrowCode + '</strong>?', function () {
                    BorrowAPI.extend(borrowId, function (err2) {
                        if (err2) showToast('error', err2.message);
                        else { showToast('success', 'Gia hạn thành công'); location.reload(); }
                    });
                });
            });
        }
    });
}

/* ========== RETURN MODAL ========== */
function openReturnModal(record) {
    let html = '';
    (record.borrowDetails || []).forEach(function (d) {
        if (d.status === 'RETURNED' || d.status === 'LOST' || d.status === 'DAMAGED') return;
        html += '<div class="border rounded p-3 mb-3 return-item">';
        html += '<div class="fw-semibold mb-2"><i class="bi bi-book me-1"></i>' + escapeHtml(d.bookTitle) + '</div>';
        html += '<div class="text-muted small mb-2">Barcode: <code>' + escapeHtml(d.barcode) + '</code></div>';
        html += '<input type="hidden" class="copy-id" value="' + d.copyId + '">';
        html += '<div class="mb-2">';
        html += '<label class="form-label small fw-semibold">Tình trạng vật lý</label>';
        html += '<select class="form-select form-select-sm item-condition">';
        html += '<option value="NORMAL">Bình thường</option>';
        html += '<option value="TORN">Rách</option>';
        html += '<option value="DAMAGED">Hư hỏng</option>';
        html += '<option value="LOST">Mất sách</option>';
        html += '</select></div>';
        // FIX: thêm input fineAmount - backend ReturnItemCondition có trường fineAmount
        html += '<div class="row g-2 fine-fields" style="display:none">';
        html += '<div class="col-md-5"><label class="form-label small">Tiền phạt (VNĐ)</label>';
        html += '<input type="number" class="form-control form-control-sm item-fine" min="0" placeholder="0" value="0"></div>';
        html += '<div class="col-md-7"><label class="form-label small">Lý do phạt</label>';
        html += '<input type="text" class="form-control form-control-sm item-note" placeholder="Lý do..."></div>';
        html += '</div></div>';
    });

    if (!html) {
        html = '<div class="alert alert-info">Tất cả sách đã được xử lý.</div>';
    }

    $('#returnItems').html(html);

    // Hiện/ẩn fine fields khi condition thay đổi
    $(document).off('change', '.item-condition').on('change', '.item-condition', function () {
        const val = $(this).val();
        const fineFields = $(this).closest('.return-item').find('.fine-fields');
        if (val === 'DAMAGED' || val === 'TORN' || val === 'LOST') {
            fineFields.show();
        } else {
            fineFields.hide();
        }
    });

    new bootstrap.Modal('#returnModal').show();

    $('#btnConfirmReturn').off('click').on('click', function () {
        const items = [];
        $('#returnItems .return-item').each(function () {
            const condition = $(this).find('.item-condition').val();
            const fineAmount = parseFloat($(this).find('.item-fine').val()) || 0;
            const fineReason = $(this).find('.item-note').val().trim() || null;
            items.push({
                copyId: parseInt($(this).find('.copy-id').val()),
                condition: condition,
                // copyStatus tự động: LOST/DAMAGED/TORN → UNAVAILABLE, NORMAL → AVAILABLE
                copyStatus: (condition === 'NORMAL') ? 'AVAILABLE' : 'UNAVAILABLE',
                fineAmount: fineAmount > 0 ? fineAmount : null,
                fineReason: fineReason
            });
        });

        showLoading('btnConfirmReturn');
        BorrowAPI.return(record.borrowId, { returnItems: items }, function (err) {
            hideLoading('btnConfirmReturn');
            if (err) { showToast('error', err.message); return; }
            showToast('success', 'Trả sách thành công');
            bootstrap.Modal.getInstance('#returnModal').hide();
            location.reload();
        });
    });
}

/* ========== CREATE PAGE (3 bước) ========== */
let createStep = 1, selectedReader = null, selectedCopies = [];

function initCreatePage() {
    updateCreateSteps();
    $('#btnNext').on('click', nextStep);
    $('#btnPrev').on('click', prevStep);
    $('#btnSearchReader').on('click', searchReader);
    $('#readerSearch').on('keydown', function (e) { if (e.key === 'Enter') { e.preventDefault(); searchReader(); } });
    $('#btnSearchBook').on('click', searchBooks);
    $('#bookSearch').on('keydown', function (e) { if (e.key === 'Enter') { e.preventDefault(); searchBooks(); } });
    $('#btnSubmitBorrow').on('click', submitBorrow);
}

function nextStep() {
    if (createStep === 1 && !selectedReader) { showToast('error', 'Vui lòng tìm và chọn độc giả'); return; }
    if (createStep === 2 && selectedCopies.length === 0) { showToast('error', 'Vui lòng chọn ít nhất 1 bản sao'); return; }
    if (createStep === 2 && selectedCopies.length > 3) { showToast('error', 'Tối đa 3 sách/lượt mượn'); return; }
    createStep++;
    updateCreateSteps();
}

function prevStep() {
    if (createStep > 1) { createStep--; updateCreateSteps(); }
}

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
    if (!name) { showToast('warning', 'Nhập tên để tìm kiếm'); return; }

    UserAPI.getList({ fullName: name, page: 1, pageSize: 10 }, function (err, res) {
        if (err) { showToast('error', err.message); return; }
        const items = (res.data && res.data.items) || [];
        if (!items.length) { $('#readerResult').html('<div class="alert alert-warning">Không tìm thấy độc giả nào</div>'); return; }

        let html = '<div class="list-group">';
        items.forEach(function (u) {
            html += '<button type="button" class="list-group-item list-group-item-action btn-select-reader" data-id="' + u.id + '">';
            html += '<div class="fw-semibold">' + escapeHtml(u.fullName) + '</div>';
            html += '<small class="text-muted">' + escapeHtml(u.email) + ' — ' + getBadgeStatus(u.status) + ' — Thẻ: ' + getBadgeStatus(u.cardStatus) + '</small>';
            html += '</button>';
        });
        html += '</div>';
        $('#readerResult').html(html);

        $('.btn-select-reader').on('click', function () {
            const uid = $(this).data('id');
            selectReader(uid, items.find(function (x) { return x.id === uid; }));
        });
    });
}

function selectReader(uid, basicInfo) {
    selectedReader = basicInfo;
    UserAPI.getById(uid, function (err2, res2) {
        const detail = (res2 && res2.data) ? res2.data : basicInfo;
        let warn = '';
        if (detail.status !== 'ACTIVE') warn += '<div class="alert alert-danger mb-2"><i class="bi bi-exclamation-triangle me-1"></i>Tài khoản đang bị khóa</div>';
        if (detail.cardStatus && detail.cardStatus !== 'ACTIVE') warn += '<div class="alert alert-danger mb-2"><i class="bi bi-exclamation-triangle me-1"></i>Thẻ thư viện không hợp lệ (' + detail.cardStatus + ')</div>';
        $('#readerResult').html(
            warn +
            '<div class="card border-success"><div class="card-body">' +
            '<div class="d-flex align-items-center gap-3">' +
            '<div class="rounded-circle bg-success text-white d-flex align-items-center justify-content-center fw-bold" style="width:48px;height:48px;font-size:1.3rem">' + (detail.fullName || '?').charAt(0).toUpperCase() + '</div>' +
            '<div><h6 class="mb-0">' + escapeHtml(detail.fullName || '') + '</h6>' +
            '<div class="small text-muted">' + escapeHtml(detail.email || '') + '</div>' +
            '<div class="small mt-1">Mã thẻ: <code>' + escapeHtml(detail.libraryCardCode || '—') + '</code> ' + getBadgeStatus(detail.cardStatus) + '</div>' +
            '</div></div></div></div>'
        );
        selectedReader = { id: uid, fullName: detail.fullName, email: detail.email };
    });
}

function searchBooks() {
    const kw = $('#bookSearch').val().trim();
    BookAPI.getList({ keyword: kw, page: 1, pageSize: 20 }, function (err, res) {
        if (err) return;
        const books = ((res.data && res.data.items) || []).filter(function (b) { return b.availableCopies > 0; });
        if (!books.length) {
            $('#bookResults').html('<div class="text-muted p-3 text-center">Không có sách khả dụng</div>');
            $('#copyPicker').html('');
            return;
        }
        let html = '';
        books.forEach(function (b) {
            html += '<div class="list-group-item d-flex justify-content-between align-items-center">';
            html += '<div><div class="fw-semibold">' + escapeHtml(b.title) + '</div>';
            html += '<small class="text-muted">ISBN: ' + escapeHtml(b.isbn) + ' — Còn: ' + b.availableCopies + ' bản</small></div>';
            html += '<button class="btn btn-sm btn-outline-primary btn-pick-book" data-id="' + b.bookId + '">Chọn bản sao</button>';
            html += '</div>';
        });
        $('#bookResults').html(html);
        $('.btn-pick-book').on('click', function () { loadCopiesForBook($(this).data('id')); });
    });
}

function loadCopiesForBook(bookId) {
    BookCopyAPI.getList({ bookId: bookId, status: 'AVAILABLE', page: 1, pageSize: 50 }, function (err, res) {
        if (err) return;
        const copies = (res.data && res.data.items) || [];
        if (!copies.length) { $('#copyPicker').html('<div class="alert alert-warning mt-2">Không còn bản sao nào khả dụng</div>'); return; }

        let html = '<div class="mt-3 p-3 bg-light rounded"><strong class="d-block mb-2">Chọn bản sao:</strong>';
        copies.forEach(function (c) {
            const checked = selectedCopies.find(function (x) { return x.copyId === c.copyId; });
            html += '<div class="form-check">';
            html += '<input class="form-check-input copy-check" type="checkbox" value="' + c.copyId + '" data-title="' + escapeHtml(c.bookTitle) + '" data-barcode="' + escapeHtml(c.barcode) + '"' + (checked ? ' checked' : '') + '>';
            html += '<label class="form-check-label"><code>' + escapeHtml(c.barcode) + '</code>';
            if (c.shelfLocation) html += ' — ' + escapeHtml(c.shelfLocation);
            html += '</label></div>';
        });
        html += '</div>';
        $('#copyPicker').html(html);

        $('.copy-check').on('change', function () {
            const id = parseInt($(this).val());
            if ($(this).is(':checked')) {
                if (selectedCopies.length >= 3) { $(this).prop('checked', false); showToast('warning', 'Tối đa 3 sách/lượt mượn'); return; }
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
    let html = '<ul class="list-group">';
    selectedCopies.forEach(function (c) {
        html += '<li class="list-group-item"><i class="bi bi-book me-2 text-primary"></i>' + escapeHtml(c.title) + ' <code class="ms-1">(' + escapeHtml(c.barcode) + ')</code></li>';
    });
    html += '</ul>';
    $('#confirmBooks').html(html);
    $('#confirmBorrowType').html(getBadgeStatus($('#borrowTypeSelect').val()));
}

function submitBorrow() {
    if (!selectedReader || !selectedCopies.length) { showToast('error', 'Thiếu thông tin'); return; }
    showLoading('btnSubmitBorrow');

    // FIX: BorrowType enum phải là TAKEHOME / READINGONSITE (uppercase) - không phải TakeHome
    const borrowType = $('#borrowTypeSelect').val() || 'TAKEHOME';

    BorrowAPI.create({
        readerId: selectedReader.id,
        borrowType: borrowType,  // TAKEHOME hoặc READINGONSITE
        copyIds: selectedCopies.map(function (c) { return c.copyId; })
    }, function (err, res) {
        hideLoading('btnSubmitBorrow');
        if (err) { showToast('error', err.message); return; }
        const data = res.data;
        showToast('success', 'Tạo phiếu ' + data.borrowCode + ' thành công!');
        setTimeout(function () { window.location.href = 'detail.html?borrowId=' + data.borrowId; }, 1200);
    });
}