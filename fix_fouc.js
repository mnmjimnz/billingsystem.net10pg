const fs = require('fs');
let code = fs.readFileSync('Frontend/pages/stock-transfers.html', 'utf-8');

const headScript = `
    <script>
        const theme = localStorage.getItem('theme') || 'light';
        document.documentElement.setAttribute('data-bs-theme', theme);
    </script>
</head>`;

if (!code.includes("localStorage.getItem('theme')")) {
    code = code.replace('</head>', headScript);
    fs.writeFileSync('Frontend/pages/stock-transfers.html', code, 'utf-8');
    console.log("Added FOUC fix");
}
