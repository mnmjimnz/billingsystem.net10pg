const fs = require('fs');

let html = fs.readFileSync('Frontend/pages/orders.html', 'utf-8');

const targetHeader = `<div class="form-check form-switch m-0">
                        <input class="form-check-input" type="checkbox" role="switch" id="theme-toggle">
                        <label class="form-check-label" for="theme-toggle"><i class="bi bi-moon-stars"></i></label>
                    </div>`;

const newHeader = `<div class="form-check form-switch m-0">
                        <input class="form-check-input" type="checkbox" role="switch" id="theme-toggle">
                        <label class="form-check-label" for="theme-toggle"><i class="bi bi-moon-stars"></i></label>
                    </div>
                    <div class="dropdown me-3">
                          <button class="btn border-0 bg-transparent text-body p-2" type="button" data-bs-toggle="dropdown" onclick="loadNotifications()">
                              <div class="position-relative d-inline-block">
                                  <i class="bi bi-bell fs-5"></i>
                                  <span class="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger" id="notif-badge" style="display:none; font-size: 0.6em; transform: translate(-30%, -30%) !important;"></span>
                              </div>
                          </button>
                          <ul class="dropdown-menu dropdown-menu-end shadow-sm" style="width: 300px; max-height: 400px; overflow-y: auto;" id="notif-list">
                              <li><h6 class="dropdown-header">Notificaciones Pendientes</h6></li>
                          </ul>
                      </div>
                      <button class="btn p-1 text-danger border-0" onclick="logout()" title="Cerrar Sesión">
                          <i class="bi bi-power fs-4"></i>
                      </button>`;

if (html.includes(targetHeader)) {
    html = html.replace(targetHeader, newHeader);
    fs.writeFileSync('Frontend/pages/orders.html', html, 'utf-8');
    console.log("Fixed header in orders.html");
} else {
    console.log("Target header not found in orders.html");
}

let js = fs.readFileSync('Frontend/pages/orders.js', 'utf-8');

// Modals declaration
if (!js.includes('let confirmStatusModalInstance;')) {
    js = js.replace('let deliverModalInstance;', 'let deliverModalInstance;\nlet confirmStatusModalInstance;\nlet viewOrderModalInstance;');
    js = js.replace("deliverModalInstance = new bootstrap.Modal(document.getElementById('deliverModal'));",
        "deliverModalInstance = new bootstrap.Modal(document.getElementById('deliverModal'));\n    if(document.getElementById('confirmStatusModal')) confirmStatusModalInstance = new bootstrap.Modal(document.getElementById('confirmStatusModal'));\n    if(document.getElementById('viewOrderModal')) viewOrderModalInstance = new bootstrap.Modal(document.getElementById('viewOrderModal'));");
}

// viewOrder
js = js.replace(/function viewOrder\(id\) \{[\s\S]*?\n\}/m, `async function viewOrder(id) {
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
}`);

// cancelOrder
js = js.replace(/async function cancelOrder\(id\) \{[\s\S]*?showToast\([^)]+\);[\s\S]*?\}/m, `function cancelOrder(id) {
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
}`);

fs.writeFileSync('Frontend/pages/orders.js', js, 'utf-8');
console.log("Fixed orders.js");

