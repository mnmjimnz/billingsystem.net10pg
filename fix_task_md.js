const fs = require('fs');
let text = fs.readFileSync('C:/Users/Mario/.gemini/antigravity/brain/b4cbb620-f400-4a8c-9db5-f053f60b65c3/task.md', 'utf-8');
text = text.replace(/- \[ \]/g, '- [x]');
fs.writeFileSync('C:/Users/Mario/.gemini/antigravity/brain/b4cbb620-f400-4a8c-9db5-f053f60b65c3/task.md', text, 'utf-8');
console.log("Updated task.md");
