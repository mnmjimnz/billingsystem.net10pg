const fs = require('fs');

// 1. orders.js
let ordersJs = fs.readFileSync('Frontend/pages/orders.js', 'utf8');

// Fix viewOrder property casing
const targetViewCustomer = `document.getElementById('viewOrderCustomer').innerText = order.customerName || 'N/A';`;
const replaceViewCustomer = `document.getElementById('viewOrderCustomer').innerText = order.customerName || order.customername || order.CustomerName || 'N/A';`;
ordersJs = ordersJs.replace(targetViewCustomer, replaceViewCustomer);

const targetViewBranch = `document.getElementById('viewOrderBranch').innerText = order.branchName || 'N/A';`;
const replaceViewBranch = `document.getElementById('viewOrderBranch').innerText = order.branchName || order.branchname || order.BranchName || 'N/A';`;
ordersJs = ordersJs.replace(targetViewBranch, replaceViewBranch);

const targetViewAddress = `document.getElementById('viewOrderAddress').innerText = order.deliveryAddress || 'N/A';`;
const replaceViewAddress = `document.getElementById('viewOrderAddress').innerText = order.deliveryAddress || order.deliveryaddress || order.DeliveryAddress || 'N/A';`;
ordersJs = ordersJs.replace(targetViewAddress, replaceViewAddress);

// Fix badge switch in viewOrder
const targetSwitch = `            case 'PENDING': statusBadge = '<span class="badge bg-warning text-dark px-3 py-2">PENDIENTE</span>'; break;
            case 'DELIVERED': statusBadge = '<span class="badge bg-success px-3 py-2">ENTREGADO</span>'; break;`;
const replaceSwitch = `            case 'PENDING': statusBadge = '<span class="badge bg-warning text-dark px-3 py-2">PENDIENTE</span>'; break;
            case 'SHIPPED': statusBadge = '<span class="badge bg-primary px-3 py-2">ENVIADO</span>'; break;
            case 'DELIVERED': statusBadge = '<span class="badge bg-success px-3 py-2">ENTREGADO</span>'; break;`;
if (ordersJs.includes(targetSwitch)) {
    ordersJs = ordersJs.replace(targetSwitch, replaceSwitch);
}

// Fix item product name in viewOrder
const targetProduct = `\${d.productName || 'Producto ' + d.productId}`;
const replaceProduct = `\${d.productName || d.productname || d.ProductName || 'Producto ' + (d.productId || d.productid || '')}`;
ordersJs = ordersJs.replace(targetProduct, replaceProduct);

// Add shipOrder function
const shipOrderFn = `
function shipOrder(id) {
    if (!confirmStatusModalInstance) confirmStatusModalInstance = new bootstrap.Modal(document.getElementById('confirmStatusModal'));
    document.getElementById('confirm-icon-wrap').innerHTML = '<i class="bi bi-truck text-primary"></i>';
    document.getElementById('confirm-title').innerText = 'Marcar como Enviado';
    document.getElementById('confirm-message').innerText = '¿Estás seguro de que deseas marcar este pedido como enviado?';
    
    const btn = document.getElementById('btn-confirm-action');
    btn.className = 'btn px-4 fw-semibold btn-primary';
    btn.innerText = 'Confirmar Envío';
    
    btn.onclick = async () => {
        try {
            btn.disabled = true;
            btn.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Procesando...';
            await ApiClient.request(\`/Orders/\${id}/status\`, 'PUT', { Status: 'SHIPPED' });
            if (typeof showToast === 'function') showToast("El pedido ha sido marcado como enviado.", "success");
            confirmStatusModalInstance.hide();
            loadOrders();
        } catch(e) {
            if (typeof showToast === 'function') showToast(e.message, "error");
        } finally {
            btn.disabled = false;
        }
    };
    confirmStatusModalInstance.show();
}
`;
if (!ordersJs.includes('function shipOrder(id)')) {
    ordersJs += '\n' + shipOrderFn;
}

fs.writeFileSync('Frontend/pages/orders.js', ordersJs);


// 2. store/app.js
let appJs = fs.readFileSync('Frontend/store/app.js', 'utf8');
const targetAppProduct = `\${d.productName || 'Producto ID: ' + d.productId}`;
const replaceAppProduct = `\${d.productName || d.productname || d.ProductName || 'Producto ID: ' + (d.productId || d.productid || '')}`;
appJs = appJs.replace(targetAppProduct, replaceAppProduct);

fs.writeFileSync('Frontend/store/app.js', appJs);

console.log("Done patching scripts.");
