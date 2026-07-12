BEGIN;

-- 1. Desvincular usuarios de sucursales (para evitar que se eliminen los usuarios)
UPDATE users SET branchid = NULL;

-- 2. Eliminar inventario específico por sucursal
DELETE FROM productstocks;

-- 3. Eliminar cajas y sucursales (usamos DELETE para respetar FKs si queda alguna oculta, o TRUNCATE)
-- Dado que ya vaciamos las transacciones (ventas, compras, etc.), podemos hacer esto:
TRUNCATE TABLE 
    cashregisters,
    branches
RESTART IDENTITY CASCADE;

-- 4. Eliminar logística y rutas
TRUNCATE TABLE 
    delivery_routes,
    route_stops,
    vehicles,
    drivers
RESTART IDENTITY CASCADE;

-- 5. Eliminar expedientes / asistencias (ya vaciamos planillas antes)
TRUNCATE TABLE 
    attendances
RESTART IDENTITY CASCADE;

COMMIT;
