const { Client } = require('pg');

const client = new Client({
  connectionString: 'postgres://admin:3v59bzZTlJeh4Kfc3AohbBcYU63Y6h86@dpg-d8vej7p9rddc73c9e6jg-a.oregon-postgres.render.com:5432/billing_system_dtum',
  ssl: { rejectUnauthorized: false }
});

async function run() {
  await client.connect();
  
  try {
    const res = await client.query(`
      SELECT table_name 
      FROM information_schema.tables 
      WHERE table_schema = 'public';
    `);
    console.log(res.rows.map(r => r.table_name));
  } catch (err) {
    console.error("Migration failed:", err);
  } finally {
    await client.end();
  }
}

run();
