const fs = require('fs');
let js = fs.readFileSync('Frontend/store/app.js', 'utf8');

// 1. Theme Logic
const themeLogic = `
function toggleTheme() {
    const html = document.documentElement;
    const currentTheme = html.getAttribute('data-bs-theme');
    const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
    html.setAttribute('data-bs-theme', newTheme);
    localStorage.setItem('storeTheme', newTheme);
    updateThemeIcon(newTheme);
}

function applyTheme() {
    const savedTheme = localStorage.getItem('storeTheme') || 'light'; // default to light for minimalist look
    document.documentElement.setAttribute('data-bs-theme', savedTheme);
    updateThemeIcon(savedTheme);
}

function updateThemeIcon(theme) {
    const icon = document.getElementById('themeIcon');
    if (icon) {
        if (theme === 'dark') {
            icon.className = 'bi bi-sun-fill';
        } else {
            icon.className = 'bi bi-moon-stars-fill';
        }
    }
}
`;

if (!js.includes('toggleTheme')) {
    js += '\n' + themeLogic;
    js = js.replace(/function initStore\(\) \{/, 'function initStore() {\n    applyTheme();');
}

// 2. Product Modal Logic
const modalLogic = `
function showProductModal(id) {
    // find the product in global products array (loaded by loadProducts)
    // Wait, the products are just fetched and rendered, we don't store them globally?
    // Let's modify loadProducts to store them, or just fetch again.
    // It's better to store them globally.
}
`;
// Let's check if there is a global products array.
