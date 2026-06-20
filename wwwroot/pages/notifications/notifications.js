let currentPage = 1;
const pageSize = 20;

$(document).ready(function () {
    if (!checkAuth('READER')) return;
    initLayout('Thông báo');
    loadNotifications();
    $('#btnMarkAll').on('click', function () {
        NotificationAPI.markAllRead(function (err) {
            if (err) showToast('error', err.message);
            else { showToast('success', 'Đã đánh dấu tất cả'); loadNotifications(); loadNotificationBadge(); }
        });
    });
});

function loadNotifications() {
    NotificationAPI.getList({ page: currentPage, pageSize: pageSize }, function (err, res) {
        if (err) { showToast('error', err.message); return; }
        const data = res.data;
        let html = '';
        (data.items || []).forEach(function (n) {
            html += '<div class="list-group-item ' + (n.isRead ? '' : 'list-group-item-primary') + '">';
            html += '<div class="d-flex justify-content-between align-items-start">';
            html += '<div><h6 class="mb-1">' + escapeHtml(n.title) + '</h6><p class="mb-1 small">' + escapeHtml(n.content || '') + '</p>';
            html += '<small class="text-muted">' + formatDateTime(n.createdAt) + '</small></div>';
            if (!n.isRead) {
                html += '<button class="btn btn-sm btn-outline-primary btn-read" data-id="' + n.notificationId + '"><i class="bi bi-check"></i></button>';
            }
            html += '</div></div>';
        });
        $('#notifList').html(html || '<div class="text-center text-muted py-4">Không có thông báo</div>');
        updateShowingCount((data.items||[]).length, data.totalRecords||0);
        renderPagination('pagination', data.totalRecords||0, currentPage, pageSize, function(p){currentPage=p;loadNotifications();});
        $('.btn-read').on('click', function () {
            NotificationAPI.markRead($(this).data('id'), function (err2) {
                if (!err2) loadNotifications();
            });
        });
    });
}
