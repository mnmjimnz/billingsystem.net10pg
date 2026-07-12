const { Client } = require('pg');

const client = new Client({
    connectionString: 'postgres://admin:3v59bzZTlJeh4Kfc3AohbBcYU63Y6h86@dpg-d8vej7p9rddc73c9e6jg-a.oregon-postgres.render.com:5432/billing_system_dtum',
    ssl: { rejectUnauthorized: false }
});

async function run() {
    await client.connect();
    try {
        const res = await client.query(`SELECT column_name FROM information_schema.columns WHERE table_name = 'orders'`);
        console.log("Orders columns:", res.rows.map(r => r.column_name));
        const res2 = await client.query(`SELECT column_name FROM information_schema.columns WHERE table_name = 'orderdetails'`);
        console.log("OrderDetails columns:", res2.rows.map(r => r.column_name));
    } catch(e) {
        console.log("Error:", e.message);
    }
    await client.end();
}
run();
