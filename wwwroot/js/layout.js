const NAV_ITEMS = {
    ADMIN: [
        { href: '/pages/dashboard/index.html', icon: 'bi-speedometer2', label: 'Dashboard' },
        { href: '/pages/borrow-records/index.html', icon: 'bi-journal-arrow-up', label: 'Mượn / Trả' },
        { href: '/pages/books/index.html', icon: 'bi-book', label: 'Quản lý sách' },
        { href: '/pages/categories/index.html', icon: 'bi-tags', label: 'Danh mục' },
        { href: '/pages/authors/index.html', icon: 'bi-pencil', label: 'Tác giả' },
        { href: '/pages/publishers/index.html', icon: 'bi-building', label: 'Nhà xuất bản' },
        { href: '/pages/users/index.html', icon: 'bi-people', label: 'Độc giả' },
        { href: '/pages/fines/index.html', icon: 'bi-cash-coin', label: 'Vi phạm' },
        { href: '/pages/reservations/index.html', icon: 'bi-bookmark-star', label: 'Đặt trước' },
        { href: '/pages/staff/index.html', icon: 'bi-person-badge', label: 'Nhân viên' }
    ],
    STAFF: [
        { href: '/pages/borrow-records/index.html', icon: 'bi-journal-arrow-up', label: 'Mượn / Trả' },
        { href: '/pages/books/index.html', icon: 'bi-book', label: 'Quản lý sách' },
        { href: '/pages/categories/index.html', icon: 'bi-tags', label: 'Danh mục' },
        { href: '/pages/authors/index.html', icon: 'bi-pencil', label: 'Tác giả' },
        { href: '/pages/publishers/index.html', icon: 'bi-building', label: 'Nhà xuất bản' },
        { href: '/pages/users/index.html', icon: 'bi-people', label: 'Độc giả' },
        { href: '/pages/fines/index.html', icon: 'bi-cash-coin', label: 'Vi phạm' },
        { href: '/pages/reservations/index.html', icon: 'bi-bookmark-star', label: 'Đặt trước' }
    ],
    READER: [
        { href: '/pages/book-search/index.html', icon: 'bi-search', label: 'Tra cứu sách' },
        { href: '/pages/my-borrows/index.html', icon: 'bi-journal-text', label: 'Lịch sử mượn' },
        { href: '/pages/notifications/index.html', icon: 'bi-bell', label: 'Thông báo' }
    ]
};

function initLayout(pageTitle) {
    const user = getCurrentUser();
    if (!user) return;

    $('#pageTitle').text(pageTitle);
    $('#userName').text(user.fullName);
    $('#userRole').text(user.role);
    $('#userAvatar').text(user.fullName.charAt(0).toUpperCase());

    const items = NAV_ITEMS[user.role] || NAV_ITEMS.READER;
    const currentPath = window.location.pathname;
    let navHtml = '';

    items.forEach(function (item) {
        const active = currentPath === item.href || currentPath.startsWith(item.href.replace('index.html', '')) ? ' active' : '';
        navHtml += '<a class="nav-link' + active + '" href="' + item.href + '">';
        navHtml += '<i class="bi ' + item.icon + ' me-2"></i>' + item.label + '</a>';
    });

    navHtml += '<a class="nav-link' + (currentPath.includes('/profile/') ? ' active' : '') + '" href="/pages/profile/index.html">';
    navHtml += '<i class="bi bi-person-circle me-2"></i>Cá nhân</a>';

    $('#sidebarNav').html(navHtml);

    $('#btnLogout').on('click', function () { logout(); });

    if (user.role === 'READER') {
        loadNotificationBadge();
        setInterval(loadNotificationBadge, 60000);
        $('#notificationBell').show();
        $('#notificationBell').on('click', function () {
            window.location.href = '/pages/notifications/index.html';
        });
    }

    $('#sidebarToggle').on('click', function () {
        $('#sidebar').toggleClass('collapsed');
        $('.main-content').toggleClass('expanded');
    });
}

function loadNotificationBadge() {
    NotificationAPI.getList({ page: 1, pageSize: 100 }, function (err, res) {
        if (err || !res.data) return;
        const unread = (res.data.items || []).filter(function (n) { return !n.isRead; }).length;
        const badge = $('#notificationBadge');
        if (unread > 0) {
            badge.text(unread > 99 ? '99+' : unread).show();
        } else {
            badge.hide();
        }
    });
}

function getCommonModalsHtml() {
    return '<div class="modal fade" id="confirmModal" tabindex="-1">' +
        '<div class="modal-dialog modal-sm"><div class="modal-content">' +
        '<div class="modal-header"><h5 class="modal-title" id="confirmModalTitle">Xác nhận</h5>' +
        '<button type="button" class="btn-close" data-bs-dismiss="modal"></button></div>' +
        '<div class="modal-body" id="confirmModalBody"></div>' +
        '<div class="modal-footer">' +
        '<button type="button" class="btn btn-secondary btn-sm" data-bs-dismiss="modal">Hủy</button>' +
        '<button type="button" class="btn btn-danger btn-sm" id="btnConfirmAction">Xác nhận</button>' +
        '</div></div></div></div>';
}
