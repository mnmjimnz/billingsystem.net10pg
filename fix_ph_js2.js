const fs = require('fs');
let code = fs.readFileSync('Frontend/pages/purchase-history.js', 'utf-8');

const newFunc = `async function viewPurchaseDetails(id) {
    try {
        const data = await ApiClient.request(\`/Purchases/\${id}\`);
        if (!data || !data.purchase) {
            alert('No se pudo cargar los detalles de la compra.');
            return;
        }

        const p = data.purchase;
        const invoiceNumber = p.InvoiceNumber || p.invoiceNumber || p.invoicenumber || '-';
        const dateVal = p.CreatedAt || p.createdAt || p.createdat || p.Date || p.date || new Date();
        const supplierName = p.SupplierName || p.supplierName || p.suppliername || 'N/A';
        const status = p.Status || p.status || '-';
        const paymentType = p.PaymentType || p.paymentType || p.paymenttype || '-';
        const total = p.Total || p.total || 0;

        document.getElementById('detailInvoice').innerText = invoiceNumber;
        document.getElementById('detailDate').innerText = new Date(dateVal).toLocaleString();
        document.getElementById('detailSupplier').innerText = supplierName;
        document.getElementById('detailStatus').innerText = \`\${status} - \${paymentType}\`;
        document.getElementById('detailTotal').innerText = \`$\${Number(total).toFixed(2)}\`;

        const tbody = document.getElementById('detailProductsBody');
        tbody.innerHTML = '';
        
        if (data.details && data.details.length > 0) {
            data.details.forEach(d => {
                const productCode = d.ProductCode || d.productCode || d.productcode || '-';
                const productName = d.ProductName || d.productName || d.productname || 'Producto Eliminado';
                const quantity = d.Quantity || d.quantity || 0;
                const unitCost = d.UnitCost || d.unitCost || d.unitcost || 0;
                const subtotal = d.Subtotal || d.subtotal || 0;

                tbody.innerHTML += \`
                    <tr>
                        <td>\${productCode}</td>
                        <td>\${productName}</td>
                        <td class="text-center">\${quantity}</td>
                        <td class="text-end">$\${Number(unitCost).toFixed(2)}</td>
                        <td class="text-end fw-bold">$\${Number(subtotal).toFixed(2)}</td>
                    </tr>\`;
            });
        }

        const modal = new bootstrap.Modal(document.getElementById('purchaseDetailsModal'));
        modal.show();
    } catch (e) {
        console.error(e);
        alert('Error al cargar la información de la compra.');
    }
}
`;

const startIndex = code.indexOf('async function viewPurchaseDetails(id)');
if (startIndex !== -1) {
    code = code.substring(0, startIndex) + newFunc;
    fs.writeFileSync('Frontend/pages/purchase-history.js', code, 'utf-8');
    console.log('Fixed purchase-history.js using substring');
} else {
    console.log('Function not found!');
}
