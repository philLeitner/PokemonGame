using MonsterKampf.Models;
using System.Text;
namespace MonsterKampf.Services;

// ─── Regions-Konfiguration (aus regionen.json) ───────────────────────────────
public class RegionConfig
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public int Generation { get; set; }
    public string Farbe { get; set; } = "#888";
    public string Emoji { get; set; } = "🌍";
    public int BasisEbenen { get; set; } = 30;
    public List<string> Starter { get; set; } = new();
    public List<string> StarterNamen { get; set; } = new();
    public string Professor { get; set; } = "Professor";
    public string ProfessorEmoji { get; set; } = "👨‍🔬";
    public List<ArenaLeiterConfig> Arenaleiter { get; set; } = new();
}

public class ArenaLeiterConfig
{
    public string Name { get; set; } = "";
    public string Typ { get; set; } = "";
    public string Orden { get; set; } = "";
}

// ─── Generierte Karte (Metadaten für Speichern/Laden + Fog-of-War) ───────────
public class GenerierteKarte
{
    public string SeedCode { get; set; } = "";
    public List<string> RegionsReihenfolge { get; set; } = new();
    /// <summary>Startort-ID (erster Ort der generierten Karte)</summary>
    public string StartOrtId { get; set; } = "";
    /// <summary>Alle generierten Ort-IDs in Reihenfolge (Hauptpfad)</summary>
    public List<string> OrtReihenfolge { get; set; } = new();
    /// <summary>Bis zu welchem Index (in OrtReihenfolge) ist die Karte freigeschaltet</summary>
    public int FreigeschalteBisIndex { get; set; } = 5;
    /// <summary>Welche Ort-IDs sind bereits freigeschaltet (für Fog-of-War)</summary>
    public HashSet<string> FreigeschalteteOrte { get; set; } = new();
    /// <summary>IDs der bereits besiegten Arena-Orte</summary>
    public HashSet<string> BesiegteArenen { get; set; } = new();
    /// <summary>X/Y-Koordinaten für die grafische Netzansicht (OrtId → (GridX, GridY))</summary>
    public Dictionary<string, (int X, int Y)> OrtKoordinaten { get; set; } = new();
    /// <summary>Kürzeste Distanz vom Startort (Anzahl Schritte) → bestimmt das Level</summary>
    public Dictionary<string, int> OrtDistanzen { get; set; } = new();
}

// ─── Generator ───────────────────────────────────────────────────────────────
public class KartenGenerator
{
    private static readonly char[] SeedChars =
        "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    public static string GeneriereSeedCode()
    {
        var rng = new Random();
        var sb = new StringBuilder(16);
        for (int i = 0; i < 16; i++)
            sb.Append(SeedChars[rng.Next(SeedChars.Length)]);
        return sb.ToString();
    }

    private static int SeedZuInt(string seed)
    {
        int hash = 17;
        foreach (char c in seed)
            hash = hash * 31 + c;
        return Math.Abs(hash);
    }

    public static (List<Ort> Orte, GenerierteKarte Meta) Generiere(
        string seedCode,
        List<string> regionsReihenfolge,
        List<RegionConfig> alleRegionen,
        List<Ort> alleOrte)
    {
        var rng = new Random(SeedZuInt(seedCode));
        var ergebnis = new List<Ort>();
        var meta = new GenerierteKarte
        {
            SeedCode = seedCode,
            RegionsReihenfolge = regionsReihenfolge
        };

        int ordenOffset = 0;
        // Für Koordinaten: aktueller X-Offset (Hauptpfad läuft horizontal)
        int globalX = 0;

        foreach (var regId in regionsReihenfolge)
        {
            var regCfg = alleRegionen.FirstOrDefault(r => r.Id == regId);
            if (regCfg == null) continue;

                        var regionOrte = alleOrte
                .Where(o => o.Id.StartsWith(regId + "-", StringComparison.OrdinalIgnoreCase))
                .OrderBy(o => o.Id)
                .ToList();
            if (!regionOrte.Any()) continue;

            // Liga-Orte (LigaZugang=true) kommen immer ans Ende als Boss-Abschluss
            var ligaOrte = regionOrte.Where(o => o.LigaZugang).OrderBy(o => o.Id).ToList();
            var nichtLigaOrte = regionOrte.Where(o => !o.LigaZugang).ToList();

            // Arenen in fester Reihenfolge (nach OrdenNr), Nicht-Arenen zufällig mischen
            var arenaOrte = nichtLigaOrte.Where(o => o.Arena != null)
                .OrderBy(o => o.Arena!.OrdenNr).ToList();
            var nichtArenaOrte = nichtLigaOrte.Where(o => o.Arena == null).ToList();
            // Fisher-Yates Shuffle der Nicht-Arena-Orte
            for (int s = nichtArenaOrte.Count - 1; s > 0; s--)
            {
                int j = rng.Next(s + 1);
                (nichtArenaOrte[s], nichtArenaOrte[j]) = (nichtArenaOrte[j], nichtArenaOrte[s]);
            }
            // Orte zusammenbauen: Nicht-Arena-Orte gleichmäßig zwischen Arenen verteilen
            var gemischteOrte = new List<Ort>();
            int nichtArenaProAbschnitt = arenaOrte.Count > 0
                ? nichtArenaOrte.Count / (arenaOrte.Count + 1)
                : nichtArenaOrte.Count;
            int nichtArenaIdx = 0;
            // Orte vor erster Arena
            int vorErsteArena = Math.Max(1, nichtArenaProAbschnitt);
            for (int s = 0; s < vorErsteArena && nichtArenaIdx < nichtArenaOrte.Count; s++, nichtArenaIdx++)
                gemischteOrte.Add(nichtArenaOrte[nichtArenaIdx]);
            foreach (var arena in arenaOrte)
            {
                gemischteOrte.Add(arena);
                int anzahl = Math.Max(1, nichtArenaProAbschnitt);
                for (int s = 0; s < anzahl && nichtArenaIdx < nichtArenaOrte.Count; s++, nichtArenaIdx++)
                    gemischteOrte.Add(nichtArenaOrte[nichtArenaIdx]);
            }
            // Übrige Nicht-Arena-Orte ans Ende
            while (nichtArenaIdx < nichtArenaOrte.Count)
                gemischteOrte.Add(nichtArenaOrte[nichtArenaIdx++]);

            // Liga-Orte (Siegesstraße, Indigo-Plateau usw.) als festen Abschluss
            gemischteOrte.AddRange(ligaOrte);

            var kopien = gemischteOrte.Select(o => KopiereOrt(o, ordenOffset)).ToList();

            foreach (var k in kopien)
            {
                k.Nord = null; k.Sued = null; k.Ost = null; k.West = null;
                k.NordMinOrden = 0; k.SuedMinOrden = 0; k.OstMinOrden = 0; k.WestMinOrden = 0;
                k.SperrNord = null; k.SperrSued = null; k.SperrOst = null; k.SperrWest = null;
                k.MinOrdenFürZugang = ordenOffset;
            }

            int arenaIdx = 0;
            foreach (var k in kopien)
            {
                if (k.Arena != null)
                {
                    k.MinOrdenFürZugang = ordenOffset + arenaIdx;
                    arenaIdx++;
                }
            }

            // Hauptpfad: Ost→West (horizontal), Y=0
            for (int i = 0; i < kopien.Count - 1; i++)
            {
                var a = kopien[i];
                var b = kopien[i + 1];
                a.Ost  = b.Id;
                b.West = a.Id;
            }

            // X/Y-Koordinaten für Hauptpfad setzen
            for (int i = 0; i < kopien.Count; i++)
            {
                meta.OrtKoordinaten[kopien[i].Id] = (globalX + i, 0);
            }

            // Seitenabzweigungen: nach oben (Y=-1) oder unten (Y=+1)
            // Jede Abzweigung ist ein einzelner Ort der über Nord/Süd erreichbar ist
            int abzweigRichtung = 1; // abwechselnd oben/unten
            for (int i = 2; i < kopien.Count - 2; i += rng.Next(4, 8))
            {
                int zielIdx = Math.Min(i + rng.Next(2, 5), kopien.Count - 1);
                var a = kopien[i];
                var b = kopien[zielIdx];

                // Abzweigung: a bekommt eine Nord- oder Süd-Verbindung zu b
                // (b wird dadurch "oben" oder "unten" vom Hauptpfad)
                if (abzweigRichtung > 0 && string.IsNullOrEmpty(a.Nord) && string.IsNullOrEmpty(b.Sued))
                {
                    a.Nord = b.Id;
                    b.Sued = a.Id;
                    // b bekommt Y=-1 (oben), X = Mitte zwischen a und zielIdx
                    int midX = (meta.OrtKoordinaten[a.Id].X + meta.OrtKoordinaten[b.Id].X) / 2;
                    meta.OrtKoordinaten[b.Id] = (midX, -1);
                }
                else if (abzweigRichtung < 0 && string.IsNullOrEmpty(a.Sued) && string.IsNullOrEmpty(b.Nord))
                {
                    a.Sued = b.Id;
                    b.Nord = a.Id;
                    int midX = (meta.OrtKoordinaten[a.Id].X + meta.OrtKoordinaten[b.Id].X) / 2;
                    meta.OrtKoordinaten[b.Id] = (midX, 1);
                }
                abzweigRichtung = -abzweigRichtung;
            }

            // Regionen verbinden: letzter Ort der vorherigen Region → erster Ort dieser Region
            if (ergebnis.Any() && kopien.Any())
            {
                var letzter = ergebnis.Last();
                var erster  = kopien.First();
                letzter.Ost = erster.Id;
                erster.West = letzter.Id;
            }

            ergebnis.AddRange(kopien);
            ordenOffset += regCfg.Arenaleiter.Count;
            globalX += kopien.Count + 1; // +1 Abstand zwischen Regionen
        }

        meta.StartOrtId = ergebnis.FirstOrDefault()?.Id ?? "";
        meta.OrtReihenfolge = ergebnis.Select(o => o.Id).ToList();

        // BFS: kürzeste Distanz vom Startort berechnen (= Level-Basis)
        if (!string.IsNullOrEmpty(meta.StartOrtId))
        {
            var ortMap = ergebnis.ToDictionary(o => o.Id);
            var queue = new Queue<string>();
            queue.Enqueue(meta.StartOrtId);
            meta.OrtDistanzen[meta.StartOrtId] = 0;
            while (queue.Count > 0)
            {
                var aktId = queue.Dequeue();
                if (!ortMap.TryGetValue(aktId, out var akt)) continue;
                int aktDist = meta.OrtDistanzen[aktId];
                foreach (var nachbarId in new[] { akt.Nord, akt.Sued, akt.Ost, akt.West }
                    .Where(n => !string.IsNullOrEmpty(n)))
                {
                    if (!meta.OrtDistanzen.ContainsKey(nachbarId!))
                    {
                        meta.OrtDistanzen[nachbarId!] = aktDist + 1;
                        queue.Enqueue(nachbarId!);
                    }
                }
            }
        }

        // Level direkt in die Ort-Objekte schreiben (Distanz = Level-Basis)
        foreach (var ort in ergebnis)
        {
            if (!meta.OrtDistanzen.TryGetValue(ort.Id, out int dist)) continue;
            bool istArena = ort.Arena != null;
            int minLvl = dist + 1;
            int maxLvl = dist + 2;
            if (istArena) { minLvl += 2; maxLvl += 3; }

            // WildMonster-Level überschreiben
            if (ort.WildMonster != null)
                foreach (var w in ort.WildMonster)
                { w.MinLevel = minLvl; w.MaxLevel = maxLvl; }

            // Trainer-Team-Level überschreiben
            if (ort.Trainer != null)
                foreach (var t in ort.Trainer)
                    if (t.Team != null)
                        foreach (var m in t.Team)
                            m.Level = minLvl + 1; // Trainer etwas stärker

            // Arena-Team-Level überschreiben
            if (ort.Arena?.Team != null)
                foreach (var m in ort.Arena.Team)
                    m.Level = maxLvl + 1;
        }

        // Fog-of-War: Orte bis zur ersten Arena (inkl.) freischalten
        int ersteArenaIdx = ergebnis.FindIndex(o => o.Arena != null);
        int bisIdx = ersteArenaIdx >= 0 ? ersteArenaIdx : Math.Min(4, ergebnis.Count - 1);
        meta.FreigeschalteBisIndex = bisIdx;
        for (int i = 0; i <= bisIdx; i++)
            meta.FreigeschalteteOrte.Add(ergebnis[i].Id);

        return (ergebnis, meta);
    }

    private static Ort KopiereOrt(Ort original, int ordenOffset)
    {
        // TIEFE KOPIE: WildMonster, Trainer-Teams und Arena werden neu erstellt
        // damit der Generator die Level ändern kann ohne die Originaldaten zu verändern
        var wildKopie = original.WildMonster?
            .Select(w => new WildBegegnung
            {
                MonsterId = w.MonsterId,
                MinLevel  = w.MinLevel,
                MaxLevel  = w.MaxLevel,
                Chance    = w.Chance
            }).ToList();

        var trainerKopie = original.Trainer?
            .Select(t => new TrainerKampf
            {
                Id           = t.Id,
                Name         = t.Name,
                Klasse       = t.Klasse,
                Belohnung    = t.Belohnung,
                Dialogvor    = t.Dialogvor,
                DialogNach   = t.DialogNach,
                MussBesiegt  = t.MussBesiegt,
                SperrtRichtung = t.SperrtRichtung,
                Team = t.Team.Select(m => new MonsterTeamEintrag
                {
                    MonsterId = m.MonsterId,
                    Level     = m.Level
                }).ToList()
            }).ToList();

        Arena? arenaKopie = null;
        if (original.Arena != null)
        {
            arenaKopie = new Arena
            {
                OrdenName          = original.Arena.OrdenName,
                OrdenNr            = original.Arena.OrdenNr + ordenOffset,
                Leiter             = original.Arena.Leiter,
                TypSpezialisierung = original.Arena.TypSpezialisierung,
                Beschreibung       = original.Arena.Beschreibung,
                Team = original.Arena.Team.Select(m => new MonsterTeamEintrag
                {
                    MonsterId = m.MonsterId,
                    Level     = m.Level
                }).ToList()
            };
        }

        return new Ort
        {
            Id               = original.Id,
            Name             = original.Name,
            Typ              = original.Typ,
            Farbe            = original.Farbe,
            GridX            = original.GridX,
            GridY            = original.GridY,
            Beschreibung     = original.Beschreibung,
            Arena            = arenaKopie,
            WildMonster      = wildKopie,
            Verbindungen     = new List<string>(),
            Trainer          = trainerKopie,
            HatMonsterCenter = original.HatMonsterCenter,
            HatMarkt         = original.HatMarkt,
            MarktAngebot     = original.MarktAngebot,
            NPCs             = original.NPCs,
            IstUnterirdisch  = original.IstUnterirdisch,
            LigaZugang       = false,
            BenötigtItem     = null,
            MinOrdenFürZugang = ordenOffset,
            MaxOrdenFürSperre = 0,
            Nord = null, Sued = null, Ost = null, West = null,
            NordTyp = "normal", SuedTyp = "normal", OstTyp = "normal", WestTyp = "normal",
            NordMinOrden = 0, SuedMinOrden = 0, OstMinOrden = 0, WestMinOrden = 0,
        };
    }

    public static string ExportiereKartenCode(GenerierteKarte meta)
        => $"{meta.SeedCode}:{string.Join(",", meta.RegionsReihenfolge)}";

    public static (string Seed, List<string> Regionen)? ImportiereKartenCode(string code)
    {
        var parts = code.Trim().Split(':', 2);
        if (parts.Length != 2) return null;
        var seed = parts[0].Trim();
        var regionen = parts[1].Split(',').Select(r => r.Trim().ToUpper()).ToList();
        if (seed.Length < 8 || !regionen.Any()) return null;
        return (seed, regionen);
    }
}
