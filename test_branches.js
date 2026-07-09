const https = require('https');
https.get('https://billingsystem-net10pg.onrender.com/api/Branches/paged?page=1&pageSize=10', (res) => {
  let data = '';
  res.on('data', chunk => data += chunk);
  res.on('end', () => console.log(data));
});
