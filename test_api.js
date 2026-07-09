// Make a fetch to the backend directly via node
const https = require('https');
const options = {
    hostname: 'billingsystem-net10pg.onrender.com',
    port: 443,
    path: '/api/Reports/sales?startDate=2026-06-28T06:00:00.000Z&endDate=2026-06-29T05:59:59.000Z',
    method: 'GET'
};
// wait I don't have auth token
