const fs = require('fs');

// UPDATE CART.HTML
let html = fs.readFileSync('Frontend/store/cart.html', 'utf8');

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
    /<a class="btn btn-outline-light btn-sm" href="login.html">Iniciar Sesin<\/a>/,
    '<a class="btn btn-outline-minimal" href="login.html">Iniciar Sesión</a>'
);

html = html.replace(
    /text-light me-2/g,
    'me-2'
);

// Minimalist layout replacements in cart.html
html = html.replace(
    /<div class="card shadow-sm border-0 bg-dark text-light">/,
    '<div class="card cart-card border-0">'
);
html = html.replace(
    /<div class="card shadow-sm border-0 bg-dark text-light">\s*<div class="card-body">/,
    '<div class="card cart-card border-0">\n<div class="card-body">'
);
// Make sure second card is updated too (for summary)
html = html.replace(
    /<div class="card shadow-sm border-0 bg-dark text-light">/g,
    '<div class="card cart-card border-0">'
);
html = html.replace(
    /<table class="table table-dark table-hover mb-0">/,
    '<table class="table table-borderless align-middle mb-0">'
);
html = html.replace(
    /<button class="btn btn-primary w-100 btn-lg"/,
    '<button class="btn btn-minimal w-100 btn-lg"'
);
html = html.replace(
    /class="text-primary mb-0"/,
    'class="mb-0 fw-bold"'
);
html = html.replace(
    /class="modal-content bg-dark text-light"/,
    'class="modal-content"'
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
                        document.title = settings.companyName + ' - Carrito';
                    }
                }
            } catch (e) {
                console.log("No se pudo cargar el nombre de la tienda");
            }
        }
`;

if(!html.includes('loadStoreName')) {
    html = html.replace('<script>', `<script>
${fetchSettingsCode}`);
    html = html.replace(/initStore\(\);/, 'initStore();\n            loadStoreName();');
}

fs.writeFileSync('Frontend/store/cart.html', html);


// UPDATE APP.JS rendering
let js = fs.readFileSync('Frontend/store/app.js', 'utf8');

const oldCartRender = `
        items.forEach((item, index) => {
            html += \`
                <tr>
                    <td style="width: 80px;"><img src="\${item.imageUrl || 'https://via.placeholder.com/60'}" class="img-fluid rounded" alt="\${item.name}"></td>
                    <td>\${item.name}</td>
                    <td>$\${item.price.toFixed(2)}</td>
                    <td style="width: 150px;">
                        <div class="input-group input-group-sm">
                            <button class="btn btn-outline-secondary" onclick="updateQty(\${index}, -1)">-</button>
                            <input type="text" class="form-control text-center" value="\${item.qty}" readonly>
                            <button class="btn btn-outline-secondary" onclick="updateQty(\${index}, 1)">+</button>
                        </div>
                    </td>
                    <td class="text-end fw-bold">$\${(item.price * item.qty).toFixed(2)}</td>
                    <td class="text-center">
                        <button class="btn btn-danger btn-sm" onclick="removeItem(\${index})"><i class="bi bi-trash"></i></button>
                    </td>
                </tr>
            \`;
        });
`;

const newCartRender = `
        items.forEach((item, index) => {
            html += \`
                <tr class="cart-item">
                    <td style="width: 80px;"><img src="\${item.imageUrl ? 'https://billingsystem-net10pg.onrender.com' + item.imageUrl : 'https://via.placeholder.com/60'}" class="cart-item-img" alt="\${item.name}"></td>
                    <td class="fw-semibold">\${item.name}</td>
                    <td class="text-muted">$\${item.price.toFixed(2)}</td>
                    <td style="width: 150px;">
                        <div class="input-group input-group-sm rounded-pill overflow-hidden border">
                            <button class="btn btn-light border-0 px-3" onclick="updateQty(\${index}, -1)">-</button>
                            <input type="text" class="form-control text-center border-0 bg-transparent" value="\${item.qty}" readonly>
                            <button class="btn btn-light border-0 px-3" onclick="updateQty(\${index}, 1)">+</button>
                        </div>
                    </td>
                    <td class="text-end fw-bold">$\${(item.price * item.qty).toFixed(2)}</td>
                    <td class="text-center">
                        <button class="btn btn-outline-danger border-0 btn-sm rounded-circle" onclick="removeItem(\${index})"><i class="bi bi-trash"></i></button>
                    </td>
                </tr>
            \`;
        });
`;

// Simple string replacement
js = js.replace(oldCartRender, newCartRender);

// Also update sweetalerts to avoid the dark theme colors if they were hardcoded
js = js.replace(/background:\s*'#343a40'/g, `background: '#fff'`);
js = js.replace(/color:\s*'#fff'/g, `color: '#212529'`);

fs.writeFileSync('Frontend/store/app.js', js);
console.log("cart.html and app.js updated");
