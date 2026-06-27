const { Client } = require('pg');

const connectionString = 'postgresql://admin:3v59bzZTlJeh4Kfc3AohbBcYU63Y6h86@dpg-d8vej7p9rddc73c9e6jg-a.oregon-postgres.render.com:5432/billing_system_dtum?ssl=true';

const client = new Client({
  connectionString: connectionString,
  ssl: {
    rejectUnauthorized: false
  }
});

const brands = ['Apple', 'Samsung', 'Sony', 'LG', 'ASUS', 'HP', 'Dell', 'Lenovo', 'Nintendo', 'Microsoft', 'Nikon', 'Canon', 'Xiaomi', 'Huawei', 'Razer', 'Logitech', 'Bose', 'JBL'];
const types = [
    { name: 'Smartphone', priceMin: 300, priceMax: 1500, desc: 'Teléfono inteligente de última generación' },
    { name: 'Laptop', priceMin: 500, priceMax: 3000, desc: 'Computadora portátil de alto rendimiento' },
    { name: 'Smart TV', priceMin: 400, priceMax: 2500, desc: 'Televisor inteligente 4K UHD' },
    { name: 'Tablet', priceMin: 200, priceMax: 1000, desc: 'Tableta táctil con pantalla retina' },
    { name: 'Smartwatch', priceMin: 100, priceMax: 500, desc: 'Reloj inteligente con monitor de salud' },
    { name: 'Auriculares', priceMin: 50, priceMax: 350, desc: 'Auriculares inalámbricos con cancelación de ruido' },
    { name: 'Cámara Digital', priceMin: 400, priceMax: 2000, desc: 'Cámara fotográfica profesional' },
    { name: 'Consola de Videojuegos', priceMin: 300, priceMax: 600, desc: 'Consola de entretenimiento de última generación' },
    { name: 'Monitor', priceMin: 150, priceMax: 800, desc: 'Monitor gamer alta tasa de refresco' },
    { name: 'Teclado Mecánico', priceMin: 50, priceMax: 200, desc: 'Teclado para juegos con switches mecánicos' },
    { name: 'Mouse Gamer', priceMin: 30, priceMax: 150, desc: 'Ratón óptico de alta precisión' },
    { name: 'Disco Duro SSD', priceMin: 80, priceMax: 300, desc: 'Unidad de estado sólido NVMe ultra rápida' },
    { name: 'Placa Madre', priceMin: 100, priceMax: 400, desc: 'Placa base para procesadores de última generación' },
    { name: 'Tarjeta Gráfica', priceMin: 300, priceMax: 2000, desc: 'Tarjeta de video para juegos en 4K' },
    { name: 'Procesador', priceMin: 150, priceMax: 800, desc: 'CPU de alto rendimiento para productividad' }
];

const models = ['Pro', 'Max', 'Lite', 'Ultra', 'Plus', 'Air', 'Mini', 'Series X', 'V2', 'Elite', 'Gaming', 'Xtreme', 'Studio', 'OLED', 'QLED', 'Slim'];

function getRandomItem(arr) {
    return arr[Math.floor(Math.random() * arr.length)];
}

function getRandomPrice(min, max) {
    return parseFloat((Math.random() * (max - min) + min).toFixed(2));
}

function generateProduct(index) {
    const brand = getRandomItem(brands);
    const type = getRandomItem(types);
    const model = getRandomItem(models);
    
    // Gen number like 100, 200, 990...
    const number = Math.floor(Math.random() * 99 + 1) * 10;
    
    const name = `${brand} ${type.name} ${model} ${number}`;
    const desc = `${type.desc}. Modelo exclusivo de la línea ${model}.`;
    
    const price = getRandomPrice(type.priceMin, type.priceMax);
    const cost = parseFloat((price * (Math.random() * 0.4 + 0.4)).toFixed(2)); // Cost is 40% to 80% of price
    
    const stock = Math.floor(Math.random() * 50) + 5; // 5 to 54 in stock
    const barcode = `BAR${Date.now()}${index.toString().padStart(4, '0')}`;
    
    return {
        barcode,
        name,
        description: desc,
        price,
        cost,
        stock
    };
}

async function seed() {
    try {
        await client.connect();
        console.log("Conectado a la base de datos PostgreSQL en Render");
        
        // Ensure category "Electrónicos" exists
        let catRes = await client.query(`SELECT id FROM categories WHERE name = 'Electrónicos'`);
        let categoryId;
        if (catRes.rows.length === 0) {
            const insertCat = await client.query(`INSERT INTO categories (name, description, createdat, isactive) VALUES ('Electrónicos', 'Dispositivos de tecnología', CURRENT_TIMESTAMP, true) RETURNING id`);
            categoryId = insertCat.rows[0].id;
        } else {
            categoryId = catRes.rows[0].id;
        }
        console.log("Categoría Electrónicos ID:", categoryId);

        let count = 0;
        for (let i = 0; i < 200; i++) {
            const p = generateProduct(i);
            await client.query(`
                INSERT INTO products (barcode, name, description, price, cost, stock, categoryid, createdat, isactive)
                VALUES ($1, $2, $3, $4, $5, $6, $7, CURRENT_TIMESTAMP, true)
            `, [p.barcode, p.name, p.description, p.price, p.cost, p.stock, categoryId]);
            count++;
        }
        
        console.log(`¡Se insertaron ${count} productos correctamente!`);
    } catch (e) {
        console.error("Error:", e);
    } finally {
        await client.end();
    }
}

seed();
