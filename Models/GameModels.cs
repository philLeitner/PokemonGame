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
