const fs = require('fs');
let js = fs.readFileSync('Frontend/pages/orders.js', 'utf-8');

js = js.replace(/\}\r?\n?\} catch \(e\) \{ showToast\("Error al cancelar", "error"\); \}\r?\n?\}/m, "}\n}");

// Let's just do a generic replace
js = js.replace(/\} catch \(e\) \{ showToast\("Error al cancelar", "error"\); \}/g, "");

fs.writeFileSync('Frontend/pages/orders.js', js, 'utf-8');
console.log("Fixed syntax error");
