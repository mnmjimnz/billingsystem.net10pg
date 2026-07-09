const fs = require('fs');
let html = fs.readFileSync('Frontend/store/index.html', 'utf8');

// Update Navbar
html = html.replace(
    /<nav class="navbar navbar-expand-lg navbar-dark bg-dark sticky-top shadow-sm">/,
    '<nav class="navbar navbar-expand-lg sticky-top navbar-minimal">'
);

html = html.replace(
    /<a class="navbar-brand fw-bold text-primary" href="index.html">\s*<i class="bi bi-shop me-2"><\/i>Nexus Store\s*<\/a>/,
    `<a class="navbar-brand brand-title" href="index.html">
                <i class="bi bi-bag-check me-2"></i><span id="storeNameBrand">Tienda</span>
            </a>`
);

html = html.replace(
    /<form class="d-flex mx-auto w-50" id="searchForm" onsubmit="event.preventDefault\(\); loadProducts\(1\);">\s*<div class="input-group">\s*<input type="text" class="form-control" id="searchInput" placeholder="Buscar productos...">\s*<button class="btn btn-primary" type="submit"><i class="bi bi-search"><\/i><\/button>\s*<\/div>\s*<\/form>/,
    `<form class="d-flex mx-auto w-50" id="searchForm" onsubmit="event.preventDefault(); loadProducts(1);">
                    <div class="input-group search-minimal">
                        <input type="text" class="form-control px-4 py-2" id="searchInput" placeholder="Buscar productos...">
                        <button class="btn px-4" type="submit"><i class="bi bi-search"></i></button>
                    </div>
                </form>`
);

html = html.replace(
    /<a class="btn btn-outline-light btn-sm" href="login.html">Iniciar Sesin<\/a>/,
    '<a class="btn btn-outline-minimal" href="login.html">Iniciar Sesión</a>'
);

html = html.replace(
    /<a class="btn btn-primary position-relative" href="cart.html">/,
    '<a class="btn btn-minimal position-relative ms-3" href="cart.html">'
);

html = html.replace(
    /text-light me-2/g,
    'me-2'
);

// Update Sidebar Categories
const oldSidebar = `<div class="col-md-3 mb-4">
                <div class="card shadow-sm border-0">
                    <div class="card-header bg-primary text-white fw-semibold">
                        Categoras
                    </div>
                    <ul class="list-group list-group-flush" id="categoryList">
                        <li class="list-group-item active-category" onclick="filterByCategory(0)">Todas</li>
                    </ul>
                </div>
            </div>`;

const newSidebar = `<div class="col-md-3 mb-4">
                <div class="pe-md-4">
                    <h6 class="text-uppercase fw-bold text-muted mb-3" style="letter-spacing: 1px; font-size: 0.75rem;">Categorías</h6>
                    <div class="d-flex flex-column category-list" id="categoryList">
                        <button class="category-item text-start active-category" onclick="filterByCategory(0)">Todos los Productos</button>
                    </div>
                </div>
            </div>`;
            
html = html.replace(oldSidebar, newSidebar);

// Add API base script to HTML before app.js
if(!html.includes('const API_BASE_URL')) {
    html = html.replace('<script src="app.js"></script>', `<script>
        const API_BASE_URL = 'https://billingsystem-net10pg.onrender.com/api';
    </script>
    <script src="app.js"></script>`);
}

fs.writeFileSync('Frontend/store/index.html', html);
console.log("store/index.html updated");
