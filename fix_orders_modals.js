const fs = require('fs');
let js = fs.readFileSync('Frontend/pages/orders.js', 'utf-8');

// 1. Add modal instances
if (!js.includes('let viewOrderModalInstance;')) {
    js = js.replace('let deliverModalInstance;', 'let deliverModalInstance;\nlet confirmStatusModalInstance;\nlet viewOrderModalInstance;');
}
if (!js.includes('viewOrderModalInstance =')) {
    js = js.replace("deliverModalInstance = new bootstrap.Modal(document.getElementById('deliverModal'));",
        "deliverModalInstance = new bootstrap.Modal(document.getElementById('deliverModal'));\n    confirmStatusModalInstance = new bootstrap.Modal(document.getElementById('confirmStatusModal'));\n    viewOrderModalInstance = new bootstrap.Modal(document.getElementById('viewOrderModal'));");
}

// 2. Fix cancelOrder to use custom confirm modal
const oldCancelOrder = `async function cancelOrder(id) {
    if (!confirm("¿Seguro que deseas cancelar este pedido? El stock será devuelto al inventario.")) return;
    try {
        await ApiClient.request(\`/Orders/\${id}/status\`, 'PUT', { Status: 'CANCELLED' });
        showToast("Pedido cancelado y stock devuelto.", "success");
        loadOrders(currentPage);
    } catch(e) {
        showToast(e.message, "error");
    }
}`;

const newCancelOrder = `function cancelOrder(id) {
    document.getElementById('confirm-icon-wrap').innerHTML = '<i class="bi bi-exclamation-triangle text-danger"></i>';
    document.getElementById('confirm-title').innerText = 'Cancelar Pedido';
    document.getElementById('confirm-message').innerText = '¿Seguro que deseas cancelar este pedido? El stock reservado será devuelto al inventario.';
    
    const btn = document.getElementById('btn-confirm-action');
    btn.className = 'btn px-4 fw-semibold btn-danger';
    
    btn.onclick = async () => {
        try {
            btn.disabled = true;
            btn.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Cancelando...';
            await ApiClient.request(\`/Orders/\${id}/status\`, 'PUT', { Status: 'CANCELLED' });
            showToast("Pedido cancelado y stock devuelto.", "success");
            confirmStatusModalInstance.hide();
            loadOrders(currentPage);
        } catch(e) {
            showToast(e.message, "error");
        } finally {
            btn.disabled = false;
            btn.innerText = 'Confirmar Cancelación';
        }
    };
    confirmStatusModalInstance.show();
}`;

js = js.replace(oldCancelOrder, newCancelOrder);

// 3. Fix viewOrder to use viewOrderModal
const oldViewOrder = `function viewOrder(id) {
    // For simplicity, just shows a toast or implement a viewer modal
    showToast("Función ver detalles en desarrollo.", "info");
}`;

const newViewOrder = `async function viewOrder(id) {
    try {
        const order = await ApiClient.request(\`/Orders/\${id}\`);
        document.getElementById('viewOrderId').innerText = '#' + order.id;
        document.getElementById('viewOrderCustomer').innerText = order.customerName || 'N/A';
        document.getElementById('viewOrderBranch').innerText = order.branchName || 'N/A';
        document.getElementById('viewOrderAddress').innerText = order.deliveryAddress || 'N/A';
        
        let statusBadge = '';
        switch(order.status) {
            case 'PENDING': statusBadge = '<span class="badge bg-warning text-dark px-3 py-2">PENDIENTE</span>'; break;
            case 'DELIVERED': statusBadge = '<span class="badge bg-success px-3 py-2">ENTREGADO</span>'; break;
            case 'CANCELLED': statusBadge = '<span class="badge bg-danger px-3 py-2">CANCELADO</span>'; break;
            default: statusBadge = \`<span class="badge bg-secondary px-3 py-2">\${order.status}</span>\`; break;
        }
        document.getElementById('viewOrderStatus').innerHTML = statusBadge;
        
        const tbody = document.getElementById('viewOrderItems');
        tbody.innerHTML = '';
        let total = 0;
        
        if (order.details && order.details.length > 0) {
            order.details.forEach(d => {
                const subtotal = d.quantity * d.unitPrice;
                total += subtotal;
                tbody.innerHTML += \`
                    <tr>
                        <td class="fw-medium">\${d.productName || 'Producto ' + d.productId}</td>
                        <td>\${d.quantity}</td>
                        <td>$\${d.unitPrice.toFixed(2)}</td>
                        <td class="fw-bold">$\${subtotal.toFixed(2)}</td>
                    </tr>
                \`;
            });
        } else {
            tbody.innerHTML = '<tr><td colspan="4" class="text-center text-muted">No hay productos</td></tr>';
        }
        
        document.getElementById('viewOrderTotal').innerText = '$' + total.toFixed(2);
        
        viewOrderModalInstance.show();
    } catch(e) {
        showToast(e.message, "error");
    }
}`;

js = js.replace(oldViewOrder, newViewOrder);

fs.writeFileSync('Frontend/pages/orders.js', js, 'utf-8');
console.log("Updated orders.js modals");
