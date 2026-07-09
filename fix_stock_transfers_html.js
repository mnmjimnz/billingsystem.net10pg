const fs = require('fs');
const transfersHtml = fs.readFileSync('Frontend/pages/stock-transfers.html', 'utf-8');
const productsHtml = fs.readFileSync('Frontend/pages/products.html', 'utf-8');

// 1. Get the Transfers specific page-content
const tcStart = transfersHtml.indexOf('<div class="page-content p-4">');
const tcEnd = transfersHtml.indexOf('<!DOCTYPE html>'); // It was appended before the products DOCTYPE

let transfersContent = transfersHtml.substring(tcStart, tcEnd).trim();
// The last few tags in transfersContent are:
//             </div>
//         </div>
//     </div>
// </div> // this is extra, from where? Let's just grab what's between `<h2 class="mb-4 fw-bold text-dark">Traslados de Sucursal</h2>` and `</div>` before DOCTYPE
const contentStart = transfersHtml.indexOf('<h2 class="mb-4 fw-bold text-dark">Traslados de Sucursal</h2>');
const contentEnd = transfersHtml.indexOf('<!DOCTYPE html>');
let cleanTransfersContent = transfersHtml.substring(contentStart, contentEnd);
// Trim closing divs that belong to main layout
cleanTransfersContent = cleanTransfersContent.substring(0, cleanTransfersContent.lastIndexOf('</div>\n            </div>')); // just rough trim

// 2. Get the transfer Modal
const modalStart = transfersHtml.indexOf('<!-- Transfer Modal -->');
const modalEnd = transfersHtml.indexOf('<div class="page-content p-4">');
const transferModal = transfersHtml.substring(modalStart, modalEnd).trim();

// 3. Build new HTML based on products.html
const pContentStart = productsHtml.indexOf('<div class="page-content">');
const pContentEnd = productsHtml.indexOf('</main>');

let newHtml = productsHtml.substring(0, pContentStart) + 
    '<div class="page-content p-4">\n' + 
    cleanTransfersContent + '\n' +
    '</div>\n' +
    productsHtml.substring(pContentEnd);

// Replace title
newHtml = newHtml.replace('<title>Productos - Sistema de Facturación Premium</title>', '<title>Traslados de Sucursal - Sistema de Facturación Premium</title>');

// Replace modals
const pModalStart = newHtml.indexOf('<!-- Modal Producto -->');
const pModalEnd = newHtml.indexOf('<!-- Global Toast Container -->');
newHtml = newHtml.substring(0, pModalStart) + transferModal + '\n\n' + newHtml.substring(pModalEnd);

// Replace scripts
newHtml = newHtml.replace('<script src="products.js?v=20260627004119"></script>', '<script src="stock-transfers.js"></script>');
newHtml = newHtml.replace('id="nav-products" class="nav-link active"', 'id="nav-products" class="nav-link"');

fs.writeFileSync('Frontend/pages/stock-transfers.html', newHtml, 'utf-8');
console.log("Re-fixed stock-transfers.html");
