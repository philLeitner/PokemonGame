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
    public int ErfahrungsPunkte { get; set; } = 0;
    public bool IstOhnmächtig => AktuelleKp <= 0;

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
    public List<string> BesiegteTrainer { get; set; } = new();
    public string AktuellerOrt { get; set; } = "startstadt";
    public MonsterInstanz? AktivesMonster => Team.FirstOrDefault(m => !m.IstOhnmächtig);
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
