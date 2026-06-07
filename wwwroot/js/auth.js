function getCurrentUser() {
    try {
        return JSON.parse(localStorage.getItem('currentUser') || 'null');
    } catch {
        return null;
    }
}

function clearAuth() {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('currentUser');
}

function saveAuth(accessToken, refreshToken, user) {
    localStorage.setItem('accessToken', accessToken);
    if (refreshToken) {
        localStorage.setItem('refreshToken', refreshToken);
    }
    localStorage.setItem('currentUser', JSON.stringify(user));
}

function getRoleRedirect(role) {
    switch (role) {
        case 'ADMIN': return '/pages/dashboard/index.html';
        case 'STAFF': return '/pages/borrow-records/index.html';
        case 'READER': return '/pages/book-search/index.html';
        default: return '/pages/login/index.html';
    }
}

function checkAuth(requiredRole) {
    const token = localStorage.getItem('accessToken');
    const user = getCurrentUser();

    if (!token || !user) {
        window.location.href = '/pages/login/index.html';
        return false;
    }

    if (requiredRole && user.role !== requiredRole) {
        if (!(requiredRole === 'STAFF' && user.role === 'ADMIN')) {
            window.location.href = '/pages/unauthorized/index.html';
            return false;
        }
    }

    return true;
}

function logout() {
    AuthAPI.logout(function () {
        clearAuth();
        window.location.href = '/pages/login/index.html';
    });
}

function login(email, password, onSuccess, onError) {
    AuthAPI.login({ email, password }, function (err, res) {
        if (err) {
            if (onError) onError(err);
            return;
        }
        const data = res.data;
        const userInfo = data.userInfo || data.user;
        saveAuth(data.accessToken, data.refreshToken, {
            userId: userInfo.userId,
            fullName: userInfo.fullName,
            email: userInfo.email,
            role: userInfo.role,
            avatarUrl: userInfo.avatarUrl || null
        });
        if (onSuccess) onSuccess(userInfo);
    });
}

function isAdmin() {
    const user = getCurrentUser();
    return user && user.role === 'ADMIN';
}

function isStaffOrAdmin() {
    const user = getCurrentUser();
    return user && (user.role === 'STAFF' || user.role === 'ADMIN');
}
