const fs = require('fs');
let content = fs.readFileSync('Frontend/pages/purchase-history.js', 'utf-8');

// 1. Add action column to renderTable
const oldRow = `<td class="text-end fw-bold">$\${p.total.toFixed(2)}</td>
            </tr>\`;`;
const newRow = `<td class="text-end fw-bold">$\${p.total.toFixed(2)}</td>
                <td class="text-center">
                    <button class="btn btn-sm btn-outline-primary" onclick="viewPurchaseDetails(\${p.id})">
                        <i class="bi bi-eye"></i> Ver
                    </button>
                </td>
            </tr>\`;`;
content = content.replace(oldRow, newRow);

// 2. Add viewPurchaseDetails function
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
