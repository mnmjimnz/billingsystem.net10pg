const fs = require('fs');

let productsHtml = fs.readFileSync('Frontend/pages/products.html', 'utf-8');
let transfersHtml = fs.readFileSync('Frontend/pages/stock-transfers.html', 'utf-8');

// Extract the page-content from stock-transfers
const startMarker = '<div class="page-content p-4">';
const endMarker = '</div></main>';
const startIdx = transfersHtml.indexOf(startMarker);
const endIdx = transfersHtml.indexOf(endMarker, startIdx);
const content = transfersHtml.substring(startIdx, endIdx);

// Extract the modals from stock-transfers (from endMarker to the toast-container)
const modalStartIdx = transfersHtml.indexOf('<!-- Transfer Modal -->');
const modalEndIdx = transfersHtml.indexOf('<!-- Global Toast Container -->');
let modals = '';
if (modalStartIdx !== -1 && modalEndIdx !== -1) {
    modals = transfersHtml.substring(modalStartIdx, modalEndIdx);
}

// Now take productsHtml, and replace its page-content and modals with stock-transfers ones
const prodStartIdx = productsHtml.indexOf(startMarker);
const prodEndIdx = productsHtml.indexOf(endMarker, prodStartIdx);

let newTransfersHtml = productsHtml.substring(0, prodStartIdx) + content + productsHtml.substring(prodEndIdx);

// Replace title
newTransfersHtml = newTransfersHtml.replace('<title>Productos - Sistema de Facturación</title>', '<title>Traslados de Sucursal - Sistema de Facturación</title>');

// Replace modals
const prodModalStartIdx = newTransfersHtml.indexOf('<!-- Modal Producto -->');
const prodModalEndIdx = newTransfersHtml.indexOf('<!-- Global Toast Container -->');

newTransfersHtml = newTransfersHtml.substring(0, prodModalStartIdx) + modals + newTransfersHtml.substring(prodModalEndIdx);

// Replace scripts
newTransfersHtml = newTransfersHtml.replace('<script src="products.js?v=20260627004119"></script>', '<script src="stock-transfers.js"></script>');

fs.writeFileSync('Frontend/pages/stock-transfers.html', newTransfersHtml, 'utf-8');
console.log("Fixed stock-transfers.html layout");
