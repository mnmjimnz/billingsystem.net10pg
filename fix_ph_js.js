const fs = require('fs');
let content = fs.readFileSync('Frontend/pages/purchase-history.js', 'utf-8');
const oldRow = `<td class="fw-bold">$\${item.total.toFixed(2)}</td>\n                </tr>`;
const newRow = `<td class="fw-bold">$\${item.total.toFixed(2)}</td>\n                    <td class="text-center">\n                        <button class="btn btn-sm btn-outline-primary" onclick="viewPurchaseDetails(\${item.id})">\n                            <i class="bi bi-eye"></i> Ver\n                        </button>\n                    </td>\n                </tr>`;
content = content.replace(oldRow, newRow);

const viewFunc = `
async function viewPurchaseDetails(id) {
    try {
        const data = await ApiClient.request(\`/Purchases/\${id}\`);
        if (!data || !data.purchase) {
            alert('No se pudo cargar los detalles de la compra.');
            return;
        }

        const p = data.purchase;
        document.getElementById('detailInvoice').innerText = p.invoiceNumber;
        document.getElementById('detailDate').innerText = new Date(p.createdAt || p.date).toLocaleString();
        document.getElementById('detailSupplier').innerText = p.supplierName || 'N/A';
        document.getElementById('detailStatus').innerText = \`\${p.status} - \${p.paymentType}\`;
        document.getElementById('detailTotal').innerText = \`$\${p.total.toFixed(2)}\`;

        const tbody = document.getElementById('detailProductsBody');
        tbody.innerHTML = '';
        
        data.details.forEach(d => {
            tbody.innerHTML += \`
                <tr>
                    <td>\${d.productCode || '-'}</td>
                    <td>\${d.productName || 'Producto Eliminado'}</td>
                    <td class="text-center">\${d.quantity}</td>
                    <td class="text-end">$\${d.unitCost.toFixed(2)}</td>
                    <td class="text-end fw-bold">$\${d.subtotal.toFixed(2)}</td>
                </tr>\`;
        });

        const modal = new bootstrap.Modal(document.getElementById('purchaseDetailsModal'));
        modal.show();
    } catch (e) {
        console.error(e);
        alert('Error al cargar la información de la compra.');
    }
}
`;
if (!content.includes('function viewPurchaseDetails')) {
    content += '\n' + viewFunc;
}

fs.writeFileSync('Frontend/pages/purchase-history.js', content, 'utf-8');
