const { Client } = require('pg');
const client = new Client({
  connectionString: 'postgres://admin:3v59bzZTlJeh4Kfc3AohbBcYU63Y6h86@dpg-d8vej7p9rddc73c9e6jg-a.oregon-postgres.render.com:5432/billing_system_dtum',
  ssl: { rejectUnauthorized: false }
});

async function run() {
  try {
    await client.connect();
    console.log("Connected to DB");
    const res = await client.query("ALTER TABLE Branches ADD COLUMN IF NOT EXISTS Latitude DECIMAL(10, 6), ADD COLUMN IF NOT EXISTS Longitude DECIMAL(10, 6)");
    console.log("Alter table successful", res);
  } catch (e) {
    console.error("Error", e);
  } finally {
    await client.end();
  }
}
run();
