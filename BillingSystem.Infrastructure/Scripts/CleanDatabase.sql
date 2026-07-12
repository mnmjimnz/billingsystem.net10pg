BEGIN;

-- TRUNCATE all transactional tables and reset their identity columns
TRUNCATE TABLE 
    sales, 
    saledetails, 
    purchases, 
    purchasedetails, 
    journalentries, 
    journalentrydetails, 
    cashregistersessions, 
    branchmovements, 
    inventorymovements,
    accountsreceivable,
    receivablepayments,
    accountspayable,
    payablepayments,
    payrollruns,
    payrolldetails,
    orders,
    orderdetails,
    bankreconciliations,
    bankreconciliationdetails,
    stocktransfers,
    notifications
RESTART IDENTITY CASCADE;

-- Reset product stock and cost
UPDATE products 
SET stock = 0, 
    cost = 0;

-- Si tienes una tabla de stock por sucursal, también la reseteamos
UPDATE productstocks
SET stock = 0;

COMMIT;
