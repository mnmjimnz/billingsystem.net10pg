const fs = require('fs');
let html = fs.readFileSync('Frontend/store/cart.html', 'utf8');

// Theme toggle button in navbar
const themeBtn = `
                    <li class="nav-item me-3 d-flex align-items-center">
                        <button class="btn btn-link text-decoration-none p-0" onclick="toggleTheme()" title="Cambiar Tema">
                            <i class="bi bi-moon-stars-fill text-secondary fs-5" id="themeIcon"></i>
                        </button>
                    </li>
`;
if(!html.includes('toggleTheme()')) {
    html = html.replace(
        /<li class="nav-item me-3" id="authMenu">/,
        themeBtn + '\n                    <li class="nav-item me-3" id="authMenu">'
    );
}

fs.writeFileSync('Frontend/store/cart.html', html);
console.log("cart.html updated");
