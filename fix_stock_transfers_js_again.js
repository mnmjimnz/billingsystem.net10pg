const fs = require('fs');
let code = fs.readFileSync('Frontend/pages/stock-transfers.js', 'utf-8');

// 1. Remove showToast completely
const showToastMatch = code.match(/function showToast\([^]*?\n\}/);
if (showToastMatch) {
    code = code.replace(showToastMatch[0], '');
}

// 2. Fix branchId check
code = code.replace('const branchStock = stock.find(s => s.branchId == branchId);', 'const branchStock = stock.find(s => (s.branchid || s.branchId) == branchId);');

fs.writeFileSync('Frontend/pages/stock-transfers.js', code, 'utf-8');
console.log("Fixed JS issues");
