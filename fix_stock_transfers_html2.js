const fs = require('fs');

let productsHtml = fs.readFileSync('Frontend/pages/products.html', 'utf-8');
let topbarRegex = /<header class="topbar">[\s\S]*?<\/header>/;
let topbarMatch = productsHtml.match(topbarRegex);

if (topbarMatch) {
    let topbarHtml = topbarMatch[0];
    
    let transfersHtml = fs.readFileSync('Frontend/pages/stock-transfers.html', 'utf-8');
    let oldTopbarRegex = /<header class="topbar">[\s\S]*?<\/header>/;
    transfersHtml = transfersHtml.replace(oldTopbarRegex, topbarHtml);
    
    fs.writeFileSync('Frontend/pages/stock-transfers.html', transfersHtml, 'utf-8');
    console.log("Injected topbar into stock-transfers.html");
} else {
    console.log("Could not find topbar in products.html");
}
