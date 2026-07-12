const fs = require('fs');

const path = 'Frontend/pages/orders.js';
let js = fs.readFileSync(path, 'utf8');

// 1. Update renderOrdersTable to include SHIPPED badge and buttons
const targetActions = `        if (status === 'PENDING') badge = 'bg-warning text-dark';
        if (status === 'DELIVERED') badge = 'bg-success';
        if (status === 'CANCELLED') badge = 'bg-danger';

        let actions = \`<button class="btn btn-sm btn-outline-info me-1" onclick="viewOrder(\${o.id || o.Id})" title="Ver detalles"><i class="bi bi-eye"></i></button>\`;
        if (status === 'PENDING') {
            actions += \`
                <button class="btn btn-sm btn-outline-success me-1" onclick="openDeliverModal(\${o.id || o.Id})" title="Marcar Entregado"><i class="bi bi-check-circle"></i></button>
                <button class="btn btn-sm btn-outline-danger" onclick="cancelOrder(\${o.id || o.Id})" title="Cancelar Pedido"><i class="bi bi-x-circle"></i></button>
            \`;
        }`;

const replaceActions = `        if (status === 'PENDING') badge = 'bg-warning text-dark';
        if (status === 'SHIPPED') badge = 'bg-primary';
        if (status === 'DELIVERED') badge = 'bg-success';
        if (status === 'CANCELLED') badge = 'bg-danger';

        let actions = \`<button class="btn btn-sm btn-outline-info me-1" onclick="viewOrder(\${o.id || o.Id})" title="Ver detalles"><i class="bi bi-eye"></i></button>\`;
        if (status === 'PENDING') {
            actions += \`
                <button class="btn btn-sm btn-outline-primary me-1" onclick="shipOrder(\${o.id || o.Id})" title="Marcar Enviado"><i class="bi bi-truck"></i></button>
                <button class="btn btn-sm btn-outline-success me-1" onclick="openDeliverModal(\${o.id || o.Id})" title="Marcar Entregado"><i class="bi bi-check-circle"></i></button>
                <button class="btn btn-sm btn-outline-danger" onclick="cancelOrder(\${o.id || o.Id})" title="Cancelar Pedido"><i class="bi bi-x-circle"></i></button>
            \`;
        } else if (status === 'SHIPPED') {
            actions += \`
                <button class="btn btn-sm btn-outline-success me-1" onclick="openDeliverModal(\${o.id || o.Id})" title="Marcar Entregado"><i class="bi bi-check-circle"></i></button>
                <button class="btn btn-sm btn-outline-danger" onclick="cancelOrder(\${o.id || o.Id})" title="Cancelar Pedido"><i class="bi bi-x-circle"></i></button>
            \`;
        }`;

if (js.includes('bg-warning text-dark')) {
    js = js.replace(targetActions, replaceActions);
}

// 2. Add shipOrder function
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
            showToast("El pedido ha sido marcado como enviado.", "success");
            confirmStatusModalInstance.hide();
            loadOrders();
        } catch(e) {
            showToast(e.message, "error");
        } finally {
            btn.disabled = false;
        }
    };
    confirmStatusModalInstance.show();
}
`;

if (!js.includes('function shipOrder(id)')) {
    js += '\n' + shipOrderFn;
}

fs.writeFileSync(path, js);
console.log("Updated orders.js");
