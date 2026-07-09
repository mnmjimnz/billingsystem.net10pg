const fs = require('fs');
const path = require('path');

const pagesDir = 'Frontend/pages';
const files = fs.readdirSync(pagesDir).filter(f => f.endsWith('.html'));

const bellHTML = `
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
`;

files.forEach(file => {
    let html = fs.readFileSync(path.join(pagesDir, file), 'utf-8');
    let changed = false;

    // Remove existing bell if any, to avoid duplicates
    // We can use a regex to match the <div class="dropdown"... to </ul></div>
    const bellRegex = /<div class="dropdown[^>]*>\s*<button[^>]*onclick="loadNotifications\(\)"[\s\S]*?<\/ul>\s*<\/div>/g;
    if (bellRegex.test(html)) {
        html = html.replace(bellRegex, '');
        changed = true;
    }

    // Now insert the bell right before the theme toggle
    const themeToggleRegex = /<div class="form-check form-switch m-0">/g;
    if (themeToggleRegex.test(html)) {
        html = html.replace(themeToggleRegex, bellHTML.trim() + '\n                      <div class="form-check form-switch m-0">');
        changed = true;
    }

    if (changed) {
        fs.writeFileSync(path.join(pagesDir, file), html, 'utf-8');
        console.log(`Updated bell in ${file}`);
    }
});
