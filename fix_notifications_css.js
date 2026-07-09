const fs = require('fs');
let content = fs.readFileSync('Frontend/pages/notifications.html', 'utf-8');

const oldHead = `    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css">
    <link rel="stylesheet" href="../assets/css/theme.css">`;

const newHead = `    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css">
    <link rel="stylesheet" href="../assets/css/devextreme-theme.css">`;

content = content.replace(oldHead, newHead);
fs.writeFileSync('Frontend/pages/notifications.html', content, 'utf-8');
