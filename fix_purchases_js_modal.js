const fs = require('fs');
let code = fs.readFileSync('Frontend/pages/purchases.js', 'utf-8');

// 1. Remove handleResponsiveCheckout and its listeners
code = code.replace(/function handleResponsiveCheckout\(\) \{[\s\S]*?\}\s*window\.addEventListener\('resize', handleResponsiveCheckout\);\s*handleResponsiveCheckout\(\);/, '');

// 2. Change the savePurchase button binding. 
// btn-save-purchase is now opening the modal! So we don't bind it.
// The modal has btn-modal-save-purchase which we should bind to savePurchase!
code = code.replace(
    "document.getElementById('btn-save-purchase').addEventListener('click', () => {\n        savePurchase();\n    });",
    "// Removed btn-save-purchase listener"
);

code = code.replace(
    "document.getElementById('btn-modal-save-purchase').addEventListener('click', () => {\n        savePurchase();\n    });",
    ""
);

// Add the listener for btn-modal-save-purchase correctly (if it was there or not)
code = code.replace(
    '// Removed btn-save-purchase listener',
    "document.getElementById('btn-modal-save-purchase').addEventListener('click', () => {\n        savePurchase();\n    });"
);

// 3. Ensure the modal closes on success in savePurchase
code = code.replace(
    'cart = [];\n        renderCart();\n        document.getElementById(\'invoiceInput\').value = \'\';',
    'cart = [];\n        renderCart();\n        document.getElementById(\'invoiceInput\').value = \'\';\n        const modalEl = document.getElementById(\'mobileCheckoutModal\');\n        if (modalEl) {\n            const modal = bootstrap.Modal.getInstance(modalEl);\n            if (modal) modal.hide();\n        }'
);

fs.writeFileSync('Frontend/pages/purchases.js', code, 'utf-8');
console.log("Updated purchases.js");
