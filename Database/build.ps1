$files = @(
    "init.sql",
    "migration.sql",
    "migration_roles.sql",
    "migration_orders.sql",
    "migration_product_stocks.sql",
    "migration_purchases.sql",
    "migration_movements_hr.sql",
    "migration_store.sql",
    "missing_tables.sql",
    "migration_theme_engine.sql",
    "migration_store_settings.sql"
)

Remove-Item DB_COMPLETE.SQL -ErrorAction SilentlyContinue
foreach ($file in $files) {
    Get-Content $file | Add-Content DB_COMPLETE.SQL
}
