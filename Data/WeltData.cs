using MonsterKampf.Models;

namespace MonsterKampf.Data;

/// <summary>
/// Alle Orte der Spielwelt – Städte, Routen, Arenen
/// Koordinaten: GridX/GridY für die Weltkarte (0-based)
/// </summary>
public static class WeltData
{
    public static List<Ort> AlleOrte() => new()
    {
        // ── Startbereich ──────────────────────────────────────────────────────
        new Ort
        {
            Id = "startstadt", Name = "Neubeginn", Typ = "stadt", Farbe = "green",
            GridX = 3, GridY = 7,
            Beschreibung = "Deine Heimatstadt. Hier beginnt dein Abenteuer!",
            WildMonster = new(),
            Verbindungen = new() { "route1" }
        },
        new Ort
        {
            Id = "route1", Name = "Route 1", Typ = "route", Farbe = "forest",
            GridX = 3, GridY = 6,
            Beschreibung = "Ein einfacher Weg durch hohes Gras.",
            WildMonster = new()
            {
                new() { MonsterId = "PKM-0016", MinLevel = 2, MaxLevel = 5, Chance = 30 },
                new() { MonsterId = "PKM-0019", MinLevel = 2, MaxLevel = 4, Chance = 25 },
                new() { MonsterId = "PKM-0010", MinLevel = 3, MaxLevel = 5, Chance = 20 },
            },
            Trainer = new()
            {
                new() { Id = "t_route1_1", Name = "Jungtrainer Max", Klasse = "Jungtrainer", Belohnung = 150,
                    Team = new() { new() { MonsterId = "PKM-0016", Level = 4 } },
                    Dialogvor = "He! Ich fordere dich heraus!", DialogNach = "Du bist wirklich stark..." },
            },
            Verbindungen = new() { "startstadt", "blaustadt" }
        },
        new Ort
        {
            Id = "blaustadt", Name = "Blaustadt", Typ = "stadt", Farbe = "blue",
            GridX = 3, GridY = 5,
            Beschreibung = "Eine kleine Stadt am Fluss. Hier gibt es die erste Arena!",
            Arena = new Arena
            {
                OrdenName = "Wasserorden", OrdenNr = 1, Leiter = "Marina",
                TypSpezialisierung = "Tropfen",
                Team = new()
                {
                    new() { MonsterId = "PKM-0054", Level = 12 },
                    new() { MonsterId = "PKM-0007", Level = 14 },
                }
            },
            WildMonster = new(),
            Verbindungen = new() { "route1", "route2", "route3" }
        },
        new Ort
        {
            Id = "route2", Name = "Route 2", Typ = "route", Farbe = "forest",
            GridX = 2, GridY = 5,
            Beschreibung = "Ein Weg durch dichten Wald.",
            WildMonster = new()
            {
                new() { MonsterId = "PKM-0001", MinLevel = 5, MaxLevel = 8, Chance = 20 },
                new() { MonsterId = "PKM-0043", MinLevel = 4, MaxLevel = 7, Chance = 25 },
                new() { MonsterId = "PKM-0069", MinLevel = 5, MaxLevel = 8, Chance = 15 },
            },
            Trainer = new()
            {
                new() { Id = "t_route2_1", Name = "Wanderin Lisa", Klasse = "Wanderin", Belohnung = 200,
                    Team = new() { new() { MonsterId = "PKM-0001", Level = 8 }, new() { MonsterId = "PKM-0043", Level = 7 } },
                    Dialogvor = "Ich liebe Pflanzenwesen!", DialogNach = "Meine Pflanzenwesen..." },
            },
            Verbindungen = new() { "blaustadt", "gruenstadt" }
        },
        new Ort
        {
            Id = "route3", Name = "Route 3", Typ = "route", Farbe = "forest",
            GridX = 4, GridY = 5,
            Beschreibung = "Ein felsiger Pfad Richtung Osten.",
            WildMonster = new()
            {
                new() { MonsterId = "PKM-0027", MinLevel = 6, MaxLevel = 10, Chance = 25 },
                new() { MonsterId = "PKM-0074", MinLevel = 7, MaxLevel = 10, Chance = 20 },
                new() { MonsterId = "PKM-0050", MinLevel = 5, MaxLevel = 8, Chance = 20 },
            },
            Trainer = new()
            {
                new() { Id = "t_route3_1", Name = "Kämpfer Bruno", Klasse = "Kämpfer", Belohnung = 250,
                    Team = new() { new() { MonsterId = "PKM-0056", Level = 10 }, new() { MonsterId = "PKM-0066", Level = 9 } },
                    Dialogvor = "Ich trainiere täglich!", DialogNach = "Unmöglich..." },
            },
            Verbindungen = new() { "blaustadt", "feuerberg", "rotstadt" }
        },
        new Ort
        {
            Id = "gruenstadt", Name = "Grünstadt", Typ = "stadt", Farbe = "green",
            GridX = 2, GridY = 4,
            Beschreibung = "Eine Stadt mitten im Wald. Die Arena-Leiterin liebt Pflanzenwesen.",
            Arena = new Arena
            {
                OrdenName = "Blattorden", OrdenNr = 2, Leiter = "Flora",
                TypSpezialisierung = "Blatt",
                Team = new()
                {
                    new() { MonsterId = "PKM-0001", Level = 18 },
                    new() { MonsterId = "PKM-0043", Level = 18 },
                    new() { MonsterId = "PKM-0071", Level = 20 },
                }
            },
            WildMonster = new(),
            Verbindungen = new() { "route2", "route4" }
        },
        new Ort
        {
            Id = "feuerberg", Name = "Feuerberg", Typ = "hoehle", Farbe = "red",
            GridX = 5, GridY = 5,
            Beschreibung = "Ein aktiver Vulkan. Brennen-Wesen leben hier.",
            WildMonster = new()
            {
                new() { MonsterId = "PKM-0004", MinLevel = 12, MaxLevel = 16, Chance = 20 },
                new() { MonsterId = "PKM-0037", MinLevel = 10, MaxLevel = 14, Chance = 25 },
                new() { MonsterId = "PKM-0058", MinLevel = 11, MaxLevel = 15, Chance = 20 },
            },
            Verbindungen = new() { "route3", "rotstadt" }
        },
        new Ort
        {
            Id = "rotstadt", Name = "Rotstadt", Typ = "stadt", Farbe = "red",
            GridX = 5, GridY = 4,
            Beschreibung = "Eine heiße Stadt am Vulkan. Der Arena-Leiter ist ein Feuer-Meister.",
            Arena = new Arena
            {
                OrdenName = "Feuerorden", OrdenNr = 3, Leiter = "Ignis",
                TypSpezialisierung = "Brennen",
                Team = new()
                {
                    new() { MonsterId = "PKM-0058", Level = 25 },
                    new() { MonsterId = "PKM-0077", Level = 25 },
                    new() { MonsterId = "PKM-0004", Level = 28 },
                }
            },
            WildMonster = new(),
            Verbindungen = new() { "feuerberg", "route3", "route5" }
        },
        new Ort
        {
            Id = "route4", Name = "Route 4", Typ = "route", Farbe = "forest",
            GridX = 2, GridY = 3,
            Beschreibung = "Ein langer Weg durch die Berge.",
            WildMonster = new()
            {
                new() { MonsterId = "PKM-0063", MinLevel = 15, MaxLevel = 20, Chance = 20 },
                new() { MonsterId = "PKM-0060", MinLevel = 14, MaxLevel = 18, Chance = 25 },
            },
            Trainer = new()
            {
                new() { Id = "t_route4_1", Name = "Psycho-Mia", Klasse = "Psycho-Trainerin", Belohnung = 400,
                    Team = new() { new() { MonsterId = "PKM-0063", Level = 18 }, new() { MonsterId = "PKM-0079", Level = 17 } },
                    Dialogvor = "Ich sehe deine Zukunft... du verlierst!", DialogNach = "Meine Vorhersage war falsch..." },
            },
            Verbindungen = new() { "gruenstadt", "silberstadt" }
        },
        new Ort
        {
            Id = "route5", Name = "Route 5", Typ = "route", Farbe = "gray",
            GridX = 5, GridY = 3,
            Beschreibung = "Eine steinige Route Richtung Norden.",
            WildMonster = new()
            {
                new() { MonsterId = "PKM-0074", MinLevel = 20, MaxLevel = 25, Chance = 25 },
                new() { MonsterId = "PKM-0095", MinLevel = 18, MaxLevel = 22, Chance = 20 },
            },
            Trainer = new()
            {
                new() { Id = "t_route5_1", Name = "Steinbrecher Karl", Klasse = "Bergmann", Belohnung = 450,
                    Team = new() { new() { MonsterId = "PKM-0074", Level = 22 }, new() { MonsterId = "PKM-0095", Level = 21 } },
                    Dialogvor = "Niemand kommt hier durch!", DialogNach = "Unglaublich..." },
            },
            Verbindungen = new() { "rotstadt", "goldstadt" }
        },
        new Ort
        {
            Id = "silberstadt", Name = "Silberstadt", Typ = "stadt", Farbe = "blue",
            GridX = 2, GridY = 2,
            Beschreibung = "Eine elegante Stadt. Der Arena-Leiter kämpft mit Psycho-Wesen.",
            Arena = new Arena
            {
                OrdenName = "Psychoorden", OrdenNr = 4, Leiter = "Mentis",
                TypSpezialisierung = "Psycho",
                Team = new()
                {
                    new() { MonsterId = "PKM-0064", Level = 32 },
                    new() { MonsterId = "PKM-0080", Level = 32 },
                    new() { MonsterId = "PKM-0065", Level = 35 },
                }
            },
            WildMonster = new(),
            Verbindungen = new() { "route4", "route6" }
        },
        new Ort
        {
            Id = "goldstadt", Name = "Goldstadt", Typ = "stadt", Farbe = "yellow",
            GridX = 5, GridY = 2,
            Beschreibung = "Die reichste Stadt. Der Arena-Leiter setzt auf Blitz-Wesen.",
            Arena = new Arena
            {
                OrdenName = "Blitzorden", OrdenNr = 5, Leiter = "Voltus",
                TypSpezialisierung = "Blitz",
                Team = new()
                {
                    new() { MonsterId = "PKM-0025", Level = 35 },
                    new() { MonsterId = "PKM-0026", Level = 35 },
                    new() { MonsterId = "PKM-0082", Level = 38 },
                }
            },
            WildMonster = new(),
            Verbindungen = new() { "route5", "route6", "route7" }
        },
        new Ort
        {
            Id = "route6", Name = "Route 6", Typ = "route", Farbe = "forest",
            GridX = 3, GridY = 2,
            Beschreibung = "Eine breite Route zwischen zwei Städten.",
            WildMonster = new()
            {
                new() { MonsterId = "PKM-0025", MinLevel = 25, MaxLevel = 30, Chance = 15 },
                new() { MonsterId = "PKM-0035", MinLevel = 22, MaxLevel = 28, Chance = 20 },
            },
            Verbindungen = new() { "silberstadt", "goldstadt" }
        },
        new Ort
        {
            Id = "route7", Name = "Route 7 – Eispass", Typ = "route", Farbe = "blue",
            GridX = 6, GridY = 2,
            Beschreibung = "Ein eisiger Gebirgspass. Eis-Wesen lauern hier.",
            WildMonster = new()
            {
                new() { MonsterId = "PKM-0086", MinLevel = 28, MaxLevel = 35, Chance = 20 },
                new() { MonsterId = "PKM-0087", MinLevel = 30, MaxLevel = 35, Chance = 15 },
                new() { MonsterId = "PKM-0124", MinLevel = 28, MaxLevel = 33, Chance = 10 },
            },
            Trainer = new()
            {
                new() { Id = "t_route7_1", Name = "Eismeisterin Frida", Klasse = "Eistrainerin", Belohnung = 600,
                    Team = new() { new() { MonsterId = "PKM-0087", Level = 32 }, new() { MonsterId = "PKM-0124", Level = 30 } },
                    Dialogvor = "Der Eispass gehört mir!", DialogNach = "Ich friere vor Scham..." },
            },
            Verbindungen = new() { "goldstadt", "endstadt" }
        },
        new Ort
        {
            Id = "endstadt", Name = "Endstadt", Typ = "stadt", Farbe = "purple",
            GridX = 7, GridY = 2,
            Beschreibung = "Die letzte Stadt. Hier wartet die stärkste Arena!",
            Arena = new Arena
            {
                OrdenName = "Meisterorden", OrdenNr = 8, Leiter = "Grandmaster Rex",
                TypSpezialisierung = "Drache",
                Team = new()
                {
                    new() { MonsterId = "PKM-0147", Level = 45 },
                    new() { MonsterId = "PKM-0148", Level = 48 },
                    new() { MonsterId = "PKM-0149", Level = 52 },
                }
            },
            WildMonster = new(),
            Verbindungen = new() { "route7" }
        },
    };

    public static readonly Dictionary<string, string> OrtFarben = new()
    {
        { "green",  "#166534" },
        { "blue",   "#1e3a5f" },
        { "red",    "#7f1d1d" },
        { "yellow", "#713f12" },
        { "purple", "#4a1d96" },
        { "forest", "#14532d" },
        { "cave",   "#44403c" },
        { "gray",   "#374151" },
    };
}
