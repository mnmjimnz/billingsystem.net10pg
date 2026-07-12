const { Client } = require('pg');

const client = new Client({
  connectionString: 'postgres://admin:3v59bzZTlJeh4Kfc3AohbBcYU63Y6h86@dpg-d8vej7p9rddc73c9e6jg-a.oregon-postgres.render.com:5432/billing_system_dtum',
  ssl: { rejectUnauthorized: false }
});

async function run() {
  await client.connect();
  
  const sql = `
    -- Update Users
    ALTER TABLE users ADD COLUMN IF NOT EXISTS jobtitle VARCHAR(100);
    ALTER TABLE users ADD COLUMN IF NOT EXISTS documentid VARCHAR(50);
    ALTER TABLE users ADD COLUMN IF NOT EXISTS basebonus DECIMAL(18,2);
    
    -- Update Company Settings
    ALTER TABLE companysettings ADD COLUMN IF NOT EXISTS socialsecuritypercentage DECIMAL(5,2) DEFAULT 4.83;
    ALTER TABLE companysettings ADD COLUMN IF NOT EXISTS incometaxpercentage DECIMAL(5,2) DEFAULT 0.00;
    
    -- Create Attendance Table
    CREATE TABLE IF NOT EXISTS attendances (
        id SERIAL PRIMARY KEY,
        userid INTEGER REFERENCES users(id),
        date DATE NOT NULL,
        checkintime TIME,
        checkouttime TIME,
        status VARCHAR(20) DEFAULT 'Present',
        createdat TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        updatedat TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        isactive BOOLEAN DEFAULT TRUE
    );
    
    -- Create PayrollRun Table
    CREATE TABLE IF NOT EXISTS payrollruns (
        id SERIAL PRIMARY KEY,
        periodstart DATE NOT NULL,
        periodend DATE NOT NULL,
        processeddate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        notes TEXT,
        createdat TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        updatedat TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        isactive BOOLEAN DEFAULT TRUE
    );
    
    -- Create PayrollDetail Table
    CREATE TABLE IF NOT EXISTS payrolldetails (
        id SERIAL PRIMARY KEY,
        payrollrunid INTEGER REFERENCES payrollruns(id),
        userid INTEGER REFERENCES users(id),
        basesalary DECIMAL(18,2) NOT NULL,
        extrahoursamount DECIMAL(18,2) DEFAULT 0.00,
        bonusamount DECIMAL(18,2) DEFAULT 0.00,
        deductionsamount DECIMAL(18,2) DEFAULT 0.00,
        netpay DECIMAL(18,2) NOT NULL,
        observations TEXT,
        createdat TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        updatedat TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        isactive BOOLEAN DEFAULT TRUE
    );
  `;
  
  try {
    await client.query(sql);
    console.log("Migration executed successfully!");
  } catch (err) {
    console.error("Migration failed:", err);
  } finally {
    await client.end();
  }
}

run();
