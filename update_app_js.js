const fs = require('fs');
let js = fs.readFileSync('Frontend/store/app.js', 'utf8');

// Update renderProducts
const oldRender = `
            <div class="col-sm-6 col-md-4 col-lg-3">
                <div class="card h-100 product-card">
                    <img src="\${p.imageUrl || 'https://via.placeholder.com/300?text=No+Image'}" class="card-img-top product-img" alt="\${p.name}">
                    <div class="card-body d-flex flex-column">
                        <h6 class="card-title fw-bold text-truncate" title="\${p.name}">\${p.name}</h6>
                        <p class="card-text text-muted small flex-grow-1">\${p.description ? p.description.substring(0,60) + '...' : ''}</p>
                        <h5 class="text-primary mb-3">$\${p.price.toFixed(2)}</h5>
                        <button class="btn btn-primary w-100 mt-auto" onclick="addToCart(\${p.id}, '\${p.name.replace(/'/g, "\\\\'")}', \${p.price}, '\${p.imageUrl || ''}')">
                            <i class="bi bi-cart-plus"></i> Agregar
                        </button>
                    </div>
                </div>
            </div>`;

const newRender = `
            <div class="col-sm-6 col-md-6 col-lg-4 mb-3">
                <div class="product-card h-100 d-flex flex-column">
                    <div class="product-img-wrapper">
                        <img src="\${p.imageUrl ? 'https://billingsystem-net10pg.onrender.com' + p.imageUrl : 'https://via.placeholder.com/300x300?text=Sin+Imagen'}" class="product-img" alt="\${p.name}">
                    </div>
                    <div class="p-4 d-flex flex-column flex-grow-1">
                        <h3 class="product-title text-truncate" title="\${p.name}">\${p.name}</h3>
                        <p class="small text-muted mb-3 flex-grow-1" style="line-height: 1.4;">\${p.description ? p.description.substring(0,60) + '...' : ''}</p>
                        <div class="d-flex justify-content-between align-items-center mt-auto">
                            <span class="product-price">$\${p.price.toFixed(2)}</span>
                            <button class="btn-minimal" onclick="addToCart(\${p.id}, '\${p.name.replace(/'/g, "\\\\'")}', \${p.price}, '\${p.imageUrl || ''}')">
                                <i class="bi bi-plus-lg"></i> Agregar
                            </button>
                        </div>
                    </div>
                </div>
            </div>`;

js = js.replace(oldRender, newRender);

// Wait, the original render had text-primary, let's just do a regex replace to be safe.
// Because the template literal inside my string might not match exactly.
js = js.replace(/<div class="col-sm-6 col-md-4 col-lg-3">[\s\S]*?<\/div>\s*<\/div>\s*<\/div>/, newRender);

// Update loadCategories to use buttons instead of list-group-item
js = js.replace(
    /let html = '<li class="list-group-item active-category" onclick="filterByCategory\(0\)">Todas<\/li>';\s*items\.forEach\(c => \{\s*html \+= `<li class="list-group-item" onclick="filterByCategory\(\$\{c\.id\}\)">\$\{c\.name\}<\/li>`;\s*\}\);/,
    `let html = '<button class="category-item text-start active-category" onclick="filterByCategory(0)">Todos los Productos</button>';
        items.forEach(c => {
            html += \`<button class="category-item text-start" onclick="filterByCategory(\${c.id})">\${c.name}</button>\`;
        });`
);

// Add fetchSettings function
const fetchSettingsCode = `
async function loadStoreName() {
    try {
        const res = await fetch('https://billingsystem-net10pg.onrender.com/api/Settings');
        if (res.ok) {
            const settings = await res.json();
            if (settings && settings.companyName) {
                const el = document.getElementById('storeNameBrand');
                if (el) el.innerText = settings.companyName;
                document.title = settings.companyName + ' - Tienda en Línea';
            }
        }
    } catch (e) {
        console.log("No se pudo cargar el nombre de la tienda");
    }
}
`;

if(!js.includes('loadStoreName')) {
    js = js + '\n' + fetchSettingsCode;
    js = js.replace(/initStore\(\);/, 'initStore();\n    loadStoreName();');
}

fs.writeFileSync('Frontend/store/app.js', js);
console.log("store/app.js updated");
