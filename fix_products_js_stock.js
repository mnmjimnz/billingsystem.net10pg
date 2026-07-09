const fs = require('fs');
let code = fs.readFileSync('Frontend/pages/products.js', 'utf-8');

const newButton = `
                          <button class="btn btn-sm btn-outline-success me-1 rounded-circle" onclick='showStockBreakdown(\${JSON.stringify(p)})' title="Ver Stock Sucursales"><i class="bi bi-boxes"></i></button>
`;

if (!code.includes('showStockBreakdown')) {
    code = code.replace(
        '<button class="btn btn-sm btn-outline-info',
        newButton + '                          <button class="btn btn-sm btn-outline-info'
    );
    
    code += `
async function showStockBreakdown(product) {
    try {
        const stocks = await ApiClient.request(\`/Products/\${product.id}/stock\`);
        const tbody = document.getElementById('stockBreakdownBody');
        if (stocks && stocks.length > 0) {
            tbody.innerHTML = stocks.map(s => \`
                <tr>
                    <td>\${s.branchName}</td>
                    <td class="text-end fw-bold \${s.stock > 10 ? 'text-success' : 'text-danger'}">\${s.stock}</td>
                </tr>
            \`).join('');
        } else {
            tbody.innerHTML = '<tr><td colspan="2" class="text-center">No hay registros de stock por sucursal para este producto.</td></tr>';
        }
        document.getElementById('stockBreakdownProductName').innerText = product.name;
        new bootstrap.Modal(document.getElementById('stockBreakdownModal')).show();
    } catch (e) {
        showToast("Error al cargar el stock por sucursal", "error");
    }
}
`;
    fs.writeFileSync('Frontend/pages/products.js', code, 'utf-8');
    console.log("Added showStockBreakdown to products.js");
}
