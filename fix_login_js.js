const fs = require('fs');
let content = fs.readFileSync('Frontend/pages/login.js', 'utf-8');

content = content.replace(
    "localStorage.setItem('userId', data.user.id);",
    "localStorage.setItem('userId', data.user.id);\n                localStorage.setItem('userPermissions', JSON.stringify(data.permissions || []));"
);

content = content.replace(
    "window.location.href = 'reports.html';",
    "window.location.href = 'pos.html';" // Redirect to POS or index by default instead of reports, as reports might be restricted
);

fs.writeFileSync('Frontend/pages/login.js', content, 'utf-8');
