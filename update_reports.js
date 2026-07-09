const fs = require('fs');

let html = fs.readFileSync('Frontend/pages/reports.html', 'utf8');

const oldStyles = `        /* Estilos Empresariales Específicos para Reportes */
        .report-table { font-size: 0.85rem; background-color: #ffffff; }
        .report-table th { background-color: #ffffff; text-transform: uppercase; font-size: 0.75rem; letter-spacing: 0.5px; color: #495057; border-bottom: 2px solid #e0e0e0; }
        .financial-statement-card { border-left: 4px solid #2c3e50; }`;

const newStyles = `        /* Estilos Formales de Reportes Empresariales */
        .report-page { background: #ffffff; padding: 40px 50px; box-shadow: 0 4px 12px rgba(0,0,0,0.05); border: 1px solid #e2e8f0; border-radius: 4px; margin-top: 10px; margin-bottom: 30px; }
        .report-header-title { color: #2a3a52; font-weight: 700; font-size: 2rem; margin-bottom: 25px; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; letter-spacing: -0.5px; }
        .report-meta-box { background-color: #f4f6f9; padding: 15px 20px; font-size: 0.85rem; border: 1px solid #eef0f3; width: 100%; }
        .report-meta-box p { margin-bottom: 4px; color: #495057; }
        .report-meta-label { color: #868e96; font-size: 0.75rem; text-transform: uppercase; letter-spacing: 0.5px; font-weight: 600; width: 120px; display: inline-block; }
        .report-meta-value { font-weight: 500; color: #2c3e50; }
        .formal-table { width: 100%; border-collapse: collapse; margin-top: 30px; font-size: 0.85rem; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .formal-table th { background-color: #437682; color: #ffffff; padding: 12px 15px; text-transform: uppercase; font-size: 0.75rem; font-weight: 600; letter-spacing: 0.5px; border: none; text-align: left; }
        .formal-table th.text-end { text-align: right; }
        .formal-table th:first-child { border-top-left-radius: 4px; }
        .formal-table th:last-child { border-top-right-radius: 4px; }
        .formal-table td { padding: 10px 15px; border-bottom: 1px solid #e9ecef; color: #343a40; vertical-align: middle; }
        .formal-table tbody tr:nth-child(even) { background-color: #f8fafc; }
        .formal-table tbody tr:hover { background-color: #f1f5f9; }
        .formal-table .group-header td { background-color: #d2dcea; color: #2c3e50; font-weight: bold; font-size: 0.9rem; padding: 12px 15px; border-bottom: 2px solid #b8c7dd; text-align: center; letter-spacing: 0.5px; }
        .formal-table .report-total-row td { background-color: #ffffff !important; font-weight: bold; font-size: 0.95rem; border-top: 2px solid #437682 !important; border-bottom: 2px solid #437682 !important; color: #2a3a52; padding: 15px; }
        .formal-table .badge { font-size: 0.7rem; padding: 5px 8px; font-weight: 600; letter-spacing: 0.5px; border-radius: 4px; }
        .financial-statement-card { border-left: 4px solid #437682; }`;

html = html.replace(oldStyles, newStyles);

html = html.replace('class="tab-content bg-white shadow-sm rounded-3 p-4 border-0"', 'class="tab-content border-0"');
html = html.replace('<div class="tab-pane fade show active" id="sales" role="tabpanel">', '<div class="tab-pane fade show active report-page" id="sales" role="tabpanel">');
html = html.replace('<div class="tab-pane fade d-none" id="purchases" role="tabpanel">', '<div class="tab-pane fade d-none report-page" id="purchases" role="tabpanel">');
html = html.replace('<div class="tab-pane fade d-none" id="kardex" role="tabpanel">', '<div class="tab-pane fade d-none report-page" id="kardex" role="tabpanel">');
html = html.replace('<div class="tab-pane fade d-none" id="useractivity" role="tabpanel">', '<div class="tab-pane fade d-none report-page" id="useractivity" role="tabpanel">');
html = html.replace('<div class="tab-pane fade d-none" id="financials" role="tabpanel">', '<div class="tab-pane fade d-none report-page" id="financials" role="tabpanel">');
html = html.replace('<div class="tab-pane fade d-none" id="cashflow" role="tabpanel">', '<div class="tab-pane fade d-none report-page" id="cashflow" role="tabpanel">');
html = html.replace('<div class="tab-pane fade d-none" id="stats" role="tabpanel">', '<div class="tab-pane fade d-none report-page" id="stats" role="tabpanel">');

html = html.replaceAll('class="table table-sm align-middle report-table"', 'class="formal-table"');

function getHeaderTpl(title, prefix) {
    return `                                <div class="d-flex justify-content-between align-items-start mb-4">
                                    <div style="flex: 1;">
                                        <h2 class="report-header-title">${title}</h2>
                                        <div class="row w-100 m-0">
                                            <div class="col-md-6 p-0">
                                                <div class="report-meta-box me-3">
                                                    <p><span class="report-meta-label">Sucursal:</span> <span class="report-meta-value" id="${prefix}-meta-branch">Consolidado</span></p>
                                                    <p><span class="report-meta-label">Usuario:</span> <span class="report-meta-value" id="${prefix}-meta-user">Todos</span></p>
                                                </div>
                                            </div>
                                            <div class="col-md-6 p-0">
                                                <div class="report-meta-box">
                                                    <p><span class="report-meta-label">Período:</span> <span class="report-meta-value" id="${prefix}-meta-period">01/01/2026 - 31/01/2026</span></p>
                                                    <p><span class="report-meta-label">Generado el:</span> <span class="report-meta-value" id="${prefix}-meta-printed"></span></p>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="ms-4">
                                        <button class="btn btn-sm btn-outline-danger mb-2 w-100" onclick="exportPDF('${prefix}Table', 'Reporte_${title.replace(/ /g, '_')}')"><i class="bi bi-file-pdf"></i> PDF</button>
                                        <button class="btn btn-sm btn-outline-success w-100" onclick="exportExcel('${prefix}Table', 'Reporte_${title.replace(/ /g, '_')}')"><i class="bi bi-file-excel"></i> Excel</button>
                                    </div>
                                </div>`;
}

function replaceHeader(htmlText, oldH5, title, prefix) {
    const regex = new RegExp(`<div class="d-flex justify-content-between align-items-center mb-3">\\s*<h5 class="fw-bold text-dark mb-0">${oldH5}</h5>\\s*<div>[\\s\\S]*?</div>\\s*</div>`);
    return htmlText.replace(regex, getHeaderTpl(title, prefix));
}

html = replaceHeader(html, "Detalle de Ventas Registradas", "Reporte General de Ventas", "sales");
html = replaceHeader(html, "Detalle de Adquisiciones \\(Compras\\)", "Reporte General de Compras", "purchases");
html = replaceHeader(html, "Movimientos de Inventario \\(Kardex\\)", "Movimientos de Inventario (Kardex)", "kardex");
html = replaceHeader(html, "Actividad y Cortes de Caja por Usuario", "Actividad de Usuarios y Turnos", "activity");
html = replaceHeader(html, "Estado de Flujos de Efectivo", "Estado de Flujos de Efectivo", "cashflow");

fs.writeFileSync('Frontend/pages/reports.html', html, 'utf8');

// 2. UPDATE REPORTS.JS
let js = fs.readFileSync('Frontend/pages/reports.js', 'utf8');

const metaUpdater = `
function updateReportMeta(prefix) {
    const branchSel = document.getElementById('filterBranchId');
    const branchName = branchSel.options[branchSel.selectedIndex].text;
    const userSel = document.getElementById('filterUserId');
    const userName = userSel.options[userSel.selectedIndex].text;
    const start = document.getElementById('filterStartDate').value;
    const end = document.getElementById('filterEndDate').value;
    
    const elBranch = document.getElementById(prefix + '-meta-branch');
    const elUser = document.getElementById(prefix + '-meta-user');
    const elPeriod = document.getElementById(prefix + '-meta-period');
    const elPrinted = document.getElementById(prefix + '-meta-printed');
    
    if(elBranch) elBranch.innerText = branchName;
    if(elUser) elUser.innerText = userName;
    if(elPeriod) elPeriod.innerText = \`\${start} a \${end}\`;
    if(elPrinted) elPrinted.innerText = new Date().toLocaleDateString();
}
`;

js = js.replace('/* ================= VENTAS ================= */', metaUpdater + '\n/* ================= VENTAS ================= */');

js = js.replace('        const data = await ApiClient.request(`/Reports/sales${query}`);', '        const data = await ApiClient.request(`/Reports/sales${query}`);\n        updateReportMeta("sales");');
js = js.replace('        const data = await ApiClient.request(`/Reports/purchases${query}`);', '        const data = await ApiClient.request(`/Reports/purchases${query}`);\n        updateReportMeta("purchases");');
js = js.replace('        const data = await ApiClient.request(`/Reports/kardex${getFilterParams()}`);', '        const data = await ApiClient.request(`/Reports/kardex${getFilterParams()}`);\n        updateReportMeta("kardex");');
js = js.replace('        const data = await ApiClient.request(`/Reports/user-activity${getFilterParams()}`);', '        const data = await ApiClient.request(`/Reports/user-activity${getFilterParams()}`);\n        updateReportMeta("activity");');
js = js.replace('        const data = await ApiClient.request(`/Reports/cash-flow${getFilterParams()}`);', '        const data = await ApiClient.request(`/Reports/cash-flow${getFilterParams()}`);\n        updateReportMeta("cashflow");');

fs.writeFileSync('Frontend/pages/reports.js', js, 'utf8');
