const fs = require('fs');
const path = require('path');

const directoryPath = path.join(__dirname, 'Frontend', 'pages');

fs.readdir(directoryPath, (err, files) => {
    if (err) {
        return console.log('Unable to scan directory: ' + err);
    } 

    files.forEach((file) => {
        if (file.endsWith('.html') && file !== 'login.html' && file !== 'unauthorized.html') {
            const filePath = path.join(directoryPath, file);
            let content = fs.readFileSync(filePath, 'utf-8');
            
            // Replace sidebar nav content
            const navRegex = /<nav class="sidebar-nav">[\s\S]*?<\/nav>/g;
            content = content.replace(navRegex, '<nav class="sidebar-nav"></nav>');

            // Include menu.js if not already there
            if (!content.includes('menu.js')) {
                content = content.replace('</body>', '    <script src="../assets/js/menu.js"></script>\n</body>');
            }

            fs.writeFileSync(filePath, content, 'utf-8');
            console.log('Updated ' + file);
        }
    });
});
