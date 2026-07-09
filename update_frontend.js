const fs = require('fs');

// 1. Orders HTML
let ordersHtml = fs.readFileSync('Frontend/pages/orders.html', 'utf8');
if (!ordersHtml.includes('id="pagination-container"')) {
    ordersHtml = ordersHtml.replace('</tbody>\r\n                                    </table>\r\n                                </div>\r\n                            </div>\r\n                        </div>', `</tbody>
                                    </table>
                                </div>
                                <div id="pagination-container" class="mt-4"></div>
                            </div>
                        </div>`);
    fs.writeFileSync('Frontend/pages/orders.html', ordersHtml);
}

// 2. Orders JS
let ordersJs = fs.readFileSync('Frontend/pages/orders.js', 'utf8');
ordersJs = ordersJs.replace('async function loadOrders() {', 'async function loadOrders(page = 1) {');
ordersJs = ordersJs.replace("await ApiClient.request('/Orders?pageSize=100');", "await ApiClient.request(`/Orders?page=${page}&pageSize=10`);");
if (!ordersJs.includes("renderPagination('pagination-container'")) {
    ordersJs = ordersJs.replace('renderOrdersTable();\r\n    } catch', `renderOrdersTable();\n        renderPagination('pagination-container', response, 'loadOrders');\n    } catch`);
    fs.writeFileSync('Frontend/pages/orders.js', ordersJs);
}

// 3. Stock Transfers HTML
let stHtml = fs.readFileSync('Frontend/pages/stock-transfers.html', 'utf8');
if (!stHtml.includes('id="pagination-container"')) {
    stHtml = stHtml.replace('</tbody>\r\n                                </table>\r\n                            </div>\r\n                        </div>\r\n                    </div>', `</tbody>
                                </table>
                            </div>
                            <div id="pagination-container" class="mt-4"></div>
                        </div>
                    </div>`);
    fs.writeFileSync('Frontend/pages/stock-transfers.html', stHtml);
}

// 4. Stock Transfers JS
let stJs = fs.readFileSync('Frontend/pages/stock-transfers.js', 'utf8');
stJs = stJs.replace('async function loadTransfers() {', 'async function loadTransfers(page = 1) {');
stJs = stJs.replace("await ApiClient.request('/StockTransfers') || [];", "await ApiClient.request(`/StockTransfers/paged?page=${page}&pageSize=10`);");
if (!stJs.includes("renderPagination('pagination-container'")) {
    stJs = stJs.replace('const transfers = await ApiClient.request(`/StockTransfers/paged?page=${page}&pageSize=10`);', 'const result = await ApiClient.request(`/StockTransfers/paged?page=${page}&pageSize=10`);\n        const transfers = result.items || [];\n        renderPagination(\'pagination-container\', result, \'loadTransfers\');');
    fs.writeFileSync('Frontend/pages/stock-transfers.js', stJs);
}

console.log("Frontend partially updated. Waiting to do branch-movements.");
