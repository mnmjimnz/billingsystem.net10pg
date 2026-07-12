const fs = require('fs');

function fix(file) {
    let content = fs.readFileSync(file, 'utf8');
    content = content.replace(/nav-item dropdown me-3/g, 'nav-item dropdown me-lg-3 mb-2 mb-lg-0');
    content = content.replace(/nav-item me-3 d-flex/g, 'nav-item me-lg-3 mb-2 mb-lg-0 d-flex');
    content = content.replace(/nav-item me-3" id="authMenu"/g, 'nav-item me-lg-3 mb-2 mb-lg-0" id="authMenu"');
    fs.writeFileSync(file, content);
}

fix('Frontend/store/index.html');
fix('Frontend/store/cart.html');
