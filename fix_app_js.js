const fs = require('fs');
let js = fs.readFileSync('Frontend/store/app.js', 'utf8');

// Store products globally
js = js.replace(
    /const data = await response\.json\(\);\s*renderProducts\(data\.items\);/,
    `const data = await response.json();
            products = data.items;
            renderProducts(data.items);`
);

// Update renderProducts to add click event to card
js = js.replace(
    /<div class="product-card h-100 d-flex flex-column">/,
    `<div class="product-card h-100 d-flex flex-column" style="cursor:pointer;" onclick="showProductModal(\${p.id})">`
);

// Prevent 'Agregar' button from triggering card click
js = js.replace(
    /onclick="addToCart\(\$\{p\.id\}, '\\\$\{p\.name\.replace\(\/'\/g, "\\\\\\\\'"\)\}', \$\{p\.price\}, '\\\$\{p\.imageUrl \|\| ''\}'\)"/,
    `onclick="event.stopPropagation(); addToCart(\${p.id}, '\${p.name.replace(/'/g, "\\\\'")}', \${p.price}, '\${p.imageUrl || ''}')"`
);

const newLogic = `
function showProductModal(id) {
    const p = products.find(x => x.id === id);
    if (!p) return;
    
    document.getElementById('modalProductTitle').innerText = p.name;
    document.getElementById('modalProductDesc').innerText = p.description || 'Sin descripción';
    document.getElementById('modalProductPrice').innerText = '$' + p.price.toFixed(2);
    document.getElementById('modalProductImg').src = p.imageUrl ? 'https://billingsystem-net10pg.onrender.com' + p.imageUrl : 'https://via.placeholder.com/400x400?text=Sin+Imagen';
    
    // Set up add to cart button inside modal
    const btn = document.getElementById('modalAddToCartBtn');
    btn.onclick = () => {
        addToCart(p.id, p.name, p.price, p.imageUrl || '');
        bootstrap.Modal.getInstance(document.getElementById('productDetailModal')).hide();
    };

    const modal = new bootstrap.Modal(document.getElementById('productDetailModal'));
    modal.show();
}

function toggleTheme() {
    const html = document.documentElement;
    const currentTheme = html.getAttribute('data-bs-theme');
    const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
    html.setAttribute('data-bs-theme', newTheme);
    localStorage.setItem('storeTheme', newTheme);
    updateThemeIcon(newTheme);
}

function applyTheme() {
    const savedTheme = localStorage.getItem('storeTheme') || 'light';
    document.documentElement.setAttribute('data-bs-theme', savedTheme);
    updateThemeIcon(savedTheme);
}

function updateThemeIcon(theme) {
    const icon = document.getElementById('themeIcon');
    if (icon) {
        if (theme === 'dark') {
            icon.className = 'bi bi-sun-fill text-warning';
        } else {
            icon.className = 'bi bi-moon-stars-fill text-secondary';
        }
    }
}
`;

if (!js.includes('showProductModal')) {
    js += '\n' + newLogic;
}

fs.writeFileSync('Frontend/store/app.js', js);
console.log("app.js updated");
