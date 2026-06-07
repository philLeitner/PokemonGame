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
    public string? Statuseffekt { get; set; }        // eingeschlafen, gelähmt, vergiftet, verbrannt, eingefroren
    public int? StatuseffektChance { get; set; }     // 0-100 %
    public int? Generation { get; set; }             // 1-9, 0 = unbekannt
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
    public List<string> TmAttacken { get; set; } = new();
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
                    Statuseffekt = attackeData.Statuseffekt,
                    StatuseffektChance = attackeData.StatuseffektChance,
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
    public string? Statuseffekt { get; set; }
    public int? StatuseffektChance { get; set; }
    public int? Generation { get; set; }
}

// ─── Spieler ─────────────────────────────────────────────────────────────────
public class Spieler
{
    public string Name { get; set; } = "Trainer";
    public int Geld { get; set; } = 3000;
    public string StarterMonsterId { get; set; } = ""; // gewählter Starter (PKM-0001/0004/0007)
    public List<string> Orden { get; set; } = new();
    public List<MonsterInstanz> Team { get; set; } = new();
    public List<MonsterInstanz> Box { get; set; } = new();  // Box für gefangene Monster
    public List<string> BesiegteTrainer { get; set; } = new();
    public string AktuellerOrt { get; set; } = "startstadt";
    public List<InventarItem> Inventar { get; set; } = new();
    public int AktivesMonsterIndex { get; set; } = 0;  // Welches Monster gerade aktiv ist
    public List<string> BesproacheneNPCs { get; set; } = new(); // NPCs mit denen gesprochen wurde

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
    public ZähneWallet ZähneWallet { get; set; } = new();

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
    public string? WoBekommt { get; set; } = null; // Wo/wie man das Item bekommt
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
    // Richtungs-Verbindungen für 3x3 Minimap
    public string? Nord { get; set; }
    public string? NordTyp { get; set; } = "normal"; // "normal", "untergrund", "teleport"
    public int NordMinOrden { get; set; } = 0;
    public string? Sued { get; set; }
    public string? SuedTyp { get; set; } = "normal";
    public int SuedMinOrden { get; set; } = 0;
    public string? Ost { get; set; }
    public string? OstTyp { get; set; } = "normal";
    public int OstMinOrden { get; set; } = 0;
    public string? West { get; set; }
    public string? WestTyp { get; set; } = "normal";
    public int WestMinOrden { get; set; } = 0;
    public List<TrainerKampf> Trainer { get; set; } = new();
    public bool HatMonsterCenter { get; set; } = false;
    public bool HatMarkt { get; set; } = false;
    public List<ShopItem> MarktAngebot { get; set; } = new();
    public int MinOrdenFürZugang { get; set; } = 0; // Orden-Sperre (ab diesem Orden zugänglich)
    public int MaxOrdenFürSperre { get; set; } = 0;  // Bis zu diesem Orden gesperrt (0 = kein Max)
    // Zugangs-Bedingungen
    public string? BenötigtItem { get; set; } = null;       // Item-ID die der Spieler besitzen muss
    public string? BenötigtItemName { get; set; } = null;   // Anzeigename des benötigten Items
    public string? BenötigtItemQuelle { get; set; } = null; // Wo man das benötigte Item bekommt
    public bool IstUnterirdisch { get; set; } = false;      // Unterirdische Gänge: immer zugänglich
    public bool IstStartOrt { get; set; } = false;           // Startort der Region (Prof. Eich etc.) – immer erster Ort
    public bool LigaZugang { get; set; } = false;           // Nur mit allen 8 Kanto-Orden zugänglich
    public string? Teleport { get; set; }                   // Teleport-Ziel (Ort-ID)
    public string? Untergrund { get; set; }                 // Unterirdischer Gang (Ort-ID)
    public List<GesprächsNPC> NPCs { get; set; } = new();
    // ─── Richtungs-Sperren (pro Richtung eigene Bedingung) ───────────────────────────
    public RichtungsSperre? SperrNord { get; set; } = null;
    public RichtungsSperre? SperrSued  { get; set; } = null;
    public RichtungsSperre? SperrOst  { get; set; } = null;
    public RichtungsSperre? SperrWest { get; set; } = null;
    // Ecken
    public RichtungsSperre? SperrNW   { get; set; } = null;
    public RichtungsSperre? SperrNO   { get; set; } = null;
    public RichtungsSperre? SperrSW   { get; set; } = null;
    public RichtungsSperre? SperrSO   { get; set; } = null;
}

public class GesprächsNPC
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Emoji { get; set; } = "🧓";
    public string Dialog { get; set; } = "";
    // Optionales Item das der NPC beim ersten Gespräch gibt
    public string? GibtItemId { get; set; }
    public string? GibtItemName { get; set; }
    public string? GibtItemEmoji { get; set; }
    public string? DialogNachGeschenk { get; set; }
    /// <summary>True wenn dieser NPC der Professor der Region ist (löst Wizard aus)</summary>
    public bool IstProfessor { get; set; } = false;
}

/// <summary>Sperre für eine einzelne Reiserichtung (Nord/Süd/Ost/West)</summary>
public class RichtungsSperre
{
    public int MinOrden          { get; set; } = 0;    // 0 = kein Orden-Limit
    public string? ItemId        { get; set; } = null; // Item-ID die benötigt wird
    public string? ItemName      { get; set; } = null; // Anzeigename des Items
    public string? Hinweis       { get; set; } = null; // Freitext-Hinweis für den Spieler
    public string? BenötigtOrdenName { get; set; } = null; // Orden-Name der benötigt wird
}

public class Arena
{
    public string OrdenName { get; set; } = "";
    public int OrdenNr { get; set; }
    public string Leiter { get; set; } = "";
    public string TypSpezialisierung { get; set; } = "";
    // Alias für WeltData-Kompatibilität
    public string Typ { get => TypSpezialisierung; set => TypSpezialisierung = value; }
    public string? Beschreibung { get; set; }
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
    public bool MussBesiegt { get; set; } = false; // Pflicht-Trainer: blockiert Weiterreise
    public string? SperrtRichtung { get; set; } = null; // "Nord", "Süd", "Ost", "West" - welche Richtung gesperrt ist
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
    AttackeLernen,    // Neues Angriff-Lern-Dialog
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
    // Attacken-Lern-Dialog
    public MonsterInstanz? LernMonster { get; set; }
    public AttackeData? NeueAttacke { get; set; }
    public List<AttackeData> PendingNeueAttacken { get; set; } = new();
}

// ─── Spiel-Phasen ─────────────────────────────────────────────────────────────
public enum SpielPhase
{
    Laden,
    Hauptmenü,
    StarterWahl,
    Weltkarte,
    Kampf,
    Einstellungen,
    ZähneShop,
    EigeneMapStart,  // Dialog: Spielstand kopieren oder neu starten
    AdminPanel,      // Admin-Bereich: Level, Orden, Stats ändern (kein Passwort)
    Pokédex,          // Monsterübersicht
    MonsterEditor,      // Monster-Daten bearbeiten
    RegionsWahl,         // Regionsauswahl für prozedurale Karte
    NachArenaLeiter,     // Dialog nach Arenaleiter-Sieg (neuer Starter, Level A/B)
    StarterWahlNeuRegion // Starter-Auswahl für neue Region
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
    public List<string> BesproacheneNPCs { get; set; } = new();
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
    EineRegion,     // Nur eine bestimmte Region
    EigeneMap       // Eigene selbst gestaltete Map (separater Spielstand)
}

public enum StarterModus
{
    Wählen,         // Spieler wählt selbst
    Zufällig        // Zufälliger Starter
}

public enum WildMonsterModus
{
    RouteGenau,         // Nur Monster die für diese Route definiert sind
    Zufällig,           // Zufällige Monster aus der gesamten Liste
    ZufälligMitLegär   // Zufällige Monster inkl. Legendäre
}

public enum TrainerMonsterModus
{
    Genau,              // Trainer hat definierte Monster
    Zufällig,           // Trainer hat zufällige Monster (Level beibehalten)
    ZufälligMitTypen    // Trainer hat zufällige Monster mit gleichen Typen
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
            var info = MonsterKampf.Data.ReliktDaten.Get(r);
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
    KeinFangen,             // Kein Fangen erlaubt
    NurEigenTyp,            // Nur Attacken des eigenen Typs
    // Team-Handicaps
    MaxZweiMonster,         // Max. 2 Monster im Team
    KeineEntwicklung,       // Keine Entwicklung erlaubt
    ZufälligesTeam,         // Zufälliges Team bei Neustart
    NurWildeMonster,        // Nur wilde Monster fangen
    // Level-Handicaps
    LevelKappe20,           // Max. Level 20
    LevelKappe40,           // Max. Level 40
    LevelKappe60,           // Max. Level 60
    // Nuzlocke-Varianten
    NuzlockeLeicht,         // Nur 1 Monster pro Route fangen
    NuzlockeHart,           // Ohnmächtiges Monster = verloren
    Nuzlocke,               // Standard-Nuzlocke
    NuzlockeEinsFangen,     // Nur erstes Monster pro Route
    // Geld-Handicaps
    WenigerGeld,            // 50% weniger Startgeld
    KeinGeldNachKampf,      // Kein Geld nach Kämpfen
    // Welt-Handicaps
    KeinMonsterCenter,      // Kein Monster Center
    HöhereLevel,            // Alle Gegner 5 Level höher
    // XP-Handicaps
    WenigerXp,              // 50% weniger Erfahrung
    KeinXpTeiler,           // Kein XP-Teiler erlaubt
}

// Positional record: new(Typ, Kategorie, Emoji, Name, Beschreibung, Zähne)
public record ReliktInfo(ReliktTyp Typ, string Kategorie, string Emoji, string Name, string Beschreibung, int Zähne);

// ReliktDaten ist in MonsterKampf.Data (ReliktUndZähneDaten.cs) definiert.
// Keine doppelte Definition hier.

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
    LevelBoost5,        // 3 Zähne – Team +5 Level
    LevelBoost10,       // 6 Zähne – Team +10 Level
    LevelBoost20,       // 10 Zähne – Team +20 Level
    // Stats
    StatBoostKp,        // 2 Zähne – KP +10%
    StatBoostAngriff,   // 2 Zähne – Angriff +10%
    StatBoostVerteidigung, // 2 Zähne – Verteidigung +10%
    StatBoostSpAngriff, // 2 Zähne – Sp.Angriff +10%
    StatBoostSpVerteidigung, // 2 Zähne – Sp.Verteidigung +10%
    StatBoostInitiative, // 2 Zähne – Initiative +10%
    KpBoost,            // 3 Zähne – KP +10% (alle)
    AngriffBoost,       // 3 Zähne – Angriff +10% (alle)
    VertBoost,          // 3 Zähne – Verteidigung +10% (alle)
    SpeedBoost,         // 3 Zähne – Speed +10% (alle)
    AlleStatsBoost,     // 8 Zähne – Alle Stats +10%
    // XP
    MehrXp,             // 4 Zähne – +25% Erfahrung
    XpTeiler,           // 3 Zähne – XP-Teiler (geteilt)
    VollerXpTeiler,     // 6 Zähne – Voller XP-Teiler
    XpBoost,            // 4 Zähne – XP +50%
    // Fangen
    BessereBallle,      // 2 Zähne – Fangball wirkt wie Superball
    ProfiCatcher,       // 3 Zähne – Superball wirkt wie Hyperball
    LegendärBoost10,    // 3 Zähne – Legendäre +10% Fangchance
    LegendärBoost20,    // 5 Zähne – Legendäre +20% Fangchance
    Meisterball,        // 5 Zähne – 1 Meisterball
    BessereKugeln,      // 3 Zähne – Alle Kugeln +20% Fangrate
    MehrTeamSlots,      // 5 Zähne – +2 Team-Slots
    // Geld & Markt
    MehrGeld,           // 2 Zähne – +25% Geld nach Kämpfen
    GünstigerMarkt,     // 3 Zähne – Items 20% billiger
    BessereHeilung,     // 3 Zähne – Tränke heilen 25% mehr
    GeldBoost,          // 3 Zähne – Geld +50%
    // Team
    ExtraSlot,          // 5 Zähne – +1 Team-Slot (max 6)
    BonusStatUpgrade,   // 2 Zähne – Bonus-Stat-Upgrade
}

// ZähneUpgradeDaten und ReliktDaten sind in MonsterKampf.Data (ReliktUndZähneDaten.cs) definiert.
// Keine doppelte Definition hier.
