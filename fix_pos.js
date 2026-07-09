const fs = require('fs');

let html = fs.readFileSync('Frontend/pages/pos.html', 'utf8');
html = html.replace('<input type="number" class="form-control" id="checkout-tendered" value="0" min="0" oninput="calculateChange()">',
                    '<input type="text" inputmode="decimal" class="form-control" id="checkout-tendered" value="0.00" oninput="calculateChange()">');
fs.writeFileSync('Frontend/pages/pos.html', html);

let js = fs.readFileSync('Frontend/pages/pos.js', 'utf8');

js = js.replace("const tendered = parseFloat(document.getElementById('checkout-tendered').value) || 0;", 
                "let val = document.getElementById('checkout-tendered').value.replace(',', '.');\n    const tendered = parseFloat(val) || 0;");

// There are two occurrences of this line (one in calculateChange, one in confirmSale). Let's replace the second one as well if it didn't do it globally.
// Oh wait, replace() only replaces the first occurrence!
// Let's use split.join for global replace:
js = js.split("const tendered = parseFloat(document.getElementById('checkout-tendered').value) || 0;")
       .join("let valTendered = document.getElementById('checkout-tendered').value.replace(',', '.');\n    const tendered = parseFloat(valTendered) || 0;");

js = js.replace("if (paymentType === 'CASH' && tendered < currentSaleTotal) {",
                "if (paymentType === 'CASH' && Math.round(tendered * 100) < Math.round(currentSaleTotal * 100)) {");

fs.writeFileSync('Frontend/pages/pos.js', js);
console.log("Fixed POS precision and comma issues.");
