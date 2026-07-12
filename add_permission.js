const { Client } = require('pg');
const client = new Client({
  connectionString: 'postgres://admin:3v59bzZTlJeh4Kfc3AohbBcYU63Y6h86@dpg-d8vej7p9rddc73c9e6jg-a.oregon-postgres.render.com:5432/billing_system_dtum',
  ssl: { rejectUnauthorized: false }
});
async function run() {
  await client.connect();
  try {
    const roleRes = await client.query('SELECT id FROM roles WHERE name = $1', ['Admin']);
    if(roleRes.rows.length > 0) {
       const roleId = roleRes.rows[0].id;
       const perRes = await client.query('SELECT id FROM permissions WHERE systemname = $1', ['MANAGE_ACCOUNTING']);
       let pId;
       if(perRes.rows.length === 0) {
           const ins = await client.query('INSERT INTO permissions (systemname, displayname, module) VALUES ($1, $2, $3) RETURNING id', ['MANAGE_ACCOUNTING', 'Administrar Contabilidad', 'Accounting']);
           pId = ins.rows[0].id;
       } else {
           pId = perRes.rows[0].id;
       }
       const hasRes = await client.query('SELECT * FROM rolepermissions WHERE roleid = $1 AND permissionid = $2', [roleId, pId]);
       if(hasRes.rows.length === 0) {
           await client.query('INSERT INTO rolepermissions (roleid, permissionid) VALUES ($1, $2)', [roleId, pId]);
       }
       console.log('Permission MANAGE_ACCOUNTING added successfully to Admin role.');
    }
  } finally {
    await client.end();
  }
}
run();
