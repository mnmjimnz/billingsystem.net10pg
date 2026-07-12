async function run() {
    try {
        const loginRes = await fetch("https://billingsystem-net10pg.onrender.com/api/Auth/login", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ email: "pedrop@gmail.com", password: "password" })
        });
        const loginData = await loginRes.json();
        console.log("Login:", loginRes.status, loginData);
        
        if (!loginData.token) return;

        const checkoutRes = await fetch("https://billingsystem-net10pg.onrender.com/api/Store/checkout", {
            method: "POST",
            headers: { 
                "Content-Type": "application/json",
                "Authorization": `Bearer ${loginData.token}`
            },
            body: JSON.stringify({
                items: [{ productId: 1, quantity: 1, price: 10.0 }],
                deliveryAddress: "Test",
                latitude: 0,
                longitude: 0,
                receiverName: "Test",
                notes: "Test",
                paymentMethod: "EFECTIVO"
            })
        });
        
        const txt = await checkoutRes.text();
        console.log("Checkout:", checkoutRes.status, txt);
    } catch(e) {
        console.log("Error:", e.message);
    }
}
run();
