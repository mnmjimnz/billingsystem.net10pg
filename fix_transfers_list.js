const fs = require('fs');
let code = fs.readFileSync('Frontend/pages/stock-transfers.js', 'utf-8');

code = code.replace('${t.productName}', '${t.productName || t.productname}');
code = code.replace('${t.fromBranchName}', '${t.fromBranchName || t.frombranchname}');
code = code.replace('${t.toBranchName}', '${t.toBranchName || t.tobranchname}');
code = code.replace('${t.userName}', '${t.userName || t.username}');

fs.writeFileSync('Frontend/pages/stock-transfers.js', code, 'utf-8');
console.log("Fixed undefined columns");
