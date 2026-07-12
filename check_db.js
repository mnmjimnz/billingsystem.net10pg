const { Client } = require('pg');

const client = new Client({
    connectionString: 'postgres://admin:3v59bzZTlJeh4Kfc3AohbBcYU63Y6h86@dpg-d8vej7p9rddc73c9e6jg-a.oregon-postgres.render.com:5432/billing_system_dtum',
    ssl: { rejectUnauthorized: false }
});

async function run() {
    await client.connect();
    try {
        const branches = await client.query('SELECT * FROM branches');
        console.log("Branches:", branches.rows);
        
        const customers = await client.query('SELECT * FROM customers LIMIT 1');
        console.log("Customers:", customers.rows);
    } catch(e) {
        console.log("Error:", e.message);
    }
    await client.end();
}
run();
