const fs = require('fs');
let html = fs.readFileSync('Frontend/pages/notifications.html', 'utf-8');

const newHeader = `
            <header class="topbar">
                <div class="d-flex align-items-center gap-3">
                    <button class="btn btn-light d-lg-none" id="sidebarToggle"><i class="bi bi-list"></i></button>
                    <h2 class="mb-0 fs-4 fw-bold text-dark d-none d-md-block">Historial de Notificaciones</h2>
                </div>
                <div class="d-flex align-items-center gap-4">
                    <div class="dropdown me-3">
                        <button class="btn border-0 bg-transparent text-body p-2" type="button" data-bs-toggle="dropdown" onclick="loadNotifications()">
                            <div class="position-relative d-inline-block">
                                <i class="bi bi-bell fs-5"></i>
                                <span class="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger" id="notif-badge" style="display:none; font-size: 0.6em; transform: translate(-30%, -30%) !important;"></span>
                            </div>
                        </button>
                        <ul class="dropdown-menu dropdown-menu-end shadow-sm" style="width: 300px; max-height: 400px; overflow-y: auto;" id="notif-list">
                            <li><h6 class="dropdown-header">Notificaciones Pendientes</h6></li>
                            <!-- Loaded via JS -->
                        </ul>
                    </div>
                    <div class="form-check form-switch m-0">
                        <input class="form-check-input" type="checkbox" role="switch" id="theme-toggle">
                        <label class="form-check-label" for="theme-toggle"><i class="bi bi-moon-stars"></i></label>
                    </div>
                </div>
            </header>`;

const regex = /<header class="app-header">[\s\S]*?<\/header>/;
if (regex.test(html)) {
    html = html.replace(regex, newHeader.trim());
    fs.writeFileSync('Frontend/pages/notifications.html', html, 'utf-8');
    console.log("Fixed notifications layout");
} else {
    console.log("Could not find app-header");
}
