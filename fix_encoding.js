const fs = require('fs');

function fixFile(path) {
    try {
        // Read file as a buffer
        const buffer = fs.readFileSync(path);
        
        // Decode buffer as windows-1252 to recover the original characters
        const decoded = new TextDecoder('windows-1252').decode(buffer);
        
        // Write it back as UTF-8
        fs.writeFileSync(path, decoded, 'utf-8');
        console.log("Fixed " + path);
    } catch(e) {
        console.error(e);
    }
}

fixFile('Frontend/pages/orders.html');
fixFile('Frontend/pages/orders.js');
fixFile('Frontend/pages/branches.js');
fixFile('Frontend/pages/branches.html');
