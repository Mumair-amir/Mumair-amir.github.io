// Data: array met projecten
const projecten = [
    {
        titel: "Smart Telefoon Tas (EVI)",
        beschrijving: "Een slimme telefoontas die automatisch detecteert wanneer een telefoon wordt geplaatst. De tas registreert aanwezigheid, tijdstippen en koppelt deze gegevens aan een database. Ideaal voor scholen of werkplekken waar aanwezigheid gecontroleerd moet worden.",
        afbeelding: "EVI.jpeg",
        alt: "Smart Telefoon Tas",
        sorteerdatum: "2025-10",
        datumTekst: "1-10-2025"
    },
    {
        titel: "Hotel Simulatie",
        beschrijving: "Een uitgebreide simulatie van een hotel waarin verschillende events kunnen plaatsvinden. Gasten checken in en uit, schoonmakers onderhouden kamers, klanten hebben voedsel nodig, en noodsituaties zoals brand of evacuatie kunnen optreden. De simulatie bevat een event-systeem en movement-engine.",
        afbeelding: "Hotelsc.jpeg",
        alt: "Hotel Simulatie",
        sorteerdatum: "2026-03",
        datumTekst: "1-3-2026"
    },
    {
        titel: "Smart Environment",
        beschrijving: "Smart Environment is een webapplicatie die relevante informatie kan geven over weer en andere omstandigheden in kantoorgebouwen. Dit project is nog niet afgerond. Het team gaat binnenkort beginnen met het bouwen van de code. We gaan gebruikmaken van verschillende API's en een uitgebreide database.",
        afbeelding: null,
        alt: "",
        sorteerdatum: "2026-10",
        datumTekst: "1-10-2026"
    }
];

// Elementen ophalen
const lijst = document.getElementById("projectenlijst");
const sorteerSelect = document.getElementById("sorteerSelect");

// Functie 1: maakt één projectkaart
function maakProjectKaart(project) {
    const kaart = document.createElement("article");
    kaart.classList.add("project-item");

    const kop = document.createElement("h2");
    kop.textContent = project.titel;
    kaart.appendChild(kop);

    // Datum
    const datum = document.createElement("p");
    datum.classList.add("project-datum");
    const time = document.createElement("time");
    time.setAttribute("datetime", project.sorteerdatum);
    time.textContent = project.datumTekst;
    datum.appendChild(time);
    kaart.appendChild(datum);

    // Afbeelding (alleen als die er is)
    if (project.afbeelding) {
        const img = document.createElement("img");
        img.src = project.afbeelding;
        img.alt = project.alt;
        img.classList.add("project-afbeelding");
        kaart.appendChild(img);
    }

    // Beschrijving
    const beschrijving = document.createElement("p");
    beschrijving.textContent = project.beschrijving;
    kaart.appendChild(beschrijving);

    return kaart;
}

// Functie 2: rendert een lijst van projecten
function renderProjecten(lijstVanProjecten) {
    lijst.innerHTML = ""; // lijst leegmaken

    lijstVanProjecten.forEach(function (project) {
        const kaart = maakProjectKaart(project);
        lijst.appendChild(kaart);
    });
}

// Functie 3: sorteert en rendert
function sorteerEnRender() {
    const keuze = sorteerSelect.value;
    const gesorteerd = [...projecten]; // kopie, zodat origineel niet verandert

    gesorteerd.sort(function (a, b) {
        if (keuze === "nieuw-oud") {
            return b.sorteerdatum.localeCompare(a.sorteerdatum);
        } else {
            return a.sorteerdatum.localeCompare(b.sorteerdatum);
        }
    });

    renderProjecten(gesorteerd);
}

// Event listener op de select
sorteerSelect.addEventListener("change", sorteerEnRender);

// Bij het laden: standaard sortering tonen
sorteerEnRender();