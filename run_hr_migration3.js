const { Client } = require('pg');

const client = new Client({
  connectionString: 'postgres://admin:3v59bzZTlJeh4Kfc3AohbBcYU63Y6h86@dpg-d8vej7p9rddc73c9e6jg-a.oregon-postgres.render.com:5432/billing_system_dtum',
  ssl: { rejectUnauthorized: false }
});

async function run() {
  await client.connect();
  
  try {
    // 1. Rename incometaxpercentage to afppercentage in companysettings
    console.log("Renaming incometaxpercentage to afppercentage in companysettings...");
    await client.query(`
      ALTER TABLE companysettings 
      RENAME COLUMN incometaxpercentage TO afppercentage;
    `);
  } catch (err) {
    console.log("Column incometaxpercentage might already be renamed or doesn't exist:", err.message);
  }

  try {
    // 2. Add incometaxpercentage to users
    console.log("Adding incometaxpercentage to users...");
    await client.query(`
      ALTER TABLE users
      ADD COLUMN incometaxpercentage DECIMAL(5, 2) DEFAULT 0;
    `);
  } catch (err) {
    console.log("Column incometaxpercentage might already exist in users:", err.message);
  }

  console.log("Migration finished.");
  await client.end();
}

run();
