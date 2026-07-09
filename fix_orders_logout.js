const fs = require('fs');
let js = fs.readFileSync('Frontend/pages/orders.js', 'utf-8');

if (!js.includes('function logout()')) {
    js += `

function logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    window.location.href = '../index.html';
}
`;
    fs.writeFileSync('Frontend/pages/orders.js', js, 'utf-8');
    console.log("Added logout to orders.js");
}
