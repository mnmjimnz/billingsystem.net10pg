const fs = require('fs');

function fixFile(path) {
    let html = fs.readFileSync(path, 'utf8');
    
    // Remove hardcoded dark classes
    html = html.replace(/bg-dark text-light border-secondary/g, '');
    html = html.replace(/bg-dark text-light/g, '');
    html = html.replace(/data-bs-theme="dark"/, '');
    
    // Insert theme script in head
    const themeScript = `
    <script>
        const savedTheme = localStorage.getItem('storeTheme') || 'light';
        document.documentElement.setAttribute('data-bs-theme', savedTheme);
    </script>
    `;
    
    if(!html.includes('localStorage.getItem(\'storeTheme\')')) {
        html = html.replace(/<\/head>/, themeScript + '</head>');
    }
    
    fs.writeFileSync(path, html);
    console.log(path + " updated");
}

fixFile('Frontend/store/login.html');
fixFile('Frontend/store/register.html');
