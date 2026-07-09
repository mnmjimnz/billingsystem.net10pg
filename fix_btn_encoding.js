const fs = require('fs');

function fixBtn(path) {
    let html = fs.readFileSync(path, 'utf8');
    
    // Replace the button classes without relying on the exact text content
    html = html.replace(/<a class="btn btn-outline-light btn-sm" href="login.html">/g, '<a class="btn btn-outline-minimal" href="login.html">');
    
    // Also fix the mangled text if it's there
    html = html.replace(/Iniciar Sesin/g, 'Iniciar Sesión');
    html = html.replace(/Iniciar Sesin/g, 'Iniciar Sesión');

    fs.writeFileSync(path, html);
    console.log(path + " updated");
}

fixBtn('Frontend/store/index.html');
fixBtn('Frontend/store/cart.html');
