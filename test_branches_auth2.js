const https = require('https');
const payload = JSON.stringify({ username: "admin", password: "1234" });
const req = https.request({
  hostname: 'billingsystem-net10pg.onrender.com',
  path: '/api/Auth/login',
  method: 'POST',
  headers: { 'Content-Type': 'application/json', 'Content-Length': payload.length }
}, (res) => {
  let data = '';
  res.on('data', c => data += c);
  res.on('end', () => {
    const token = JSON.parse(data).token;
    if (!token) return console.log("Login failed");
    
    https.get('https://billingsystem-net10pg.onrender.com/api/Branches/paged?page=1&pageSize=10', { headers: { Authorization: `Bearer ${token}` } }, (res2) => {
        let d = '';
        res2.on('data', c => d += c);
        res2.on('end', () => console.log(d));
    });
  });
});
req.write(payload);
req.end();
