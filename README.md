# Monster Kampf 🎮⚔️

Ein Blazor WebAssembly Spiel – Monster sammeln, trainieren und kämpfen!

## Features

- **1025 Monster** mit individuellen Namen, Typen und Stats
- **920 Attacken** mit deutschen Namen, Typ-Effektivität und AP-System
- **18 Typen** mit vollständiger Effektivitäts-Tabelle (x2 / x0.5 / x0)
- **Kampfsystem** mit Schaden-Berechnung, STAB-Bonus, Typ-Multiplikatoren
- **Weltkarte** mit 15 Orten, Routen, Arenen und Trainer-Kämpfen
- **8 Arenen** mit Orden-System
- **Map-Editor** zum Erstellen und Bearbeiten von Orten
- **Starter-Wahl** aus 3 Monstern (Blatt / Brennen / Tropfen)

## Tech-Stack

- **C# / Blazor WebAssembly** (.NET 8)
- **Keine externen Abhängigkeiten** – reines C# + HTML/CSS
- Bereit für **Android/iOS** via .NET MAUI (spätere Erweiterung)

## Projekt-Struktur

```
MonsterKampf/
├── Models/
│   └── GameModels.cs       ← Alle Datenmodelle (Monster, Attacken, Kampf, Welt)
├── Data/
│   ├── TypeChart.cs        ← 18-Typen Effektivitäts-Tabelle (C#)
│   └── WeltData.cs         ← Alle Orte, Arenen, Trainer, Wilde Monster
├── Services/
│   └── GameService.cs      ← Spiellogik, Kampf-Engine, Navigation
├── Pages/
│   ├── Hauptmenü.razor     ← Startbildschirm
│   ├── StarterWahl.razor   ← Monster-Auswahl
│   ├── Weltkarte.razor     ← Weltkarte mit Orten
│   ├── Kampf.razor         ← Kampf-Bildschirm
│   └── MapEditor.razor     ← Map-Editor
└── wwwroot/
    ├── data/
    │   ├── monster.json    ← 1025 Monster
    │   ├── attacken.json   ← 920 Attacken
    │   └── typen.json      ← 18 Typen
    └── css/
        └── game.css        ← Retro-RPG Design
```

## Starten

```bash
dotnet run
```

Dann im Browser: `https://localhost:5001`

## Daten-Format

### monster.json
```json
{
  "id": "PKM-0001",
  "name": "Liralf",
  "typen": ["Blatt", "Gift"],
  "bild": "MonsterBilder/PKM-0001.png",
  "stats": { "kp": 47, "angriff": 51, ... },
  "attacken": [{ "attacke_id": "MOV-0033", "level": 1 }],
  "entwickelt_zu": "PKM-0002",
  "entwicklung_level": 17
}
```

### attacken.json
```json
{
  "id": "MOV-0033",
  "name": "Rempelschlag",
  "typ": "Normal",
  "kategorie": "Physisch",
  "staerke": 38,
  "genauigkeit": 98,
  "ap": 35
}
```

## Bilder hinzufügen

Bilder in `wwwroot/images/monsters/PKM-XXXX.png` ablegen.  
Format: 96×96 oder 128×128 PNG mit transparentem Hintergrund.

## Zukünftige Erweiterungen

- [ ] Monster-Fang-System (Fangbälle)
- [ ] Inventar & Items
- [ ] Speichersystem (LocalStorage)
- [ ] Mehr Orte & Routen
- [ ] Android/iOS via .NET MAUI
- [ ] Online-Kämpfe
