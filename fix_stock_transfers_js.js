const fs = require('fs');
let code = fs.readFileSync('Frontend/pages/stock-transfers.js', 'utf-8');

const oldInit = `function initUserProfile() {
    document.getElementById('logoutBtn')?.addEventListener('click', logout);
    const userName = localStorage.getItem('userName');
    const roleName = localStorage.getItem('roleName');
    if (userName) {
        document.getElementById('userProfileName').innerText = userName;
        document.getElementById('userProfileRole').innerText = roleName;
    }
}`;

const newInit = `function initUserProfile() {
    document.getElementById('logoutBtn')?.addEventListener('click', logout);
    const userName = localStorage.getItem('userName');
    const roleName = localStorage.getItem('roleName');
    if (userName) {
        const nameEl = document.getElementById('userProfileName');
        const roleEl = document.getElementById('userProfileRole');
        if (nameEl) nameEl.innerText = userName;
        if (roleEl) roleEl.innerText = roleName;
    }
}`;

code = code.replace(oldInit, newInit);
fs.writeFileSync('Frontend/pages/stock-transfers.js', code, 'utf-8');
console.log("Fixed initUserProfile in stock-transfers.js");
