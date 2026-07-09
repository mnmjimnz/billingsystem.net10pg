const fs = require('fs');
let code = fs.readFileSync('Frontend/pages/stock-transfers.js', 'utf-8');

code = code.replace('${t.productName || t.productname}', '${t.product?.name || ""}');
code = code.replace('${t.fromBranchName || t.frombranchname}', '${t.fromBranch?.name || ""}');
code = code.replace('${t.toBranchName || t.tobranchname}', '${t.toBranch?.name || ""}');
code = code.replace('${t.userName || t.username}', '${t.user?.fullName || ""}');

fs.writeFileSync('Frontend/pages/stock-transfers.js', code, 'utf-8');
console.log("Fixed JS final nested properties");
