const https = require('https');

function ping() {
    https.get('https://billingsystem-net10pg.onrender.com/migrate-orders', (res) => {
        let data = '';
        res.on('data', chunk => data += chunk);
        res.on('end', () => {
            if (res.statusCode === 200) {
                console.log("Migration successful: ", data);
                process.exit(0);
            } else {
                console.log(`Waiting for backend deployment... status: ${res.statusCode}`);
                setTimeout(ping, 10000);
            }
        });
    }).on('error', (e) => {
        console.log(`Error: ${e.message}, retrying...`);
        setTimeout(ping, 10000);
    });
}
ping();
