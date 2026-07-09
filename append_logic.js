const fs = require('fs');
let js = fs.readFileSync('Frontend/store/app.js', 'utf8');

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

if (!js.includes('function showProductModal')) {
    js += '\n' + newLogic;
}

// Make sure event.stopPropagation() is in the button so it doesn't open modal when clicking "Agregar"
js = js.replace(
    /onclick="addToCart\(\$\{p\.id\}, '\\\$\{p\.name\.replace\(\/'\/g, "\\\\\\\\'"\)\}', \$\{p\.price\}, '\\\$\{p\.imageUrl \|\| ''\}'\)"/g,
    `onclick="event.stopPropagation(); addToCart(\${p.id}, '\${p.name.replace(/'/g, "\\\\'")}', \${p.price}, '\${p.imageUrl || ''}')"`
);

fs.writeFileSync('Frontend/store/app.js', js);
console.log("Appended missing functions to app.js");
