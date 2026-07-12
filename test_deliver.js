async function run() {
    try {
        const adminLogin = await fetch("https://billingsystem-net10pg.onrender.com/api/Auth/login", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ email: "admin", password: "password" })
        });
        const adminData = await adminLogin.json();
        
        console.log("Admin token:", adminData.token ? "OK" : "FAIL");

    } catch(e) {
        console.log("Error:", e.message);
    }
}
run();
