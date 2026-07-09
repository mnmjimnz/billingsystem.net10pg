const https = require('https');

function ping() {
    https.get('https://billingsystem-net10pg.onrender.com/add-permission-transfers', (res) => {
        if (res.statusCode === 200) {
            console.log('Migration successful!');
            process.exit(0);
        } else {
            console.log('Got ' + res.statusCode + ', retrying in 10s...');
            setTimeout(ping, 10000);
        }
    }).on('error', (e) => {
        console.log('Error: ' + e.message + ', retrying in 10s...');
        setTimeout(ping, 10000);
    });
}
ping();
