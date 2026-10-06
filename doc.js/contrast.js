const contrastToggle = document.querySelector("#contrast-toggle");

function pasContrastAan() {
    if (!contrastToggle) return;

    const isHoogContrast = document.body.classList.contains("hoog-contrast");
    contrastToggle.setAttribute("aria-pressed", String(isHoogContrast));
    contrastToggle.textContent = isHoogContrast ? "Laag contrast" : "Hoog contrast";
}

function toggleContrast() {
    document.body.classList.toggle("hoog-contrast");
    const isHoogContrast = document.body.classList.contains("hoog-contrast");
    localStorage.setItem("hoogContrast", isHoogContrast ? "aan" : "uit");
    pasContrastAan();
}

// Herstel de keuze bij het laden
if (localStorage.getItem("hoogContrast") === "aan") {
    document.body.classList.add("hoog-contrast");
}
pasContrastAan();

if (contrastToggle) {
    contrastToggle.addEventListener("click", toggleContrast);
}