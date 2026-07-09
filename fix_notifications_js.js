const fs = require('fs');
let code = fs.readFileSync('Frontend/pages/notifications.js', 'utf-8');

const oldTr = '<tr class="${rowClass}">';
const newTr = '<tr class="${rowClass}" style="cursor: pointer;" onclick="window.location.href=\'${n.type === \'WARNING\' ? \'payables.html\' : \'receivables.html\'}\'">';

if (code.includes(oldTr)) {
    code = code.replace(oldTr, newTr);
    fs.writeFileSync('Frontend/pages/notifications.js', code, 'utf-8');
    console.log("Fixed notifications table row click");
} else {
    console.log("Could not find row HTML in notifications.js");
}
