const fs = require('fs');
let html = fs.readFileSync('Frontend/pages/reports.html', 'utf-8');

const oldHeader = `<header class="topbar bg-white border-bottom">
                <div class="d-flex align-items-center gap-3">
                    <button class="btn btn-light d-lg-none" id="sidebarToggle"><i class="bi bi-list"></i></button>
                    <h5 class="mb-0 fw-bold text-dark"><i class="bi bi-briefcase-fill text-primary"></i> MÃ³dulo Contable y Reportes Empresariales</h5>
                </div>
                <div class="d-flex align-items-center gap-4">
                    <!-- ... Theme Toggle & Notifs ... -->
                    
                    <!-- Theme toggle -->
                    <div class="theme-toggle me-3">
                        <input type="checkbox" id="themeSwitch" class="theme-switch-input">
                        <label for="themeSwitch" class="theme-switch-label">
                            <i class="bi bi-sun text-warning"></i>
                            <i class="bi bi-moon text-light"></i>
                        </label>
                    </div>
                    <div class="dropdown">
                        <button class="btn border-0 bg-transparent text-body p-2" type="button" data-bs-toggle="dropdown" onclick="loadNotifications()">
                            <div class="position-relative d-inline-block">
                                <i class="bi bi-bell fs-5"></i>
                                <span class="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger" id="notif-badge" style="display:none; font-size: 0.6em; transform: translate(-30%, -30%) !important;"></span>
                            </div>
                        </button>
                        <ul class="dropdown-menu dropdown-menu-end shadow-sm" style="width: 300px; max-height: 400px; overflow-y: auto;" id="notif-list">
                            <li><h6 class="dropdown-header">Notificaciones Pendientes</h6></li>
                        </ul>
                    </div>
                </div>
            </header>

            <div class="page-content p-4">`;

const newHeader = `<header class="topbar">
                <div class="d-flex align-items-center gap-3">
                    <button class="btn btn-light d-lg-none" id="sidebarToggle"><i class="bi bi-list"></i></button>
                </div>
                <div class="d-flex align-items-center gap-4">
                    <div class="dropdown">
                        <button class="btn border-0 bg-transparent text-body p-2" type="button" data-bs-toggle="dropdown" onclick="loadNotifications()">
                            <div class="position-relative d-inline-block">
                                <i class="bi bi-bell fs-5"></i>
                                <span class="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger" id="notif-badge" style="display:none; font-size: 0.6em; transform: translate(-30%, -30%) !important;"></span>
                            </div>
                        </button>
                        <ul class="dropdown-menu dropdown-menu-end shadow-sm" style="width: 300px; max-height: 400px; overflow-y: auto;" id="notif-list">
                            <li><h6 class="dropdown-header">Notificaciones Pendientes</h6></li>
                        </ul>
                    </div>
                    <div class="form-check form-switch m-0">
                        <input class="form-check-input" type="checkbox" role="switch" id="theme-toggle">
                        <label class="form-check-label" for="theme-toggle"><i class="bi bi-moon-stars"></i></label>
                    </div>
                </div>
            </header>

            <div class="page-content p-4">
                <h2 class="mb-4 fw-bold text-dark"><i class="bi bi-briefcase-fill text-primary me-2"></i> Módulo Contable y Reportes Empresariales</h2>`;

html = html.replace(oldHeader, newHeader);
fs.writeFileSync('Frontend/pages/reports.html', html, 'utf-8');
