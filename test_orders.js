async function run() {
    try {
        const loginRes = await fetch("https://billingsystem-net10pg.onrender.com/api/Auth/login", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ email: "pedrop@gmail.com", password: "password" })
        });
        const loginData = await loginRes.json();
        
        const ordersRes = await fetch("https://billingsystem-net10pg.onrender.com/api/Store/orders", {
            headers: { 'Authorization': `Bearer ${loginData.token}` }
        });
        const storeOrders = await ordersRes.json();
        console.log("Store Orders (first order):", JSON.stringify(storeOrders[0], null, 2));

        const adminLogin = await fetch("https://billingsystem-net10pg.onrender.com/api/Auth/login", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ email: "admin@nexus.com", password: "password" })
        });
        const adminData = await adminLogin.json();
        const adminRes = await fetch(`https://billingsystem-net10pg.onrender.com/api/Orders/${storeOrders[0].id}`, {
            headers: { 'Authorization': `Bearer ${adminData.token}` }
        });
        const adminOrder = await adminRes.json();
        console.log("Admin Order (first order):", JSON.stringify(adminOrder, null, 2));

    } catch(e) {
        console.log("Error:", e.message);
    }
}
run();
