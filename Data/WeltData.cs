using MonsterKampf.Models;

namespace MonsterKampf.Data;

/// <summary>
/// Alle 468 Orte aus 9 Regionen – generiert aus pokemon_game_data.json v3
/// </summary>
public static class WeltData
{
    // Farb-Mapping fuer Ort-Typen
    public static readonly Dictionary<string, string> OrtFarben = new()
    {
        { "blue", "#2563eb" },
        { "green", "#16a34a" },
        { "red", "#dc2626" },
        { "yellow", "#ca8a04" },
        { "purple", "#9333ea" },
        { "orange", "#ea580c" },
        { "gray", "#4b5563" },
        { "teal", "#0d9488" },
        { "pink", "#db2777" },
        { "indigo", "#4f46e5" },
        { "cyan", "#0891b2" },
        { "lime", "#65a30d" },
        { "brown", "#78350f" },
        { "slate", "#475569" },
    };

    private static List<ShopItem> StandardMarkt() => new()
    {
        new() { Id="item-001", Name="Heiltrank",   Beschreibung="Heilt 20 KP.",   Preis=300,  Emoji="🧪", Kategorie="Heilung" },
        new() { Id="item-002", Name="Supertrank",  Beschreibung="Heilt 50 KP.",   Preis=700,  Emoji="💊", Kategorie="Heilung" },
        new() { Id="item-003", Name="Hypertrank",  Beschreibung="Heilt 200 KP.",  Preis=1200, Emoji="💉", Kategorie="Heilung" },
        new() { Id="item-004", Name="Monsterball", Beschreibung="Faengt Monster.", Preis=200,  Emoji="⚪", Kategorie="Fangen" },
        new() { Id="item-005", Name="Superball",   Beschreibung="Bessere Chance.", Preis=600,  Emoji="🔵", Kategorie="Fangen" },
        new() { Id="item-006", Name="Hyperball",   Beschreibung="Hohe Chance.",    Preis=1200, Emoji="🟡", Kategorie="Fangen" },
        new() { Id="item-007", Name="Beleber",     Beschreibung="Belebt Monster.", Preis=1500, Emoji="💫", Kategorie="Heilung" },
        new() { Id="item-008", Name="Antidot",     Beschreibung="Heilt Gift.",     Preis=100,  Emoji="🟢", Kategorie="Status" },
    };

    public static List<Ort> AlleOrte() => new()
    {
        // === Kanto (60 Orte) ===
        new() {
            Id = "KAN-0001", Name = "Pallet Town", Typ = "ort",
            Farbe = "purple", GridX = 4, GridY = 12,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAN-0002",
            Ost = "KAN-0043",
            Verbindungen = new() { "KAN-0002", "KAN-0043" },
            Trainer = new() {
                new() {
                    Id = "KAN-RIV-001", Name = "Blau", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Startkampf", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0001", Level = 5 }, new() { MonsterId = "PKM-0004", Level = 5 }, new() { MonsterId = "PKM-0007", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0002", Name = "Route 1", Typ = "ort",
            Farbe = "green", GridX = 4, GridY = 11,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAN-0003",
            Süd = "KAN-0001",
            Verbindungen = new() { "KAN-0003", "KAN-0001" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0016", MinLevel = 2, MaxLevel = 5, Chance = 50 }, // Taubsi
                new() { MonsterId = "PKM-0019", MinLevel = 2, MaxLevel = 4, Chance = 50 }, // Rattfratz
            },
            Trainer = new() {
                new() {
                    Id = "UNO-K-001", Name = "Bianca", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Bianca auf Route 1", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0495", Level = 5 }, new() { MonsterId = "PKM-0498", Level = 5 }, new() { MonsterId = "PKM-0501", Level = 5 } },
                },
                new() {
                    Id = "UNO-T-001", Name = "Youngster", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "ALO-T-001", Name = "Hau", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0003", Name = "Viridian City", Typ = "ort",
            Farbe = "purple", GridX = 4, GridY = 10,
            HatMonsterCenter = true, HatMarkt = true,
            Nord = "KAN-0005",
            Süd = "KAN-0002",
            West = "KAN-0004",
            Verbindungen = new() { "KAN-0005", "KAN-0002", "KAN-0004" },
            Trainer = new() {
                new() {
                    Id = "KAN-GYM-008", Name = "Giovanni", Klasse = "Arena/Boss",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-GYM-008-HGSS", Name = "Blau", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            MarktAngebot = new() {
                new() { Id = "item-001", Name = "Pokéball", Preis = 200, Emoji = "🛍️", Kategorie = "Markt" },
                new() { Id = "item-002", Name = "Trank", Preis = 300, Emoji = "🛍️", Kategorie = "Markt" },
                new() { Id = "item-003", Name = "Gegengift", Preis = 100, Emoji = "🛍️", Kategorie = "Markt" },
            },
            Arena = new() {
                Leiter = "Giovanni", OrdenName = "Erdorden",
                OrdenNr = 8, TypSpezialisierung = "Normal",
                Team = new() {
                    new() { MonsterId = "PKM-0111", Level = 45 }, // Rihorn
                    new() { MonsterId = "PKM-0051", Level = 42 }, // Digdri
                    new() { MonsterId = "PKM-0031", Level = 44 }, // Nidoqueen
                    new() { MonsterId = "PKM-0034", Level = 45 }, // Nidoking
                },
            },
        },
        new() {
            Id = "KAN-0004", Name = "Route 22", Typ = "ort",
            Farbe = "green", GridX = 1, GridY = 10,
            HatMonsterCenter = false, HatMarkt = false,
            Ost = "KAN-0003",
            Verbindungen = new() { "KAN-0003" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0019", MinLevel = 2, MaxLevel = 4, Chance = 30 }, // Rattfratz
                new() { MonsterId = "PKM-0056", MinLevel = 3, MaxLevel = 5, Chance = 30 }, // Menki
                new() { MonsterId = "PKM-0021", MinLevel = 3, MaxLevel = 5, Chance = 40 }, // Habitak
            },
            Trainer = new() {
                new() {
                    Id = "KAN-RIV-002", Name = "Blau", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Optional", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-RIV-007", Name = "Blau", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Vor Liga", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0130", Level = 45 }, new() { MonsterId = "PKM-0003", Level = 45 }, new() { MonsterId = "PKM-0006", Level = 45 } },
                },
            },
        },
        new() {
            Id = "KAN-0005", Name = "Route 2", Typ = "ort",
            Farbe = "green", GridX = 4, GridY = 7,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAN-0007",
            Süd = "KAN-0003",
            Verbindungen = new() { "KAN-0007", "KAN-0003" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0016", MinLevel = 3, MaxLevel = 5, Chance = 40 }, // Taubsi
                new() { MonsterId = "PKM-0019", MinLevel = 3, MaxLevel = 5, Chance = 30 }, // Rattfratz
                new() { MonsterId = "PKM-0010", MinLevel = 4, MaxLevel = 5, Chance = 30 }, // Raupy
            },
            Trainer = new() {
                new() {
                    Id = "UNO-T-002", Name = "Cheren", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAL-T-001", Name = "Youngster", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "GAL-T-001", Name = "Hop", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "ALO-OPT-001", Name = "Yuka", Klasse = "Pokémon-Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Früher Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0006", Name = "Viridian Forest", Typ = "ort",
            Farbe = "forest", GridX = 3, GridY = 8,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAN-0005",
            Verbindungen = new() { "KAN-0005" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0010", MinLevel = 3, MaxLevel = 5, Chance = 30 }, // Raupy
                new() { MonsterId = "PKM-0013", MinLevel = 3, MaxLevel = 5, Chance = 30 }, // Hornliu
                new() { MonsterId = "PKM-0025", MinLevel = 3, MaxLevel = 5, Chance = 5 }, // Pikachu
                new() { MonsterId = "PKM-0011", MinLevel = 4, MaxLevel = 6, Chance = 20 }, // Safcon
                new() { MonsterId = "PKM-0014", MinLevel = 4, MaxLevel = 6, Chance = 15 }, // Kokuna
            },
            Trainer = new() {
                new() {
                    Id = "t-vf-01", Name = "Käfersammler Rick", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Ich fordere dich heraus!", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0007", Name = "Pewter City", Typ = "ort",
            Farbe = "purple", GridX = 4, GridY = 6,
            HatMonsterCenter = true, HatMarkt = true,
            Nord = "KAN-0008",
            Süd = "KAN-0005",
            Verbindungen = new() { "KAN-0008", "KAN-0005" },
            Trainer = new() {
                new() {
                    Id = "KAN-GYM-001", Name = "Rocko", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-GYM-001-HGSS", Name = "Rocko", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            MarktAngebot = new() {
                new() { Id = "item-001", Name = "Pokéball", Preis = 200, Emoji = "🛍️", Kategorie = "Markt" },
                new() { Id = "item-002", Name = "Trank", Preis = 300, Emoji = "🛍️", Kategorie = "Markt" },
                new() { Id = "item-004", Name = "Fluchtseil", Preis = 550, Emoji = "🛍️", Kategorie = "Markt" },
            },
            Arena = new() {
                Leiter = "Rocko", OrdenName = "Felsorden",
                OrdenNr = 1, TypSpezialisierung = "Normal",
                Team = new() {
                    new() { MonsterId = "PKM-0074", Level = 12 }, // Kleinstein
                    new() { MonsterId = "PKM-0095", Level = 14 }, // Onix
                },
            },
        },
        new() {
            Id = "KAN-0008", Name = "Route 3", Typ = "ort",
            Farbe = "green", GridX = 5, GridY = 6,
            HatMonsterCenter = false, HatMarkt = false,
            Ost = "KAN-0009",
            Verbindungen = new() { "KAN-0009" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0021", MinLevel = 6, MaxLevel = 8, Chance = 35 }, // Habitak
                new() { MonsterId = "PKM-0027", MinLevel = 6, MaxLevel = 8, Chance = 25 }, // Sandan
                new() { MonsterId = "PKM-0032", MinLevel = 6, MaxLevel = 7, Chance = 20 }, // Nidoran♂
                new() { MonsterId = "PKM-0029", MinLevel = 6, MaxLevel = 7, Chance = 20 }, // Nidoran♀
            },
            Trainer = new() {
                new() {
                    Id = "t-r3-01", Name = "Wanderer Liam", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Ich fordere dich heraus!", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "t-r3-02", Name = "Göre Lisa", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Ich fordere dich heraus!", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0009", Name = "Mt. Moon", Typ = "ort",
            Farbe = "cave", GridX = 7, GridY = 6,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAN-0008",
            Ost = "KAN-0010",
            Verbindungen = new() { "KAN-0008", "KAN-0010" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0041", MinLevel = 7, MaxLevel = 10, Chance = 50 }, // Zubat
                new() { MonsterId = "PKM-0035", MinLevel = 8, MaxLevel = 12, Chance = 5 }, // Piepi
                new() { MonsterId = "PKM-0046", MinLevel = 8, MaxLevel = 10, Chance = 25 }, // Paras
                new() { MonsterId = "PKM-0074", MinLevel = 8, MaxLevel = 10, Chance = 20 }, // Kleinstein
            },
            Trainer = new() {
                new() {
                    Id = "KAN-NPC-1002", Name = "Super Nerd", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "JOH-K-007", Name = "Silver", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Rivale am Mt. Moon (Kanto)", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0160", Level = 35 }, new() { MonsterId = "PKM-0154", Level = 35 }, new() { MonsterId = "PKM-0157", Level = 35 } },
                },
                new() {
                    Id = "KAN-T-003", Name = "Team Rocket Rüpel", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-T-004", Name = "Nerd (Fossil)", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0010", Name = "Route 4", Typ = "ort",
            Farbe = "green", GridX = 9, GridY = 6,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAN-0009",
            Ost = "KAN-0011",
            Verbindungen = new() { "KAN-0009", "KAN-0011" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0021", MinLevel = 8, MaxLevel = 12, Chance = 35 }, // Habitak
                new() { MonsterId = "PKM-0023", MinLevel = 8, MaxLevel = 12, Chance = 35 }, // Rettan
                new() { MonsterId = "PKM-0056", MinLevel = 10, MaxLevel = 12, Chance = 30 }, // Menki
            },
            Trainer = new() {
                new() {
                    Id = "UNO-T-006", Name = "Bianca", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAL-T-003", Name = "Lass", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0011", Name = "Cerulean City", Typ = "ort",
            Farbe = "purple", GridX = 11, GridY = 6,
            HatMonsterCenter = true, HatMarkt = true,
            Nord = "KAN-0012",
            Süd = "KAN-0014",
            West = "KAN-0010",
            Ost = "KAN-0020",
            Verbindungen = new() { "KAN-0012", "KAN-0014", "KAN-0010", "KAN-0020" },
            Trainer = new() {
                new() {
                    Id = "KAN-GYM-002", Name = "Misty", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-GYM-002-HGSS", Name = "Misty", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            MarktAngebot = new() {
                new() { Id = "item-001", Name = "Pokéball", Preis = 200, Emoji = "🛍️", Kategorie = "Markt" },
                new() { Id = "item-002", Name = "Trank", Preis = 300, Emoji = "🛍️", Kategorie = "Markt" },
                new() { Id = "item-005", Name = "Superball", Preis = 600, Emoji = "🛍️", Kategorie = "Markt" },
                new() { Id = "item-006", Name = "Supertrank", Preis = 700, Emoji = "🛍️", Kategorie = "Markt" },
            },
            Arena = new() {
                Leiter = "Misty", OrdenName = "Quellorden",
                OrdenNr = 2, TypSpezialisierung = "Normal",
                Team = new() {
                    new() { MonsterId = "PKM-0120", Level = 18 }, // Sterndu
                    new() { MonsterId = "PKM-0121", Level = 21 }, // Starmie
                },
            },
        },
        new() {
            Id = "KAN-0012", Name = "Route 24", Typ = "ort",
            Farbe = "green", GridX = 11, GridY = 4,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAN-0013",
            Süd = "KAN-0011",
            Verbindungen = new() { "KAN-0013", "KAN-0011" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0010", MinLevel = 7, MaxLevel = 8, Chance = 20 }, // Raupy
                new() { MonsterId = "PKM-0063", MinLevel = 8, MaxLevel = 12, Chance = 15 }, // Abra
                new() { MonsterId = "PKM-0043", MinLevel = 12, MaxLevel = 14, Chance = 25 }, // Myrapla
                new() { MonsterId = "PKM-0069", MinLevel = 12, MaxLevel = 14, Chance = 25 }, // Knofensa
            },
            Trainer = new() {
                new() {
                    Id = "t-r24-01", Name = "Camper Ethan", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Ich fordere dich heraus!", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "t-r24-02", Name = "Picknickerin Dana", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Ich fordere dich heraus!", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0013", Name = "Route 25", Typ = "ort",
            Farbe = "blue", GridX = 12, GridY = 4,
            HatMonsterCenter = false, HatMarkt = false,
            Ost = "KAN-0012",
            Verbindungen = new() { "KAN-0012" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0043", MinLevel = 13, MaxLevel = 17, Chance = 20 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-NPC-1008", Name = "Camper Florian", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0014", Name = "Route 5", Typ = "ort",
            Farbe = "green", GridX = 11, GridY = 7,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "KAN-0011",
            Verbindungen = new() { "KAN-0011" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0043", MinLevel = 13, MaxLevel = 15, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0060", MinLevel = 13, MaxLevel = 15, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 16, MaxLevel = 20, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 9, MaxLevel = 11, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0819", MinLevel = 16, MaxLevel = 20, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAL-K-001", Name = "Serena/Calem", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Rivale auf Route 5", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0650", Level = 12 }, new() { MonsterId = "PKM-0653", Level = 12 }, new() { MonsterId = "PKM-0656", Level = 12 } },
                },
                new() {
                    Id = "GAL-OPT-001", Name = "Adrian", Klasse = "Pokémon Breeder",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Früher Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0015", Name = "Route 6", Typ = "ort",
            Farbe = "blue", GridX = 11, GridY = 10,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAN-0016",
            Süd = "KAL-0010",
            Verbindungen = new() { "KAN-0016", "KAL-0010" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0043", MinLevel = 13, MaxLevel = 15, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0060", MinLevel = 13, MaxLevel = 15, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0585", MinLevel = 20, MaxLevel = 24, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 11, MaxLevel = 13, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0819", MinLevel = 22, MaxLevel = 26, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-NPC-1009", Name = "Jr. Trainerin", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0016", Name = "Vermilion City", Typ = "ort",
            Farbe = "purple", GridX = 11, GridY = 11,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "KAL-0012",
            West = "KAL-0023",
            Verbindungen = new() { "KAL-0012", "KAL-0023" },
            Trainer = new() {
                new() {
                    Id = "KAN-GYM-003", Name = "Lt. Surge", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-GYM-003-HGSS", Name = "Surge", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Major Bob", OrdenName = "Donnerorden",
                OrdenNr = 11, TypSpezialisierung = "Elektro",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 65 } },
            },
        },
        new() {
            Id = "KAN-0017", Name = "S.S. Anne", Typ = "ort",
            Farbe = "green", GridX = 11, GridY = 12,
            HatMonsterCenter = false, HatMarkt = false,
            Trainer = new() {
                new() {
                    Id = "KAN-RIV-004", Name = "Blau", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "vor Cut", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-T-006", Name = "Sailor", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0018", Name = "Route 11", Typ = "ort",
            Farbe = "blue", GridX = 13, GridY = 11,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0026",
            Ost = "KAN-0016",
            Verbindungen = new() { "KAL-0026", "KAN-0016" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0023", MinLevel = 15, MaxLevel = 20, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 28, MaxLevel = 32, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 21, MaxLevel = 23, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-NPC-1010", Name = "Gentleman Dirk", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0019", Name = "Route 12", Typ = "ort",
            Farbe = "blue", GridX = 14, GridY = 10,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0023",
            Süd = "KAL-0028",
            West = "KAL-0028",
            Verbindungen = new() { "KAL-0023", "KAL-0028", "KAL-0028" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0069", MinLevel = 15, MaxLevel = 20, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0016", MinLevel = 18, MaxLevel = 22, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0019", MinLevel = 18, MaxLevel = 22, Chance = 35 }, // Unbekannt
                new() { MonsterId = "PKM-0585", MinLevel = 28, MaxLevel = 32, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0072", MinLevel = 22, MaxLevel = 26, Chance = 60 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-T-013", Name = "Fisher", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAL-OPT-002", Name = "Hugo", Klasse = "Sky Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Sky Battle", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0020", Name = "Route 9", Typ = "ort",
            Farbe = "green", GridX = 12, GridY = 6,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0021",
            Ost = "KAN-0011",
            Verbindungen = new() { "KAL-0021", "KAN-0011" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0016", MinLevel = 18, MaxLevel = 22, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 26, MaxLevel = 30, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 17, MaxLevel = 19, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0072", MinLevel = 34, MaxLevel = 38, Chance = 60 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-NPC-1011", Name = "Hiker", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-T-007", Name = "Hiker", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0021", Name = "Route 10", Typ = "ort",
            Farbe = "blue", GridX = 14, GridY = 6,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "KAN-0024",
            Ost = "KAL-0019",
            Verbindungen = new() { "KAN-0024", "KAL-0019" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0081", MinLevel = 22, MaxLevel = 25, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 28, MaxLevel = 32, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 19, MaxLevel = 21, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0819", MinLevel = 38, MaxLevel = 42, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAL-T-006", Name = "Tierno", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0022", Name = "Rock Tunnel", Typ = "ort",
            Farbe = "cave", GridX = 14, GridY = 8,
            HatMonsterCenter = false, HatMarkt = false,
            WildMonster = new() {
                new() { MonsterId = "PKM-0041", MinLevel = 15, MaxLevel = 20, Chance = 50 }, // Unbekannt
                new() { MonsterId = "PKM-0074", MinLevel = 15, MaxLevel = 20, Chance = 25 }, // Unbekannt
                new() { MonsterId = "PKM-0066", MinLevel = 15, MaxLevel = 20, Chance = 10 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-NPC-1012", Name = "Pokémaniac", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-T-008", Name = "Hiker", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0023", Name = "Power Plant", Typ = "ort",
            Farbe = "blue", GridX = 15, GridY = 7,
            HatMonsterCenter = false, HatMarkt = false,
            WildMonster = new() {
                new() { MonsterId = "PKM-0081", MinLevel = 22, MaxLevel = 30, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0082", MinLevel = 22, MaxLevel = 30, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0145", MinLevel = 50, MaxLevel = 50, Chance = 1 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-LEG-001", Name = "Zapdos", Klasse = "Legendär",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Optional", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0145", Level = 50 } },
                },
            },
        },
        new() {
            Id = "KAN-0024", Name = "Lavender Town", Typ = "ort",
            Farbe = "purple", GridX = 14, GridY = 9,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0016",
            Süd = "KAL-0021",
            West = "KAL-0026",
            Verbindungen = new() { "KAL-0016", "KAL-0021", "KAL-0026" },
        },
        new() {
            Id = "KAN-0025", Name = "Pokémon Tower", Typ = "ort",
            Farbe = "green", GridX = 15, GridY = 9,
            HatMonsterCenter = false, HatMarkt = false,
            Trainer = new() {
                new() {
                    Id = "KAN-RIV-005", Name = "Blau", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0017", Level = 25 } },
                },
                new() {
                    Id = "KAN-GHOST-001", Name = "Marowak-Geist", Klasse = "Story",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "nicht fangbar", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0105", Level = 30 } },
                },
                new() {
                    Id = "KAN-NPC-1015", Name = "Channeler", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0026", Name = "Route 8", Typ = "ort",
            Farbe = "green", GridX = 12, GridY = 9,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "KAL-0014",
            Ost = "KAN-0024",
            Verbindungen = new() { "KAL-0014", "KAN-0024" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0052", MinLevel = 18, MaxLevel = 22, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0072", MinLevel = 25, MaxLevel = 30, Chance = 60 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 15, MaxLevel = 17, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0819", MinLevel = 30, MaxLevel = 34, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-NPC-1013", Name = "Gambler", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0027", Name = "Route 7", Typ = "ort",
            Farbe = "green", GridX = 10, GridY = 9,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "KAN-0028",
            Ost = "KAL-0016",
            Verbindungen = new() { "KAN-0028", "KAL-0016" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0052", MinLevel = 18, MaxLevel = 22, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0056", MinLevel = 18, MaxLevel = 22, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 22, MaxLevel = 26, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 13, MaxLevel = 15, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0819", MinLevel = 26, MaxLevel = 30, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAL-OPT-001", Name = "Brigitte", Klasse = "Pokémon Breeder",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Frühe Route", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0028", Name = "Celadon City", Typ = "ort",
            Farbe = "purple", GridX = 9, GridY = 9,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0033",
            West = "KAL-0016",
            Ost = "KAL-0014",
            Verbindungen = new() { "KAL-0033", "KAL-0016", "KAL-0014" },
            Trainer = new() {
                new() {
                    Id = "KAN-GYM-004", Name = "Erika", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-GYM-004-HGSS", Name = "Erika", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Erika", OrdenName = "Farborden",
                OrdenNr = 12, TypSpezialisierung = "Pflanze",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 70 } },
            },
        },
        new() {
            Id = "KAN-0029", Name = "Team Rocket Hideout", Typ = "ort",
            Farbe = "green", GridX = 9, GridY = 10,
            HatMonsterCenter = false, HatMarkt = false,
        },
        new() {
            Id = "KAN-0030", Name = "Route 16", Typ = "ort",
            Farbe = "green", GridX = 7, GridY = 9,
            HatMonsterCenter = false, HatMarkt = false,
            Ost = "KAN-0028",
            Verbindungen = new() { "KAN-0028" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0019", MinLevel = 20, MaxLevel = 25, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 32, MaxLevel = 36, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 28, MaxLevel = 32, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-T-016", Name = "Biker", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0031", Name = "Route 17", Typ = "ort",
            Farbe = "green", GridX = 7, GridY = 10,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAN-0033",
            Süd = "KAN-0037",
            Verbindungen = new() { "KAN-0033", "KAN-0037" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0019", MinLevel = 22, MaxLevel = 28, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0072", MinLevel = 35, MaxLevel = 40, Chance = 60 }, // Unbekannt
                new() { MonsterId = "PKM-0459", MinLevel = 30, MaxLevel = 35, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-T-017", Name = "Biker", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0032", Name = "Route 18", Typ = "ort",
            Farbe = "green", GridX = 7, GridY = 14,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0036",
            West = "KAN-0036",
            Verbindungen = new() { "KAL-0036", "KAN-0036" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0019", MinLevel = 22, MaxLevel = 28, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 35, MaxLevel = 40, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 30, MaxLevel = 35, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-T-018", Name = "Bird Keeper", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0033", Name = "Route 13", Typ = "ort",
            Farbe = "blue", GridX = 13, GridY = 13,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0029",
            Ost = "KAL-0026",
            Verbindungen = new() { "KAL-0029", "KAL-0026" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0016", MinLevel = 18, MaxLevel = 22, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0072", MinLevel = 30, MaxLevel = 35, Chance = 60 }, // Unbekannt
                new() { MonsterId = "PKM-0551", MinLevel = 25, MaxLevel = 30, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-NPC-1018", Name = "Bird Keeper", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-T-014", Name = "Bird Keeper", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0034", Name = "Route 14", Typ = "ort",
            Farbe = "green", GridX = 12, GridY = 13,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0031",
            Ost = "KAL-0028",
            Verbindungen = new() { "KAL-0031", "KAL-0028" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0016", MinLevel = 20, MaxLevel = 25, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0704", MinLevel = 25, MaxLevel = 15, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 30, MaxLevel = 35, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 25, MaxLevel = 30, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-NPC-1019", Name = "Bird Keeper", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-T-015", Name = "Biker", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0035", Name = "Route 15", Typ = "ort",
            Farbe = "green", GridX = 10, GridY = 14,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAN-0036",
            Ost = "KAL-0029",
            Verbindungen = new() { "KAN-0036", "KAL-0029" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0016", MinLevel = 22, MaxLevel = 28, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 30, MaxLevel = 35, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 28, MaxLevel = 32, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-NPC-1020", Name = "Jr. Trainer", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0036", Name = "Fuchsia City", Typ = "ort",
            Farbe = "purple", GridX = 9, GridY = 14,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0037",
            Süd = "KAL-0037",
            West = "KAL-0039",
            Ost = "KAL-0031",
            Verbindungen = new() { "KAL-0037", "KAL-0037", "KAL-0039", "KAL-0031" },
            Trainer = new() {
                new() {
                    Id = "KAN-GYM-005", Name = "Koga", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-GYM-005-HGSS", Name = "Koga", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Janina", OrdenName = "Seelenorden",
                OrdenNr = 13, TypSpezialisierung = "Gift",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 75 } },
            },
        },
        new() {
            Id = "KAN-0037", Name = "Safari Zone", Typ = "ort",
            Farbe = "blue", GridX = 9, GridY = 13,
            HatMonsterCenter = false, HatMarkt = false,
            WildMonster = new() {
                new() { MonsterId = "PKM-0111", MinLevel = 25, MaxLevel = 30, Chance = 15 }, // Unbekannt
                new() { MonsterId = "PKM-0113", MinLevel = 25, MaxLevel = 30, Chance = 4 }, // Unbekannt
                new() { MonsterId = "PKM-0115", MinLevel = 25, MaxLevel = 30, Chance = 4 }, // Unbekannt
                new() { MonsterId = "PKM-0147", MinLevel = 15, MaxLevel = 20, Chance = 1 }, // Unbekannt
                new() { MonsterId = "PKM-0115", MinLevel = 30, MaxLevel = 5, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0127", MinLevel = 30, MaxLevel = 5, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0246", MinLevel = 15, MaxLevel = 20, Chance = 5 }, // Unbekannt
                new() { MonsterId = "PKM-0231", MinLevel = 15, MaxLevel = 20, Chance = 10 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-NPC-1017", Name = "Keine Trainer", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0038", Name = "Route 19", Typ = "ort",
            Farbe = "blue", GridX = 9, GridY = 15,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "KAL-0041",
            Ost = "KAN-0036",
            Verbindungen = new() { "KAL-0041", "KAN-0036" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0072", MinLevel = 20, MaxLevel = 25, Chance = 60 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 2, MaxLevel = 4, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 32, MaxLevel = 36, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-T-019", Name = "Swimmer", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0039", Name = "Route 20", Typ = "ort",
            Farbe = "blue", GridX = 5, GridY = 16,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0039",
            Süd = "KAN-0041",
            Verbindungen = new() { "KAL-0039", "KAN-0041" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0072", MinLevel = 20, MaxLevel = 25, Chance = 60 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 32, MaxLevel = 36, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-T-020", Name = "Swimmer", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0040", Name = "Seafoam Islands", Typ = "ort",
            Farbe = "blue", GridX = 6, GridY = 15,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0041",
            Ost = "KAL-0041",
            Verbindungen = new() { "KAL-0041", "KAL-0041" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0144", MinLevel = 50, MaxLevel = 50, Chance = 1 }, // Unbekannt
                new() { MonsterId = "PKM-0086", MinLevel = 30, MaxLevel = 35, Chance = 20 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-LEG-002", Name = "Articuno", Klasse = "Legendär",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Optional", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0144", Level = 50 } },
                },
                new() {
                    Id = "KAN-NPC-1021", Name = "Stromer", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Pyro", OrdenName = "Vulkanorden",
                OrdenNr = 15, TypSpezialisierung = "Feuer",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 85 } },
            },
        },
        new() {
            Id = "KAN-0041", Name = "Cinnabar Island", Typ = "ort",
            Farbe = "purple", GridX = 4, GridY = 16,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0041",
            Ost = "KAL-0043",
            Verbindungen = new() { "KAL-0041", "KAL-0043" },
            Trainer = new() {
                new() {
                    Id = "KAN-GYM-007", Name = "Pyro/Blaine", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-GYM-007-HGSS", Name = "Blaine", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Pyro", OrdenName = "Vulkanorden",
                OrdenNr = 7, TypSpezialisierung = "Feuer",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 45 } },
            },
        },
        new() {
            Id = "KAN-0042", Name = "Pokémon Mansion", Typ = "ort",
            Farbe = "green", GridX = 3, GridY = 16,
            HatMonsterCenter = false, HatMarkt = false,
            WildMonster = new() {
                new() { MonsterId = "PKM-0109", MinLevel = 30, MaxLevel = 38, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0110", MinLevel = 30, MaxLevel = 38, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0126", MinLevel = 30, MaxLevel = 38, Chance = 10 }, // Unbekannt
                new() { MonsterId = "PKM-0125", MinLevel = 30, MaxLevel = 38, Chance = 10 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-NPC-1022", Name = "Scientist", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0043", Name = "Route 21", Typ = "ort",
            Farbe = "blue", GridX = 4, GridY = 13,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAN-0001",
            Süd = "KAN-0041",
            Verbindungen = new() { "KAN-0001", "KAN-0041" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0069", MinLevel = 25, MaxLevel = 30, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0072", MinLevel = 20, MaxLevel = 25, Chance = 60 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 35, MaxLevel = 40, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-T-021", Name = "Swimmer", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0044", Name = "Saffron City", Typ = "ort",
            Farbe = "purple", GridX = 11, GridY = 9,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0010",
            Süd = "KAL-0012",
            West = "KAL-0016",
            Ost = "KAL-0014",
            Verbindungen = new() { "KAL-0010", "KAL-0012", "KAL-0016", "KAL-0014" },
            Trainer = new() {
                new() {
                    Id = "KAN-GYM-006", Name = "Sabrina", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-GYM-006-HGSS", Name = "Sabrina", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Sabrina", OrdenName = "Sumpforden",
                OrdenNr = 14, TypSpezialisierung = "Psycho",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 80 } },
            },
        },
        new() {
            Id = "KAN-0045", Name = "Silph Co.", Typ = "ort",
            Farbe = "green", GridX = 11, GridY = 16,
            HatMonsterCenter = false, HatMarkt = false,
            Trainer = new() {
                new() {
                    Id = "KAN-RIV-006", Name = "Blau", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-BOSS-002", Name = "Giovanni", Klasse = "Boss",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-T-011", Name = "Giovanni", Klasse = "Boss",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0046", Name = "Fighting Dojo", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
        },
        new() {
            Id = "KAN-0047", Name = "Diglett's Cave", Typ = "ort",
            Farbe = "cave", GridX = 5, GridY = 8,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0023",
            Ost = "KAN-0018",
            Verbindungen = new() { "KAL-0023", "KAN-0018" },
        },
        new() {
            Id = "KAN-0048", Name = "Underground Path (R5-R6)", Typ = "ort",
            Farbe = "green", GridX = 12, GridY = 17,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAN-0014",
            Süd = "KAN-0015",
            Verbindungen = new() { "KAN-0014", "KAN-0015" },
        },
        new() {
            Id = "KAN-0049", Name = "Underground Path (R7-R8)", Typ = "ort",
            Farbe = "green", GridX = 13, GridY = 16,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAN-0027",
            Ost = "KAN-0026",
            Verbindungen = new() { "KAN-0027", "KAN-0026" },
        },
        new() {
            Id = "KAN-0050", Name = "Route 23", Typ = "ort",
            Farbe = "blue", GridX = 1, GridY = 9,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "KAN-0051",
            Ost = "KAN-0004",
            Verbindungen = new() { "KAN-0051", "KAN-0004" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0016", MinLevel = 30, MaxLevel = 35, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0056", MinLevel = 30, MaxLevel = 36, Chance = 20 }, // Unbekannt
            },
        },
        new() {
            Id = "KAN-0051", Name = "Victory Road", Typ = "ort",
            Farbe = "cave", GridX = 1, GridY = 8,
            HatMonsterCenter = false, HatMarkt = false,
            West = "JOH-0055",
            Ost = "KAN-0050",
            Verbindungen = new() { "JOH-0055", "KAN-0050" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0041", MinLevel = 35, MaxLevel = 40, Chance = 50 }, // Unbekannt
                new() { MonsterId = "PKM-0095", MinLevel = 35, MaxLevel = 40, Chance = 15 }, // Unbekannt
                new() { MonsterId = "PKM-0105", MinLevel = 35, MaxLevel = 40, Chance = 10 }, // Unbekannt
                new() { MonsterId = "PKM-0041", MinLevel = 40, MaxLevel = 46, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0041", MinLevel = 45, MaxLevel = 50, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0041", MinLevel = 40, MaxLevel = 46, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0074", MinLevel = 40, MaxLevel = 46, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 40, MaxLevel = 46, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-LEG-003", Name = "Moltres", Klasse = "Legendär",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Optional", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0146", Level = 50 } },
                },
                new() {
                    Id = "KAN-NPC-1023", Name = "Cooltrainer", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-T-022", Name = "Cooltrainer", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "JOH-T-025", Name = "Cooltrainer", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "HOE-T-014", Name = "Wally", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "HOE-T-015", Name = "Cooltrainer", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0052", Name = "Indigo Plateau", Typ = "ort",
            Farbe = "green", GridX = 1, GridY = 7,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "KAN-0051",
            Verbindungen = new() { "KAN-0051" },
            Trainer = new() {
                new() {
                    Id = "KAN-E4-001", Name = "Lorelei", Klasse = "Top4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-E4-002", Name = "Bruno", Klasse = "Top4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-E4-003", Name = "Agathe", Klasse = "Top4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-E4-004", Name = "Siegfried", Klasse = "Top4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-CHAMP-001", Name = "Blau", Klasse = "Champion",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0130", Level = 57 } },
                },
                new() {
                    Id = "JOH-K-020", Name = "Will", Klasse = "Top4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Psycho-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAN-0053", Name = "Cerulean Cave", Typ = "ort",
            Farbe = "cave", GridX = 10, GridY = 4,
            HatMonsterCenter = false, HatMarkt = false,
            WildMonster = new() {
                new() { MonsterId = "PKM-0063", MinLevel = 55, MaxLevel = 60, Chance = 10 }, // Unbekannt
                new() { MonsterId = "PKM-0144", MinLevel = 50, MaxLevel = 50, Chance = 1 }, // Unbekannt
                new() { MonsterId = "PKM-0150", MinLevel = 70, MaxLevel = 70, Chance = 1 }, // Unbekannt
                new() { MonsterId = "PKM-0041", MinLevel = 46, MaxLevel = 60, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-LEG-004", Name = "Mewtwo", Klasse = "Legendär",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Optional", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0150", Level = 70 } },
                },
            },
        },
        new() {
            Id = "KAN-0054", Name = "One Island (Eiland Eins)", Typ = "ort",
            Farbe = "purple", GridX = 4, GridY = 17,
            HatMonsterCenter = false, HatMarkt = false,
        },
        new() {
            Id = "KAN-0055", Name = "Two Island (Eiland Zwei)", Typ = "ort",
            Farbe = "purple", GridX = 4, GridY = 18,
            HatMonsterCenter = false, HatMarkt = false,
        },
        new() {
            Id = "KAN-0056", Name = "Three Island (Eiland Drei)", Typ = "ort",
            Farbe = "purple", GridX = 5, GridY = 18,
            HatMonsterCenter = false, HatMarkt = false,
        },
        new() {
            Id = "KAN-0057", Name = "Four Island (Eiland Vier)", Typ = "ort",
            Farbe = "purple", GridX = 6, GridY = 18,
            HatMonsterCenter = false, HatMarkt = false,
        },
        new() {
            Id = "KAN-0058", Name = "Five Island (Eiland Fünf)", Typ = "ort",
            Farbe = "purple", GridX = 7, GridY = 18,
            HatMonsterCenter = false, HatMarkt = false,
        },
        new() {
            Id = "KAN-0059", Name = "Six Island (Eiland Sechs)", Typ = "ort",
            Farbe = "purple", GridX = 8, GridY = 18,
            HatMonsterCenter = false, HatMarkt = false,
        },
        new() {
            Id = "KAN-0060", Name = "Seven Island (Eiland Sieben)", Typ = "ort",
            Farbe = "purple", GridX = 9, GridY = 18,
            HatMonsterCenter = false, HatMarkt = false,
        },
        // === Johto (60 Orte) ===
        new() {
            Id = "JOH-0001", Name = "New Bark Town", Typ = "ort",
            Farbe = "purple", GridX = 25, GridY = 10,
            HatMonsterCenter = false, HatMarkt = false,
            West = "JOH-0002",
            Verbindungen = new() { "JOH-0002" },
            Trainer = new() {
                new() {
                    Id = "JOH-K-001", Name = "Silver", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Erster Rivalen-Kampf", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0158", Level = 5 }, new() { MonsterId = "PKM-0152", Level = 5 }, new() { MonsterId = "PKM-0155", Level = 5 } },
                },
                new() {
                    Id = "JOH-RIV-001", Name = "Silber", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "JOH-0002", Name = "Route 29", Typ = "ort",
            Farbe = "green", GridX = 23, GridY = 10,
            HatMonsterCenter = false, HatMarkt = false,
            West = "JOH-0003",
            Ost = "JOH-0001",
            Verbindungen = new() { "JOH-0003", "JOH-0001" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0016", MinLevel = 2, MaxLevel = 5, Chance = 35 }, // Taubsi
                new() { MonsterId = "PKM-0161", MinLevel = 2, MaxLevel = 4, Chance = 40 }, // Wiesor
                new() { MonsterId = "PKM-0163", MinLevel = 2, MaxLevel = 4, Chance = 25 }, // Hoothoot
            },
            Trainer = new() {
                new() {
                    Id = "JOH-RIV-002", Name = "Silber", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "JOH-0003", Name = "Cherrygrove City", Typ = "ort",
            Farbe = "purple", GridX = 22, GridY = 10,
            HatMonsterCenter = true, HatMarkt = true,
            Nord = "JOH-0004",
            Ost = "JOH-0002",
            Verbindungen = new() { "JOH-0004", "JOH-0002" },
            Trainer = new() {
                new() {
                    Id = "JOH-K-002", Name = "Silver", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Silver stiehlt Starter", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0158", Level = 9 }, new() { MonsterId = "PKM-0152", Level = 9 }, new() { MonsterId = "PKM-0155", Level = 9 } },
                },
                new() {
                    Id = "JOH-RIV-003", Name = "Silber", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            MarktAngebot = new() {
                new() { Id = "item-001", Name = "Pokéball", Preis = 200, Emoji = "🛍️", Kategorie = "Markt" },
                new() { Id = "item-002", Name = "Trank", Preis = 300, Emoji = "🛍️", Kategorie = "Markt" },
                new() { Id = "item-003", Name = "Gegengift", Preis = 100, Emoji = "🛍️", Kategorie = "Markt" },
            },
        },
        new() {
            Id = "JOH-0004", Name = "Route 30", Typ = "ort",
            Farbe = "green", GridX = 22, GridY = 9,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "JOH-0005",
            Süd = "JOH-0003",
            Verbindungen = new() { "JOH-0005", "JOH-0003" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0010", MinLevel = 3, MaxLevel = 5, Chance = 30 }, // Raupy
                new() { MonsterId = "PKM-0016", MinLevel = 3, MaxLevel = 6, Chance = 35 }, // Taubsi
                new() { MonsterId = "PKM-0187", MinLevel = 4, MaxLevel = 6, Chance = 20 }, // Hoppspross
                new() { MonsterId = "PKM-0194", MinLevel = 4, MaxLevel = 6, Chance = 15 }, // Felino
            },
            Trainer = new() {
                new() {
                    Id = "t-r30-01", Name = "Youngster Joey", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Ich fordere dich heraus!", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "JOH-0005", Name = "Route 31", Typ = "ort",
            Farbe = "green", GridX = 22, GridY = 8,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "JOH-0004",
            West = "JOH-0006",
            Verbindungen = new() { "JOH-0004", "JOH-0006" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0069", MinLevel = 4, MaxLevel = 6, Chance = 30 }, // Knofensa
                new() { MonsterId = "PKM-0016", MinLevel = 4, MaxLevel = 6, Chance = 35 }, // Taubsi
                new() { MonsterId = "PKM-0187", MinLevel = 5, MaxLevel = 7, Chance = 20 }, // Hoppspross
            },
            Trainer = new() {
                new() {
                    Id = "JOH-T-003", Name = "Bug Catcher", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "JOH-0006", Name = "Violet City", Typ = "ort",
            Farbe = "purple", GridX = 21, GridY = 8,
            HatMonsterCenter = true, HatMarkt = true,
            Süd = "JOH-0007",
            Ost = "JOH-0005",
            Verbindungen = new() { "JOH-0007", "JOH-0005" },
            MarktAngebot = new() {
                new() { Id = "item-001", Name = "Pokéball", Preis = 200, Emoji = "🛍️", Kategorie = "Markt" },
                new() { Id = "item-002", Name = "Trank", Preis = 300, Emoji = "🛍️", Kategorie = "Markt" },
                new() { Id = "item-004", Name = "Fluchtseil", Preis = 550, Emoji = "🛍️", Kategorie = "Markt" },
            },
            Arena = new() {
                Leiter = "Falk", OrdenName = "Federorden",
                OrdenNr = 1, TypSpezialisierung = "Normal",
                Team = new() {
                    new() { MonsterId = "PKM-0016", Level = 7 }, // Taubsi
                    new() { MonsterId = "PKM-0017", Level = 9 }, // Tauboga
                },
            },
        },
        new() {
            Id = "JOH-0007", Name = "Violet City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "JOH-0005",
            West = "JOH-0022",
            Ost = "JOH-0009",
            Verbindungen = new() { "JOH-0005", "JOH-0022", "JOH-0009" },
            Trainer = new() {
                new() {
                    Id = "JOH-K-010", Name = "Falkner", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Flug-Typ; Sturzflug-Orden", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "JOH-GYM-001", Name = "Falkner", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "JOH-RIV-004", Name = "Silber", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Falkner", OrdenName = "Sturmorden",
                OrdenNr = 1, TypSpezialisierung = "Flug",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 15 } },
            },
        },
        new() {
            Id = "JOH-0008", Name = "Sprout Tower", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "JOH-0007",
            Verbindungen = new() { "JOH-0007" },
            Trainer = new() {
                new() {
                    Id = "JOH-T-001", Name = "Sage", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "JOH-0009", Name = "Route 32", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "JOH-0007",
            Süd = "JOH-0010",
            Verbindungen = new() { "JOH-0007", "JOH-0010" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0060", MinLevel = 12, MaxLevel = 25, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0187", MinLevel = 12, MaxLevel = 20, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "JOH-T-004", Name = "Fisher", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "JOH-0010", Name = "Ruins of Alph", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "JOH-0009",
            Ost = "JOH-0022",
            Verbindungen = new() { "JOH-0009", "JOH-0022" },
        },
        new() {
            Id = "JOH-0011", Name = "Union Cave", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "JOH-0009",
            Ost = "JOH-0012",
            Verbindungen = new() { "JOH-0009", "JOH-0012" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0074", MinLevel = 12, MaxLevel = 20, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0041", MinLevel = 12, MaxLevel = 30, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0054", MinLevel = 20, MaxLevel = 90, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0131", MinLevel = 30, MaxLevel = 10, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "JOH-T-005", Name = "Hiker", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "JOH-0012", Name = "Route 33", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "JOH-0011",
            Ost = "JOH-0013",
            Verbindungen = new() { "JOH-0011", "JOH-0013" },
        },
        new() {
            Id = "JOH-0013", Name = "Azalea Town", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "JOH-0015",
            Ost = "JOH-0012",
            Verbindungen = new() { "JOH-0015", "JOH-0012" },
            Trainer = new() {
                new() {
                    Id = "JOH-K-011", Name = "Bugsy", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Käfer-Typ; Scyther mit Schnellen Angriff", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "JOH-GYM-002", Name = "Bugsy", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "JOH-RIV-005", Name = "Silber", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Bugsy", OrdenName = "Bienenorden",
                OrdenNr = 2, TypSpezialisierung = "Käfer",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 20 } },
            },
        },
        new() {
            Id = "JOH-0014", Name = "Slowpoke Well", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "JOH-0013",
            Verbindungen = new() { "JOH-0013" },
            Trainer = new() {
                new() {
                    Id = "JOH-K-030", Name = "Rocket-Rüpel", Klasse = "Team Rocket",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Team Rocket Einführung", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "JOH-T-006", Name = "Team Rocket Proton", Klasse = "Boss",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "JOH-0015", Name = "Ilex Forest", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "JOH-0016",
            Ost = "JOH-0013",
            Verbindungen = new() { "JOH-0016", "JOH-0013" },
        },
        new() {
            Id = "JOH-0016", Name = "Route 34", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "JOH-0017",
            Süd = "JOH-0015",
            Verbindungen = new() { "JOH-0017", "JOH-0015" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0183", MinLevel = 15, MaxLevel = 25, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "JOH-T-008", Name = "Policeman", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "JOH-0017", Name = "Goldenrod City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "JOH-0016",
            West = "JOH-0020",
            Verbindungen = new() { "JOH-0016", "JOH-0020" },
            Trainer = new() {
                new() {
                    Id = "JOH-K-004", Name = "Silver", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Rivale in Goldenrod", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0169", Level = 20 }, new() { MonsterId = "PKM-0159", Level = 20 }, new() { MonsterId = "PKM-0153", Level = 20 } },
                },
                new() {
                    Id = "JOH-K-012", Name = "Whitney", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Normal-Typ; Miltank mit Walzer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "JOH-GYM-003", Name = "Whitney", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "JOH-RIV-006", Name = "Silber", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Whitney", OrdenName = "Einfachorden",
                OrdenNr = 3, TypSpezialisierung = "Normal",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 25 } },
            },
        },
        new() {
            Id = "JOH-0018", Name = "Goldenrod Radio Tower", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "JOH-0017",
            Verbindungen = new() { "JOH-0017" },
            Trainer = new() {
                new() {
                    Id = "JOH-K-032", Name = "Archer (2)", Klasse = "Team Rocket",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Zweiter Archer-Kampf", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "JOH-T-024", Name = "Team Rocket Archer", Klasse = "Boss",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "JOH-0019", Name = "Goldenrod Underground", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "JOH-0017",
            Verbindungen = new() { "JOH-0017" },
        },
        new() {
            Id = "JOH-0020", Name = "Route 35", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "JOH-0021",
            West = "JOH-0017",
            Verbindungen = new() { "JOH-0021", "JOH-0017" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0029", MinLevel = 15, MaxLevel = 20, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0032", MinLevel = 15, MaxLevel = 20, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "JOH-T-009", Name = "Youngster", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "JOH-0021", Name = "National Park", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "JOH-0020",
            Ost = "JOH-0022",
            Verbindungen = new() { "JOH-0020", "JOH-0022" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0123", MinLevel = 15, MaxLevel = 5, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0127", MinLevel = 15, MaxLevel = 5, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "JOH-T-010", Name = "Picnicker", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "JOH-0022", Name = "Route 36", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "JOH-0010",
            West = "JOH-0007",
            Ost = "JOH-0023",
            Verbindungen = new() { "JOH-0010", "JOH-0007", "JOH-0023" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0185", MinLevel = 17, MaxLevel = 10, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "JOH-T-011", Name = "Schoolboy", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "JOH-0023", Name = "Route 37", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "JOH-0024",
            Süd = "JOH-0022",
            Verbindungen = new() { "JOH-0024", "JOH-0022" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0133", MinLevel = 17, MaxLevel = 15, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "JOH-0024", Name = "Ecruteak City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "JOH-0023",
            West = "JOH-0027",
            Ost = "JOH-0039",
            Verbindungen = new() { "JOH-0023", "JOH-0027", "JOH-0039" },
            Trainer = new() {
                new() {
                    Id = "JOH-K-005", Name = "Silver", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Rivale im Burned Tower", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0160", Level = 25 }, new() { MonsterId = "PKM-0154", Level = 25 }, new() { MonsterId = "PKM-0157", Level = 25 } },
                },
                new() {
                    Id = "JOH-K-013", Name = "Morty", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Geist-Typ; Gengar mit Hypnose+Traumfresser", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "JOH-GYM-004", Name = "Morty", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "JOH-RIV-007", Name = "Silber", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Morty", OrdenName = "Nebelorden",
                OrdenNr = 4, TypSpezialisierung = "Geist",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 30 } },
            },
        },
        new() {
            Id = "JOH-0025", Name = "Burned Tower", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "JOH-0024",
            Verbindungen = new() { "JOH-0024" },
            Trainer = new() {
                new() {
                    Id = "JOH-T-012", Name = "Team Rocket Archer", Klasse = "Boss",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "JOH-0026", Name = "Bell Tower", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "JOH-0024",
            Verbindungen = new() { "JOH-0024" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0250", MinLevel = 45, MaxLevel = 1, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "JOH-0027", Name = "Route 38", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "JOH-0029",
            Ost = "JOH-0024",
            Verbindungen = new() { "JOH-0029", "JOH-0024" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0241", MinLevel = 22, MaxLevel = 20, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0128", MinLevel = 22, MaxLevel = 20, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "JOH-T-013", Name = "Lass", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "OPT-JOH-001", Name = "Valerie", Klasse = "Schönheit",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Optional", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "JOH-0028", Name = "Route 39", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "JOH-0029",
            Ost = "JOH-0027",
            Verbindungen = new() { "JOH-0029", "JOH-0027" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0241", MinLevel = 22, MaxLevel = 30, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "JOH-T-014", Name = "Farmer", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "JOH-0029", Name = "Olivine City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "JOH-0031",
            Ost = "JOH-0028",
            Verbindungen = new() { "JOH-0031", "JOH-0028" },
            Trainer = new() {
                new() {
                    Id = "JOH-K-015", Name = "Jasmine", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Stahl-Typ; Steelix mit Eisenschwanz", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "JOH-GYM-006", Name = "Jasmine", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Jasmine", OrdenName = "Mineralorden",
                OrdenNr = 6, TypSpezialisierung = "Stahl",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 40 } },
            },
        },
        new() {
            Id = "JOH-0030", Name = "Olivine Lighthouse", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "JOH-0029",
            Verbindungen = new() { "JOH-0029" },
            Trainer = new() {
                new() {
                    Id = "JOH-T-015", Name = "Sailor", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "JOH-0031", Name = "Route 40", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "JOH-0034",
            Ost = "JOH-0029",
            Verbindungen = new() { "JOH-0034", "JOH-0029" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0072", MinLevel = 25, MaxLevel = 90, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "JOH-T-016", Name = "Swimmer", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "JOH-0032", Name = "Route 41", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "JOH-0033",
            West = "JOH-0034",
            Ost = "JOH-0031",
            Verbindungen = new() { "JOH-0033", "JOH-0034", "JOH-0031" },
        },
        new() {
            Id = "JOH-0033", Name = "Whirl Islands", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "JOH-0032",
            Ost = "JOH-0032",
            Verbindungen = new() { "JOH-0032", "JOH-0032" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0041", MinLevel = 25, MaxLevel = 30, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0249", MinLevel = 45, MaxLevel = 1, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "JOH-T-017", Name = "Sailor", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "JOH-0034", Name = "Cianwood City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Ost = "JOH-0032",
            Verbindungen = new() { "JOH-0032" },
            Trainer = new() {
                new() {
                    Id = "JOH-K-014", Name = "Chuck", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Kampf-Typ; Poliwrath mit Wasserpuls", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "JOH-GYM-005", Name = "Chuck", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Chuck", OrdenName = "Sturmorden",
                OrdenNr = 5, TypSpezialisierung = "Kampf",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 35 } },
            },
        },
        new() {
            Id = "JOH-0035", Name = "Route 47", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "JOH-0036",
            Ost = "JOH-0034",
            Verbindungen = new() { "JOH-0036", "JOH-0034" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0231", MinLevel = 25, MaxLevel = 30, Chance = 20 }, // Unbekannt
            },
        },
        new() {
            Id = "JOH-0036", Name = "Route 48", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Ost = "JOH-0035",
            Verbindungen = new() { "JOH-0035" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0234", MinLevel = 25, MaxLevel = 30, Chance = 20 }, // Unbekannt
            },
        },
        new() {
            Id = "JOH-0037", Name = "Safari Zone", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            WildMonster = new() {
                new() { MonsterId = "PKM-0111", MinLevel = 25, MaxLevel = 30, Chance = 15 }, // Unbekannt
                new() { MonsterId = "PKM-0113", MinLevel = 25, MaxLevel = 30, Chance = 4 }, // Unbekannt
                new() { MonsterId = "PKM-0115", MinLevel = 25, MaxLevel = 30, Chance = 4 }, // Unbekannt
                new() { MonsterId = "PKM-0147", MinLevel = 15, MaxLevel = 20, Chance = 1 }, // Unbekannt
                new() { MonsterId = "PKM-0115", MinLevel = 30, MaxLevel = 5, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0127", MinLevel = 30, MaxLevel = 5, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0246", MinLevel = 15, MaxLevel = 20, Chance = 5 }, // Unbekannt
                new() { MonsterId = "PKM-0231", MinLevel = 15, MaxLevel = 20, Chance = 10 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-NPC-1017", Name = "Keine Trainer", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "JOH-0038", Name = "Cliff Cave", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "JOH-0035",
            Verbindungen = new() { "JOH-0035" },
        },
        new() {
            Id = "JOH-0039", Name = "Route 42", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "JOH-0040",
            West = "JOH-0024",
            Ost = "JOH-0041",
            Verbindungen = new() { "JOH-0040", "JOH-0024", "JOH-0041" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0234", MinLevel = 22, MaxLevel = 15, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "JOH-T-018", Name = "Hiker", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "JOH-0040", Name = "Mt. Mortar", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "JOH-0039",
            Ost = "JOH-0039",
            Verbindungen = new() { "JOH-0039", "JOH-0039" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0236", MinLevel = 20, MaxLevel = 20, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "JOH-0041", Name = "Mahogany Town", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "JOH-0043",
            West = "JOH-0039",
            Ost = "JOH-0045",
            Verbindungen = new() { "JOH-0043", "JOH-0039", "JOH-0045" },
            Trainer = new() {
                new() {
                    Id = "JOH-K-006", Name = "Silver", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Rivale vor Mahogany", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0160", Level = 30 }, new() { MonsterId = "PKM-0154", Level = 30 }, new() { MonsterId = "PKM-0157", Level = 30 } },
                },
                new() {
                    Id = "JOH-K-016", Name = "Pryce", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Eis-Typ; Piloswine mit Blizzard", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "JOH-GYM-007", Name = "Pryce", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "JOH-RIV-008", Name = "Silber", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Pryce", OrdenName = "Gletscherorden",
                OrdenNr = 7, TypSpezialisierung = "Eis",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 45 } },
            },
        },
        new() {
            Id = "JOH-0042", Name = "Team Rocket HQ", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "JOH-0041",
            Verbindungen = new() { "JOH-0041" },
            Trainer = new() {
                new() {
                    Id = "JOH-K-031", Name = "Archer", Klasse = "Team Rocket",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Team Rocket Anführer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "JOH-T-019", Name = "Team Rocket Petrel", Klasse = "Boss",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "JOH-T-020", Name = "Team Rocket Ariana", Klasse = "Boss",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "JOH-0043", Name = "Route 43", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "JOH-0044",
            Süd = "JOH-0041",
            Verbindungen = new() { "JOH-0044", "JOH-0041" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0193", MinLevel = 20, MaxLevel = 24, Chance = 5 }, // Unbekannt
            },
        },
        new() {
            Id = "JOH-0044", Name = "Lake of Rage", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "JOH-0043",
            Verbindungen = new() { "JOH-0043" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0130", MinLevel = 20, MaxLevel = 1, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "JOH-K-033", Name = "Lance (NPC)", Klasse = "Lance",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Lance als Verbündeter", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "JOH-0045", Name = "Route 44", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "JOH-0041",
            Ost = "JOH-0046",
            Verbindungen = new() { "JOH-0041", "JOH-0046" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0062", MinLevel = 28, MaxLevel = 20, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0199", MinLevel = 22, MaxLevel = 26, Chance = 20 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "JOH-T-022", Name = "Fisher", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "JOH-0046", Name = "Ice Path", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "JOH-0045",
            Ost = "JOH-0047",
            Verbindungen = new() { "JOH-0045", "JOH-0047" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0220", MinLevel = 25, MaxLevel = 30, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0238", MinLevel = 25, MaxLevel = 20, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "JOH-T-021", Name = "Boarder", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "JOH-0047", Name = "Blackthorn City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "JOH-0048",
            West = "JOH-0046",
            Ost = "JOH-0049",
            Verbindungen = new() { "JOH-0048", "JOH-0046", "JOH-0049" },
            Trainer = new() {
                new() {
                    Id = "JOH-K-017", Name = "Clair", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Drachen-Typ; Kingdra schwer zu kontern", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "JOH-GYM-008", Name = "Clair", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "JOH-RIV-009", Name = "Silber", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Clair", OrdenName = "Aufstiegsorden",
                OrdenNr = 8, TypSpezialisierung = "Drache",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 50 } },
            },
        },
        new() {
            Id = "JOH-0048", Name = "Dragon's Den", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "JOH-0047",
            Verbindungen = new() { "JOH-0047" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0147", MinLevel = 20, MaxLevel = 30, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "JOH-K-034", Name = "Clair (Prüfung)", Klasse = "NPC",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Prüfung im Drachenschrein", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "JOH-0049", Name = "Route 45", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "JOH-0047",
            West = "JOH-0006",
            Ost = "JOH-0050",
            Verbindungen = new() { "JOH-0047", "JOH-0006", "JOH-0050" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0207", MinLevel = 20, MaxLevel = 25, Chance = 20 }, // Unbekannt
            },
        },
        new() {
            Id = "JOH-0050", Name = "Route 46", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "JOH-0049",
            Süd = "JOH-0002",
            Verbindungen = new() { "JOH-0049", "JOH-0002" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0161", MinLevel = 2, MaxLevel = 5, Chance = 30 }, // Unbekannt
            },
        },
        new() {
            Id = "JOH-0051", Name = "Route 27", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "JOH-0052",
            West = "JOH-0001",
            Ost = "JOH-0053",
            Verbindungen = new() { "JOH-0052", "JOH-0001", "JOH-0053" },
        },
        new() {
            Id = "JOH-0052", Name = "Tohjo Falls", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "JOH-0051",
            Ost = "JOH-0051",
            Verbindungen = new() { "JOH-0051", "JOH-0051" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0129", MinLevel = 20, MaxLevel = 25, Chance = 60 }, // Unbekannt
            },
        },
        new() {
            Id = "JOH-0053", Name = "Route 26", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "JOH-0051",
            Verbindungen = new() { "JOH-0051" },
        },
        new() {
            Id = "JOH-0054", Name = "Victory Road (Johto)", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "JOH-0053",
            Ost = "JOH-0055",
            Verbindungen = new() { "JOH-0053", "JOH-0055" },
        },
        new() {
            Id = "JOH-0055", Name = "Indigo Plateau", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "JOH-0054",
            Verbindungen = new() { "JOH-0054" },
            Trainer = new() {
                new() {
                    Id = "KAN-E4-001", Name = "Lorelei", Klasse = "Top4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-E4-002", Name = "Bruno", Klasse = "Top4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-E4-003", Name = "Agathe", Klasse = "Top4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-E4-004", Name = "Siegfried", Klasse = "Top4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-CHAMP-001", Name = "Blau", Klasse = "Champion",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0130", Level = 57 } },
                },
                new() {
                    Id = "JOH-K-020", Name = "Will", Klasse = "Top4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Psycho-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "JOH-0056", Name = "Mt. Silver", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "JOH-0057",
            Verbindungen = new() { "JOH-0057" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0246", MinLevel = 45, MaxLevel = 15, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0058", MinLevel = 40, MaxLevel = 20, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0059", MinLevel = 36, MaxLevel = 48, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0025", MinLevel = 40, MaxLevel = 50, Chance = 5 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "JOH-K-025", Name = "Red", Klasse = "Postgame",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Stärkster Trainer im Spiel; Pikachu Lv88", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "JOH-RIV-010", Name = "Silber", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "JOH-0057", Name = "Route 28", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "JOH-0056",
            Verbindungen = new() { "JOH-0056" },
        },
        new() {
            Id = "JOH-0058", Name = "Battle Frontier (Johto)", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "JOH-0059",
            Verbindungen = new() { "JOH-0059" },
        },
        new() {
            Id = "JOH-0059", Name = "Frontier Access", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "JOH-0031",
            Ost = "JOH-0058",
            Verbindungen = new() { "JOH-0031", "JOH-0058" },
        },
        new() {
            Id = "JOH-0060", Name = "Sinjoh Ruins", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
        },
        // === Hoenn (74 Orte) ===
        new() {
            Id = "HOE-0001", Name = "Littleroot Town", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "HOE-0004",
            Ost = "HOE-0002",
            Verbindungen = new() { "HOE-0004", "HOE-0002" },
        },
        new() {
            Id = "HOE-0002", Name = "Route 101", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "HOE-0003",
            Süd = "HOE-0001",
            Verbindungen = new() { "HOE-0003", "HOE-0001" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0263", MinLevel = 4, MaxLevel = 55, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0261", MinLevel = 4, MaxLevel = 45, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0003", Name = "Oldale Town", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "HOE-0004",
            Süd = "HOE-0002",
            West = "HOE-0005",
            Verbindungen = new() { "HOE-0004", "HOE-0002", "HOE-0005" },
        },
        new() {
            Id = "HOE-0004", Name = "Route 103", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "HOE-0003",
            West = "HOE-0007",
            Verbindungen = new() { "HOE-0003", "HOE-0007" },
            Trainer = new() {
                new() {
                    Id = "HOE-K-001", Name = "Brendan/May", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Rivale auf Route 103", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0252", Level = 5 }, new() { MonsterId = "PKM-0255", Level = 5 }, new() { MonsterId = "PKM-0258", Level = 5 } },
                },
                new() {
                    Id = "HOE-T-001", Name = "Youngster", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "HOE-0005", Name = "Route 102", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0006",
            Ost = "HOE-0003",
            Verbindungen = new() { "HOE-0006", "HOE-0003" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0270", MinLevel = 5, MaxLevel = 30, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0273", MinLevel = 5, MaxLevel = 30, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0006", Name = "Petalburg City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0007",
            Ost = "HOE-0005",
            Verbindungen = new() { "HOE-0007", "HOE-0005" },
            Trainer = new() {
                new() {
                    Id = "HOE-K-014", Name = "Norman", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Normal-Typ; Slaking mit Truant", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "HOE-GYM-005", Name = "Norman", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Norman", OrdenName = "Gleichgewichtsorden",
                OrdenNr = 5, TypSpezialisierung = "Normal",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 35 } },
            },
        },
        new() {
            Id = "HOE-0007", Name = "Route 104", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "HOE-0008",
            West = "HOE-0009",
            Ost = "HOE-0006",
            Verbindungen = new() { "HOE-0008", "HOE-0009", "HOE-0006" },
            Trainer = new() {
                new() {
                    Id = "HOE-T-002", Name = "Youngster", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "HOE-OPT-004", Name = "Gina & Mia", Klasse = "Zwillinge",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Doppelkampf", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "HOE-0008", Name = "Petalburg Woods", Typ = "ort",
            Farbe = "forest", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            WildMonster = new() {
                new() { MonsterId = "PKM-0285", MinLevel = 8, MaxLevel = 20, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0287", MinLevel = 8, MaxLevel = 20, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "HOE-K-030", Name = "Grunt", Klasse = "Team Magma/Aqua",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Erster Bösewicht-Kampf", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "HOE-T-003", Name = "Bug Catcher", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "HOE-0009", Name = "Rustboro City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "HOE-0012",
            Süd = "HOE-0007",
            Ost = "HOE-0010",
            Verbindungen = new() { "HOE-0012", "HOE-0007", "HOE-0010" },
            Trainer = new() {
                new() {
                    Id = "HOE-K-010", Name = "Roxanne", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Gestein-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "HOE-GYM-001", Name = "Roxanne", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Roxanne", OrdenName = "Steinorden",
                OrdenNr = 1, TypSpezialisierung = "Gestein",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 15 } },
            },
        },
        new() {
            Id = "HOE-0010", Name = "Route 116", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0011",
            Ost = "HOE-0009",
            Verbindungen = new() { "HOE-0011", "HOE-0009" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0263", MinLevel = 8, MaxLevel = 12, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0300", MinLevel = 8, MaxLevel = 12, Chance = 5 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0011", Name = "Rusturf Tunnel", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0010",
            Ost = "HOE-0013",
            Verbindungen = new() { "HOE-0010", "HOE-0013" },
        },
        new() {
            Id = "HOE-0012", Name = "Route 115", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "HOE-0030",
            Ost = "HOE-0009",
            Verbindungen = new() { "HOE-0030", "HOE-0009" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0333", MinLevel = 20, MaxLevel = 24, Chance = 30 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0013", Name = "Route 117", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0014",
            Ost = "HOE-0015",
            Verbindungen = new() { "HOE-0014", "HOE-0015" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0263", MinLevel = 13, MaxLevel = 16, Chance = 30 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0014", Name = "Verdanturf Town", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0013",
            Ost = "HOE-0011",
            Verbindungen = new() { "HOE-0013", "HOE-0011" },
        },
        new() {
            Id = "HOE-0015", Name = "Mauville City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "HOE-0027",
            Süd = "HOE-0016",
            West = "HOE-0013",
            Ost = "HOE-0037",
            Verbindungen = new() { "HOE-0027", "HOE-0016", "HOE-0013", "HOE-0037" },
            Trainer = new() {
                new() {
                    Id = "HOE-K-003", Name = "Wally", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Wally-Kampf", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0281", Level = 16 } },
                },
                new() {
                    Id = "HOE-K-012", Name = "Wattson", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Elektro-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "HOE-GYM-003", Name = "Wattson", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Wattson", OrdenName = "Dynamoorden",
                OrdenNr = 3, TypSpezialisierung = "Elektro",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 25 } },
            },
        },
        new() {
            Id = "HOE-0016", Name = "Route 110", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "HOE-0015",
            Süd = "HOE-0017",
            Verbindungen = new() { "HOE-0015", "HOE-0017" },
            Trainer = new() {
                new() {
                    Id = "HOE-K-002", Name = "Brendan/May", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Rivale auf Cycling Road", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0253", Level = 18 }, new() { MonsterId = "PKM-0256", Level = 18 } },
                },
                new() {
                    Id = "HOE-T-004", Name = "Wally", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0278", Level = 16 } },
                },
                new() {
                    Id = "HOE-T-005", Name = "Cooltrainer", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "OPT-HOE-001", Name = "Alex", Klasse = "Pokéfan",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Cycling Road", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "HOE-OPT-001", Name = "Isabel", Klasse = "Pokéfan",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Cycling Road", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "HOE-0017", Name = "Slateport City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "HOE-0016",
            Ost = "HOE-0018",
            Verbindungen = new() { "HOE-0016", "HOE-0018" },
        },
        new() {
            Id = "HOE-0018", Name = "Route 109", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "HOE-0017",
            Süd = "HOE-0019",
            Verbindungen = new() { "HOE-0017", "HOE-0019" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0278", MinLevel = 10, MaxLevel = 15, Chance = 30 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0019", Name = "Route 108", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "HOE-0018",
            Süd = "HOE-0021",
            Verbindungen = new() { "HOE-0018", "HOE-0021" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0072", MinLevel = 15, MaxLevel = 20, Chance = 60 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0020", Name = "Abandoned Ship", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0019",
            Verbindungen = new() { "HOE-0019" },
        },
        new() {
            Id = "HOE-0021", Name = "Route 107", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "HOE-0019",
            Süd = "HOE-0022",
            Verbindungen = new() { "HOE-0019", "HOE-0022" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0072", MinLevel = 15, MaxLevel = 20, Chance = 60 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0022", Name = "Dewford Town", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "HOE-0023",
            Süd = "HOE-0021",
            Verbindungen = new() { "HOE-0023", "HOE-0021" },
            Trainer = new() {
                new() {
                    Id = "HOE-K-011", Name = "Brawly", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Kampf-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "HOE-GYM-002", Name = "Brawly", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Brawly", OrdenName = "Knöchelorden",
                OrdenNr = 2, TypSpezialisierung = "Kampf",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 20 } },
            },
        },
        new() {
            Id = "HOE-0023", Name = "Route 106", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "HOE-0024",
            Süd = "HOE-0022",
            Verbindungen = new() { "HOE-0024", "HOE-0022" },
        },
        new() {
            Id = "HOE-0024", Name = "Granite Cave", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0023",
            Verbindungen = new() { "HOE-0023" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0304", MinLevel = 10, MaxLevel = 20, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0302", MinLevel = 10, MaxLevel = 15, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0025", Name = "Route 105", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "HOE-0023",
            Süd = "HOE-0007",
            Verbindungen = new() { "HOE-0023", "HOE-0007" },
        },
        new() {
            Id = "HOE-0026", Name = "Island Cave", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0025",
            Verbindungen = new() { "HOE-0025" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0378", MinLevel = 40, MaxLevel = 1, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0378", MinLevel = 40, MaxLevel = 40, Chance = 1 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0027", Name = "Route 111", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "HOE-0028",
            Süd = "HOE-0015",
            Ost = "HOE-0033",
            Verbindungen = new() { "HOE-0028", "HOE-0015", "HOE-0033" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0328", MinLevel = 20, MaxLevel = 20, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0331", MinLevel = 20, MaxLevel = 15, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "HOE-T-006", Name = "Hiker", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "HOE-OPT-002", Name = "Clark", Klasse = "Wanderer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Wüstenrand", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "HOE-0028", Name = "Route 112", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "HOE-0030",
            West = "HOE-0029",
            Ost = "HOE-0027",
            Verbindungen = new() { "HOE-0030", "HOE-0029", "HOE-0027" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0322", MinLevel = 18, MaxLevel = 22, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "HOE-T-007", Name = "Hiker", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "HOE-0029", Name = "Fiery Path", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0028",
            Ost = "HOE-0028",
            Verbindungen = new() { "HOE-0028", "HOE-0028" },
        },
        new() {
            Id = "HOE-0030", Name = "Mt. Chimney", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0031",
            Verbindungen = new() { "HOE-0031" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0322", MinLevel = 18, MaxLevel = 30, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "HOE-K-031", Name = "Maxie", Klasse = "Team Magma",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Maxie am Meteorit", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "HOE-K-032", Name = "Archie", Klasse = "Team Aqua",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Archie am Meteorit", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "HOE-T-008", Name = "Team Magma Maxie", Klasse = "Boss",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "HOE-T-009", Name = "Team Aqua Archie", Klasse = "Boss",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "HOE-0031", Name = "Jagged Pass", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "HOE-0032",
            West = "HOE-0030",
            Verbindungen = new() { "HOE-0032", "HOE-0030" },
        },
        new() {
            Id = "HOE-0032", Name = "Lavaridge Town", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "HOE-0028",
            Süd = "HOE-0031",
            Verbindungen = new() { "HOE-0028", "HOE-0031" },
            Trainer = new() {
                new() {
                    Id = "HOE-K-013", Name = "Flannery", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Feuer-Typ; Smaragd: Torkoal", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "HOE-GYM-004", Name = "Flannery", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Flannery", OrdenName = "Wärmeorden",
                OrdenNr = 4, TypSpezialisierung = "Feuer",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 30 } },
            },
        },
        new() {
            Id = "HOE-0033", Name = "Route 113", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0034",
            Ost = "HOE-0027",
            Verbindungen = new() { "HOE-0034", "HOE-0027" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0327", MinLevel = 22, MaxLevel = 20, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0227", MinLevel = 22, MaxLevel = 15, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0034", Name = "Fallarbor Town", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0035",
            Ost = "HOE-0033",
            Verbindungen = new() { "HOE-0035", "HOE-0033" },
        },
        new() {
            Id = "HOE-0035", Name = "Route 114", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0036",
            Ost = "HOE-0034",
            Verbindungen = new() { "HOE-0036", "HOE-0034" },
        },
        new() {
            Id = "HOE-0036", Name = "Meteor Falls", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0035",
            Ost = "HOE-0012",
            Verbindungen = new() { "HOE-0035", "HOE-0012" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0371", MinLevel = 30, MaxLevel = 5, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0337", MinLevel = 30, MaxLevel = 20, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0338", MinLevel = 30, MaxLevel = 20, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0037", Name = "Route 118", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0038",
            Ost = "HOE-0015",
            Verbindungen = new() { "HOE-0038", "HOE-0015" },
        },
        new() {
            Id = "HOE-0038", Name = "Route 119", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0039",
            Ost = "HOE-0037",
            Verbindungen = new() { "HOE-0039", "HOE-0037" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0357", MinLevel = 30, MaxLevel = 15, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0352", MinLevel = 30, MaxLevel = 5, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "HOE-K-004", Name = "Brendan/May", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Rivale am Weather Institute", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0254", Level = 29 }, new() { MonsterId = "PKM-0257", Level = 29 } },
                },
                new() {
                    Id = "HOE-T-010", Name = "Wally", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "HOE-OPT-003", Name = "Yasu", Klasse = "Ninja Boy",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Versteckt im Gras", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "HOE-0039", Name = "Fortree City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0040",
            Ost = "HOE-0038",
            Verbindungen = new() { "HOE-0040", "HOE-0038" },
            Trainer = new() {
                new() {
                    Id = "HOE-K-015", Name = "Winona", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Flug-Typ; Altaria mit Draco Meteor", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "HOE-GYM-006", Name = "Winona", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Winona", OrdenName = "Federorden",
                OrdenNr = 6, TypSpezialisierung = "Flug",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 40 } },
            },
        },
        new() {
            Id = "HOE-0040", Name = "Route 120", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "HOE-0041",
            West = "HOE-0042",
            Ost = "HOE-0039",
            Verbindungen = new() { "HOE-0041", "HOE-0042", "HOE-0039" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0359", MinLevel = 32, MaxLevel = 5, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0041", Name = "Ancient Tomb", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0040",
            Verbindungen = new() { "HOE-0040" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0379", MinLevel = 40, MaxLevel = 1, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0379", MinLevel = 40, MaxLevel = 40, Chance = 1 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0042", Name = "Route 121", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "HOE-0044",
            West = "HOE-0043",
            Ost = "HOE-0040",
            Verbindungen = new() { "HOE-0044", "HOE-0043", "HOE-0040" },
        },
        new() {
            Id = "HOE-0043", Name = "Safari Zone", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0042",
            Verbindungen = new() { "HOE-0042" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0111", MinLevel = 25, MaxLevel = 30, Chance = 15 }, // Unbekannt
                new() { MonsterId = "PKM-0113", MinLevel = 25, MaxLevel = 30, Chance = 4 }, // Unbekannt
                new() { MonsterId = "PKM-0115", MinLevel = 25, MaxLevel = 30, Chance = 4 }, // Unbekannt
                new() { MonsterId = "PKM-0147", MinLevel = 15, MaxLevel = 20, Chance = 1 }, // Unbekannt
                new() { MonsterId = "PKM-0115", MinLevel = 30, MaxLevel = 5, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0127", MinLevel = 30, MaxLevel = 5, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0246", MinLevel = 15, MaxLevel = 20, Chance = 5 }, // Unbekannt
                new() { MonsterId = "PKM-0231", MinLevel = 15, MaxLevel = 20, Chance = 10 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-NPC-1017", Name = "Keine Trainer", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "HOE-0044", Name = "Lilycove City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0050",
            Ost = "HOE-0042",
            Verbindungen = new() { "HOE-0050", "HOE-0042" },
            Trainer = new() {
                new() {
                    Id = "HOE-K-005", Name = "Brendan/May", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Rivale in Lilycove", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0254", Level = 32 }, new() { MonsterId = "PKM-0257", Level = 32 } },
                },
            },
        },
        new() {
            Id = "HOE-0045", Name = "Team Aqua Hideout", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "HOE-0044",
            Verbindungen = new() { "HOE-0044" },
        },
        new() {
            Id = "HOE-0046", Name = "Team Magma Hideout", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "HOE-0031",
            Verbindungen = new() { "HOE-0031" },
        },
        new() {
            Id = "HOE-0047", Name = "Route 122", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "HOE-0042",
            Süd = "HOE-0048",
            Verbindungen = new() { "HOE-0042", "HOE-0048" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0072", MinLevel = 25, MaxLevel = 30, Chance = 60 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0048", Name = "Mt. Pyre", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0047",
            Verbindungen = new() { "HOE-0047" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0355", MinLevel = 30, MaxLevel = 30, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0353", MinLevel = 30, MaxLevel = 30, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0049", Name = "Route 123", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0047",
            Ost = "HOE-0037",
            Verbindungen = new() { "HOE-0047", "HOE-0037" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0352", MinLevel = 28, MaxLevel = 32, Chance = 20 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0050", Name = "Route 124", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0051",
            Ost = "HOE-0044",
            Verbindungen = new() { "HOE-0051", "HOE-0044" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0369", MinLevel = 35, MaxLevel = 30, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0072", MinLevel = 30, MaxLevel = 35, Chance = 60 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0051", Name = "Mossdeep City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0052",
            Ost = "HOE-0050",
            Verbindungen = new() { "HOE-0052", "HOE-0050" },
            Trainer = new() {
                new() {
                    Id = "HOE-K-016", Name = "Tate & Liza", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Psycho-Typ; Doppelkampf", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "HOE-GYM-007R", Name = "Tate & Liza", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Tate & Liza", OrdenName = "Regenorden",
                OrdenNr = 7, TypSpezialisierung = "Psycho",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 45 } },
            },
        },
        new() {
            Id = "HOE-0052", Name = "Route 125", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "HOE-0053",
            Süd = "HOE-0051",
            Verbindungen = new() { "HOE-0053", "HOE-0051" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0072", MinLevel = 30, MaxLevel = 35, Chance = 60 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0053", Name = "Shoal Cave", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0052",
            Verbindungen = new() { "HOE-0052" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0361", MinLevel = 35, MaxLevel = 30, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0363", MinLevel = 35, MaxLevel = 25, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0361", MinLevel = 30, MaxLevel = 35, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0363", MinLevel = 30, MaxLevel = 35, Chance = 20 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0054", Name = "Route 126", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "HOE-0052",
            Süd = "HOE-0055",
            Verbindungen = new() { "HOE-0052", "HOE-0055" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0072", MinLevel = 30, MaxLevel = 35, Chance = 60 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0055", Name = "Sootopolis City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "HOE-0054",
            Verbindungen = new() { "HOE-0054" },
            Trainer = new() {
                new() {
                    Id = "HOE-K-017", Name = "Wallace (R/S) / Juan (E)", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Wasser-Typ; Wallace=Champ in Smaragd", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "HOE-GYM-008R", Name = "Wallace (Rubin/Saphir) / Juan (Smaragd)", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Juan/Wallace", OrdenName = "Regenorden",
                OrdenNr = 8, TypSpezialisierung = "Wasser",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 50 } },
            },
        },
        new() {
            Id = "HOE-0056", Name = "Cave of Origin", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "HOE-0055",
            Verbindungen = new() { "HOE-0055" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0382", MinLevel = 45, MaxLevel = 1, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0383", MinLevel = 45, MaxLevel = 1, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0057", Name = "Route 127", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "HOE-0054",
            Süd = "HOE-0051",
            Verbindungen = new() { "HOE-0054", "HOE-0051" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0072", MinLevel = 30, MaxLevel = 35, Chance = 60 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0058", Name = "Route 128", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "HOE-0057",
            Süd = "HOE-0059",
            Verbindungen = new() { "HOE-0057", "HOE-0059" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0072", MinLevel = 35, MaxLevel = 40, Chance = 60 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0059", Name = "Seafloor Cavern", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0058",
            Verbindungen = new() { "HOE-0058" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0318", MinLevel = 35, MaxLevel = 20, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "HOE-K-033", Name = "Maxie (final)", Klasse = "Team Magma",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Maxie vor Groudon", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "HOE-K-034", Name = "Archie (final)", Klasse = "Team Aqua",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Archie vor Kyogre", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "HOE-T-012", Name = "Team Aqua Archie", Klasse = "Boss",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "HOE-0060", Name = "Route 129", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "HOE-0058",
            Süd = "HOE-0061",
            Verbindungen = new() { "HOE-0058", "HOE-0061" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0072", MinLevel = 35, MaxLevel = 40, Chance = 60 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0061", Name = "Route 130", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "HOE-0060",
            Süd = "HOE-0062",
            Verbindungen = new() { "HOE-0060", "HOE-0062" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0072", MinLevel = 35, MaxLevel = 40, Chance = 60 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0062", Name = "Route 131", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "HOE-0061",
            Süd = "HOE-0063",
            Verbindungen = new() { "HOE-0061", "HOE-0063" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0072", MinLevel = 35, MaxLevel = 40, Chance = 60 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0063", Name = "Pacifidlog Town", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "HOE-0062",
            Süd = "HOE-0064",
            Verbindungen = new() { "HOE-0062", "HOE-0064" },
        },
        new() {
            Id = "HOE-0064", Name = "Route 132", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "HOE-0063",
            Süd = "HOE-0065",
            Verbindungen = new() { "HOE-0063", "HOE-0065" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0072", MinLevel = 35, MaxLevel = 40, Chance = 60 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0065", Name = "Route 133", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "HOE-0064",
            Süd = "HOE-0066",
            Verbindungen = new() { "HOE-0064", "HOE-0066" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0072", MinLevel = 35, MaxLevel = 40, Chance = 60 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0066", Name = "Route 134", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "HOE-0065",
            Süd = "HOE-0067",
            Verbindungen = new() { "HOE-0065", "HOE-0067" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0072", MinLevel = 35, MaxLevel = 40, Chance = 60 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0067", Name = "Sealed Chamber", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0066",
            Verbindungen = new() { "HOE-0066" },
        },
        new() {
            Id = "HOE-0068", Name = "Desert Ruins", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0027",
            Verbindungen = new() { "HOE-0027" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0377", MinLevel = 40, MaxLevel = 1, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0377", MinLevel = 40, MaxLevel = 40, Chance = 1 }, // Unbekannt
            },
        },
        new() {
            Id = "HOE-0069", Name = "Route 119 (Süd)", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "HOE-0037",
            Verbindungen = new() { "HOE-0037" },
        },
        new() {
            Id = "HOE-0070", Name = "Weather Institute", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0038",
            Verbindungen = new() { "HOE-0038" },
            Trainer = new() {
                new() {
                    Id = "HOE-T-011", Name = "Team Aqua/Magma", Klasse = "Boss",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "HOE-0071", Name = "Ever Grande City", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0058",
            Verbindungen = new() { "HOE-0058" },
            Trainer = new() {
                new() {
                    Id = "HOE-K-020", Name = "Sidney", Klasse = "Top4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Unlicht-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "HOE-K-021", Name = "Phoebe", Klasse = "Top4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Geist-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "HOE-K-022", Name = "Glacia", Klasse = "Top4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Eis-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "HOE-K-023", Name = "Drake", Klasse = "Top4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Drachen-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "HOE-K-024", Name = "Steven (R/S) / Wallace (E)", Klasse = "Champion",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Stahl-Typ (Steven) / Wasser-Typ (Wallace Smaragd)", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "HOE-E4-001", Name = "Sidney", Klasse = "Top4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "HOE-0072", Name = "Victory Road (Hoenn)", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0071",
            Verbindungen = new() { "HOE-0071" },
            Trainer = new() {
                new() {
                    Id = "HOE-K-006", Name = "Wally", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Wally letzter Kampf", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0282", Level = 44 } },
                },
            },
        },
        new() {
            Id = "HOE-0073", Name = "Battle Resort", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
        },
        new() {
            Id = "HOE-0074", Name = "Sky Pillar", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "HOE-0062",
            Verbindungen = new() { "HOE-0062" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0384", MinLevel = 70, MaxLevel = 1, Chance = 0 }, // Unbekannt
            },
        },
        // === Sinnoh (65 Orte) ===
        new() {
            Id = "SIN-0001", Name = "Twinleaf Town", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "SIN-0002",
            Verbindungen = new() { "SIN-0002" },
        },
        new() {
            Id = "SIN-0002", Name = "Route 201", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "SIN-0003",
            Süd = "SIN-0001",
            Ost = "SIN-0004",
            Verbindungen = new() { "SIN-0003", "SIN-0001", "SIN-0004" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0396", MinLevel = 4, MaxLevel = 55, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0387", MinLevel = 5, MaxLevel = 1, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "SIN-K-001", Name = "Barry", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Barry auf Route 201", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0387", Level = 5 }, new() { MonsterId = "PKM-0390", Level = 5 }, new() { MonsterId = "PKM-0393", Level = 5 } },
                },
                new() {
                    Id = "SIN-T-001", Name = "Barry", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "SIN-0003", Name = "Lake Verity", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "SIN-0002",
            Verbindungen = new() { "SIN-0002" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0481", MinLevel = 50, MaxLevel = 1, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "SIN-0004", Name = "Sandgem Town", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "SIN-0058",
            West = "SIN-0005",
            Ost = "SIN-0002",
            Verbindungen = new() { "SIN-0058", "SIN-0005", "SIN-0002" },
        },
        new() {
            Id = "SIN-0005", Name = "Route 202", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "SIN-0006",
            Ost = "SIN-0004",
            Verbindungen = new() { "SIN-0006", "SIN-0004" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0396", MinLevel = 5, MaxLevel = 45, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "SIN-0006", Name = "Jubilife City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "SIN-0011",
            Süd = "SIN-0005",
            West = "SIN-0045",
            Ost = "SIN-0007",
            Verbindungen = new() { "SIN-0011", "SIN-0005", "SIN-0045", "SIN-0007" },
            Trainer = new() {
                new() {
                    Id = "SIN-K-002", Name = "Barry", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Barry in Jubilife", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0387", Level = 9 }, new() { MonsterId = "PKM-0390", Level = 9 }, new() { MonsterId = "PKM-0393", Level = 9 } },
                },
                new() {
                    Id = "SIN-T-003", Name = "Team Galactic Cyrus", Klasse = "Boss",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "SIN-0007", Name = "Route 203", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "SIN-0008",
            Ost = "SIN-0006",
            Verbindungen = new() { "SIN-0008", "SIN-0006" },
            Trainer = new() {
                new() {
                    Id = "SIN-T-002", Name = "Barry", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "SIN-0008", Name = "Oreburgh Gate", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "SIN-0007",
            Ost = "SIN-0009",
            Verbindungen = new() { "SIN-0007", "SIN-0009" },
        },
        new() {
            Id = "SIN-0009", Name = "Oreburgh City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "SIN-0020",
            Süd = "SIN-0008",
            Verbindungen = new() { "SIN-0020", "SIN-0008" },
            Trainer = new() {
                new() {
                    Id = "SIN-K-010", Name = "Roark", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Gestein-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "SIN-GYM-001", Name = "Roark", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Roark", OrdenName = "Kohleorden",
                OrdenNr = 1, TypSpezialisierung = "Gestein",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 15 } },
            },
        },
        new() {
            Id = "SIN-0010", Name = "Oreburgh Mine", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "SIN-0009",
            Verbindungen = new() { "SIN-0009" },
        },
        new() {
            Id = "SIN-0011", Name = "Route 204", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "SIN-0013",
            Süd = "SIN-0006",
            Verbindungen = new() { "SIN-0013", "SIN-0006" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0396", MinLevel = 5, MaxLevel = 7, Chance = 30 }, // Unbekannt
            },
        },
        new() {
            Id = "SIN-0012", Name = "Ravaged Path", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
        },
        new() {
            Id = "SIN-0013", Name = "Floaroma Town", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "SIN-0015",
            Süd = "SIN-0011",
            Verbindungen = new() { "SIN-0015", "SIN-0011" },
        },
        new() {
            Id = "SIN-0014", Name = "Valley Windworks", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "SIN-0015",
            Verbindungen = new() { "SIN-0015" },
        },
        new() {
            Id = "SIN-0015", Name = "Route 205", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "SIN-0017",
            Ost = "SIN-0013",
            Verbindungen = new() { "SIN-0017", "SIN-0013" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0396", MinLevel = 8, MaxLevel = 12, Chance = 30 }, // Unbekannt
            },
        },
        new() {
            Id = "SIN-0016", Name = "Eterna Forest", Typ = "ort",
            Farbe = "forest", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            WildMonster = new() {
                new() { MonsterId = "PKM-0420", MinLevel = 12, MaxLevel = 30, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0092", MinLevel = 12, MaxLevel = 20, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "SIN-0017", Name = "Eterna City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "SIN-0019",
            West = "SIN-0030",
            Ost = "SIN-0015",
            Verbindungen = new() { "SIN-0019", "SIN-0030", "SIN-0015" },
            Trainer = new() {
                new() {
                    Id = "SIN-K-003", Name = "Barry", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Barry in Eterna", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0388", Level = 19 }, new() { MonsterId = "PKM-0391", Level = 19 } },
                },
                new() {
                    Id = "SIN-K-011", Name = "Gardenia", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Pflanz-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "SIN-GYM-002", Name = "Gardenia", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "SIN-T-004", Name = "Team Galactic Jupiter", Klasse = "Boss",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Gardenia", OrdenName = "Waldorden",
                OrdenNr = 2, TypSpezialisierung = "Pflanze",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 20 } },
            },
        },
        new() {
            Id = "SIN-0018", Name = "Team Galaxis Gebäude (Eterna)", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "SIN-0017",
            Verbindungen = new() { "SIN-0017" },
        },
        new() {
            Id = "SIN-0019", Name = "Route 206", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "SIN-0017",
            Süd = "SIN-0020",
            Verbindungen = new() { "SIN-0017", "SIN-0020" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0396", MinLevel = 15, MaxLevel = 18, Chance = 30 }, // Unbekannt
            },
        },
        new() {
            Id = "SIN-0020", Name = "Route 207", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "SIN-0009",
            Verbindungen = new() { "SIN-0009" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0396", MinLevel = 10, MaxLevel = 14, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "SIN-T-005", Name = "Barry", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "SIN-0021", Name = "Mt. Coronet", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "SIN-0055",
            West = "SIN-0020",
            Ost = "SIN-0022",
            Verbindungen = new() { "SIN-0055", "SIN-0020", "SIN-0022" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0436", MinLevel = 25, MaxLevel = 30, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0041", MinLevel = 25, MaxLevel = 30, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "SIN-T-010", Name = "Team Galactic Cyrus", Klasse = "Boss",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "SIN-0022", Name = "Route 208", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "SIN-0023",
            Ost = "SIN-0021",
            Verbindungen = new() { "SIN-0023", "SIN-0021" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0420", MinLevel = 18, MaxLevel = 22, Chance = 30 }, // Unbekannt
            },
        },
        new() {
            Id = "SIN-0023", Name = "Hearthome City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "SIN-0031",
            West = "SIN-0024",
            Ost = "SIN-0022",
            Verbindungen = new() { "SIN-0031", "SIN-0024", "SIN-0022" },
            Trainer = new() {
                new() {
                    Id = "SIN-K-004", Name = "Barry", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Barry in Hearthome", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0389", Level = 24 }, new() { MonsterId = "PKM-0392", Level = 24 } },
                },
                new() {
                    Id = "SIN-K-014", Name = "Fantina", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Geist-Typ; Platin: Arena 3", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "SIN-GYM-003P", Name = "Fantina", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "SIN-GYM-005", Name = "Fantina (D/P) / Maylene (Platin)", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "SIN-T-006", Name = "Barry", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Fantina", OrdenName = "Rampenlichtorden",
                OrdenNr = 3, TypSpezialisierung = "Geist",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 25 } },
            },
        },
        new() {
            Id = "SIN-0024", Name = "Route 209", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "SIN-0026",
            Ost = "SIN-0023",
            Verbindungen = new() { "SIN-0026", "SIN-0023" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0396", MinLevel = 15, MaxLevel = 18, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "OPT-SIN-001", Name = "Shelly", Klasse = "Cowgirl",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Nahe Trostu", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "SIN-0025", Name = "Lost Tower", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "SIN-0024",
            Verbindungen = new() { "SIN-0024" },
        },
        new() {
            Id = "SIN-0026", Name = "Solaceon Town", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "SIN-0028",
            Süd = "SIN-0024",
            Verbindungen = new() { "SIN-0028", "SIN-0024" },
        },
        new() {
            Id = "SIN-0027", Name = "Solaceon Ruins", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "SIN-0026",
            Verbindungen = new() { "SIN-0026" },
        },
        new() {
            Id = "SIN-0028", Name = "Route 210", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "SIN-0026",
            West = "SIN-0029",
            Ost = "SIN-0038",
            Verbindungen = new() { "SIN-0026", "SIN-0029", "SIN-0038" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0234", MinLevel = 25, MaxLevel = 15, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0396", MinLevel = 18, MaxLevel = 22, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "SIN-OPT-001", Name = "Erika", Klasse = "Ace Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Nebelroute", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "SIN-0029", Name = "Celestic Town", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "SIN-0030",
            Ost = "SIN-0028",
            Verbindungen = new() { "SIN-0030", "SIN-0028" },
            Trainer = new() {
                new() {
                    Id = "SIN-T-009", Name = "Team Galactic Cyrus", Klasse = "Boss",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "SIN-0030", Name = "Route 211", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "SIN-0029",
            West = "SIN-0021",
            Ost = "SIN-0017",
            Verbindungen = new() { "SIN-0029", "SIN-0021", "SIN-0017" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0396", MinLevel = 20, MaxLevel = 24, Chance = 30 }, // Unbekannt
            },
        },
        new() {
            Id = "SIN-0031", Name = "Route 212", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "SIN-0032",
            Ost = "SIN-0023",
            Verbindungen = new() { "SIN-0032", "SIN-0023" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0420", MinLevel = 22, MaxLevel = 26, Chance = 30 }, // Unbekannt
            },
        },
        new() {
            Id = "SIN-0032", Name = "Pastoria City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "SIN-0034",
            Ost = "SIN-0031",
            Verbindungen = new() { "SIN-0034", "SIN-0031" },
            Trainer = new() {
                new() {
                    Id = "SIN-K-013", Name = "Crasher Wake", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Wasser-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "SIN-GYM-004", Name = "Crasher Wake", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "SIN-T-007", Name = "Team Galactic Saturn", Klasse = "Boss",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "SIN-T-008", Name = "Barry", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Crasher Wake", OrdenName = "Regenorden",
                OrdenNr = 5, TypSpezialisierung = "Wasser",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 35 } },
            },
        },
        new() {
            Id = "SIN-0033", Name = "Great Marsh", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "SIN-0032",
            Verbindungen = new() { "SIN-0032" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0114", MinLevel = 25, MaxLevel = 5, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0455", MinLevel = 25, MaxLevel = 5, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "SIN-0034", Name = "Route 213", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "SIN-0035",
            Ost = "SIN-0032",
            Verbindungen = new() { "SIN-0035", "SIN-0032" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0072", MinLevel = 25, MaxLevel = 30, Chance = 60 }, // Unbekannt
            },
        },
        new() {
            Id = "SIN-0035", Name = "Route 214", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "SIN-0036",
            Ost = "SIN-0034",
            Verbindungen = new() { "SIN-0036", "SIN-0034" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0396", MinLevel = 22, MaxLevel = 26, Chance = 30 }, // Unbekannt
            },
        },
        new() {
            Id = "SIN-0036", Name = "Veilstone City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "SIN-0038",
            Ost = "SIN-0035",
            Verbindungen = new() { "SIN-0038", "SIN-0035" },
            Trainer = new() {
                new() {
                    Id = "SIN-K-012", Name = "Maylene", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Kampf-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "SIN-GYM-003DP", Name = "Maylene", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Maylene", OrdenName = "Kobolzorden",
                OrdenNr = 4, TypSpezialisierung = "Kampf",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 30 } },
            },
        },
        new() {
            Id = "SIN-0037", Name = "Team Galaxis HQ", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "SIN-0036",
            Verbindungen = new() { "SIN-0036" },
            Trainer = new() {
                new() {
                    Id = "SIN-K-030", Name = "Cyrus", Klasse = "Team Galaxis",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Cyrus erster Kampf", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "SIN-0038", Name = "Route 215", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "SIN-0036",
            Ost = "SIN-0028",
            Verbindungen = new() { "SIN-0036", "SIN-0028" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0396", MinLevel = 22, MaxLevel = 26, Chance = 30 }, // Unbekannt
            },
        },
        new() {
            Id = "SIN-0039", Name = "Route 216", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "SIN-0040",
            Ost = "SIN-0021",
            Verbindungen = new() { "SIN-0040", "SIN-0021" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0459", MinLevel = 30, MaxLevel = 30, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0220", MinLevel = 30, MaxLevel = 35, Chance = 30 }, // Unbekannt
            },
        },
        new() {
            Id = "SIN-0040", Name = "Route 217", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "SIN-0041",
            Ost = "SIN-0039",
            Verbindungen = new() { "SIN-0041", "SIN-0039" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0220", MinLevel = 30, MaxLevel = 20, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0220", MinLevel = 30, MaxLevel = 35, Chance = 30 }, // Unbekannt
            },
        },
        new() {
            Id = "SIN-0041", Name = "Acuity Lakefront", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "SIN-0040",
            West = "SIN-0042",
            Ost = "SIN-0043",
            Verbindungen = new() { "SIN-0040", "SIN-0042", "SIN-0043" },
        },
        new() {
            Id = "SIN-0042", Name = "Lake Acuity", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "SIN-0041",
            Verbindungen = new() { "SIN-0041" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0480", MinLevel = 50, MaxLevel = 1, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "SIN-0043", Name = "Snowpoint City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "SIN-0039",
            Süd = "SIN-0041",
            Verbindungen = new() { "SIN-0039", "SIN-0041" },
            Trainer = new() {
                new() {
                    Id = "SIN-K-016", Name = "Candice", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Eis-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "SIN-GYM-007", Name = "Candice", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Candice", OrdenName = "Eisorden",
                OrdenNr = 7, TypSpezialisierung = "Eis",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 45 } },
            },
        },
        new() {
            Id = "SIN-0044", Name = "Snowpoint Temple", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "SIN-0043",
            Verbindungen = new() { "SIN-0043" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0480", MinLevel = 50, MaxLevel = 50, Chance = 1 }, // Unbekannt
            },
        },
        new() {
            Id = "SIN-0045", Name = "Route 218", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "SIN-0046",
            Ost = "SIN-0006",
            Verbindungen = new() { "SIN-0046", "SIN-0006" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0072", MinLevel = 25, MaxLevel = 30, Chance = 60 }, // Unbekannt
            },
        },
        new() {
            Id = "SIN-0046", Name = "Canalave City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "SIN-0045",
            Süd = "SIN-0047",
            Verbindungen = new() { "SIN-0045", "SIN-0047" },
            Trainer = new() {
                new() {
                    Id = "SIN-K-005", Name = "Barry", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Barry in Canalave", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0389", Level = 35 }, new() { MonsterId = "PKM-0392", Level = 35 } },
                },
                new() {
                    Id = "SIN-K-015", Name = "Byron", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Stahl-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "SIN-GYM-006", Name = "Byron", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Byron", OrdenName = "Mineorden",
                OrdenNr = 6, TypSpezialisierung = "Stahl",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 40 } },
            },
        },
        new() {
            Id = "SIN-0047", Name = "Iron Island", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "SIN-0046",
            Verbindungen = new() { "SIN-0046" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0304", MinLevel = 30, MaxLevel = 20, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0304", MinLevel = 30, MaxLevel = 35, Chance = 30 }, // Unbekannt
            },
        },
        new() {
            Id = "SIN-0048", Name = "Lake Valor", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "SIN-0035",
            Verbindungen = new() { "SIN-0035" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0482", MinLevel = 50, MaxLevel = 1, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "SIN-0049", Name = "Valor Lakefront", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "SIN-0048",
            West = "SIN-0034",
            Ost = "SIN-0050",
            Verbindungen = new() { "SIN-0048", "SIN-0034", "SIN-0050" },
        },
        new() {
            Id = "SIN-0050", Name = "Route 222", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "SIN-0051",
            Ost = "SIN-0049",
            Verbindungen = new() { "SIN-0051", "SIN-0049" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0396", MinLevel = 30, MaxLevel = 35, Chance = 30 }, // Unbekannt
            },
        },
        new() {
            Id = "SIN-0051", Name = "Sunyshore City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "SIN-0052",
            Ost = "SIN-0050",
            Verbindungen = new() { "SIN-0052", "SIN-0050" },
            Trainer = new() {
                new() {
                    Id = "SIN-K-017", Name = "Volkner", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Elektro-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "SIN-GYM-008", Name = "Volkner", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Volkner", OrdenName = "Voltaorden",
                OrdenNr = 8, TypSpezialisierung = "Elektro",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 50 } },
            },
        },
        new() {
            Id = "SIN-0052", Name = "Route 223", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Ost = "SIN-0051",
            Verbindungen = new() { "SIN-0051" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0072", MinLevel = 35, MaxLevel = 40, Chance = 60 }, // Unbekannt
            },
        },
        new() {
            Id = "SIN-0053", Name = "Victory Road (Sinnoh)", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "SIN-0052",
            Verbindungen = new() { "SIN-0052" },
            Trainer = new() {
                new() {
                    Id = "SIN-K-006", Name = "Barry", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Barry letzter Kampf", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0389", Level = 49 }, new() { MonsterId = "PKM-0392", Level = 49 } },
                },
            },
        },
        new() {
            Id = "SIN-0054", Name = "Pokémon League (Sinnoh)", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "SIN-0053",
            Verbindungen = new() { "SIN-0053" },
            Trainer = new() {
                new() {
                    Id = "SIN-K-020", Name = "Aaron", Klasse = "Top4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Käfer-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "SIN-K-021", Name = "Bertha", Klasse = "Top4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Boden-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "SIN-K-022", Name = "Flint", Klasse = "Top4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Feuer-Typ (D/P gemischt)", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "SIN-K-023", Name = "Lucian", Klasse = "Top4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Psycho-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "SIN-K-024", Name = "Cynthia", Klasse = "Champion",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Stärkste Trainerin; Garchomp berühmt", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "SIN-0055", Name = "Spear Pillar", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "SIN-0021",
            Verbindungen = new() { "SIN-0021" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0483", MinLevel = 47, MaxLevel = 1, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0484", MinLevel = 47, MaxLevel = 1, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "SIN-K-031", Name = "Cyrus (final)", Klasse = "Team Galaxis",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Cyrus letzter Kampf", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0484", Level = 45 } },
                },
            },
        },
        new() {
            Id = "SIN-0056", Name = "Distortion World", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "SIN-0055",
            Verbindungen = new() { "SIN-0055" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0487", MinLevel = 47, MaxLevel = 1, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "SIN-0057", Name = "Turnback Cave", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "SIN-0035",
            Verbindungen = new() { "SIN-0035" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0487", MinLevel = 70, MaxLevel = 70, Chance = 1 }, // Unbekannt
            },
        },
        new() {
            Id = "SIN-0058", Name = "Route 219", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "SIN-0004",
            Süd = "SIN-0059",
            Verbindungen = new() { "SIN-0004", "SIN-0059" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0072", MinLevel = 25, MaxLevel = 30, Chance = 60 }, // Unbekannt
            },
        },
        new() {
            Id = "SIN-0059", Name = "Route 220", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "SIN-0058",
            Süd = "SIN-0060",
            Verbindungen = new() { "SIN-0058", "SIN-0060" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0072", MinLevel = 25, MaxLevel = 30, Chance = 60 }, // Unbekannt
            },
        },
        new() {
            Id = "SIN-0060", Name = "Route 221", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "SIN-0059",
            Süd = "SIN-0061",
            Verbindungen = new() { "SIN-0059", "SIN-0061" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0396", MinLevel = 25, MaxLevel = 30, Chance = 30 }, // Unbekannt
            },
        },
        new() {
            Id = "SIN-0061", Name = "Pal Park", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "SIN-0060",
            Verbindungen = new() { "SIN-0060" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0019", MinLevel = 50, MaxLevel = 70, Chance = 100 }, // Unbekannt
            },
        },
        new() {
            Id = "SIN-0062", Name = "Battle Zone", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "SIN-0063",
            Verbindungen = new() { "SIN-0063" },
            Trainer = new() {
                new() {
                    Id = "SIN-OPT-003", Name = "Marina", Klasse = "Ace Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Postgame", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "SIN-0063", Name = "Route 225", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "SIN-0064",
            Ost = "SIN-0062",
            Verbindungen = new() { "SIN-0064", "SIN-0062" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0396", MinLevel = 40, MaxLevel = 46, Chance = 30 }, // Unbekannt
            },
        },
        new() {
            Id = "SIN-0064", Name = "Route 226", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Ost = "SIN-0063",
            Verbindungen = new() { "SIN-0063" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0072", MinLevel = 40, MaxLevel = 46, Chance = 60 }, // Unbekannt
            },
        },
        new() {
            Id = "SIN-0065", Name = "Stark Mountain", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            WildMonster = new() {
                new() { MonsterId = "PKM-0485", MinLevel = 70, MaxLevel = 1, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0485", MinLevel = 70, MaxLevel = 70, Chance = 1 }, // Unbekannt
            },
        },
        // === Unova (54 Orte) ===
        new() {
            Id = "UNO-0001", Name = "Nuvema Town", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0002",
            Verbindungen = new() { "KAL-0002" },
        },
        new() {
            Id = "UNO-0002", Name = "Route 1", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "UNO-0003",
            Süd = "UNO-0001",
            Verbindungen = new() { "UNO-0003", "UNO-0001" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0016", MinLevel = 2, MaxLevel = 4, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0019", MinLevel = 2, MaxLevel = 4, Chance = 45 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 4, MaxLevel = 55, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0504", MinLevel = 4, MaxLevel = 45, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0016", MinLevel = 2, MaxLevel = 4, Chance = 45 }, // Unbekannt
                new() { MonsterId = "PKM-0019", MinLevel = 2, MaxLevel = 4, Chance = 45 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 2, MaxLevel = 4, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0509", MinLevel = 2, MaxLevel = 4, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "UNO-K-001", Name = "Bianca", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Bianca auf Route 1", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0495", Level = 5 }, new() { MonsterId = "PKM-0498", Level = 5 }, new() { MonsterId = "PKM-0501", Level = 5 } },
                },
                new() {
                    Id = "UNO-T-001", Name = "Youngster", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "ALO-T-001", Name = "Hau", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "UNO-0003", Name = "Accumula Town", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0004",
            Süd = "KAL-0002",
            Verbindungen = new() { "KAL-0004", "KAL-0002" },
            Trainer = new() {
                new() {
                    Id = "UNO-K-002", Name = "Cheren", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Cheren in Accumula", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0495", Level = 7 }, new() { MonsterId = "PKM-0498", Level = 7 }, new() { MonsterId = "PKM-0501", Level = 7 } },
                },
            },
        },
        new() {
            Id = "UNO-0004", Name = "Route 2", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "UNO-0005",
            Süd = "UNO-0003",
            Verbindungen = new() { "UNO-0005", "UNO-0003" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0016", MinLevel = 3, MaxLevel = 5, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0019", MinLevel = 3, MaxLevel = 5, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0052", MinLevel = 3, MaxLevel = 5, Chance = 5 }, // Unbekannt
                new() { MonsterId = "PKM-0056", MinLevel = 3, MaxLevel = 5, Chance = 5 }, // Unbekannt
                new() { MonsterId = "PKM-0016", MinLevel = 3, MaxLevel = 5, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0019", MinLevel = 3, MaxLevel = 5, Chance = 35 }, // Unbekannt
                new() { MonsterId = "PKM-0021", MinLevel = 3, MaxLevel = 5, Chance = 10 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 4, MaxLevel = 6, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "UNO-T-002", Name = "Cheren", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAL-T-001", Name = "Youngster", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "GAL-T-001", Name = "Hop", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "ALO-OPT-001", Name = "Yuka", Klasse = "Pokémon-Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Früher Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "UNO-0005", Name = "Striaton City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0006",
            Süd = "KAL-0004",
            Ost = "UNO-0006",
            Verbindungen = new() { "KAL-0006", "KAL-0004", "UNO-0006" },
            Trainer = new() {
                new() {
                    Id = "UNO-K-010", Name = "Cilan/Chili/Cress", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Starter-abhängig; Pansage/Pansear/Panpour", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0511", Level = 12 }, new() { MonsterId = "PKM-0513", Level = 12 }, new() { MonsterId = "PKM-0514", Level = 12 } },
                },
                new() {
                    Id = "UNO-GYM-001", Name = "Cilan/Chili/Cress", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Cilan/Chili/Cress", OrdenName = "Trilobitorden",
                OrdenNr = 1, TypSpezialisierung = "Pflanze/Feuer/Wasser",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 15 } },
            },
        },
        new() {
            Id = "UNO-0006", Name = "Dreamyard", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "UNO-0005",
            Verbindungen = new() { "UNO-0005" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0517", MinLevel = 17, MaxLevel = 30, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "UNO-0007", Name = "Route 3", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "UNO-0009",
            Süd = "UNO-0005",
            Verbindungen = new() { "UNO-0009", "UNO-0005" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0016", MinLevel = 6, MaxLevel = 8, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0021", MinLevel = 6, MaxLevel = 8, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0039", MinLevel = 6, MaxLevel = 8, Chance = 5 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 6, MaxLevel = 8, Chance = 20 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-NPC-1000", Name = "Youngster Jan", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-NPC-1001", Name = "Lass Anna", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-T-001", Name = "Lass", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-T-002", Name = "Youngster", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAL-T-002", Name = "Shauna", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "OPT-KAN-001", Name = "Ben", Klasse = "Junge",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Vor Mt. Moon", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "UNO-0008", Name = "Wellspring Cave", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0006",
            Verbindungen = new() { "KAL-0006" },
            Trainer = new() {
                new() {
                    Id = "UNO-T-003", Name = "Team Plasma", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "UNO-0009", Name = "Nacrene City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "UNO-0010",
            Süd = "KAL-0006",
            Verbindungen = new() { "UNO-0010", "KAL-0006" },
            Trainer = new() {
                new() {
                    Id = "UNO-K-003", Name = "Cheren", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Cheren in Nacrene", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0496", Level = 18 }, new() { MonsterId = "PKM-0499", Level = 18 } },
                },
                new() {
                    Id = "UNO-K-011", Name = "Lenora", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Normal-Typ; Watchog mit Retaliate", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "UNO-GYM-002", Name = "Lenora", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Lenora", OrdenName = "Grundorden",
                OrdenNr = 2, TypSpezialisierung = "Normal",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 20 } },
            },
        },
        new() {
            Id = "UNO-0010", Name = "Pinwheel Forest", Typ = "ort",
            Farbe = "forest", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "UNO-0011",
            Ost = "UNO-0009",
            Verbindungen = new() { "UNO-0011", "UNO-0009" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0511", MinLevel = 18, MaxLevel = 20, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "UNO-T-004", Name = "Team Plasma N", Klasse = "Boss",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0519", Level = 13 } },
                },
            },
        },
        new() {
            Id = "UNO-0011", Name = "Skyarrow Bridge", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "UNO-0012",
            Ost = "UNO-0010",
            Verbindungen = new() { "UNO-0012", "UNO-0010" },
        },
        new() {
            Id = "UNO-0012", Name = "Castelia City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0008",
            Ost = "UNO-0011",
            Verbindungen = new() { "KAL-0008", "UNO-0011" },
            Trainer = new() {
                new() {
                    Id = "UNO-K-004", Name = "Bianca", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Bianca in Castelia", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0496", Level = 21 }, new() { MonsterId = "PKM-0499", Level = 21 } },
                },
                new() {
                    Id = "UNO-K-012", Name = "Burgh", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Käfer-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "UNO-GYM-003", Name = "Burgh", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "UNO-T-005", Name = "Team Plasma Ghetsis", Klasse = "Boss",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0509", Level = 22 } },
                },
            },
            Arena = new() {
                Leiter = "Burgh", OrdenName = "Insektorden",
                OrdenNr = 3, TypSpezialisierung = "Käfer",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 25 } },
            },
        },
        new() {
            Id = "UNO-0013", Name = "Castelia Sewers", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "UNO-0012",
            Verbindungen = new() { "UNO-0012" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0109", MinLevel = 16, MaxLevel = 20, Chance = 30 }, // Unbekannt
            },
        },
        new() {
            Id = "UNO-0014", Name = "Route 4", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "UNO-0017",
            Ost = "UNO-0012",
            Verbindungen = new() { "UNO-0017", "UNO-0012" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0021", MinLevel = 8, MaxLevel = 12, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0027", MinLevel = 8, MaxLevel = 12, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0023", MinLevel = 8, MaxLevel = 12, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0058", MinLevel = 8, MaxLevel = 12, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0551", MinLevel = 16, MaxLevel = 20, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 7, MaxLevel = 9, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0819", MinLevel = 8, MaxLevel = 12, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "UNO-T-006", Name = "Bianca", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAL-T-003", Name = "Lass", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "UNO-0015", Name = "Desert Resort", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0008",
            Verbindungen = new() { "KAL-0008" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0551", MinLevel = 22, MaxLevel = 30, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0554", MinLevel = 22, MaxLevel = 20, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "UNO-T-007", Name = "Team Plasma N", Klasse = "Boss",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "UNO-0016", Name = "Relic Castle", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "UNO-0015",
            Verbindungen = new() { "UNO-0015" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0562", MinLevel = 30, MaxLevel = 30, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0637", MinLevel = 70, MaxLevel = 1, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "UNO-0017", Name = "Nimbasa City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0033",
            Süd = "KAL-0019",
            West = "KAL-0010",
            Ost = "KAL-0008",
            Verbindungen = new() { "KAL-0033", "KAL-0019", "KAL-0010", "KAL-0008" },
            Trainer = new() {
                new() {
                    Id = "UNO-K-005", Name = "N", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "N in Nimbasa", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "UNO-K-013", Name = "Elesa", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Elektro-Typ; Zebstrika mit Volt Switch", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "UNO-GYM-004", Name = "Elesa", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "UNO-T-008", Name = "Cheren", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Elesa", OrdenName = "Voltaorden",
                OrdenNr = 4, TypSpezialisierung = "Elektro",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 30 } },
            },
        },
        new() {
            Id = "UNO-0018", Name = "Route 5", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "UNO-0019",
            Ost = "UNO-0017",
            Verbindungen = new() { "UNO-0019", "UNO-0017" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0043", MinLevel = 13, MaxLevel = 15, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0060", MinLevel = 13, MaxLevel = 15, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 16, MaxLevel = 20, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 9, MaxLevel = 11, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0819", MinLevel = 16, MaxLevel = 20, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAL-K-001", Name = "Serena/Calem", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Rivale auf Route 5", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0650", Level = 12 }, new() { MonsterId = "PKM-0653", Level = 12 }, new() { MonsterId = "PKM-0656", Level = 12 } },
                },
                new() {
                    Id = "GAL-OPT-001", Name = "Adrian", Klasse = "Pokémon Breeder",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Früher Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "UNO-0019", Name = "Driftveil Drawbridge", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "UNO-0020",
            Ost = "KAL-0010",
            Verbindungen = new() { "UNO-0020", "KAL-0010" },
        },
        new() {
            Id = "UNO-0020", Name = "Driftveil City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0012",
            Ost = "UNO-0019",
            Verbindungen = new() { "KAL-0012", "UNO-0019" },
            Trainer = new() {
                new() {
                    Id = "UNO-K-014", Name = "Clay", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Boden-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "UNO-GYM-005", Name = "Clay", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "UNO-T-009", Name = "Team Plasma N", Klasse = "Boss",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Clay", OrdenName = "Jetorden",
                OrdenNr = 5, TypSpezialisierung = "Boden",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 35 } },
            },
        },
        new() {
            Id = "UNO-0021", Name = "Cold Storage", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "UNO-0020",
            Verbindungen = new() { "UNO-0020" },
        },
        new() {
            Id = "UNO-0022", Name = "Route 6", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "UNO-0023",
            Ost = "UNO-0020",
            Verbindungen = new() { "UNO-0023", "UNO-0020" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0043", MinLevel = 13, MaxLevel = 15, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0060", MinLevel = 13, MaxLevel = 15, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0585", MinLevel = 20, MaxLevel = 24, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 11, MaxLevel = 13, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0819", MinLevel = 22, MaxLevel = 26, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-NPC-1009", Name = "Jr. Trainerin", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "UNO-0023", Name = "Chargestone Cave", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0012",
            Ost = "UNO-0024",
            Verbindungen = new() { "KAL-0012", "UNO-0024" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0599", MinLevel = 25, MaxLevel = 30, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0602", MinLevel = 25, MaxLevel = 15, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "UNO-0024", Name = "Mistralton City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0014",
            Süd = "UNO-0023",
            Verbindungen = new() { "KAL-0014", "UNO-0023" },
            Trainer = new() {
                new() {
                    Id = "UNO-K-015", Name = "Skyla", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Flug-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "UNO-GYM-006", Name = "Skyla", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Skyla", OrdenName = "Jetorden",
                OrdenNr = 6, TypSpezialisierung = "Flug",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 40 } },
            },
        },
        new() {
            Id = "UNO-0025", Name = "Celestial Tower", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "KAL-0014",
            Verbindungen = new() { "KAL-0014" },
        },
        new() {
            Id = "UNO-0026", Name = "Route 7", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "UNO-0027",
            Ost = "UNO-0024",
            Verbindungen = new() { "UNO-0027", "UNO-0024" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0052", MinLevel = 18, MaxLevel = 22, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0056", MinLevel = 18, MaxLevel = 22, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 22, MaxLevel = 26, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 13, MaxLevel = 15, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0819", MinLevel = 26, MaxLevel = 30, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAL-OPT-001", Name = "Brigitte", Klasse = "Pokémon Breeder",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Frühe Route", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "UNO-0027", Name = "Twist Mountain", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0014",
            Ost = "UNO-0028",
            Verbindungen = new() { "KAL-0014", "UNO-0028" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0613", MinLevel = 32, MaxLevel = 25, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "UNO-T-010", Name = "Bianca", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "UNO-0028", Name = "Icirrus City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "UNO-0029",
            Ost = "UNO-0027",
            Verbindungen = new() { "UNO-0029", "UNO-0027" },
            Trainer = new() {
                new() {
                    Id = "UNO-K-016", Name = "Brycen", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Eis-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "UNO-GYM-007", Name = "Brycen", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Brycen", OrdenName = "Eisorden",
                OrdenNr = 7, TypSpezialisierung = "Eis",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 45 } },
            },
        },
        new() {
            Id = "UNO-0029", Name = "Dragonspiral Tower", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "UNO-0028",
            Verbindungen = new() { "UNO-0028" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0621", MinLevel = 35, MaxLevel = 20, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0621", MinLevel = 35, MaxLevel = 40, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0635", MinLevel = 40, MaxLevel = 46, Chance = 5 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "UNO-T-011", Name = "Team Plasma N", Klasse = "Boss",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "UNO-0030", Name = "Route 8", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "UNO-0031",
            Ost = "UNO-0028",
            Verbindungen = new() { "UNO-0031", "UNO-0028" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0052", MinLevel = 18, MaxLevel = 22, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0072", MinLevel = 25, MaxLevel = 30, Chance = 60 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 15, MaxLevel = 17, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0819", MinLevel = 30, MaxLevel = 34, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-NPC-1013", Name = "Gambler", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "UNO-0031", Name = "Tubeline Bridge", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0019",
            Ost = "KAL-0016",
            Verbindungen = new() { "KAL-0019", "KAL-0016" },
        },
        new() {
            Id = "UNO-0032", Name = "Route 9", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "UNO-0017",
            West = "UNO-0033",
            Ost = "UNO-0031",
            Verbindungen = new() { "UNO-0017", "UNO-0033", "UNO-0031" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0016", MinLevel = 18, MaxLevel = 22, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 26, MaxLevel = 30, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 17, MaxLevel = 19, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0072", MinLevel = 34, MaxLevel = 38, Chance = 60 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-NPC-1011", Name = "Hiker", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-T-007", Name = "Hiker", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "UNO-0033", Name = "Opelucid City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0021",
            Süd = "KAL-0019",
            Verbindungen = new() { "KAL-0021", "KAL-0019" },
            Trainer = new() {
                new() {
                    Id = "UNO-K-017", Name = "Drayden (Schwarz) / Iris (Weiß)", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Drachen-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "UNO-GYM-008S", Name = "Drayden", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "UNO-GYM-008W", Name = "Iris", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Drayden", OrdenName = "Legendenorden",
                OrdenNr = 8, TypSpezialisierung = "Drache",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 50 } },
            },
        },
        new() {
            Id = "UNO-0034", Name = "Route 10", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "UNO-0035",
            Ost = "UNO-0033",
            Verbindungen = new() { "UNO-0035", "UNO-0033" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0081", MinLevel = 22, MaxLevel = 25, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 28, MaxLevel = 32, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 19, MaxLevel = 21, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0819", MinLevel = 38, MaxLevel = 42, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAL-T-006", Name = "Tierno", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "UNO-0035", Name = "Victory Road (Unova)", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0021",
            Ost = "UNO-0036",
            Verbindungen = new() { "KAL-0021", "UNO-0036" },
        },
        new() {
            Id = "UNO-0036", Name = "Pokémon League (Unova)", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "UNO-0035",
            Verbindungen = new() { "UNO-0035" },
            Trainer = new() {
                new() {
                    Id = "UNO-K-020", Name = "Shauntal", Klasse = "Top4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Geist-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "UNO-K-021", Name = "Marshal", Klasse = "Top4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Kampf-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "UNO-K-022", Name = "Grimsley", Klasse = "Top4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Unlicht-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "UNO-K-023", Name = "Caitlin", Klasse = "Top4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Psycho-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "UNO-K-024", Name = "Alder", Klasse = "Champion",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Gemischte Typen", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "UNO-K-025", Name = "Iris", Klasse = "Champion",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Drachen-Typ; Iris aus BW", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "UNO-0037", Name = "N's Castle", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "UNO-0036",
            Verbindungen = new() { "UNO-0036" },
            Trainer = new() {
                new() {
                    Id = "UNO-K-030", Name = "N", Klasse = "Team Plasma",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "N mit Legendärem", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0644", Level = 50 }, new() { MonsterId = "PKM-0643", Level = 50 } },
                },
                new() {
                    Id = "UNO-K-031", Name = "Ghetsis", Klasse = "Team Plasma",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Ghetsis finaler Kampf", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "UNO-0038", Name = "Aspertia City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0039",
            Verbindungen = new() { "KAL-0039" },
            Trainer = new() {
                new() {
                    Id = "UNO-T-014", Name = "Hugh", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Cheren", OrdenName = "Trilobitorden",
                OrdenNr = 1, TypSpezialisierung = "Normal",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 15 } },
            },
        },
        new() {
            Id = "UNO-0039", Name = "Virbank City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "KAL-0041",
            Verbindungen = new() { "KAL-0041" },
            Trainer = new() {
                new() {
                    Id = "UNO-T-015", Name = "Team Plasma Zinzolin", Klasse = "Boss",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Roxie", OrdenName = "Giforden",
                OrdenNr = 2, TypSpezialisierung = "Gift",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 20 } },
            },
        },
        new() {
            Id = "UNO-0040", Name = "Pokéstar Studios", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "UNO-0039",
            Verbindungen = new() { "UNO-0039" },
        },
        new() {
            Id = "UNO-0041", Name = "Floccesy Town", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAN-0004",
            Süd = "KAL-0039",
            Verbindungen = new() { "KAN-0004", "KAL-0039" },
        },
        new() {
            Id = "UNO-0042", Name = "Humilau City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "KAN-0004",
            Verbindungen = new() { "KAN-0004" },
            Trainer = new() {
                new() {
                    Id = "UNO-T-017", Name = "Hugh", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "UNO-0043", Name = "Join Avenue", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "UNO-0017",
            Verbindungen = new() { "UNO-0017" },
        },
        new() {
            Id = "UNO-0044", Name = "PWT (Pokemon World Tournament)", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "UNO-0020",
            Verbindungen = new() { "UNO-0020" },
        },
        new() {
            Id = "UNO-0045", Name = "Black City / White Forest", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "KAL-0031",
            Verbindungen = new() { "KAL-0031" },
        },
        new() {
            Id = "UNO-0046", Name = "Giant Chasm", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0028",
            Verbindungen = new() { "KAL-0028" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0646", MinLevel = 75, MaxLevel = 1, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0459", MinLevel = 40, MaxLevel = 46, Chance = 20 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "UNO-T-018", Name = "Team Plasma Ghetsis", Klasse = "Boss",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "UNO-0047", Name = "Abyssal Ruins", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0028",
            Verbindungen = new() { "KAL-0028" },
        },
        new() {
            Id = "UNO-0048", Name = "Reversal Mountain", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0019",
            Verbindungen = new() { "KAL-0019" },
        },
        new() {
            Id = "UNO-0049", Name = "Lostlorn Forest", Typ = "ort",
            Farbe = "forest", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0033",
            Verbindungen = new() { "KAL-0033" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0571", MinLevel = 30, MaxLevel = 5, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "UNO-0050", Name = "Liberty Garden", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "UNO-0012",
            Verbindungen = new() { "UNO-0012" },
        },
        new() {
            Id = "UNO-0051", Name = "Mistralton Cave", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0012",
            Verbindungen = new() { "KAL-0012" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0638", MinLevel = 42, MaxLevel = 1, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "UNO-0052", Name = "Moor of Icirrus", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0016",
            Verbindungen = new() { "KAL-0016" },
        },
        new() {
            Id = "UNO-0053", Name = "Undella Town", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0029",
            Süd = "KAL-0028",
            Verbindungen = new() { "KAL-0029", "KAL-0028" },
        },
        new() {
            Id = "UNO-0054", Name = "Village Bridge", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0023",
            Ost = "KAL-0026",
            Verbindungen = new() { "KAL-0023", "KAL-0026" },
        },
        // === Kalos (53 Orte) ===
        new() {
            Id = "KAL-0001", Name = "Vaniville Town", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0002",
            Verbindungen = new() { "KAL-0002" },
        },
        new() {
            Id = "KAL-0002", Name = "Route 1", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0003",
            Süd = "KAL-0001",
            Verbindungen = new() { "KAL-0003", "KAL-0001" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0016", MinLevel = 2, MaxLevel = 4, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0019", MinLevel = 2, MaxLevel = 4, Chance = 45 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 4, MaxLevel = 55, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0504", MinLevel = 4, MaxLevel = 45, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0016", MinLevel = 2, MaxLevel = 4, Chance = 45 }, // Unbekannt
                new() { MonsterId = "PKM-0019", MinLevel = 2, MaxLevel = 4, Chance = 45 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 2, MaxLevel = 4, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0509", MinLevel = 2, MaxLevel = 4, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "UNO-K-001", Name = "Bianca", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Bianca auf Route 1", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0495", Level = 5 }, new() { MonsterId = "PKM-0498", Level = 5 }, new() { MonsterId = "PKM-0501", Level = 5 } },
                },
                new() {
                    Id = "UNO-T-001", Name = "Youngster", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "ALO-T-001", Name = "Hau", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAL-0003", Name = "Aquacorde Town", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0004",
            Süd = "KAL-0002",
            Verbindungen = new() { "KAL-0004", "KAL-0002" },
        },
        new() {
            Id = "KAL-0004", Name = "Route 2", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0005",
            Süd = "KAL-0003",
            Verbindungen = new() { "KAL-0005", "KAL-0003" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0016", MinLevel = 3, MaxLevel = 5, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0019", MinLevel = 3, MaxLevel = 5, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0052", MinLevel = 3, MaxLevel = 5, Chance = 5 }, // Unbekannt
                new() { MonsterId = "PKM-0056", MinLevel = 3, MaxLevel = 5, Chance = 5 }, // Unbekannt
                new() { MonsterId = "PKM-0016", MinLevel = 3, MaxLevel = 5, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0019", MinLevel = 3, MaxLevel = 5, Chance = 35 }, // Unbekannt
                new() { MonsterId = "PKM-0021", MinLevel = 3, MaxLevel = 5, Chance = 10 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 4, MaxLevel = 6, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "UNO-T-002", Name = "Cheren", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAL-T-001", Name = "Youngster", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "GAL-T-001", Name = "Hop", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "ALO-OPT-001", Name = "Yuka", Klasse = "Pokémon-Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Früher Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAL-0005", Name = "Santalune Forest", Typ = "ort",
            Farbe = "forest", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0004",
            Ost = "KAL-0006",
            Verbindungen = new() { "KAL-0004", "KAL-0006" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0010", MinLevel = 5, MaxLevel = 30, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0025", MinLevel = 5, MaxLevel = 15, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 5, MaxLevel = 30, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAL-T-004", Name = "Bug Catcher", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAL-0006", Name = "Route 3", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0007",
            Ost = "KAL-0005",
            Verbindungen = new() { "KAL-0007", "KAL-0005" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0016", MinLevel = 6, MaxLevel = 8, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0021", MinLevel = 6, MaxLevel = 8, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0039", MinLevel = 6, MaxLevel = 8, Chance = 5 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 6, MaxLevel = 8, Chance = 20 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-NPC-1000", Name = "Youngster Jan", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-NPC-1001", Name = "Lass Anna", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-T-001", Name = "Lass", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-T-002", Name = "Youngster", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAL-T-002", Name = "Shauna", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "OPT-KAN-001", Name = "Ben", Klasse = "Junge",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Vor Mt. Moon", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAL-0007", Name = "Santalune City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0008",
            Ost = "KAL-0006",
            Verbindungen = new() { "KAL-0008", "KAL-0006" },
            Trainer = new() {
                new() {
                    Id = "KAL-K-010", Name = "Viola", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Käfer-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAL-GYM-001", Name = "Viola", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Viola", OrdenName = "Käferorden",
                OrdenNr = 1, TypSpezialisierung = "Käfer",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 15 } },
            },
        },
        new() {
            Id = "KAL-0008", Name = "Route 4", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0009",
            Ost = "KAL-0007",
            Verbindungen = new() { "KAL-0009", "KAL-0007" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0021", MinLevel = 8, MaxLevel = 12, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0027", MinLevel = 8, MaxLevel = 12, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0023", MinLevel = 8, MaxLevel = 12, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0058", MinLevel = 8, MaxLevel = 12, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0551", MinLevel = 16, MaxLevel = 20, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 7, MaxLevel = 9, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0819", MinLevel = 8, MaxLevel = 12, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "UNO-T-006", Name = "Bianca", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAL-T-003", Name = "Lass", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAL-0009", Name = "Lumiose City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0029",
            Süd = "KAL-0010",
            West = "KAL-0028",
            Ost = "KAL-0008",
            Verbindungen = new() { "KAL-0029", "KAL-0010", "KAL-0028", "KAL-0008" },
            Trainer = new() {
                new() {
                    Id = "KAL-K-014", Name = "Clemont", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Elektro-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAL-GYM-005", Name = "Clemont", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Clemont", OrdenName = "Voltaorden",
                OrdenNr = 5, TypSpezialisierung = "Elektro",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 35 } },
            },
        },
        new() {
            Id = "KAL-0010", Name = "Route 5", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0011",
            Ost = "KAL-0009",
            Verbindungen = new() { "KAL-0011", "KAL-0009" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0043", MinLevel = 13, MaxLevel = 15, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0060", MinLevel = 13, MaxLevel = 15, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 16, MaxLevel = 20, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 9, MaxLevel = 11, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0819", MinLevel = 16, MaxLevel = 20, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAL-K-001", Name = "Serena/Calem", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Rivale auf Route 5", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0650", Level = 12 }, new() { MonsterId = "PKM-0653", Level = 12 }, new() { MonsterId = "PKM-0656", Level = 12 } },
                },
                new() {
                    Id = "GAL-OPT-001", Name = "Adrian", Klasse = "Pokémon Breeder",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Früher Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAL-0011", Name = "Camphrier Town", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0012",
            Ost = "KAL-0010",
            Verbindungen = new() { "KAL-0012", "KAL-0010" },
        },
        new() {
            Id = "KAL-0012", Name = "Route 6", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0013",
            Ost = "KAL-0011",
            Verbindungen = new() { "KAL-0013", "KAL-0011" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0043", MinLevel = 13, MaxLevel = 15, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0060", MinLevel = 13, MaxLevel = 15, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0585", MinLevel = 20, MaxLevel = 24, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 11, MaxLevel = 13, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0819", MinLevel = 22, MaxLevel = 26, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-NPC-1009", Name = "Jr. Trainerin", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAL-0013", Name = "Parfum Palace", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0012",
            Verbindungen = new() { "KAL-0012" },
        },
        new() {
            Id = "KAL-0014", Name = "Route 7", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0015",
            Ost = "KAL-0012",
            Verbindungen = new() { "KAL-0015", "KAL-0012" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0052", MinLevel = 18, MaxLevel = 22, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0056", MinLevel = 18, MaxLevel = 22, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 22, MaxLevel = 26, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 13, MaxLevel = 15, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0819", MinLevel = 26, MaxLevel = 30, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAL-OPT-001", Name = "Brigitte", Klasse = "Pokémon Breeder",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Frühe Route", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAL-0015", Name = "Connecting Cave", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0014",
            Ost = "KAL-0016",
            Verbindungen = new() { "KAL-0014", "KAL-0016" },
        },
        new() {
            Id = "KAL-0016", Name = "Route 8", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0017",
            Ost = "KAL-0015",
            Verbindungen = new() { "KAL-0017", "KAL-0015" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0052", MinLevel = 18, MaxLevel = 22, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0072", MinLevel = 25, MaxLevel = 30, Chance = 60 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 15, MaxLevel = 17, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0819", MinLevel = 30, MaxLevel = 34, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-NPC-1013", Name = "Gambler", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAL-0017", Name = "Ambrette Town", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0019",
            Ost = "KAL-0016",
            Verbindungen = new() { "KAL-0019", "KAL-0016" },
        },
        new() {
            Id = "KAL-0018", Name = "Glittering Cave", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0019",
            Verbindungen = new() { "KAL-0019" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0696", MinLevel = 18, MaxLevel = 20, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0698", MinLevel = 18, MaxLevel = 20, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAL-T-005", Name = "Team Flare", Klasse = "Boss",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAL-0019", Name = "Route 9", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0018",
            Ost = "KAL-0017",
            Verbindungen = new() { "KAL-0018", "KAL-0017" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0016", MinLevel = 18, MaxLevel = 22, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 26, MaxLevel = 30, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 17, MaxLevel = 19, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0072", MinLevel = 34, MaxLevel = 38, Chance = 60 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-NPC-1011", Name = "Hiker", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-T-007", Name = "Hiker", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAL-0020", Name = "Cyllage City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0021",
            Ost = "KAL-0019",
            Verbindungen = new() { "KAL-0021", "KAL-0019" },
            Trainer = new() {
                new() {
                    Id = "KAL-K-011", Name = "Grant", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Gestein/Eis-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAL-GYM-002", Name = "Grant", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Grant", OrdenName = "Klippenorden",
                OrdenNr = 2, TypSpezialisierung = "Gestein",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 20 } },
            },
        },
        new() {
            Id = "KAL-0021", Name = "Route 10", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0022",
            Ost = "KAL-0020",
            Verbindungen = new() { "KAL-0022", "KAL-0020" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0081", MinLevel = 22, MaxLevel = 25, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 28, MaxLevel = 32, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 19, MaxLevel = 21, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0819", MinLevel = 38, MaxLevel = 42, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAL-T-006", Name = "Tierno", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAL-0022", Name = "Geosenge Town", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0023",
            Ost = "KAL-0021",
            Verbindungen = new() { "KAL-0023", "KAL-0021" },
            Trainer = new() {
                new() {
                    Id = "KAL-T-007", Name = "Team Flare Lysandre", Klasse = "Boss",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAL-0023", Name = "Route 11", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0024",
            Ost = "KAL-0022",
            Verbindungen = new() { "KAL-0024", "KAL-0022" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0023", MinLevel = 15, MaxLevel = 20, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 28, MaxLevel = 32, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 21, MaxLevel = 23, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-NPC-1010", Name = "Gentleman Dirk", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAL-0024", Name = "Shalour City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0026",
            Ost = "KAL-0023",
            Verbindungen = new() { "KAL-0026", "KAL-0023" },
            Trainer = new() {
                new() {
                    Id = "KAL-K-002", Name = "Serena/Calem", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Rivale in Shalour", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0651", Level = 26 }, new() { MonsterId = "PKM-0654", Level = 26 } },
                },
                new() {
                    Id = "KAL-K-012", Name = "Korrina", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Kampf-Typ; Mega-Lucario", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAL-GYM-003", Name = "Korrina", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Korrina", OrdenName = "Rumpelorden",
                OrdenNr = 3, TypSpezialisierung = "Kampf",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 25 } },
            },
        },
        new() {
            Id = "KAL-0025", Name = "Tower of Mastery", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "KAL-0024",
            Verbindungen = new() { "KAL-0024" },
        },
        new() {
            Id = "KAL-0026", Name = "Route 12", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0048",
            Ost = "KAL-0024",
            Verbindungen = new() { "KAL-0048", "KAL-0024" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0069", MinLevel = 15, MaxLevel = 20, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0016", MinLevel = 18, MaxLevel = 22, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0019", MinLevel = 18, MaxLevel = 22, Chance = 35 }, // Unbekannt
                new() { MonsterId = "PKM-0585", MinLevel = 28, MaxLevel = 32, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0072", MinLevel = 22, MaxLevel = 26, Chance = 60 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-T-013", Name = "Fisher", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAL-OPT-002", Name = "Hugo", Klasse = "Sky Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Sky Battle", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAL-0027", Name = "Coumarine City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0028",
            Ost = "KAL-0026",
            Verbindungen = new() { "KAL-0028", "KAL-0026" },
            Trainer = new() {
                new() {
                    Id = "KAL-K-003", Name = "Serena/Calem", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Rivale in Coumarine", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0652", Level = 32 }, new() { MonsterId = "PKM-0655", Level = 32 } },
                },
                new() {
                    Id = "KAL-K-013", Name = "Ramos", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Pflanz-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAL-GYM-004", Name = "Ramos", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Ramos", OrdenName = "Pflanzenorden",
                OrdenNr = 4, TypSpezialisierung = "Pflanze",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 30 } },
            },
        },
        new() {
            Id = "KAL-0028", Name = "Route 13", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0009",
            Ost = "KAL-0027",
            Verbindungen = new() { "KAL-0009", "KAL-0027" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0016", MinLevel = 18, MaxLevel = 22, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0072", MinLevel = 30, MaxLevel = 35, Chance = 60 }, // Unbekannt
                new() { MonsterId = "PKM-0551", MinLevel = 25, MaxLevel = 30, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-NPC-1018", Name = "Bird Keeper", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-T-014", Name = "Bird Keeper", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAL-0029", Name = "Route 14", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0030",
            Ost = "KAL-0009",
            Verbindungen = new() { "KAL-0030", "KAL-0009" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0016", MinLevel = 20, MaxLevel = 25, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0704", MinLevel = 25, MaxLevel = 15, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 30, MaxLevel = 35, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 25, MaxLevel = 30, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-NPC-1019", Name = "Bird Keeper", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAN-T-015", Name = "Biker", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAL-0030", Name = "Laverre City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0031",
            Ost = "KAL-0029",
            Verbindungen = new() { "KAL-0031", "KAL-0029" },
            Trainer = new() {
                new() {
                    Id = "KAL-K-015", Name = "Valerie", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Fee-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAL-GYM-006", Name = "Valerie", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Valerie", OrdenName = "Feenorden",
                OrdenNr = 6, TypSpezialisierung = "Fee",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 40 } },
            },
        },
        new() {
            Id = "KAL-0031", Name = "Route 15", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0032",
            Ost = "KAL-0030",
            Verbindungen = new() { "KAL-0032", "KAL-0030" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0016", MinLevel = 22, MaxLevel = 28, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 30, MaxLevel = 35, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 28, MaxLevel = 32, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-NPC-1020", Name = "Jr. Trainer", Klasse = "NPC-Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Zusätzlicher Route-Trainer", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAL-0032", Name = "Dendemille Town", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0033",
            Ost = "KAL-0031",
            Verbindungen = new() { "KAL-0033", "KAL-0031" },
        },
        new() {
            Id = "KAL-0033", Name = "Route 16", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0034",
            Ost = "KAL-0032",
            Verbindungen = new() { "KAL-0034", "KAL-0032" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0019", MinLevel = 20, MaxLevel = 25, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 32, MaxLevel = 36, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 28, MaxLevel = 32, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-T-016", Name = "Biker", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAL-0034", Name = "Frost Cavern", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0033",
            Verbindungen = new() { "KAL-0033" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0460", MinLevel = 35, MaxLevel = 20, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0459", MinLevel = 35, MaxLevel = 40, Chance = 30 }, // Unbekannt
            },
        },
        new() {
            Id = "KAL-0035", Name = "Anistar City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0036",
            Ost = "KAL-0033",
            Verbindungen = new() { "KAL-0036", "KAL-0033" },
            Trainer = new() {
                new() {
                    Id = "KAL-K-016", Name = "Olympia", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Psycho-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAL-GYM-007", Name = "Olympia", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Olympia", OrdenName = "Psychoorden",
                OrdenNr = 7, TypSpezialisierung = "Psycho",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 45 } },
            },
        },
        new() {
            Id = "KAL-0036", Name = "Route 17", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0035",
            Ost = "KAL-0037",
            Verbindungen = new() { "KAL-0035", "KAL-0037" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0019", MinLevel = 22, MaxLevel = 28, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0072", MinLevel = 35, MaxLevel = 40, Chance = 60 }, // Unbekannt
                new() { MonsterId = "PKM-0459", MinLevel = 30, MaxLevel = 35, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-T-017", Name = "Biker", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAL-0037", Name = "Route 18", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0038",
            Ost = "KAL-0036",
            Verbindungen = new() { "KAL-0038", "KAL-0036" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0019", MinLevel = 22, MaxLevel = 28, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 35, MaxLevel = 40, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 30, MaxLevel = 35, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-T-018", Name = "Bird Keeper", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAL-0038", Name = "Couriway Town", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0039",
            Ost = "KAL-0037",
            Verbindungen = new() { "KAL-0039", "KAL-0037" },
        },
        new() {
            Id = "KAL-0039", Name = "Route 19", Typ = "ort",
            Farbe = "forest", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0042",
            Ost = "KAL-0038",
            Verbindungen = new() { "KAL-0042", "KAL-0038" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0072", MinLevel = 20, MaxLevel = 25, Chance = 60 }, // Unbekannt
                new() { MonsterId = "PKM-0506", MinLevel = 2, MaxLevel = 4, Chance = 30 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 32, MaxLevel = 36, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-T-019", Name = "Swimmer", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAL-0040", Name = "Pokémon Village", Typ = "ort",
            Farbe = "forest", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0041",
            Verbindungen = new() { "KAL-0041" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0571", MinLevel = 35, MaxLevel = 5, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0352", MinLevel = 38, MaxLevel = 42, Chance = 20 }, // Unbekannt
            },
        },
        new() {
            Id = "KAL-0041", Name = "Route 20", Typ = "ort",
            Farbe = "forest", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0040",
            Ost = "KAL-0042",
            Verbindungen = new() { "KAL-0040", "KAL-0042" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0072", MinLevel = 20, MaxLevel = 25, Chance = 60 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 32, MaxLevel = 36, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-T-020", Name = "Swimmer", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAL-0042", Name = "Snowbelle City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0043",
            Ost = "KAL-0041",
            Verbindungen = new() { "KAL-0043", "KAL-0041" },
            Trainer = new() {
                new() {
                    Id = "KAL-K-017", Name = "Wulfric", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Eis-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAL-GYM-008", Name = "Wulfric", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Wulfric", OrdenName = "Eisorden",
                OrdenNr = 8, TypSpezialisierung = "Eis",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 50 } },
            },
        },
        new() {
            Id = "KAL-0043", Name = "Route 21", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0044",
            Ost = "KAL-0042",
            Verbindungen = new() { "KAL-0044", "KAL-0042" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0069", MinLevel = 25, MaxLevel = 30, Chance = 20 }, // Unbekannt
                new() { MonsterId = "PKM-0072", MinLevel = 20, MaxLevel = 25, Chance = 60 }, // Unbekannt
                new() { MonsterId = "PKM-0661", MinLevel = 35, MaxLevel = 40, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAN-T-021", Name = "Swimmer", Klasse = "Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAL-0044", Name = "Victory Road (Kalos)", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0043",
            Ost = "KAL-0045",
            Verbindungen = new() { "KAL-0043", "KAL-0045" },
        },
        new() {
            Id = "KAL-0045", Name = "Pokémon League (Kalos)", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "KAL-0044",
            Verbindungen = new() { "KAL-0044" },
            Trainer = new() {
                new() {
                    Id = "KAL-K-020", Name = "Malva", Klasse = "Top4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Feuer-Typ; Team Flare Mitglied", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAL-K-021", Name = "Siebold", Klasse = "Top4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Wasser-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAL-K-022", Name = "Wikstrom", Klasse = "Top4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Stahl-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAL-K-023", Name = "Drasna", Klasse = "Top4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Drachen-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "KAL-K-024", Name = "Diantha", Klasse = "Champion",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Fee/Psycho-Typ; Mega-Gardevoir", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAL-0046", Name = "Team Flare Secret HQ", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "KAL-0022",
            Verbindungen = new() { "KAL-0022" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0716", MinLevel = 50, MaxLevel = 1, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0717", MinLevel = 50, MaxLevel = 1, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "KAL-K-031", Name = "Lysandre (final)", Klasse = "Team Flare",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Lysandre letzter Kampf", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0717", Level = 49 } },
                },
            },
        },
        new() {
            Id = "KAL-0047", Name = "Terminus Cave", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0037",
            Verbindungen = new() { "KAL-0037" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0718", MinLevel = 70, MaxLevel = 1, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "KAL-0048", Name = "Azure Bay", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0026",
            Verbindungen = new() { "KAL-0026" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0072", MinLevel = 30, MaxLevel = 35, Chance = 60 }, // Unbekannt
            },
        },
        new() {
            Id = "KAL-0049", Name = "Reflection Cave", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0023",
            Ost = "KAL-0024",
            Verbindungen = new() { "KAL-0023", "KAL-0024" },
        },
        new() {
            Id = "KAL-0050", Name = "Kalos Power Plant", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0028",
            Verbindungen = new() { "KAL-0028" },
        },
        new() {
            Id = "KAL-0051", Name = "Sea Spirit's Den", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0048",
            Verbindungen = new() { "KAL-0048" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0249", MinLevel = 70, MaxLevel = 1, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "KAL-0052", Name = "Kiloude City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Trainer = new() {
                new() {
                    Id = "KAL-OPT-005", Name = "Calem/Serena", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Postgame Kampf", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "KAL-0053", Name = "Battle Maison", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "KAL-0052",
            Verbindungen = new() { "KAL-0052" },
            Trainer = new() {
                new() {
                    Id = "KAL-OPT-004", Name = "Nita", Klasse = "Battle Chatelaine",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Postgame", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        // === Alola (44 Orte) ===
        new() {
            Id = "ALO-0001", Name = "Iki Town", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Trainer = new() {
                new() {
                    Id = "ALO-K-001", Name = "Hau", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Hau erster Kampf", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0722", Level = 8 }, new() { MonsterId = "PKM-0725", Level = 8 }, new() { MonsterId = "PKM-0728", Level = 8 } },
                },
                new() {
                    Id = "ALO-K-020", Name = "Hala", Klasse = "Kahuna",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Kampf-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "ALO-KAH-001", Name = "Hala", Klasse = "Kahuna",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "ALO-T-002", Name = "Hala", Klasse = "Kahuna",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "ALO-0002", Name = "Route 1 (Melemele)", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "ALO-0001",
            Verbindungen = new() { "ALO-0001" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0722", MinLevel = 5, MaxLevel = 1, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0742", MinLevel = 5, MaxLevel = 30, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "ALO-0003", Name = "Hau'oli City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0004",
            Verbindungen = new() { "KAL-0004" },
        },
        new() {
            Id = "ALO-0004", Name = "Route 2 (Melemele)", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Ost = "ALO-0003",
            Verbindungen = new() { "ALO-0003" },
        },
        new() {
            Id = "ALO-0005", Name = "Verdant Cavern", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0004",
            Verbindungen = new() { "KAL-0004" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0734", MinLevel = 10, MaxLevel = 30, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0019", MinLevel = 10, MaxLevel = 30, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0744", MinLevel = 8, MaxLevel = 12, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "ALO-K-010", Name = "Ilima (Totem: Gumshoos/Raticate)", Klasse = "Prüfung",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Normal-Typ; Totem-Pokémon", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0735", Level = 12 }, new() { MonsterId = "PKM-0020", Level = 12 } },
                },
                new() {
                    Id = "ALO-T-003", Name = "Totem Gumshoos/Raticate", Klasse = "Prüfung",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Ilima", OrdenName = "Normalprüfung",
                OrdenNr = 1, TypSpezialisierung = "Normal",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 15 } },
            },
        },
        new() {
            Id = "ALO-0006", Name = "Melemele Sea", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0002",
            Verbindungen = new() { "KAL-0002" },
        },
        new() {
            Id = "ALO-0007", Name = "Ten Carat Hill", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
        },
        new() {
            Id = "ALO-0008", Name = "Heahea City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0008",
            Süd = "KAL-0006",
            Verbindungen = new() { "KAL-0008", "KAL-0006" },
        },
        new() {
            Id = "ALO-0009", Name = "Route 4 (Akala)", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "ALO-0010",
            Ost = "ALO-0008",
            Verbindungen = new() { "ALO-0010", "ALO-0008" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0744", MinLevel = 12, MaxLevel = 16, Chance = 20 }, // Unbekannt
            },
        },
        new() {
            Id = "ALO-0010", Name = "Paniola Town", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0010",
            Süd = "KAL-0008",
            Verbindungen = new() { "KAL-0010", "KAL-0008" },
        },
        new() {
            Id = "ALO-0011", Name = "Route 5 (Akala)", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "ALO-0012",
            Ost = "ALO-0010",
            Verbindungen = new() { "ALO-0012", "ALO-0010" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0744", MinLevel = 14, MaxLevel = 18, Chance = 20 }, // Unbekannt
            },
        },
        new() {
            Id = "ALO-0012", Name = "Brooklet Hill", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0010",
            Verbindungen = new() { "KAL-0010" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0746", MinLevel = 18, MaxLevel = 30, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0746", MinLevel = 16, MaxLevel = 20, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "ALO-K-011", Name = "Lana (Totem: Wishiwashi)", Klasse = "Prüfung",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Wasser-Typ; Totem-Pokémon", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0746", Level = 20 } },
                },
                new() {
                    Id = "ALO-T-004", Name = "Totem Wishiwashi", Klasse = "Prüfung",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0746", Level = 20 } },
                },
            },
            Arena = new() {
                Leiter = "Lana", OrdenName = "Wasserprüfung",
                OrdenNr = 2, TypSpezialisierung = "Wasser",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 20 } },
            },
        },
        new() {
            Id = "ALO-0013", Name = "Route 6 (Akala)", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "ALO-0014",
            Ost = "ALO-0012",
            Verbindungen = new() { "ALO-0014", "ALO-0012" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0744", MinLevel = 16, MaxLevel = 20, Chance = 20 }, // Unbekannt
            },
        },
        new() {
            Id = "ALO-0014", Name = "Royal Avenue", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0014",
            Ost = "KAL-0012",
            Verbindungen = new() { "KAL-0014", "KAL-0012" },
        },
        new() {
            Id = "ALO-0015", Name = "Wela Volcano Park", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0014",
            Verbindungen = new() { "KAL-0014" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0058", MinLevel = 20, MaxLevel = 30, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0757", MinLevel = 20, MaxLevel = 24, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "ALO-K-012", Name = "Kiawe (Totem: Marowak)", Klasse = "Prüfung",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Feuer-Typ; Totem-Pokémon", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0105", Level = 22 } },
                },
                new() {
                    Id = "ALO-T-005", Name = "Totem Salazzle", Klasse = "Prüfung",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0758", Level = 22 } },
                },
            },
            Arena = new() {
                Leiter = "Kiawe", OrdenName = "Feuerprüfung",
                OrdenNr = 3, TypSpezialisierung = "Feuer",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 25 } },
            },
        },
        new() {
            Id = "ALO-0016", Name = "Route 8 (Akala)", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "ALO-0017",
            Ost = "KAL-0014",
            Verbindungen = new() { "ALO-0017", "KAL-0014" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0744", MinLevel = 20, MaxLevel = 24, Chance = 20 }, // Unbekannt
            },
        },
        new() {
            Id = "ALO-0017", Name = "Lush Jungle", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0016",
            Verbindungen = new() { "KAL-0016" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0753", MinLevel = 23, MaxLevel = 20, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0753", MinLevel = 20, MaxLevel = 24, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "ALO-K-013", Name = "Mallow (Totem: Lurantis)", Klasse = "Prüfung",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Pflanz-Typ; Totem-Pokémon", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0754", Level = 24 } },
                },
                new() {
                    Id = "ALO-T-006", Name = "Totem Lurantis", Klasse = "Prüfung",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0754", Level = 24 } },
                },
            },
            Arena = new() {
                Leiter = "Mallow", OrdenName = "Pflanzenprüfung",
                OrdenNr = 4, TypSpezialisierung = "Pflanze",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 30 } },
            },
        },
        new() {
            Id = "ALO-0018", Name = "Diglett's Tunnel", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0016",
            Ost = "KAL-0019",
            Verbindungen = new() { "KAL-0016", "KAL-0019" },
        },
        new() {
            Id = "ALO-0019", Name = "Konikoni City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0019",
            Ost = "ALO-0018",
            Verbindungen = new() { "KAL-0019", "ALO-0018" },
            Trainer = new() {
                new() {
                    Id = "ALO-K-021", Name = "Olivia", Klasse = "Kahuna",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Gestein-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "ALO-KAH-002", Name = "Olivia", Klasse = "Kahuna",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "ALO-0020", Name = "Akala Outskirts", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "ALO-0019",
            Verbindungen = new() { "ALO-0019" },
            Trainer = new() {
                new() {
                    Id = "ALO-OPT-002", Name = "Keoni", Klasse = "Ace Trainer",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Optional", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "ALO-0021", Name = "Ruins of Life", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "ALO-0020",
            Verbindungen = new() { "ALO-0020" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0786", MinLevel = 60, MaxLevel = 1, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "ALO-0022", Name = "Malie City", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0021",
            Süd = "KAL-0019",
            Verbindungen = new() { "KAL-0021", "KAL-0019" },
            Trainer = new() {
                new() {
                    Id = "ALO-KAH-003", Name = "Nanu", Klasse = "Kahuna",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "ALO-0023", Name = "Route 10 (Ula'ula)", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "ALO-0024",
            Ost = "ALO-0022",
            Verbindungen = new() { "ALO-0024", "ALO-0022" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0744", MinLevel = 24, MaxLevel = 28, Chance = 20 }, // Unbekannt
            },
        },
        new() {
            Id = "ALO-0024", Name = "Mount Hokulani", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0021",
            Verbindungen = new() { "KAL-0021" },
            Trainer = new() {
                new() {
                    Id = "ALO-K-014", Name = "Sophocles (Totem: Vikavolt)", Klasse = "Prüfung",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Elektro/Stahl-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0738", Level = 29 } },
                },
            },
            Arena = new() {
                Leiter = "Sophocles", OrdenName = "Eisprüfung",
                OrdenNr = 6, TypSpezialisierung = "Elektro",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 40 } },
            },
        },
        new() {
            Id = "ALO-0025", Name = "Route 11 (Ula'ula)", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "ALO-0026",
            Ost = "ALO-0022",
            Verbindungen = new() { "ALO-0026", "ALO-0022" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0744", MinLevel = 26, MaxLevel = 30, Chance = 20 }, // Unbekannt
            },
        },
        new() {
            Id = "ALO-0026", Name = "Tapu Village", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0026",
            Ost = "KAL-0023",
            Verbindungen = new() { "KAL-0026", "KAL-0023" },
        },
        new() {
            Id = "ALO-0027", Name = "Route 12 (Ula'ula)", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0028",
            Ost = "ALO-0026",
            Verbindungen = new() { "KAL-0028", "ALO-0026" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0551", MinLevel = 26, MaxLevel = 30, Chance = 30 }, // Unbekannt
            },
        },
        new() {
            Id = "ALO-0028", Name = "Route 13 (Ula'ula)", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "ALO-0029",
            Ost = "KAL-0026",
            Verbindungen = new() { "ALO-0029", "KAL-0026" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0551", MinLevel = 28, MaxLevel = 32, Chance = 30 }, // Unbekannt
            },
        },
        new() {
            Id = "ALO-0029", Name = "Haina Desert", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0028",
            Verbindungen = new() { "KAL-0028" },
        },
        new() {
            Id = "ALO-0030", Name = "Po Town", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0036",
            Verbindungen = new() { "KAL-0036" },
            Trainer = new() {
                new() {
                    Id = "ALO-K-022", Name = "Nanu", Klasse = "Kahuna",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Unlicht-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "ALO-0031", Name = "Aether Paradise", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Trainer = new() {
                new() {
                    Id = "ALO-K-040", Name = "Lusamine", Klasse = "Aether Foundation",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Lusamine erster Kampf", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "ALO-T-008", Name = "Lusamine", Klasse = "Boss",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "ALO-0032", Name = "Ula'ula Meadow", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0033",
            Verbindungen = new() { "KAL-0033" },
            Trainer = new() {
                new() {
                    Id = "ALO-K-015", Name = "Acerola (Totem: Mimikyu)", Klasse = "Prüfung",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Geist-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0778", Level = 35 } },
                },
            },
        },
        new() {
            Id = "ALO-0033", Name = "Seafolk Village", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "ALO-0034",
            Verbindungen = new() { "ALO-0034" },
        },
        new() {
            Id = "ALO-0034", Name = "Poni Wilds", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "ALO-0035",
            Ost = "ALO-0033",
            Verbindungen = new() { "ALO-0035", "ALO-0033" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0744", MinLevel = 38, MaxLevel = 42, Chance = 20 }, // Unbekannt
            },
        },
        new() {
            Id = "ALO-0035", Name = "Ancient Poni Path", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "ALO-0036",
            Ost = "ALO-0034",
            Verbindungen = new() { "ALO-0036", "ALO-0034" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0744", MinLevel = 40, MaxLevel = 44, Chance = 20 }, // Unbekannt
            },
        },
        new() {
            Id = "ALO-0036", Name = "Poni Plains", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Ost = "ALO-0035",
            Verbindungen = new() { "ALO-0035" },
        },
        new() {
            Id = "ALO-0037", Name = "Exeggutor Island", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "ALO-0034",
            Verbindungen = new() { "ALO-0034" },
            Trainer = new() {
                new() {
                    Id = "ALO-K-023", Name = "Hapu", Klasse = "Kahuna",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Boden-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "ALO-KAH-004", Name = "Hapu", Klasse = "Kahuna",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "ALO-0038", Name = "Vast Poni Canyon", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "ALO-0035",
            Verbindungen = new() { "ALO-0035" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0782", MinLevel = 40, MaxLevel = 20, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0621", MinLevel = 40, MaxLevel = 44, Chance = 20 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "ALO-K-016", Name = "Hapu (Totem: Kommo-o)", Klasse = "Prüfung",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Drachen-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0784", Level = 45 } },
                },
                new() {
                    Id = "ALO-T-012", Name = "Totem Kommo-o", Klasse = "Prüfung",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0784", Level = 45 } },
                },
            },
            Arena = new() {
                Leiter = "Mina/Hapu", OrdenName = "Dracheprüfung",
                OrdenNr = 7, TypSpezialisierung = "Fee/Boden",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 45 } },
            },
        },
        new() {
            Id = "ALO-0039", Name = "Altar of the Sunne/Moone", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "ALO-0038",
            Verbindungen = new() { "ALO-0038" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0019", MinLevel = 55, MaxLevel = 55, Chance = 1 }, // Unbekannt
            },
        },
        new() {
            Id = "ALO-0040", Name = "Ruins of Conflict", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "ALO-0006",
            Verbindungen = new() { "ALO-0006" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0785", MinLevel = 60, MaxLevel = 1, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "ALO-0041", Name = "Ruins of Abundance", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "ALO-0029",
            Verbindungen = new() { "ALO-0029" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0787", MinLevel = 60, MaxLevel = 1, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "ALO-0042", Name = "Ruins of Hope", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            WildMonster = new() {
                new() { MonsterId = "PKM-0788", MinLevel = 60, MaxLevel = 1, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "ALO-0043", Name = "Battle Tree", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "ALO-0036",
            Verbindungen = new() { "ALO-0036" },
            Trainer = new() {
                new() {
                    Id = "ALO-OPT-004", Name = "Nein", Klasse = "Red",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "ALO-OPT-005", Name = "Nein", Klasse = "Blue",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "ALO-0044", Name = "Ultra Space", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
        },
        // === Galar (31 Orte) ===
        new() {
            Id = "GAL-0001", Name = "Postwick", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0002",
            Verbindungen = new() { "KAL-0002" },
        },
        new() {
            Id = "GAL-0002", Name = "Route 1 (Galar)", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "GAL-0003",
            Süd = "GAL-0001",
            Verbindungen = new() { "GAL-0003", "GAL-0001" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0810", MinLevel = 5, MaxLevel = 1, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0263", MinLevel = 5, MaxLevel = 30, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "GAL-K-001", Name = "Hop", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Hop erster Kampf", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0810", Level = 5 }, new() { MonsterId = "PKM-0813", Level = 5 }, new() { MonsterId = "PKM-0816", Level = 5 } },
                },
            },
        },
        new() {
            Id = "GAL-0003", Name = "Wedgehurst", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0004",
            Süd = "KAL-0002",
            Verbindungen = new() { "KAL-0004", "KAL-0002" },
        },
        new() {
            Id = "GAL-0004", Name = "Route 2 (Galar)", Typ = "ort",
            Farbe = "forest", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "GAL-0005",
            Ost = "GAL-0003",
            Verbindungen = new() { "GAL-0005", "GAL-0003" },
        },
        new() {
            Id = "GAL-0005", Name = "Slumbering Weald", Typ = "ort",
            Farbe = "forest", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0004",
            Verbindungen = new() { "KAL-0004" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0888", MinLevel = 70, MaxLevel = 1, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0889", MinLevel = 70, MaxLevel = 1, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "GAL-0006", Name = "Motostoke", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0006",
            Ost = "KAL-0004",
            Verbindungen = new() { "KAL-0006", "KAL-0004" },
            Trainer = new() {
                new() {
                    Id = "GAL-K-012", Name = "Kabu", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Feuer-Typ; Dynamax", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "GAL-GYM-003", Name = "Kabu", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "GAL-T-004", Name = "Kabu", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Kabu", OrdenName = "Feuerorden",
                OrdenNr = 3, TypSpezialisierung = "Feuer",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 25 } },
            },
        },
        new() {
            Id = "GAL-0007", Name = "Wild Area", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "GAL-0003",
            West = "GAL-0006",
            Ost = "KAL-0010",
            Verbindungen = new() { "GAL-0003", "GAL-0006", "KAL-0010" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0835", MinLevel = 25, MaxLevel = 20, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0019", MinLevel = 15, MaxLevel = 55, Chance = 100 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "GAL-OPT-002", Name = "Tessa", Klasse = "Pokémon Ranger",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Optional", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "GAL-0008", Name = "Route 3 (Galar)", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "GAL-0009",
            Ost = "GAL-0006",
            Verbindungen = new() { "GAL-0009", "GAL-0006" },
        },
        new() {
            Id = "GAL-0009", Name = "Galar Mine", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0006",
            Ost = "KAL-0008",
            Verbindungen = new() { "KAL-0006", "KAL-0008" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0838", MinLevel = 15, MaxLevel = 30, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "GAL-0010", Name = "Route 4 (Galar)", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "GAL-0011",
            Ost = "GAL-0009",
            Verbindungen = new() { "GAL-0011", "GAL-0009" },
        },
        new() {
            Id = "GAL-0011", Name = "Turffield", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0010",
            Süd = "KAL-0008",
            Verbindungen = new() { "KAL-0010", "KAL-0008" },
            Trainer = new() {
                new() {
                    Id = "GAL-K-002", Name = "Hop", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Hop in Turffield", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0811", Level = 17 }, new() { MonsterId = "PKM-0814", Level = 17 } },
                },
                new() {
                    Id = "GAL-K-010", Name = "Milo", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Pflanz-Typ; Dynamax", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "GAL-GYM-001", Name = "Milo", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "GAL-T-002", Name = "Milo", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Milo", OrdenName = "Grasorden",
                OrdenNr = 1, TypSpezialisierung = "Pflanze",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 15 } },
            },
        },
        new() {
            Id = "GAL-0012", Name = "Route 5 (Galar)", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "GAL-0007",
            West = "GAL-0013",
            Ost = "GAL-0011",
            Verbindungen = new() { "GAL-0007", "GAL-0013", "GAL-0011" },
        },
        new() {
            Id = "GAL-0013", Name = "Hulbury", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0012",
            Süd = "KAL-0010",
            Verbindungen = new() { "KAL-0012", "KAL-0010" },
        },
        new() {
            Id = "GAL-0014", Name = "Route 6 (Galar)", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "GAL-0006",
            Ost = "GAL-0013",
            Verbindungen = new() { "GAL-0006", "GAL-0013" },
        },
        new() {
            Id = "GAL-0015", Name = "Galar Mine No. 2", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0012",
            Ost = "GAL-0006",
            Verbindungen = new() { "KAL-0012", "GAL-0006" },
        },
        new() {
            Id = "GAL-0016", Name = "Stow-on-Side", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0014",
            Süd = "KAL-0012",
            Verbindungen = new() { "KAL-0014", "KAL-0012" },
        },
        new() {
            Id = "GAL-0017", Name = "Route 7 (Galar)", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "GAL-0024",
            Ost = "GAL-0016",
            Verbindungen = new() { "GAL-0024", "GAL-0016" },
        },
        new() {
            Id = "GAL-0018", Name = "Glimwood Tangle", Typ = "ort",
            Farbe = "forest", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "KAL-0014",
            Ost = "KAL-0016",
            Verbindungen = new() { "KAL-0014", "KAL-0016" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0856", MinLevel = 35, MaxLevel = 20, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0859", MinLevel = 35, MaxLevel = 20, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "GAL-0019", Name = "Ballonlea", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0016",
            Süd = "GAL-0018",
            Verbindungen = new() { "KAL-0016", "GAL-0018" },
        },
        new() {
            Id = "GAL-0020", Name = "Route 8 (Galar)", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "GAL-0021",
            Ost = "GAL-0019",
            Verbindungen = new() { "GAL-0021", "GAL-0019" },
        },
        new() {
            Id = "GAL-0021", Name = "Circhester", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0019",
            Süd = "KAL-0016",
            Verbindungen = new() { "KAL-0019", "KAL-0016" },
        },
        new() {
            Id = "GAL-0022", Name = "Route 9 (Galar)", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "GAL-0023",
            Ost = "GAL-0021",
            Verbindungen = new() { "GAL-0023", "GAL-0021" },
        },
        new() {
            Id = "GAL-0023", Name = "Spikemuth", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0019",
            Verbindungen = new() { "KAL-0019" },
            Trainer = new() {
                new() {
                    Id = "GAL-K-018", Name = "Piers", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Unlicht-Typ; KEIN Dynamax", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "GAL-GYM-007", Name = "Piers", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "GAL-T-010", Name = "Piers", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Piers", OrdenName = "Dunkelorden",
                OrdenNr = 7, TypSpezialisierung = "Unlicht",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 45 } },
            },
        },
        new() {
            Id = "GAL-0024", Name = "Hammerlocke", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0014",
            West = "KAL-0012",
            Verbindungen = new() { "KAL-0014", "KAL-0012" },
            Trainer = new() {
                new() {
                    Id = "GAL-K-003", Name = "Hop", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Hop in Hammerlocke", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0812", Level = 32 }, new() { MonsterId = "PKM-0815", Level = 32 } },
                },
                new() {
                    Id = "GAL-K-019", Name = "Raihan", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Drachen-Typ; Dynamax; Doppelkampf", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "GAL-GYM-008", Name = "Raihan", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "GAL-T-011", Name = "Raihan", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Raihan", OrdenName = "Dracheorden",
                OrdenNr = 8, TypSpezialisierung = "Drache",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 50 } },
            },
        },
        new() {
            Id = "GAL-0025", Name = "Route 10 (Galar)", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            West = "GAL-0026",
            Ost = "GAL-0024",
            Verbindungen = new() { "GAL-0026", "GAL-0024" },
        },
        new() {
            Id = "GAL-0026", Name = "Wyndon", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "GAL-0027",
            Süd = "KAL-0021",
            Verbindungen = new() { "GAL-0027", "KAL-0021" },
        },
        new() {
            Id = "GAL-0027", Name = "Rose Tower", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "GAL-0026",
            Verbindungen = new() { "GAL-0026" },
            Trainer = new() {
                new() {
                    Id = "GAL-K-030", Name = "Oleana", Klasse = "Oleana",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Oleana", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "GAL-0028", Name = "Pokémon League (Galar)", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "GAL-0026",
            Verbindungen = new() { "GAL-0026" },
            Trainer = new() {
                new() {
                    Id = "GAL-K-020", Name = "Marnie", Klasse = "Turnier",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Unlicht-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "GAL-K-021", Name = "Hop", Klasse = "Turnier",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Rivale", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0812", Level = 47 }, new() { MonsterId = "PKM-0815", Level = 47 } },
                },
                new() {
                    Id = "GAL-K-022", Name = "Leon", Klasse = "Champion",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Gemischte Typen; Dynamax-Charizard", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "GAL-0029", Name = "Energy Plant", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "GAL-0024",
            Verbindungen = new() { "GAL-0024" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0890", MinLevel = 60, MaxLevel = 1, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "GAL-K-031", Name = "Eternatus", Klasse = "Eternatus",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Fangen nötig; Zacian/Zamazenta helfen", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0890", Level = 60 } },
                },
            },
        },
        new() {
            Id = "GAL-0030", Name = "Isle of Armor", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            WildMonster = new() {
                new() { MonsterId = "PKM-0819", MinLevel = 20, MaxLevel = 40, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "GAL-OPT-003", Name = "Hyde", Klasse = "Dojo Student",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "DLC", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "GAL-0031", Name = "Crown Tundra", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            WildMonster = new() {
                new() { MonsterId = "PKM-0875", MinLevel = 60, MaxLevel = 65, Chance = 30 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "GAL-OPT-004", Name = "Nein", Klasse = "Peony",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        // === Paldea (27 Orte) ===
        new() {
            Id = "PAL-0001", Name = "Mesagoza", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "KAL-0002",
            Verbindungen = new() { "KAL-0002" },
            Trainer = new() {
                new() {
                    Id = "PAL-K-002", Name = "Nemona", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Nemona in Mesagoza", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0907", Level = 20 }, new() { MonsterId = "PKM-0910", Level = 20 } },
                },
                new() {
                    Id = "PAL-T-001", Name = "Nemona", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "PAL-0002", Name = "Poco Path", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "PAL-0001",
            Verbindungen = new() { "PAL-0001" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0906", MinLevel = 5, MaxLevel = 1, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0921", MinLevel = 5, MaxLevel = 30, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0935", MinLevel = 5, MaxLevel = 20, Chance = 0 }, // Unbekannt
            },
        },
        new() {
            Id = "PAL-0003", Name = "Los Platos", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Nord = "PAL-0002",
            Verbindungen = new() { "PAL-0002" },
            Trainer = new() {
                new() {
                    Id = "PAL-K-001", Name = "Nemona", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Nemona erster Kampf", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0906", Level = 8 }, new() { MonsterId = "PKM-0909", Level = 8 }, new() { MonsterId = "PKM-0912", Level = 8 } },
                },
            },
        },
        new() {
            Id = "PAL-0004", Name = "Cortondo", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Trainer = new() {
                new() {
                    Id = "PAL-K-010", Name = "Katy", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Käfer-Typ; Tera-Teddiursa", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "PAL-GYM-001", Name = "Katy", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "PAL-T-002", Name = "Katy", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Katy", OrdenName = "Grasorden",
                OrdenNr = 1, TypSpezialisierung = "Käfer",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 15 } },
            },
        },
        new() {
            Id = "PAL-0005", Name = "Artazon", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Trainer = new() {
                new() {
                    Id = "PAL-K-011", Name = "Brassius", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Pflanz-Typ; Tera-Sudowoodo", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "PAL-GYM-002", Name = "Brassius", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "PAL-T-003", Name = "Brassius", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Brassius", OrdenName = "Normalorden",
                OrdenNr = 2, TypSpezialisierung = "Pflanze",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 20 } },
            },
        },
        new() {
            Id = "PAL-0006", Name = "Levincia", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Trainer = new() {
                new() {
                    Id = "PAL-K-012", Name = "Iono", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Elektro-Typ; Tera-Mismagius", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "PAL-GYM-003", Name = "Iono", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "PAL-T-004", Name = "Iono", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Iono", OrdenName = "Elektroorden",
                OrdenNr = 3, TypSpezialisierung = "Elektro",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 25 } },
            },
        },
        new() {
            Id = "PAL-0007", Name = "Cascarrafa", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Trainer = new() {
                new() {
                    Id = "PAL-K-013", Name = "Kofu", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Wasser-Typ; Kofu-Quest nötig", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "PAL-GYM-004", Name = "Kofu", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "PAL-T-005", Name = "Kofu", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Kofu", OrdenName = "Wasserorden",
                OrdenNr = 4, TypSpezialisierung = "Wasser",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 30 } },
            },
        },
        new() {
            Id = "PAL-0008", Name = "Medali", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Trainer = new() {
                new() {
                    Id = "PAL-K-014", Name = "Larry", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Normal-Typ; Geheimgericht-Rätsel", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "PAL-GYM-005", Name = "Larry", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "PAL-T-006", Name = "Larry", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Larry", OrdenName = "Normalorden",
                OrdenNr = 5, TypSpezialisierung = "Normal",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 35 } },
            },
        },
        new() {
            Id = "PAL-0009", Name = "Montenevera", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Trainer = new() {
                new() {
                    Id = "PAL-K-015", Name = "Ryme", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Geist-Typ; Doppelkampf-Rap", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "PAL-GYM-006", Name = "Ryme", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "PAL-T-007", Name = "Ryme", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Ryme", OrdenName = "Geistorden",
                OrdenNr = 6, TypSpezialisierung = "Geist",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 40 } },
            },
        },
        new() {
            Id = "PAL-0010", Name = "Alfornada", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Trainer = new() {
                new() {
                    Id = "PAL-K-016", Name = "Tulip", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Psycho-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "PAL-GYM-007", Name = "Tulip", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "PAL-T-008", Name = "Tulip", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Tulip", OrdenName = "Psychoorden",
                OrdenNr = 7, TypSpezialisierung = "Psycho",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 45 } },
            },
        },
        new() {
            Id = "PAL-0011", Name = "Glaseado Mountain", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            WildMonster = new() {
                new() { MonsterId = "PKM-0872", MinLevel = 35, MaxLevel = 20, Chance = 0 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "PAL-K-017", Name = "Grusha", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Eis-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "PAL-GYM-008", Name = "Grusha", Klasse = "Arena",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "PAL-T-018", Name = "Great Tusk/Iron Treads", Klasse = "Titan",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "PAL-OPT-003", Name = "Rico", Klasse = "Snowboarder",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Schneegebiet", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
            Arena = new() {
                Leiter = "Grusha", OrdenName = "Eisorden",
                OrdenNr = 8, TypSpezialisierung = "Eis",
                Team = new() { new() { MonsterId = "PKM-0001", Level = 50 } },
            },
        },
        new() {
            Id = "PAL-0012", Name = "Stony Cliff Titan", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Trainer = new() {
                new() {
                    Id = "PAL-K-040", Name = "Klawf", Klasse = "Titan",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Gestein-Typ; Arven dabei", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0950", Level = 16 } },
                },
            },
        },
        new() {
            Id = "PAL-0013", Name = "Open Sky Titan", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Trainer = new() {
                new() {
                    Id = "PAL-K-041", Name = "Bombirdier", Klasse = "Titan",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Flug-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0962", Level = 19 } },
                },
            },
        },
        new() {
            Id = "PAL-0014", Name = "Lurking Steel Titan", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Trainer = new() {
                new() {
                    Id = "PAL-K-042", Name = "Orthworm", Klasse = "Titan",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Stahl-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0968", Level = 28 } },
                },
            },
        },
        new() {
            Id = "PAL-0015", Name = "Quaking Earth Titan", Typ = "ort",
            Farbe = "cave", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Trainer = new() {
                new() {
                    Id = "PAL-K-043", Name = "Great Tusk/Iron Treads", Klasse = "Titan",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Boden-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0984", Level = 44 }, new() { MonsterId = "PKM-0990", Level = 44 } },
                },
            },
        },
        new() {
            Id = "PAL-0016", Name = "False Dragon Titan", Typ = "ort",
            Farbe = "blue", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Trainer = new() {
                new() {
                    Id = "PAL-K-044", Name = "Tatsugiri+Dondozo", Klasse = "Titan",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Drachen/Wasser-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "PAL-0017", Name = "Team Star Scherengang-Basis", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Trainer = new() {
                new() {
                    Id = "PAL-K-030", Name = "Giacomo", Klasse = "Team Star",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Unlicht-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "PAL-0018", Name = "Team Star Flammengang-Basis", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Trainer = new() {
                new() {
                    Id = "PAL-K-031", Name = "Mela", Klasse = "Team Star",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Feuer-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "PAL-0019", Name = "Team Star Giftgang-Basis", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Trainer = new() {
                new() {
                    Id = "PAL-K-032", Name = "Atticus", Klasse = "Team Star",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Gift-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "PAL-0020", Name = "Team Star Feengang-Basis", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Trainer = new() {
                new() {
                    Id = "PAL-K-033", Name = "Ortega", Klasse = "Team Star",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Fee-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "PAL-0021", Name = "Team Star Kampfgang-Basis", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Trainer = new() {
                new() {
                    Id = "PAL-K-034", Name = "Eri", Klasse = "Team Star",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Kampf-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "PAL-0022", Name = "Area Zero", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "PAL-0001",
            Verbindungen = new() { "PAL-0001" },
            WildMonster = new() {
                new() { MonsterId = "PKM-0984", MinLevel = 60, MaxLevel = 10, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0990", MinLevel = 60, MaxLevel = 10, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-1007", MinLevel = 68, MaxLevel = 1, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-1008", MinLevel = 68, MaxLevel = 1, Chance = 0 }, // Unbekannt
                new() { MonsterId = "PKM-0019", MinLevel = 55, MaxLevel = 70, Chance = 100 }, // Unbekannt
            },
            Trainer = new() {
                new() {
                    Id = "PAL-T-020", Name = "Arven", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "PAL-T-021", Name = "Nemona", Klasse = "Rivale",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "PAL-T-022", Name = "Professor Sada/Turo", Klasse = "Boss",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "PAL-OPT-007", Name = "Dr. Alba", Klasse = "Forscher",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Endgame", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "PAL-0023", Name = "Porto Marinada", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
        },
        new() {
            Id = "PAL-0024", Name = "Zapapico", Typ = "ort",
            Farbe = "purple", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
        },
        new() {
            Id = "PAL-0025", Name = "Pokémon League (Paldea)", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "PAL-0001",
            Verbindungen = new() { "PAL-0001" },
            Trainer = new() {
                new() {
                    Id = "PAL-K-020", Name = "Rika", Klasse = "Elite4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Boden-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "PAL-K-021", Name = "Poppy", Klasse = "Elite4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Stahl-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "PAL-K-022", Name = "Larry", Klasse = "Elite4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Flug-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "PAL-K-023", Name = "Hassel", Klasse = "Elite4",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Drachen-Typ", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "PAL-K-024", Name = "Geeta", Klasse = "Champion",
                    Belohnung = 200, MussBesiegt = true,
                    Dialogvor = "Gemischte Typen; Tera-Glimmora", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "PAL-0026", Name = "Kitakami", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "PAL-0001",
            Verbindungen = new() { "PAL-0001" },
            Trainer = new() {
                new() {
                    Id = "PAL-OPT-004", Name = "Masamune", Klasse = "Ogre Clan",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "Starker Optionalkampf", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
        new() {
            Id = "PAL-0027", Name = "Blueberry Academy", Typ = "ort",
            Farbe = "green", GridX = -1, GridY = -1,
            HatMonsterCenter = false, HatMarkt = false,
            Süd = "PAL-0001",
            Verbindungen = new() { "PAL-0001" },
            Trainer = new() {
                new() {
                    Id = "PAL-OPT-005", Name = "Aria", Klasse = "BB League Student",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "DLC Endgame", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
                new() {
                    Id = "PAL-OPT-008", Name = "Cyril", Klasse = "BB League Elite",
                    Belohnung = 200, MussBesiegt = false,
                    Dialogvor = "DLC Endgame", DialogNach = "Gut gekaempft!",
                    Team = new() { new() { MonsterId = "PKM-0019", Level = 5 } },
                },
            },
        },
    };
}