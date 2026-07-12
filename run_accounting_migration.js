const { Client } = require('pg');

const client = new Client({
  connectionString: 'postgres://admin:3v59bzZTlJeh4Kfc3AohbBcYU63Y6h86@dpg-d8vej7p9rddc73c9e6jg-a.oregon-postgres.render.com:5432/billing_system_dtum',
  ssl: { rejectUnauthorized: false }
});

async function run() {
  await client.connect();
  
  const sql = `
    -- Create Accounts Table
    CREATE TABLE IF NOT EXISTS accounts (
        id SERIAL PRIMARY KEY,
        code VARCHAR(20) NOT NULL UNIQUE,
        name VARCHAR(100) NOT NULL,
        type VARCHAR(50) NOT NULL,
        parentaccountid INTEGER REFERENCES accounts(id),
        level INTEGER DEFAULT 1,
        allowstransactions BOOLEAN DEFAULT true,
        description TEXT,
        createdat TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        updatedat TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        isactive BOOLEAN DEFAULT TRUE
    );

    -- Create BankAccounts Table
    CREATE TABLE IF NOT EXISTS bankaccounts (
        id SERIAL PRIMARY KEY,
        bankname VARCHAR(100) NOT NULL,
        accountnumber VARCHAR(50) NOT NULL,
        currency VARCHAR(10) DEFAULT 'USD',
        currentbalance DECIMAL(18,2) DEFAULT 0.00,
        linkedaccountid INTEGER REFERENCES accounts(id),
        createdat TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        updatedat TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        isactive BOOLEAN DEFAULT TRUE
    );

    -- Create JournalEntries Table if it doesn't exist
    CREATE TABLE IF NOT EXISTS journalentries (
        id SERIAL PRIMARY KEY,
        date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
        description TEXT,
        referencetype VARCHAR(50),
        referenceid INTEGER,
        createdat TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        updatedat TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        isactive BOOLEAN DEFAULT TRUE
    );

    -- Create JournalEntryDetails Table if it doesn't exist
    CREATE TABLE IF NOT EXISTS journalentrydetails (
        id SERIAL PRIMARY KEY,
        journalentryid INTEGER REFERENCES journalentries(id),
        accountid INTEGER REFERENCES accounts(id),
        accountcode VARCHAR(50),
        accountname VARCHAR(100),
        debit DECIMAL(18,2) DEFAULT 0.00,
        credit DECIMAL(18,2) DEFAULT 0.00,
        createdat TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        updatedat TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        isactive BOOLEAN DEFAULT TRUE
    );
    
    -- Ensure accountid column exists if table was already created before
    ALTER TABLE journalentrydetails ADD COLUMN IF NOT EXISTS accountid INTEGER REFERENCES accounts(id);

    -- Create BankReconciliations Table
    CREATE TABLE IF NOT EXISTS bankreconciliations (
        id SERIAL PRIMARY KEY,
        bankaccountid INTEGER REFERENCES bankaccounts(id),
        statementdate DATE NOT NULL,
        statementbalance DECIMAL(18,2) NOT NULL,
        status VARCHAR(20) DEFAULT 'Draft',
        notes TEXT,
        createdat TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        updatedat TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        isactive BOOLEAN DEFAULT TRUE
    );

    -- Create BankReconciliationDetails Table
    CREATE TABLE IF NOT EXISTS bankreconciliationdetails (
        id SERIAL PRIMARY KEY,
        bankreconciliationid INTEGER REFERENCES bankreconciliations(id),
        journalentrydetailid INTEGER REFERENCES journalentrydetails(id),
        iscleared BOOLEAN DEFAULT false,
        createdat TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        updatedat TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        isactive BOOLEAN DEFAULT TRUE
    );
  `;

  try {
    console.log("Executing schema creation...");
    await client.query(sql);
    console.log("Schema creation executed successfully!");
    
    // Seed standard chart of accounts
    console.log("Seeding Chart of Accounts...");
    
    const countRes = await client.query("SELECT COUNT(*) FROM accounts");
    if (parseInt(countRes.rows[0].count) === 0) {
        const accounts = [
            // 1. ACTIVOS
            { code: '1', name: 'Activos', type: 'Asset', parent: null, level: 1, allowsTx: false },
            { code: '1.01', name: 'Activos Circulantes', type: 'Asset', parent: '1', level: 2, allowsTx: false },
            { code: '1.01.01', name: 'Caja General', type: 'Asset', parent: '1.01', level: 3, allowsTx: true },
            { code: '1.01.02', name: 'Bancos', type: 'Asset', parent: '1.01', level: 3, allowsTx: true },
            { code: '1.01.03', name: 'Cuentas por Cobrar Clientes', type: 'Asset', parent: '1.01', level: 3, allowsTx: true },
            { code: '1.01.04', name: 'Inventario de Mercancía', type: 'Asset', parent: '1.01', level: 3, allowsTx: true },
            { code: '1.02', name: 'Activos No Circulantes', type: 'Asset', parent: '1', level: 2, allowsTx: false },
            { code: '1.02.01', name: 'Mobiliario y Equipo', type: 'Asset', parent: '1.02', level: 3, allowsTx: true },
            { code: '1.02.02', name: 'Equipo de Cómputo', type: 'Asset', parent: '1.02', level: 3, allowsTx: true },
            
            // 2. PASIVOS
            { code: '2', name: 'Pasivos', type: 'Liability', parent: null, level: 1, allowsTx: false },
            { code: '2.01', name: 'Pasivos a Corto Plazo', type: 'Liability', parent: '2', level: 2, allowsTx: false },
            { code: '2.01.01', name: 'Cuentas por Pagar Proveedores', type: 'Liability', parent: '2.01', level: 3, allowsTx: true },
            { code: '2.01.02', name: 'Impuestos por Pagar', type: 'Liability', parent: '2.01', level: 3, allowsTx: true },
            { code: '2.01.03', name: 'Retenciones por Pagar (ISSS/AFP)', type: 'Liability', parent: '2.01', level: 3, allowsTx: true },
            { code: '2.01.04', name: 'Sueldos por Pagar', type: 'Liability', parent: '2.01', level: 3, allowsTx: true },
            
            // 3. CAPITAL
            { code: '3', name: 'Capital', type: 'Equity', parent: null, level: 1, allowsTx: false },
            { code: '3.01', name: 'Capital Social', type: 'Equity', parent: '3', level: 2, allowsTx: true },
            { code: '3.02', name: 'Utilidades Acumuladas', type: 'Equity', parent: '3', level: 2, allowsTx: true },
            { code: '3.03', name: 'Utilidad del Ejercicio', type: 'Equity', parent: '3', level: 2, allowsTx: true },
            
            // 4. INGRESOS
            { code: '4', name: 'Ingresos', type: 'Revenue', parent: null, level: 1, allowsTx: false },
            { code: '4.01', name: 'Ingresos Operativos', type: 'Revenue', parent: '4', level: 2, allowsTx: false },
            { code: '4.01.01', name: 'Ventas de Mercancía', type: 'Revenue', parent: '4.01', level: 3, allowsTx: true },
            { code: '4.01.02', name: 'Ventas de Servicios', type: 'Revenue', parent: '4.01', level: 3, allowsTx: true },
            
            // 5. COSTOS
            { code: '5', name: 'Costos', type: 'Cost', parent: null, level: 1, allowsTx: false },
            { code: '5.01', name: 'Costo de Ventas', type: 'Cost', parent: '5', level: 2, allowsTx: false },
            { code: '5.01.01', name: 'Costo de Mercancía Vendida', type: 'Cost', parent: '5.01', level: 3, allowsTx: true },
            
            // 6. GASTOS
            { code: '6', name: 'Gastos', type: 'Expense', parent: null, level: 1, allowsTx: false },
            { code: '6.01', name: 'Gastos de Operación', type: 'Expense', parent: '6', level: 2, allowsTx: false },
            { code: '6.01.01', name: 'Gastos de Sueldos y Salarios', type: 'Expense', parent: '6.01', level: 3, allowsTx: true },
            { code: '6.01.02', name: 'Gastos de Alquiler', type: 'Expense', parent: '6.01', level: 3, allowsTx: true },
            { code: '6.01.03', name: 'Gastos de Servicios Públicos', type: 'Expense', parent: '6.01', level: 3, allowsTx: true },
            { code: '6.01.04', name: 'Gastos de Oficina', type: 'Expense', parent: '6.01', level: 3, allowsTx: true }
        ];

        let idMap = {};
        for (let acc of accounts) {
            let parentId = acc.parent ? idMap[acc.parent] : null;
            const res = await client.query(`
                INSERT INTO accounts (code, name, type, parentaccountid, level, allowstransactions, createdat, isactive)
                VALUES ($1, $2, $3, $4, $5, $6, CURRENT_TIMESTAMP, TRUE)
                RETURNING id
            `, [acc.code, acc.name, acc.type, parentId, acc.level, acc.allowsTx]);
            idMap[acc.code] = res.rows[0].id;
        }
        
        console.log("Standard Chart of Accounts seeded successfully.");
    } else {
        console.log("Accounts table is not empty, skipping seed.");
    }

  } catch (err) {
    console.error("Migration failed:", err);
  } finally {
    await client.end();
  }
}

run();
