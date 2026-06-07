const BASE_URL = '/api';

function getAuthHeaders() {
    const headers = { 'Content-Type': 'application/json' };
    const token = localStorage.getItem('accessToken');
    if (token) {
        headers['Authorization'] = 'Bearer ' + token;
    }
    return headers;
}

function handleApiError(xhr, callback) {
    const res = xhr.responseJSON || {};
    const message = res.message || res.error || 'Lỗi không xác định';

    if (xhr.status === 400) {
        if (res.errors) {
            callback({ errors: res.errors, message }, null);
        } else {
            callback({ message: message || 'Dữ liệu không hợp lệ' }, null);
        }
    } else if (xhr.status === 401) {
        handleTokenRefresh(function () {
            callback({ message: 'Phiên đăng nhập hết hạn' }, null);
        });
    } else if (xhr.status === 403) {
        callback({ message: 'Bạn không có quyền thực hiện thao tác này' }, null);
    } else if (xhr.status === 404) {
        callback({ message: 'Không tìm thấy dữ liệu' }, null);
    } else if (xhr.status === 409) {
        callback({ message: message || 'Dữ liệu đã tồn tại' }, null);
    } else {
        callback({ message: 'Lỗi hệ thống. Vui lòng thử lại sau' }, null);
    }
}

let isRefreshing = false;
let refreshQueue = [];

function handleTokenRefresh(onFail) {
    const refreshToken = localStorage.getItem('refreshToken');
    if (!refreshToken || isRefreshing) {
        clearAuth();
        if (onFail) onFail();
        window.location.href = '/pages/login/index.html';
        return;
    }

    isRefreshing = true;
    $.ajax({
        url: BASE_URL + '/auth/refresh',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ refreshToken }),
        success: function (res) {
            isRefreshing = false;
            if (res.data && res.data.accessToken) {
                localStorage.setItem('accessToken', res.data.accessToken);
                refreshQueue.forEach(function (cb) { cb(); });
                refreshQueue = [];
            } else {
                clearAuth();
                window.location.href = '/pages/login/index.html';
            }
        },
        error: function () {
            isRefreshing = false;
            clearAuth();
            if (onFail) onFail();
            window.location.href = '/pages/login/index.html';
        }
    });
}

function apiCall(method, endpoint, data, callback) {
    const settings = {
        url: BASE_URL + endpoint,
        method: method,
        headers: getAuthHeaders(),
        success: function (res) {
            callback(null, res);
        },
        error: function (xhr) {
            if (xhr.status === 401 && localStorage.getItem('refreshToken')) {
                refreshQueue.push(function () {
                    apiCall(method, endpoint, data, callback);
                });
                handleTokenRefresh(function () {
                    handleApiError(xhr, callback);
                });
                return;
            }
            if (xhr.status === 401) {
                clearAuth();
                window.location.href = '/pages/login/index.html';
                return;
            }
            handleApiError(xhr, callback);
        }
    };

    if (data !== null && data !== undefined) {
        if (method === 'GET') {
            settings.data = data;
        } else {
            settings.data = JSON.stringify(data);
        }
    }

    $.ajax(settings);
}

function apiGet(endpoint, params, callback) {
    apiCall('GET', endpoint, params || null, callback);
}

function apiPost(endpoint, data, callback) {
    apiCall('POST', endpoint, data, callback);
}

function apiPut(endpoint, data, callback) {
    apiCall('PUT', endpoint, data, callback);
}

function apiPatch(endpoint, data, callback) {
    apiCall('PATCH', endpoint, data, callback);
}

function apiDelete(endpoint, callback) {
    apiCall('DELETE', endpoint, null, callback);
}

const AuthAPI = {
    login: function (data, cb) { apiPost('/auth/login', data, cb); },
    register: function (data, cb) { apiPost('/auth/register', data, cb); },
    logout: function (cb) { apiPost('/auth/logout', {}, cb); },
    resetPassword: function (data, cb) { apiPut('/auth/reset-password', data, cb); },
    forgotPassword: function (data, cb) { apiPost('/auth/forgot-password', data, cb); }
};

const BookAPI = {
    getList: function (params, cb) { apiGet('/books', params, cb); },
    getById: function (id, cb) { apiGet('/books/' + id, null, cb); },
    create: function (data, cb) { apiPost('/books', data, cb); },
    update: function (id, data, cb) { apiPut('/books/' + id, data, cb); },
    delete: function (id, cb) { apiDelete('/books/' + id, cb); }
};

const BookCopyAPI = {
    getList: function (params, cb) { apiGet('/book-copies', params, cb); },
    getById: function (id, cb) { apiGet('/book-copies/' + id, null, cb); },
    create: function (bookId, data, cb) { apiPost('/book-copies/book/' + bookId, data, cb); },
    updateStatus: function (id, params, cb) {
        $.ajax({
            url: BASE_URL + '/book-copies/' + id + '/status',
            method: 'PATCH',
            headers: getAuthHeaders(),
            data: params,
            success: function (res) { cb(null, res); },
            error: function (xhr) { handleApiError(xhr, cb); }
        });
    },
    update: function (id, params, cb) { apiPut('/book-copies/' + id, params, cb); },
    delete: function (id, cb) { apiDelete('/book-copies/' + id, cb); }
};

const CategoryAPI = {
    getList: function (cb) { apiGet('/categories', null, cb); },
    getById: function (id, cb) { apiGet('/categories/' + id, null, cb); },
    create: function (data, cb) { apiPost('/categories', data, cb); },
    update: function (id, data, cb) { apiPut('/categories/' + id, data, cb); },
    delete: function (id, cb) { apiDelete('/categories/' + id, cb); }
};

const AuthorAPI = {
    getList: function (params, cb) { apiGet('/authors', params, cb); },
    getById: function (id, cb) { apiGet('/authors/' + id, null, cb); },
    create: function (data, cb) { apiPost('/authors', data, cb); },
    update: function (id, data, cb) { apiPut('/authors/' + id, data, cb); },
    delete: function (id, cb) { apiDelete('/authors/' + id, cb); }
};

const PublisherAPI = {
    getList: function (params, cb) { apiGet('/publishers', params, cb); },
    getById: function (id, cb) { apiGet('/publishers/' + id, null, cb); },
    create: function (data, cb) { apiPost('/publishers', data, cb); },
    update: function (id, data, cb) { apiPut('/publishers/' + id, data, cb); },
    delete: function (id, cb) { apiDelete('/publishers/' + id, cb); }
};

const UserAPI = {
    getList: function (params, cb) { apiGet('/users', params, cb); },
    getStaffs: function (params, cb) { apiGet('/users/staffs', params, cb); },
    getById: function (id, cb) { apiGet('/users/' + id, null, cb); },
    update: function (id, data, cb) { apiPut('/users/' + id, data, cb); },
    updateProfile: function (data, cb) { apiPut('/users/me/profile', data, cb); },
    updateStatus: function (id, data, cb) { apiPatch('/users/' + id + '/status', data, cb); },
    updateCardStatus: function (id, data, cb) { apiPatch('/users/' + id + '/card-status', data, cb); },
    createStaff: function (data, cb) { apiPost('/users/staff', data, cb); },
    getBorrowRecords: function (userId, params, cb) { apiGet('/users/' + userId + '/borrow-records', params, cb); },
    getFines: function (userId, params, cb) { apiGet('/users/' + userId + '/fines', params, cb); }
};

const BorrowAPI = {
    getList: function (params, cb) { apiGet('/borrow-records', params, cb); },
    getById: function (id, cb) { apiGet('/borrow-records/' + id, null, cb); },
    create: function (data, cb) { apiPost('/borrow-records', data, cb); },
    return: function (id, data, cb) { apiPatch('/borrow-records/' + id + '/return', data, cb); },
    cancel: function (id, cb) { apiPatch('/borrow-records/' + id + '/cancel', {}, cb); },
    extend: function (id, cb) { apiPatch('/borrow-records/' + id + '/extend', {}, cb); },
    requestExtension: function (id, cb) { apiPost('/borrow-records/' + id + '/extension-requests', {}, cb); }
};

const FineAPI = {
    getList: function (params, cb) { apiGet('/fines', params, cb); },
    getById: function (id, cb) { apiGet('/fines/' + id, null, cb); },
    create: function (data, cb) { apiPost('/fines', data, cb); },
    pay: function (id, cb) { apiPatch('/fines/' + id + '/pay', {}, cb); }
};

const ReservationAPI = {
    getList: function (params, cb) { apiGet('/reservations', params, cb); },
    create: function (data, cb) { apiPost('/reservations', data, cb); },
    cancel: function (id, cb) { apiPatch('/reservations/' + id + '/cancel', {}, cb); },
    complete: function (id, cb) { apiPatch('/reservations/' + id + '/complete', {}, cb); }
};

const NotificationAPI = {
    getList: function (params, cb) { apiGet('/notifications', params, cb); },
    markRead: function (id, cb) { apiPatch('/notifications/' + id + '/read', {}, cb); },
    markAllRead: function (cb) { apiPatch('/notifications/read-all', {}, cb); }
};

const StatAPI = {
    overview: function (cb) { apiGet('/statistic/overviews', null, cb); },
    overdue: function (cb) { apiGet('/statistic/overdue', null, cb); },
    topBooks: function (top, cb) { apiGet('/statistic/top-books', { top: top || 10 }, cb); }
};
