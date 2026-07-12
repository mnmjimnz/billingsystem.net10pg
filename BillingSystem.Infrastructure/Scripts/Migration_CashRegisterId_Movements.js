const { Client } = require('pg');

const client = new Client({
  connectionString: 'postgres://admin:3v59bzZTlJeh4Kfc3AohbBcYU63Y6h86@dpg-d8vej7p9rddc73c9e6jg-a.oregon-postgres.render.com:5432/billing_system_dtum',
  ssl: { rejectUnauthorized: false }
});

async function run() {
  await client.connect();
  console.log('Connected to Render DB');

  try {
    await client.query(`
      ALTER TABLE branchmovements ADD COLUMN IF NOT EXISTS cashregisterid INTEGER;
    `);
    console.log('Column cashregisterid added to branchmovements.');
  } catch (err) {
    console.error('Error:', err);
  } finally {
    await client.end();
  }
}

run();
