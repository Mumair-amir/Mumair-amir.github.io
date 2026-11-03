const form = document.querySelector("#contact-form");//zoekt met een CSS-selector

const velden = [
    { id: "naam", boodschap: "Vul minimaal 2 tekens in." },
    { id: "email", boodschap: "Vul een geldig e-mailadres in." },
    { id: "bericht", boodschap: "Schrijf minimaal 10 tekens." }
];

function valideerVeld(veld) {
    const input = document.querySelector(`#${veld.id}`); //een waarde in een tekst plakken
    const foutmelding = document.querySelector(`#${veld.id}-error`);//
    const geldig = input.checkValidity();//checkt regels van html

    input.setAttribute("aria-invalid", String(!geldig)); // checkt en meldt fout
    foutmelding.textContent = geldig ? "" : veld.boodschap; // geldig boodschap wis dan alles

    return geldig;
}

form.addEventListener("submit", (event) => { //een korte schrijfwijze van een functie
    event.preventDefault();

    const alleGeldig = velden.map(valideerVeld).every(Boolean);
    const status = document.querySelector("#form-status");

    if (!alleGeldig) {
        status.textContent = "Er zijn nog fouten in het formulier.";
        return;
    }

    status.textContent = "Bericht verzonden! Bedankt.";
    form.reset();
});