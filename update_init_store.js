const fs = require('fs');
let js = fs.readFileSync('Frontend/store/app.js', 'utf8');

js = js.replace(
    /function initStore\(\) \{/,
    'function initStore() {\n    loadStoreName();'
);

fs.writeFileSync('Frontend/store/app.js', js);
console.log("initStore updated");
