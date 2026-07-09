const fs = require('fs');

let transfersHtml = fs.readFileSync('Frontend/pages/stock-transfers.html', 'utf-8');

transfersHtml = transfersHtml.replace(
    '<div class="content-wrapper p-4">',
    `<div class="page-content p-4">
        <h2 class="mb-4 fw-bold text-dark">Traslados de Sucursal</h2>
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h4 class="fw-bold mb-0 text-secondary">Historial de Traslados</h4>
            <button class="btn btn-primary shadow-sm rounded-pill px-4 fw-semibold" onclick="openTransferModal()">
                <i class="bi bi-truck me-2"></i> Nuevo Traslado
            </button>
        </div>`
);
transfersHtml = transfersHtml.replace('</main>', '</div></main>');

fs.writeFileSync('Frontend/pages/stock-transfers.html', transfersHtml, 'utf-8');

let js = fs.readFileSync('Frontend/pages/stock-transfers.js', 'utf-8');
js = js.replace(
    'const sidebarHtml = await fetch(\'../components/sidebar.html\').then(r => r.text());\n        document.getElementById(\'sidebar-container\').innerHTML = sidebarHtml;\n        const profileHtml = await fetch(\'../components/user-profile.html\').then(r => r.text());\n        document.getElementById(\'user-profile-container\').innerHTML = profileHtml;',
    ''
);
js = js.replace('await loadLayout();', 'initSidebar();');
js = js.replace(
    'async function loadLayout() {',
    'function initSidebar() {'
);
fs.writeFileSync('Frontend/pages/stock-transfers.js', js, 'utf-8');
console.log("Fixed stock-transfers button and JS");
