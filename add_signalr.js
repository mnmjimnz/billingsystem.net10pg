const fs = require('fs');
let html = fs.readFileSync('Frontend/pages/purchase-history.html', 'utf-8');

if (!html.includes('signalr-client.js')) {
    html = html.replace(
        '<script src="../assets/js/apiClient.js?v=20260627004119"></script>',
        '<script src="../assets/js/apiClient.js?v=20260627004119"></script>\n    <script src="../assets/js/signalr-client.js?v=20260627004119"></script>'
    );
    
    // Check if signalr.min.js is present (it might be missing too!)
    if (!html.includes('signalr.min.js')) {
        html = html.replace(
            '<!-- Libraries -->',
            '<!-- Libraries -->\n    <script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/8.0.0/signalr.min.js"></script>'
        );
    }

    fs.writeFileSync('Frontend/pages/purchase-history.html', html, 'utf-8');
    console.log("Added signalr-client.js to purchase-history.html");
} else {
    console.log("Already exists");
}
