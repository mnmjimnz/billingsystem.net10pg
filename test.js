const { Client } = require('pg');
const client = new Client({
    connectionString: 'postgres://admin:3v59bzZTlJeh4Kfc3AohbBcYU63Y6h86@dpg-d8vej7p9rddc73c9e6jg-a.oregon-postgres.render.com:5432/billing_system_dtum',
    ssl: { rejectUnauthorized: false }
});
async function run() {
    await client.connect();
    try {
        await client.query('UPDATE companysettings SET "ActiveThemeId" = 1');
        console.log('success updating ActiveThemeId');
    } catch(e) {
        console.log('Error updating ActiveThemeId:', e.message);
    }
    
    try {
        const res = await client.query('SELECT * FROM companysettings LIMIT 1');
        console.log(res.rows[0]);
    } catch(e) {
        console.log('Error selecting:', e.message);
    }
    await client.end();
}
run();
