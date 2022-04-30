document.addEventListener("DOMContentLoaded", function () {

    /* 🌌 GET ELEMENTS */
    const bg = document.querySelector(".bg");
    const bg1 = document.querySelectorAll('#no-process').length;
    const particleContainer = document.querySelector(".particles");

    /* 🚀 FLOATING CARDS */
    const titles = [
        "ASDC Vetting", "ACG Vetting", "AHCC Vetting",
        "IPA Stage", "IAM Integ", "Remote Test",
        "Content Vetting", "Whitelisting", "Lab Test",
        "Design", "Auto Committee", "IPA Stage", "Testing", "Deploy", "UI Fix", "IAM Integ", "Arch Vetting","Foreclosed"
    ];

    if (bg && bg1==0) {
        for (let i = 0; i < 8; i++) {
            let card = document.createElement("div");
            card.className = "card";

            card.innerHTML = `
                ${titles[Math.floor(Math.random() * titles.length)]}
                <br>✔ Progress
            `;

            card.style.left = Math.random() * 100 + "vw";
            card.style.animationDuration = (8 + Math.random() * 6) + "s";
            card.style.animationDelay = Math.random() * 5 + "s";

            // random direction (more natural)
            card.style.animationDirection = Math.random() > 0.5 ? "normal" : "reverse";

            bg.appendChild(card);
        }
    }

    /* ✨ PARTICLES */
    if (particleContainer) {
        for (let i = 0; i < 40; i++) {
            let particle = document.createElement("span");

            particle.style.left = Math.random() * 100 + "vw";
            particle.style.animationDuration = (5 + Math.random() * 5) + "s";
            particle.style.animationDelay = Math.random() * 5 + "s";

            particleContainer.appendChild(particle);
        }
    }

});


/* 🔐 OPTIONAL (for future use) */
function enter() {
    const intro = document.getElementById("intro");
    const login = document.getElementById("login");

    if (intro) intro.style.display = "none";
    if (login) login.style.display = "flex";
}