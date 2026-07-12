const { Client } = require('pg');
const client = new Client({
    connectionString: 'postgres://admin:3v59bzZTlJeh4Kfc3AohbBcYU63Y6h86@dpg-d8vej7p9rddc73c9e6jg-a.oregon-postgres.render.com:5432/billing_system_dtum',
    ssl: { rejectUnauthorized: false }
});
async function run() {
    await client.connect();
    try {
        const res = await client.query('SELECT * FROM "ThemeSettings"');
        console.table(res.rows);
    } catch(e) {
        console.error(e);
    } finally {
        await client.end();
    }
}
run();
