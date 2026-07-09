const fs = require('fs');

function addPagination(path, insertBeforeFile) {
    let html = fs.readFileSync(path, 'utf8');
    if (!html.includes('pagination.js')) {
        html = html.replace(`<script src="${insertBeforeFile}"></script>`, `<script src="../assets/js/pagination.js"></script>\n    <script src="${insertBeforeFile}"></script>`);
        fs.writeFileSync(path, html);
        console.log("Added to " + path);
    }
}

addPagination('Frontend/pages/orders.html', 'orders.js');
addPagination('Frontend/pages/stock-transfers.html', 'stock-transfers.js');
addPagination('Frontend/pages/branch-movements.html', 'branch-movements.js');

console.log("Done");
