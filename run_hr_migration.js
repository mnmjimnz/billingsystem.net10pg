const { Client } = require('pg');

const client = new Client({
  connectionString: 'postgres://admin:3v59bzZTlJeh4Kfc3AohbBcYU63Y6h86@dpg-d8vej7p9rddc73c9e6jg-a.oregon-postgres.render.com:5432/billing_system_dtum',
  ssl: { rejectUnauthorized: false }
});

async function run() {
  await client.connect();
  
  const sql = `
    -- Update Users
    ALTER TABLE users ADD COLUMN IF NOT EXISTS "JobTitle" VARCHAR(100);
    ALTER TABLE users ADD COLUMN IF NOT EXISTS "DocumentId" VARCHAR(50);
    ALTER TABLE users ADD COLUMN IF NOT EXISTS "BaseBonus" DECIMAL(18,2);
    
    -- Update Company Settings
    ALTER TABLE companysettings ADD COLUMN IF NOT EXISTS "SocialSecurityPercentage" DECIMAL(5,2) DEFAULT 4.83;
    ALTER TABLE companysettings ADD COLUMN IF NOT EXISTS "IncomeTaxPercentage" DECIMAL(5,2) DEFAULT 0.00;
    
    -- Create Attendance Table
    CREATE TABLE IF NOT EXISTS attendances (
        "Id" SERIAL PRIMARY KEY,
        "UserId" INTEGER REFERENCES users("Id"),
        "Date" DATE NOT NULL,
        "CheckInTime" TIME,
        "CheckOutTime" TIME,
        "Status" VARCHAR(20) DEFAULT 'Present',
        "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    );
    
    -- Create PayrollRun Table
    CREATE TABLE IF NOT EXISTS payroll_runs (
        "Id" SERIAL PRIMARY KEY,
        "PeriodStart" DATE NOT NULL,
        "PeriodEnd" DATE NOT NULL,
        "ProcessedDate" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        "Notes" TEXT,
        "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    );
    
    -- Create PayrollDetail Table
    CREATE TABLE IF NOT EXISTS payroll_details (
        "Id" SERIAL PRIMARY KEY,
        "PayrollRunId" INTEGER REFERENCES payroll_runs("Id"),
        "UserId" INTEGER REFERENCES users("Id"),
        "BaseSalary" DECIMAL(18,2) NOT NULL,
        "ExtraHoursAmount" DECIMAL(18,2) DEFAULT 0.00,
        "BonusAmount" DECIMAL(18,2) DEFAULT 0.00,
        "DeductionsAmount" DECIMAL(18,2) DEFAULT 0.00,
        "NetPay" DECIMAL(18,2) NOT NULL,
        "Observations" TEXT,
        "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
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
