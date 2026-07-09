const fs = require('fs');
let content = fs.readFileSync('Frontend/pages/purchase-history.js', 'utf-8');

const regex = /<td class="fw-bold">\$\$\{item\.total\.toFixed\(2\)\}<\/td>\s*<\/tr>/;
const replacement = `<td class="fw-bold">$\${item.total.toFixed(2)}</td>
                    <td class="text-center">
                        <button class="btn btn-sm btn-outline-primary" onclick="viewPurchaseDetails(\${item.id})">
                            <i class="bi bi-eye"></i> Ver
                        </button>
                    </td>
                </tr>`;

if (regex.test(content)) {
    content = content.replace(regex, replacement);
    fs.writeFileSync('Frontend/pages/purchase-history.js', content, 'utf-8');
    console.log("Replaced successfully.");
} else {
    console.log("Regex did not match.");
}
