const fs = require('fs');
const path = require('path');

const pagesDir = 'Frontend/pages';
const files = fs.readdirSync(pagesDir).filter(f => f.endsWith('.html'));

const sidebarFooterRegex = /<div class="sidebar-footer">[\s\S]*?<\/div>/;
const sidebarHeaderRegex = /<a href="#" class="sidebar-brand">[\s\S]*?<\/a>/;

const newSidebarHeader = `<div class="d-flex w-100 justify-content-between align-items-center pe-3">
                <a href="#" class="sidebar-brand mb-0">
                    <i class="bi bi-hexagon-fill"></i> <span id="brand-name">Nexus POS</span>
                </a>
                <button class="btn p-1 text-danger border-0" onclick="logout()" title="Cerrar Sesión">
                    <i class="bi bi-power fs-4"></i>
                </button>
            </div>`;

for (const file of files) {
    const filePath = path.join(pagesDir, file);
    let html = fs.readFileSync(filePath, 'utf8');

    // Remove sidebar footer
    html = html.replace(sidebarFooterRegex, '');

    // Replace sidebar brand with brand + logout
    html = html.replace(sidebarHeaderRegex, newSidebarHeader);

    // Specific to reports.html
    if (file === 'reports.html') {
        const themeToggleHtml = `
                    <!-- Theme toggle -->
                    <div class="theme-toggle me-3">
                        <input type="checkbox" id="themeSwitch" class="theme-switch-input">
                        <label for="themeSwitch" class="theme-switch-label">
                            <i class="bi bi-sun text-warning"></i>
                            <i class="bi bi-moon text-light"></i>
                        </label>
                    </div>
                    <div class="dropdown">`;
        html = html.replace('<div class="dropdown">', themeToggleHtml);
    }

    // Specific to purchases.html
    if (file === 'purchases.html') {
        // Add ID to cart container
        html = html.replace('<div class="col-lg-4">', '<div class="col-lg-4" id="cart-panel">');
        
        // Add mobile handle
        const mobileHandle = `
                            <div class="mobile-cart-header" onclick="document.getElementById('cart-panel').classList.toggle('expanded')">
                                <div class="mobile-cart-handle"></div>
                                <div class="mobile-cart-title">detalles de compra</div>
                            </div>
                            <div class="card-header`;
        html = html.replace('<div class="card-header', mobileHandle);
    }

    // Specific to pos.html
    if (file === 'pos.html') {
        html = html.replace('<div class="col-lg-4">', '<div class="col-lg-4" id="cart-panel">');
        
        const mobileHandle = `
                            <div class="mobile-cart-header" onclick="document.getElementById('cart-panel').classList.toggle('expanded')">
                                <div class="mobile-cart-handle"></div>
                                <div class="mobile-cart-title">detalles de venta</div>
                            </div>
                            <div class="card-header`;
        html = html.replace('<div class="card-header', mobileHandle);
    }

    fs.writeFileSync(filePath, html, 'utf8');
}

// Update CSS
const cssPath = 'Frontend/assets/css/devextreme-theme.css';
let css = fs.readFileSync(cssPath, 'utf8');

const mobileCartCss = `
/* Mobile Bottom Sheet for Cart */
@media (max-width: 991.98px) {
    #cart-panel {
        position: fixed;
        bottom: 0;
        left: 0;
        width: 100%;
        z-index: 1050;
        background: var(--dx-bg-color);
        border-top-left-radius: 20px;
        border-top-right-radius: 20px;
        box-shadow: 0 -5px 20px rgba(0,0,0,0.15);
        transform: translateY(calc(100% - 70px));
        transition: transform 0.3s cubic-bezier(0.25, 0.8, 0.25, 1);
        height: 85vh;
        display: flex;
        flex-direction: column;
        padding: 0;
    }
    #cart-panel.expanded {
        transform: translateY(0);
    }
    #cart-panel > .card {
        height: 100%;
        border: none !important;
        border-radius: 0;
        box-shadow: none !important;
        background: transparent;
        overflow-y: auto;
    }
    .mobile-cart-header {
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        min-height: 70px;
        cursor: pointer;
        background: var(--dx-bg-color);
        border-top-left-radius: 20px;
        border-top-right-radius: 20px;
        border-bottom: 1px solid var(--dx-border-color);
    }
    .mobile-cart-handle {
        width: 40px;
        height: 5px;
        background: #000;
        border-radius: 5px;
        margin-bottom: 10px;
    }
    [data-bs-theme="dark"] .mobile-cart-handle {
        background: #fff;
    }
    .mobile-cart-title {
        font-weight: bold;
        font-size: 1.1rem;
        color: var(--dx-text-color);
    }
    /* En móviles, la lista de productos debe verse como cuadricula si es necesario (ya usa col-md-6) */
}
@media (min-width: 992px) {
    .mobile-cart-header { display: none; }
}
`;

if (!css.includes('Mobile Bottom Sheet for Cart')) {
    fs.writeFileSync(cssPath, css + '\n' + mobileCartCss, 'utf8');
}
