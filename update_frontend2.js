const fs = require('fs');

// 5. Branch Movements HTML
let bmHtml = fs.readFileSync('Frontend/pages/branch-movements.html', 'utf8');
if (!bmHtml.includes('id="pagination-container"')) {
    bmHtml = bmHtml.replace('</tbody>\r\n                                </table>\r\n                            </div>\r\n                        </div>\r\n                    </div>', `</tbody>
                                </table>
                            </div>
                            <div id="pagination-container" class="mt-4"></div>
                        </div>
                    </div>`);
    fs.writeFileSync('Frontend/pages/branch-movements.html', bmHtml);
}

// 6. Branch Movements JS
let bmJs = fs.readFileSync('Frontend/pages/branch-movements.js', 'utf8');
if (!bmJs.includes("renderPagination('pagination-container'")) {
    bmJs = bmJs.replace("const items = await ApiClient.request(`/BranchMovements/branch/${branchId}`);", "const result = await ApiClient.request(`/BranchMovements/branch/${branchId}/paged?page=${page}&pageSize=10`);\n        const items = result.items || [];");
    bmJs = bmJs.replace("renderMovements();\n    } catch (e)", "renderMovements();\n        renderPagination('pagination-container', result, 'loadMovements');\n    } catch (e)");
    fs.writeFileSync('Frontend/pages/branch-movements.js', bmJs);
}

console.log("Branch movements frontend updated.");
