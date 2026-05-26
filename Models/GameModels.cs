namespace MonsterKampf.Models;

// ─── Typ-System ──────────────────────────────────────────────────────────────
public class TypInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public List<string> X2Gegen { get; set; } = new();
    public List<string> X05Gegen { get; set; } = new();
    public List<string> X0Gegen { get; set; } = new();
    public List<string> SchwachGegen { get; set; } = new();
    public List<string> ResistentGegen { get; set; } = new();
    public List<string> ImmunGegen { get; set; } = new();
}

// ─── Attacken ────────────────────────────────────────────────────────────────
public class AttackeData
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Typ { get; set; } = "";
    public string Kategorie { get; set; } = "";
    public int? Staerke { get; set; }
    public int? Genauigkeit { get; set; }
    public int? Ap { get; set; }
}

// ─── Monster (Spezies-Definition aus JSON) ───────────────────────────────────
public class MonsterData
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public List<string> Typen { get; set; } = new();
    public string Bild { get; set; } = "";
    public Dictionary<string, int> Stats { get; set; } = new();
    public List<AttackenLernEintrag> Attacken { get; set; } = new();
    public string? EntwickeltZu { get; set; }
    public string? EntwicklungName { get; set; }
    public int? EntwicklungLevel { get; set; }
    // Fangrate: 0-255 (höher = leichter zu fangen), Standard 45
    public int Fangrate { get; set; } = 45;
}

public class AttackenLernEintrag
{
    public string AttackeId { get; set; } = "";
    public int Level { get; set; }
}

// ─── Monster-Instanz (im Kampf / im Team) ────────────────────────────────────
public class MonsterInstanz
{
    public string SpeziesId { get; set; } = "";
    public string Name { get; set; } = "";
    public string? Spitzname { get; set; }   // optionaler Spitzname
    public List<string> Typen { get; set; } = new();
    public string Bild { get; set; } = "";
    public int Level { get; set; } = 5;
    public int MaxKp { get; set; }
    public int AktuelleKp { get; set; }
    public int Angriff { get; set; }
    public int Verteidigung { get; set; }
    public int SpezialAngriff { get; set; }
    public int SpezialVerteidigung { get; set; }
    public int Initiative { get; set; }
    public List<AttackeInstanz> Attacken { get; set; } = new();
    public string Status { get; set; } = "none"; // none, vergiftet, gelähmt, verbrannt, eingeschlafen, eingefroren
    public int StatusZähler { get; set; } = 0;   // Schlaf-Runden etc.
    public int ErfahrungsPunkte { get; set; } = 0;
    public int Fangrate { get; set; } = 45;
    // Entwicklungs-Daten (aus Spezies übernommen)
    public string? EntwickeltZu { get; set; }
    public string? EntwicklungName { get; set; }
    public int? EntwicklungLevel { get; set; }
    public bool IstOhnmächtig => AktuelleKp <= 0;
    public string AngezeigterName => !string.IsNullOrEmpty(Spitzname) ? Spitzname : Name;

    public static MonsterInstanz VonSpezies(MonsterData spezies, int level, List<AttackeData> alleAttacken)
    {
        float levelFaktor = level / 50f;
        var instanz = new MonsterInstanz
        {
            SpeziesId = spezies.Id,
            Name = spezies.Name,
            Typen = new List<string>(spezies.Typen),
            Bild = spezies.Bild,
            Level = level,
            Angriff = (int)(spezies.Stats.GetValueOrDefault("angriff", 50) * levelFaktor) + 5,
            Verteidigung = (int)(spezies.Stats.GetValueOrDefault("verteidigung", 50) * levelFaktor) + 5,
            SpezialAngriff = (int)(spezies.Stats.GetValueOrDefault("spezialangriff", 50) * levelFaktor) + 5,
            SpezialVerteidigung = (int)(spezies.Stats.GetValueOrDefault("spezialverteidigung", 50) * levelFaktor) + 5,
            Initiative = (int)(spezies.Stats.GetValueOrDefault("initiative", 50) * levelFaktor) + 5,
            Fangrate = spezies.Fangrate,
            EntwickeltZu = spezies.EntwickeltZu,
            EntwicklungName = spezies.EntwicklungName,
            EntwicklungLevel = spezies.EntwicklungLevel,
        };
        int basisKp = spezies.Stats.GetValueOrDefault("kp", 45);
        instanz.MaxKp = (int)((basisKp * 2 * level) / 100f) + level + 10;
        instanz.AktuelleKp = instanz.MaxKp;

        // Lernbare Attacken bis zum aktuellen Level
        var lernbar = spezies.Attacken
            .Where(a => a.Level <= level)
            .OrderByDescending(a => a.Level)
            .Take(4)
            .ToList();

        foreach (var eintrag in lernbar)
        {
            var attackeData = alleAttacken.FirstOrDefault(a => a.Id == eintrag.AttackeId);
            if (attackeData != null)
            {
                instanz.Attacken.Add(new AttackeInstanz
                {
                    Id = attackeData.Id,
                    Name = attackeData.Name,
                    Typ = attackeData.Typ,
                    Kategorie = attackeData.Kategorie,
                    Staerke = attackeData.Staerke,
                    Genauigkeit = attackeData.Genauigkeit,
                    MaxAp = attackeData.Ap ?? 10,
                    AktuelleAp = attackeData.Ap ?? 10,
                });
            }
        }

        // Fallback: mindestens eine Attacke
        if (instanz.Attacken.Count == 0)
        {
            instanz.Attacken.Add(new AttackeInstanz
            {
                Id = "MOV-0033",
                Name = "Rempelschlag",
                Typ = "Normal",
                Kategorie = "Physisch",
                Staerke = 40,
                Genauigkeit = 100,
                MaxAp = 35,
                AktuelleAp = 35,
            });
        }

        return instanz;
    }
}

public class AttackeInstanz
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Typ { get; set; } = "";
    public string Kategorie { get; set; } = "";
    public int? Staerke { get; set; }
    public int? Genauigkeit { get; set; }
    public int MaxAp { get; set; }
    public int AktuelleAp { get; set; }
    public bool HatAp => AktuelleAp > 0;
}

// ─── Spieler ─────────────────────────────────────────────────────────────────
public class Spieler
{
    public string Name { get; set; } = "Trainer";
    public int Geld { get; set; } = 3000;
    public List<string> Orden { get; set; } = new();
    public List<MonsterInstanz> Team { get; set; } = new();
    public List<MonsterInstanz> Box { get; set; } = new();  // Box für gefangene Monster
    public List<string> BesiegteTrainer { get; set; } = new();
    public string AktuellerOrt { get; set; } = "startstadt";
    public List<InventarItem> Inventar { get; set; } = new();
    public int AktivesMonsterIndex { get; set; } = 0;  // Welches Monster gerade aktiv ist

    public MonsterInstanz? AktivesMonster
    {
        get
        {
            // Zuerst versuche den gespeicherten Index
            if (AktivesMonsterIndex >= 0 && AktivesMonsterIndex < Team.Count
                && !Team[AktivesMonsterIndex].IstOhnmächtig)
                return Team[AktivesMonsterIndex];
            // Fallback: erstes nicht-ohnmächtiges Monster
            var fallback = Team.FirstOrDefault(m => !m.IstOhnmächtig);
            if (fallback != null)
                AktivesMonsterIndex = Team.IndexOf(fallback);
            return fallback;
        }
    }

    public bool AlleOhnmächtig => Team.All(m => m.IstOhnmächtig);

    public InventarItem? GetItem(string itemId) => Inventar.FirstOrDefault(i => i.ItemId == itemId);
    public int GetItemMenge(string itemId) => GetItem(itemId)?.Menge ?? 0;

    public void ItemHinzufügen(string itemId, string name, string emoji, int menge = 1)
    {
        var vorhandenes = Inventar.FirstOrDefault(i => i.ItemId == itemId);
        if (vorhandenes != null)
            vorhandenes.Menge += menge;
        else
            Inventar.Add(new InventarItem { ItemId = itemId, Name = name, Emoji = emoji, Menge = menge });
    }

    public bool ItemVerwenden(string itemId)
    {
        var item = GetItem(itemId);
        if (item == null || item.Menge <= 0) return false;
        item.Menge--;
        if (item.Menge <= 0) Inventar.Remove(item);
        return true;
    }
}

// ─── Items / Shop ─────────────────────────────────────────────────────────────
public class ShopItem
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Beschreibung { get; set; } = "";
    public int Preis { get; set; }
    public string Emoji { get; set; } = "💊";
    public string Kategorie { get; set; } = "Heilung";
}

public class InventarItem
{
    public string ItemId { get; set; } = "";
    public string Name { get; set; } = "";
    public string Emoji { get; set; } = "💊";
    public int Menge { get; set; }
}

// ─── Karte / Welt ─────────────────────────────────────────────────────────────
public class Ort
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Typ { get; set; } = "stadt"; // stadt, route, hoehle, wald
    public string Farbe { get; set; } = "blue";
    public int GridX { get; set; }
    public int GridY { get; set; }
    public string? Beschreibung { get; set; }
    public Arena? Arena { get; set; }
    public List<WildBegegnung> WildMonster { get; set; } = new();
    public List<string> Verbindungen { get; set; } = new();
    public List<TrainerKampf> Trainer { get; set; } = new();
    public bool HatMonsterCenter { get; set; } = false;
    public bool HatMarkt { get; set; } = false;
    public List<ShopItem> MarktAngebot { get; set; } = new();
    public int MinOrdenFürZugang { get; set; } = 0; // Orden-Sperre
}

public class Arena
{
    public string OrdenName { get; set; } = "";
    public int OrdenNr { get; set; }
    public string Leiter { get; set; } = "";
    public string TypSpezialisierung { get; set; } = "";
    public List<MonsterTeamEintrag> Team { get; set; } = new();
}

public class WildBegegnung
{
    public string MonsterId { get; set; } = "";
    public int MinLevel { get; set; } = 3;
    public int MaxLevel { get; set; } = 8;
    public int Chance { get; set; } = 20; // 0-100
}

public class TrainerKampf
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Klasse { get; set; } = "Trainer";
    public int Belohnung { get; set; } = 200;
    public List<MonsterTeamEintrag> Team { get; set; } = new();
    public string Dialogvor { get; set; } = "Auf zum Kampf!";
    public string DialogNach { get; set; } = "Gut gekämpft!";
}

public class MonsterTeamEintrag
{
    public string MonsterId { get; set; } = "";
    public int Level { get; set; } = 10;
}

// ─── Kampf-Zustand ───────────────────────────────────────────────────────────
public enum KampfPhase
{
    Intro,
    SpielerZug,
    GegnerZug,
    MonsterWechsel,   // Spieler muss neues Monster wählen
    ItemWählen,       // Spieler wählt Item
    Fangen,           // Fangversuch läuft
    Evolution,        // Entwicklungs-Animation
    Ergebnis,
    Beendet
}

public enum KampfTyp
{
    Wild,
    Trainer,
    Arena
}

public class KampfZustand
{
    public KampfTyp Typ { get; set; }
    public MonsterInstanz SpielerMonster { get; set; } = new();
    public MonsterInstanz GegnerMonster { get; set; } = new();
    public string GegnerName { get; set; } = "Wildes Monster";
    public KampfPhase Phase { get; set; } = KampfPhase.Intro;
    public List<string> Log { get; set; } = new();
    public bool SpielerGewonnen { get; set; }
    public int BelohnungGeld { get; set; }
    public int ErfahrungGewonnen { get; set; }
    public string? TrainerId { get; set; }
    public string? OrtId { get; set; }
    // Trainer-Kampf: welches Monster des Trainers gerade dran ist
    public int TrainerMonsterIndex { get; set; } = 0;
    public TrainerKampf? AktuellerTrainer { get; set; }
    // Fangen
    public bool KannFangen => Typ == KampfTyp.Wild;
    // Evolution nach Kampf
    public MonsterInstanz? EntwickeltSichMonster { get; set; }
    public string? EntwickeltSichZuName { get; set; }
    // Flucht-Zähler (für Flucht-Formel)
    public int FluchtVersuche { get; set; } = 0;
}

// ─── Spiel-Phasen ─────────────────────────────────────────────────────────────
public enum SpielPhase
{
    Laden,
    Hauptmenü,
    StarterWahl,
    Weltkarte,
    Kampf,
    MapEditor
}

// ─── Speicherstand ────────────────────────────────────────────────────────────
public class SpielstandDaten
{
    public string SpielerName { get; set; } = "";
    public int Geld { get; set; }
    public string AktuellerOrt { get; set; } = "startstadt";
    public List<string> Orden { get; set; } = new();
    public List<string> BesiegteTrainer { get; set; } = new();
    public List<GespeichertesMonster> Team { get; set; } = new();
    public List<GespeichertesMonster> Box { get; set; } = new();
    public List<GespeichertesItem> Inventar { get; set; } = new();
}

public class GespeichertesMonster
{
    public string SpeziesId { get; set; } = "";
    public string Name { get; set; } = "";
    public string? Spitzname { get; set; }
    public int Level { get; set; }
    public int AktuelleKp { get; set; }
    public int MaxKp { get; set; }
    public int ErfahrungsPunkte { get; set; }
    public string Status { get; set; } = "none";
    public int Angriff { get; set; }
    public int Verteidigung { get; set; }
    public int SpezialAngriff { get; set; }
    public int SpezialVerteidigung { get; set; }
    public int Initiative { get; set; }
    public List<GespeicherteAttacke> Attacken { get; set; } = new();
}

public class GespeicherteAttacke
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public int AktuelleAp { get; set; }
    public int MaxAp { get; set; }
}

public class GespeichertesItem
{
    public string ItemId { get; set; } = "";
    public string Name { get; set; } = "";
    public string Emoji { get; set; } = "💊";
    public int Menge { get; set; }
}

// ─── Spiel-Einstellungen ──────────────────────────────────────────────────────
public enum MapModus
{
    AlleRegionen,   // Alle Regionen zugänglich
    EineRegion      // Nur eine bestimmte Region
}

public enum StarterModus
{
    Wählen,         // Spieler wählt selbst
    Zufällig        // Zufälliger Starter
}

public enum WildMonsterModus
{
    RouteGenau,     // Nur Monster die für diese Route definiert sind
    Zufällig        // Zufällige Monster aus der gesamten Liste
}

public enum TrainerMonsterModus
{
    Genau,          // Trainer hat definierte Monster
    Zufällig        // Trainer hat zufällige Monster
}

public class SpielEinstellungen
{
    public MapModus KartenModus { get; set; } = MapModus.AlleRegionen;
    public int RegionAuswahl { get; set; } = 1;           // 1-8 wenn EineRegion
    public StarterModus StarterModus { get; set; } = StarterModus.Wählen;
    public int StarterRegion { get; set; } = 0;           // 0 = alle Regionen
    public WildMonsterModus WildModus { get; set; } = WildMonsterModus.RouteGenau;
    public TrainerMonsterModus TrainerModus { get; set; } = TrainerMonsterModus.Genau;
    public List<ReliktTyp> AktiveRelikte { get; set; } = new();

    public bool HatRelikt(ReliktTyp typ) => AktiveRelikte.Contains(typ);

    public void ReliktToggle(ReliktTyp typ)
    {
        if (AktiveRelikte.Contains(typ))
            AktiveRelikte.Remove(typ);
        else
            AktiveRelikte.Add(typ);
    }

    // Gesamte Zähne aus aktiven Relikten
    public int GetGesamtZähne()
    {
        int total = 0;
        foreach (var r in AktiveRelikte)
        {
            var info = ReliktDaten.Get(r);
            if (info != null) total += info.Zähne;
        }
        return total;
    }

    // Bonus-Stat-Upgrades aus Zähnen (alle 5 Zähne = 1 Bonus)
    public int GetBonusStatUpgrades() => GetGesamtZähne() / 5;
}

// ─── Relikte (Handicaps) ──────────────────────────────────────────────────────
public enum ReliktTyp
{
    // Kampf-Handicaps
    NurEinMonster,          // Nur 1 Monster im Team erlaubt
    KeinHeilen,             // Kein Monster Center nutzbar
    KeinMarkt,              // Kein Markt nutzbar
    NurPokeball,            // Nur einfache Fangbälle
    KeinFliehen,            // Kann nicht aus Kämpfen fliehen
    // Level-Handicaps
    LevelKappe20,           // Max. Level 20
    LevelKappe40,           // Max. Level 40
    LevelKappe60,           // Max. Level 60
    // Nuzlocke-Varianten
    NuzlockeLeicht,         // Nur 1 Monster pro Route fangen
    NuzlockeHart,           // Ohnmächtiges Monster = verloren
    // Geld-Handicaps
    WenigerGeld,            // 50% weniger Startgeld
    KeinGeldNachKampf,      // Kein Geld nach Kämpfen
    // XP-Handicaps
    WenigerXp,              // 50% weniger Erfahrung
    KeinXpTeiler,           // Kein XP-Teiler erlaubt
}

public class ReliktInfo
{
    public ReliktTyp Typ { get; set; }
    public string Name { get; set; } = "";
    public string Beschreibung { get; set; } = "";
    public string Kategorie { get; set; } = "";
    public int Zähne { get; set; }
    public string Emoji { get; set; } = "💀";
}

public static class ReliktDaten
{
    public static readonly List<ReliktInfo> Alle = new()
    {
        // Kampf
        new() { Typ = ReliktTyp.NurEinMonster,    Name = "Einzelkämpfer",      Beschreibung = "Nur 1 Monster im Team erlaubt",           Kategorie = "Kampf",  Zähne = 5,  Emoji = "👤" },
        new() { Typ = ReliktTyp.KeinHeilen,        Name = "Kein Center",        Beschreibung = "Monster Center kann nicht genutzt werden", Kategorie = "Kampf",  Zähne = 4,  Emoji = "🚫" },
        new() { Typ = ReliktTyp.KeinMarkt,         Name = "Kein Markt",         Beschreibung = "Markt kann nicht genutzt werden",          Kategorie = "Kampf",  Zähne = 3,  Emoji = "🏪" },
        new() { Typ = ReliktTyp.NurPokeball,       Name = "Nur Fangball",       Beschreibung = "Nur einfache Fangbälle erlaubt",           Kategorie = "Kampf",  Zähne = 2,  Emoji = "⚪" },
        new() { Typ = ReliktTyp.KeinFliehen,       Name = "Kein Fliehen",       Beschreibung = "Kann nicht aus Kämpfen fliehen",           Kategorie = "Kampf",  Zähne = 3,  Emoji = "🔒" },
        // Level
        new() { Typ = ReliktTyp.LevelKappe20,      Name = "Level-Kappe 20",     Beschreibung = "Monster können max. Level 20 erreichen",   Kategorie = "Level",  Zähne = 6,  Emoji = "📊" },
        new() { Typ = ReliktTyp.LevelKappe40,      Name = "Level-Kappe 40",     Beschreibung = "Monster können max. Level 40 erreichen",   Kategorie = "Level",  Zähne = 4,  Emoji = "📊" },
        new() { Typ = ReliktTyp.LevelKappe60,      Name = "Level-Kappe 60",     Beschreibung = "Monster können max. Level 60 erreichen",   Kategorie = "Level",  Zähne = 2,  Emoji = "📊" },
        // Nuzlocke
        new() { Typ = ReliktTyp.NuzlockeLeicht,    Name = "Nuzlocke (leicht)",  Beschreibung = "Nur 1 Monster pro Route fangen",           Kategorie = "Nuzlocke", Zähne = 4, Emoji = "🎯" },
        new() { Typ = ReliktTyp.NuzlockeHart,      Name = "Nuzlocke (hart)",    Beschreibung = "Ohnmächtiges Monster ist dauerhaft weg",   Kategorie = "Nuzlocke", Zähne = 8, Emoji = "💀" },
        // Geld
        new() { Typ = ReliktTyp.WenigerGeld,       Name = "Armut",              Beschreibung = "50% weniger Startgeld",                    Kategorie = "Geld",   Zähne = 2,  Emoji = "💸" },
        new() { Typ = ReliktTyp.KeinGeldNachKampf, Name = "Kein Kampfgeld",     Beschreibung = "Kein Geld nach gewonnenen Kämpfen",        Kategorie = "Geld",   Zähne = 3,  Emoji = "💰" },
        // XP
        new() { Typ = ReliktTyp.WenigerXp,         Name = "Weniger XP",         Beschreibung = "50% weniger Erfahrungspunkte",             Kategorie = "XP",     Zähne = 3,  Emoji = "⬇️" },
        new() { Typ = ReliktTyp.KeinXpTeiler,      Name = "Kein XP-Teiler",     Beschreibung = "XP-Teiler-Upgrades sind gesperrt",         Kategorie = "XP",     Zähne = 2,  Emoji = "❌" },
    };

    public static ReliktInfo? Get(ReliktTyp typ) => Alle.Find(r => r.Typ == typ);
    public static List<string> GetKategorien() => Alle.Select(r => r.Kategorie).Distinct().ToList();
    public static List<ReliktInfo> GetNachKategorie(string kat) => Alle.Where(r => r.Kategorie == kat).ToList();
}

// ─── Zähne-Wallet & Upgrades ─────────────────────────────────────────────────
public class ZähneWallet
{
    public int GesamtZähne { get; set; } = 0;
    public int AusgegebeneZähne { get; set; } = 0;
    public int VerfügbareZähne => GesamtZähne - AusgegebeneZähne;
    public int BonusStatUpgradesVerdient => GesamtZähne / 5;
    public int BonusStatUpgradesGenutzt { get; set; } = 0;
    public int BonusStatUpgradesVerfügbar => BonusStatUpgradesVerdient - BonusStatUpgradesGenutzt;
    public List<ZähneUpgrade> GekaufteUpgrades { get; set; } = new();

    public bool KannBezahlen(int kosten) => VerfügbareZähne >= kosten;
    public bool Ausgeben(int kosten)
    {
        if (!KannBezahlen(kosten)) return false;
        AusgegebeneZähne += kosten;
        return true;
    }
    public void Verdienen(int menge) => GesamtZähne += menge;
    public bool HatUpgrade(ZähneUpgrade upgrade) => GekaufteUpgrades.Contains(upgrade);
}

public enum ZähneUpgrade
{
    // Level-Boosts
    LevelBoost25,       // 3 Zähne – 1 Monster +25 Level
    LevelBoost50,       // 5 Zähne – 1 Monster +50 Level
    // Stats
    StatBoostKp,        // 2 Zähne – KP +10%
    StatBoostAngriff,   // 2 Zähne – Angriff +10%
    StatBoostVerteidigung, // 2 Zähne – Verteidigung +10%
    StatBoostSpAngriff, // 2 Zähne – Sp.Angriff +10%
    StatBoostSpVerteidigung, // 2 Zähne – Sp.Verteidigung +10%
    StatBoostInitiative, // 2 Zähne – Initiative +10%
    // XP
    MehrXp,             // 4 Zähne – +25% Erfahrung
    XpTeiler,           // 3 Zähne – XP-Teiler (geteilt)
    VollerXpTeiler,     // 6 Zähne – Voller XP-Teiler
    // Fangen
    BessereBallle,      // 2 Zähne – Fangball wirkt wie Superball
    ProfiCatcher,       // 3 Zähne – Superball wirkt wie Hyperball
    LegendärBoost10,    // 3 Zähne – Legendäre +10% Fangchance
    LegendärBoost20,    // 5 Zähne – Legendäre +20% Fangchance
    Meisterball,        // 5 Zähne – 1 Meisterball
    // Geld & Markt
    MehrGeld,           // 2 Zähne – +25% Geld nach Kämpfen
    GünstigerMarkt,     // 3 Zähne – Items 20% billiger
    BessereHeilung,     // 3 Zähne – Tränke heilen 25% mehr
    // Team
    ExtraSlot,          // 5 Zähne – +1 Team-Slot (max 6)
}

public static class ZähneUpgradeDaten
{
    public record UpgradeInfo(ZähneUpgrade Typ, string Name, string Beschreibung, int Kosten, string Emoji);

    public static readonly List<UpgradeInfo> Alle = new()
    {
        new(ZähneUpgrade.LevelBoost25,          "Level-Boost +25",       "1 Monster bekommt +25 Level",                3, "⬆️"),
        new(ZähneUpgrade.LevelBoost50,          "Level-Boost +50",       "1 Monster bekommt +50 Level",                5, "⬆️"),
        new(ZähneUpgrade.StatBoostKp,           "KP +10%",               "KP eines Monsters dauerhaft +10%",           2, "❤️"),
        new(ZähneUpgrade.StatBoostAngriff,      "Angriff +10%",          "Angriff eines Monsters dauerhaft +10%",      2, "⚔️"),
        new(ZähneUpgrade.StatBoostVerteidigung, "Verteidigung +10%",     "Verteidigung eines Monsters dauerhaft +10%", 2, "🛡️"),
        new(ZähneUpgrade.StatBoostSpAngriff,    "Sp.Angriff +10%",       "Sp.Angriff eines Monsters dauerhaft +10%",   2, "✨"),
        new(ZähneUpgrade.StatBoostSpVerteidigung,"Sp.Verteidigung +10%", "Sp.Verteidigung eines Monsters +10%",        2, "🔮"),
        new(ZähneUpgrade.StatBoostInitiative,   "Initiative +10%",       "Initiative eines Monsters dauerhaft +10%",   2, "💨"),
        new(ZähneUpgrade.MehrXp,                "Mehr XP",               "+25% Erfahrungspunkte",                      4, "📈"),
        new(ZähneUpgrade.XpTeiler,              "XP-Teiler",             "Alle Monster bekommen XP (geteilt)",         3, "🔄"),
        new(ZähneUpgrade.VollerXpTeiler,        "Voller XP-Teiler",      "Alle Monster bekommen volle XP",             6, "🔄"),
        new(ZähneUpgrade.BessereBallle,         "Bessere Bälle",         "Fangball wirkt wie Superball",               2, "⚪"),
        new(ZähneUpgrade.ProfiCatcher,          "Profi-Fänger",          "Superball wirkt wie Hyperball",              3, "🎯"),
        new(ZähneUpgrade.LegendärBoost10,       "+10% Legendär-Chance",  "Legendäre 10% leichter fangen",             3, "⭐"),
        new(ZähneUpgrade.LegendärBoost20,       "+20% Legendär-Chance",  "Legendäre 20% leichter fangen",             5, "🌟"),
        new(ZähneUpgrade.Meisterball,           "+1 Meisterball",        "1 Meisterball (100% Fangchance)",            5, "🔵"),
        new(ZähneUpgrade.MehrGeld,              "Mehr Geld",             "+25% Geld nach Kämpfen",                     2, "💰"),
        new(ZähneUpgrade.GünstigerMarkt,        "Günstiger Markt",       "Items 20% billiger im Markt",                3, "🏪"),
        new(ZähneUpgrade.BessereHeilung,        "Stärkere Heilung",      "Tränke heilen 25% mehr HP",                  3, "💊"),
        new(ZähneUpgrade.ExtraSlot,             "Extra Team-Slot",       "+1 Team-Slot (max. 6 Monster)",              5, "➕"),
    };

    public static int GetKosten(ZähneUpgrade typ) => Alle.Find(u => u.Typ == typ)?.Kosten ?? 0;
    public static UpgradeInfo? Get(ZähneUpgrade typ) => Alle.Find(u => u.Typ == typ);
    public static List<string> GetKategorien() => new() { "Level", "Stats", "XP", "Fangen", "Geld & Markt", "Team" };
    public static List<UpgradeInfo> GetNachKategorie(string kat) => kat switch
    {
        "Level"      => Alle.Where(u => u.Typ is ZähneUpgrade.LevelBoost25 or ZähneUpgrade.LevelBoost50).ToList(),
        "Stats"      => Alle.Where(u => u.Typ is ZähneUpgrade.StatBoostKp or ZähneUpgrade.StatBoostAngriff or ZähneUpgrade.StatBoostVerteidigung or ZähneUpgrade.StatBoostSpAngriff or ZähneUpgrade.StatBoostSpVerteidigung or ZähneUpgrade.StatBoostInitiative).ToList(),
        "XP"         => Alle.Where(u => u.Typ is ZähneUpgrade.MehrXp or ZähneUpgrade.XpTeiler or ZähneUpgrade.VollerXpTeiler).ToList(),
        "Fangen"     => Alle.Where(u => u.Typ is ZähneUpgrade.BessereBallle or ZähneUpgrade.ProfiCatcher or ZähneUpgrade.LegendärBoost10 or ZähneUpgrade.LegendärBoost20 or ZähneUpgrade.Meisterball).ToList(),
        "Geld & Markt" => Alle.Where(u => u.Typ is ZähneUpgrade.MehrGeld or ZähneUpgrade.GünstigerMarkt or ZähneUpgrade.BessereHeilung).ToList(),
        "Team"       => Alle.Where(u => u.Typ is ZähneUpgrade.ExtraSlot).ToList(),
        _            => Alle
    };
}
