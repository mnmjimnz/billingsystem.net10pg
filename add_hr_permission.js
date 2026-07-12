const { Client } = require('pg');

const client = new Client({
  connectionString: 'postgres://admin:3v59bzZTlJeh4Kfc3AohbBcYU63Y6h86@dpg-d8vej7p9rddc73c9e6jg-a.oregon-postgres.render.com:5432/billing_system_dtum',
  ssl: { rejectUnauthorized: false }
});

async function run() {
  await client.connect();
  
  try {
    const res = await client.query(`
      INSERT INTO permissions (SystemName, DisplayName, Description, Module, CreatedAt, IsActive)
      VALUES ('MANAGE_HR', 'Recursos Humanos', 'Permite gestionar expedientes, control de asistencia y planillas de pago.', 'RRHH', CURRENT_TIMESTAMP, true)
      ON CONFLICT DO NOTHING;
    `);
    console.log("Permission added.");
  } catch (err) {
    console.error("Failed:", err);
  } finally {
    await client.end();
  }
}

run();
