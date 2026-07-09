const fs = require('fs');
const path = require('path');

function removeHardcodedTheme(file) {
    let html = fs.readFileSync(file, 'utf8');
    html = html.replace(/<html lang="es" data-bs-theme="dark">/g, '<html lang="es">');
    
    // Also inject inline script to prevent flicker
    const inlineScript = `
    <script>
        const savedTheme = localStorage.getItem('storeTheme') || 'light';
        document.documentElement.setAttribute('data-bs-theme', savedTheme);
    </script>
    `;
    if (!html.includes('localStorage.getItem(\'storeTheme\')')) {
        html = html.replace(/<\/head>/, inlineScript + '</head>');
    }
    
    // Bust cache
    const ts = Date.now();
    html = html.replace(/app\.js(\?v=[0-9]+)?/g, `app.js?v=${ts}`);
    html = html.replace(/style\.css(\?v=[0-9]+)?/g, `style.css?v=${ts}`);
    
    fs.writeFileSync(file, html);
    console.log(file + " updated");
}

removeHardcodedTheme('Frontend/store/index.html');
removeHardcodedTheme('Frontend/store/cart.html');
removeHardcodedTheme('Frontend/store/login.html'); // Just in case, update cache
removeHardcodedTheme('Frontend/store/register.html'); // Just in case, update cache

