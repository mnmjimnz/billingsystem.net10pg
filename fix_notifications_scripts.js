const fs = require('fs');
let html = fs.readFileSync('Frontend/pages/notifications.html', 'utf-8');

const oldScripts = `    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    <script src="../assets/js/apiClient.js"></script>
    <script src="../assets/js/ui-core.js"></script>
    <script src="../assets/js/menu.js"></script>
    <script src="notifications.js"></script>`;

const newScripts = `    <script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/8.0.0/signalr.min.js"></script>
    <script src="../assets/js/theme.js"></script>
    <script src="../assets/js/toast.js"></script>
    <script src="../assets/js/apiClient.js"></script>
    <script src="../assets/js/signalr-client.js"></script>
    <script src="notifications.js"></script>
    <script src="../assets/js/ui-core.js"></script>
    <script src="../assets/js/menu.js"></script>`;

if (html.includes('<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>')) {
    html = html.replace(oldScripts, newScripts);
    fs.writeFileSync('Frontend/pages/notifications.html', html, 'utf-8');
    console.log("Fixed notifications scripts");
} else {
    console.log("Could not find old scripts");
}
