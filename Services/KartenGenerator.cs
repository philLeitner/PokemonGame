using MonsterKampf.Models;
using System.Text;
using System.Text.Json;

namespace MonsterKampf.Services;

// ─── Generierte Karte ────────────────────────────────────────────────────────
public class GenerierteKarte
{
    public string SeedCode { get; set; } = "";
    public List<string> RegionsReihenfolge { get; set; } = new(); // ["KAN","JOH",...]
    public List<GenerierteEbene> Ebenen { get; set; } = new();
    public int AktuelleEbeneIndex { get; set; } = 0;
    public int FreigeschalteBisIndex { get; set; } = 0; // Nebel des Krieges
}

public class GenerierteEbene
{
    public int Index { get; set; }           // 0-basierter Index in der Gesamtliste
    public string Id { get; set; } = "";     // z.B. "KAN-E001"
    public string Name { get; set; } = "";   // z.B. "Route 1", "Mondberg EG"
    public EbenenTyp Typ { get; set; }
    public string RegionId { get; set; } = "";
    public int RegionIndex { get; set; }     // 0 = erste Region
    // Grid-Position (für Anzeige)
    public int GridX { get; set; }
    public int GridY { get; set; }
    // Verbindungen (Indices der verbundenen Ebenen)
    public List<int> Verbindungen { get; set; } = new();
    // Arenaleiter-Daten (wenn Typ == Arenaleiter)
    public string? ArenaLeiterName { get; set; }
    public string? ArenaTyp { get; set; }
    public string? OrdenName { get; set; }
    public int OrdenNummer { get; set; }
    // Professor-Daten (wenn Typ == Professor)
    public string? ProfessorName { get; set; }
    // Sperrung: erst nach X Orden zugänglich
    public int MinOrden { get; set; } = 0;
    // Farbe für Anzeige
    public string Farbe { get; set; } = "#374151";
}

public enum EbenenTyp
{
    Route,
    Stadt,
    Hoehle,
    Wald,
    Gebaeude,
    Insel,
    Arenaleiter,
    Professor   // Startpunkt jeder Region (Ebene 0 = Profzone)
}

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

// ─── Generator ───────────────────────────────────────────────────────────────
public class KartenGenerator
{
    private static readonly char[] SeedChars =
        "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    // Ebenentyp-Namen für die Anzeige
    private static readonly string[] RoutenNamen = {
        "Route", "Pfad", "Weg", "Straße", "Pass"
    };
    private static readonly string[] HoehlenNamen = {
        "Höhle", "Grotte", "Tunnel", "Schacht", "Stollen"
    };
    private static readonly string[] WaldNamen = {
        "Wald", "Forst", "Hain", "Dickicht", "Wildnis"
    };
    private static readonly string[] StadtNamen = {
        "Stadt", "Dorf", "Siedlung", "Hafen", "Marktort"
    };
    private static readonly string[] GebaeudeNamen = {
        "Turm", "Kraftwerk", "Fabrik", "Labor", "Ruine"
    };
    private static readonly string[] InselNamen = {
        "Insel", "Eiland", "Atoll", "Riff", "Archipel"
    };

    // Farben pro Ebenentyp
    private static readonly Dictionary<EbenenTyp, string> TypFarben = new()
    {
        { EbenenTyp.Route,       "#2d4a2d" },
        { EbenenTyp.Stadt,       "#4a3a6b" },
        { EbenenTyp.Hoehle,      "#4a3a2d" },
        { EbenenTyp.Wald,        "#1a4a1a" },
        { EbenenTyp.Gebaeude,    "#4a4a1a" },
        { EbenenTyp.Insel,       "#1a3a4a" },
        { EbenenTyp.Arenaleiter, "#6b1a1a" },
        { EbenenTyp.Professor,   "#1a4a6b" },
    };

    /// <summary>Generiert einen zufälligen 16-stelligen Seed-Code.</summary>
    public static string GeneriereSeedCode()
    {
        var rng = new Random();
        var sb = new StringBuilder(16);
        for (int i = 0; i < 16; i++)
            sb.Append(SeedChars[rng.Next(SeedChars.Length)]);
        return sb.ToString();
    }

    /// <summary>Wandelt einen Seed-Code in eine deterministische Zahl um.</summary>
    private static int SeedZuInt(string seed)
    {
        int hash = 17;
        foreach (char c in seed)
            hash = hash * 31 + c;
        return Math.Abs(hash);
    }

    /// <summary>Generiert eine komplette Karte aus Seed + Regionsauswahl.</summary>
    public static GenerierteKarte Generiere(string seedCode, List<string> regionsReihenfolge, List<RegionConfig> alleRegionen)
    {
        var rng = new Random(SeedZuInt(seedCode));
        var karte = new GenerierteKarte
        {
            SeedCode = seedCode,
            RegionsReihenfolge = regionsReihenfolge
        };

        int globalIndex = 0;
        int ordenOffset = 0; // Orden-Nummer über alle Regionen

        foreach (var regId in regionsReihenfolge)
        {
            var regCfg = alleRegionen.FirstOrDefault(r => r.Id == regId);
            if (regCfg == null) continue;

            int regIndex = regionsReihenfolge.IndexOf(regId);

            // Ebenenanzahl: BasisEbenen ±3
            int anzahlEbenen = regCfg.BasisEbenen + rng.Next(-3, 4);

            // Ebenen für diese Region generieren
            var regionEbenen = GeneriereRegionEbenen(rng, regCfg, regIndex, anzahlEbenen, ordenOffset);

            // Globale Indices zuweisen
            foreach (var ebene in regionEbenen)
            {
                ebene.Index = globalIndex++;
                karte.Ebenen.Add(ebene);
            }

            ordenOffset += regCfg.Arenaleiter.Count;
        }

        // Grid-Positionen berechnen (9×9 scrollbares Grid)
        BerechneGridPositionen(karte.Ebenen, rng);

        return karte;
    }

    private static List<GenerierteEbene> GeneriereRegionEbenen(
        Random rng, RegionConfig cfg, int regIndex, int anzahl, int ordenOffset)
    {
        var ebenen = new List<GenerierteEbene>();
        int localIdx = 0;

        // ── Ebene 0: Professor-Start ──────────────────────────────────────
        ebenen.Add(new GenerierteEbene
        {
            Id = $"{cfg.Id}-E{localIdx:D3}",
            Name = $"{cfg.Professor}s Labor",
            Typ = EbenenTyp.Professor,
            RegionId = cfg.Id,
            RegionIndex = regIndex,
            ProfessorName = cfg.Professor,
            Farbe = TypFarben[EbenenTyp.Professor],
            MinOrden = ordenOffset == 0 ? 0 : ordenOffset
        });
        localIdx++;

        // ── Arenaleiter gleichmäßig verteilen ────────────────────────────
        int arenaCount = cfg.Arenaleiter.Count;
        // Abstand zwischen Arenaen (ohne Prof-Ebene und ohne letzte Arena)
        int routenProAbschnitt = (anzahl - 1 - arenaCount) / arenaCount;
        if (routenProAbschnitt < 2) routenProAbschnitt = 2;

        // Ebenentyp-Verteilung für normale Ebenen
        // 50% Route, 20% Höhle, 15% Wald, 10% Stadt, 5% Gebäude/Insel
        var normalTypen = new (EbenenTyp Typ, int Gewicht)[]
        {
            (EbenenTyp.Route,    50),
            (EbenenTyp.Hoehle,   20),
            (EbenenTyp.Wald,     15),
            (EbenenTyp.Stadt,    10),
            (EbenenTyp.Gebaeude,  3),
            (EbenenTyp.Insel,     2),
        };
        int totalGewicht = normalTypen.Sum(t => t.Gewicht);

        int routeNummer = 1;
        int hoehleNummer = 1;
        int waldNummer = 1;
        int stadtNummer = 1;
        int gebaeudeNummer = 1;
        int inselNummer = 1;

        for (int arenaIdx = 0; arenaIdx < arenaCount; arenaIdx++)
        {
            var arena = cfg.Arenaleiter[arenaIdx];
            int ordenNr = ordenOffset + arenaIdx + 1;

            // Routen vor der Arena
            int routenVorArena = routenProAbschnitt + rng.Next(-1, 2);
            if (routenVorArena < 1) routenVorArena = 1;

            for (int r = 0; r < routenVorArena && localIdx < anzahl - arenaCount + arenaIdx; r++)
            {
                var typ = WähleTyp(rng, normalTypen, totalGewicht);
                string name = GeneriereEbenenName(rng, typ, cfg.Name,
                    ref routeNummer, ref hoehleNummer, ref waldNummer,
                    ref stadtNummer, ref gebaeudeNummer, ref inselNummer);

                ebenen.Add(new GenerierteEbene
                {
                    Id = $"{cfg.Id}-E{localIdx:D3}",
                    Name = name,
                    Typ = typ,
                    RegionId = cfg.Id,
                    RegionIndex = regIndex,
                    Farbe = TypFarben[typ],
                    MinOrden = ordenOffset + arenaIdx // Erst nach vorheriger Arena zugänglich
                });
                localIdx++;
            }

            // Arena-Ebene
            ebenen.Add(new GenerierteEbene
            {
                Id = $"{cfg.Id}-ARENA{arenaIdx + 1:D2}",
                Name = $"Arena: {arena.Name}",
                Typ = EbenenTyp.Arenaleiter,
                RegionId = cfg.Id,
                RegionIndex = regIndex,
                ArenaLeiterName = arena.Name,
                ArenaTyp = arena.Typ,
                OrdenName = arena.Orden,
                OrdenNummer = ordenNr,
                Farbe = TypFarben[EbenenTyp.Arenaleiter],
                MinOrden = ordenOffset + arenaIdx
            });
            localIdx++;
        }

        // Restliche Ebenen auffüllen
        while (ebenen.Count < anzahl)
        {
            var typ = WähleTyp(rng, normalTypen, totalGewicht);
            string name = GeneriereEbenenName(rng, typ, cfg.Name,
                ref routeNummer, ref hoehleNummer, ref waldNummer,
                ref stadtNummer, ref gebaeudeNummer, ref inselNummer);

            ebenen.Add(new GenerierteEbene
            {
                Id = $"{cfg.Id}-E{localIdx:D3}",
                Name = name,
                Typ = typ,
                RegionId = cfg.Id,
                RegionIndex = regIndex,
                Farbe = TypFarben[typ],
                MinOrden = ordenOffset + arenaCount - 1
            });
            localIdx++;
        }

        // Verbindungen: jede Ebene verbindet sich mit der nächsten (Hauptpfad)
        // + zufällige Seitenverbindungen (10% Chance)
        for (int i = 0; i < ebenen.Count - 1; i++)
        {
            ebenen[i].Verbindungen.Add(i + 1);
            ebenen[i + 1].Verbindungen.Add(i);
        }
        // Seitenverbindungen
        for (int i = 0; i < ebenen.Count - 2; i++)
        {
            if (rng.Next(10) == 0) // 10% Chance
            {
                int ziel = i + 2 + rng.Next(Math.Min(3, ebenen.Count - i - 2));
                if (!ebenen[i].Verbindungen.Contains(ziel))
                {
                    ebenen[i].Verbindungen.Add(ziel);
                    ebenen[ziel].Verbindungen.Add(i);
                }
            }
        }

        return ebenen;
    }

    private static EbenenTyp WähleTyp(Random rng, (EbenenTyp Typ, int Gewicht)[] typen, int total)
    {
        int roll = rng.Next(total);
        int sum = 0;
        foreach (var (typ, gewicht) in typen)
        {
            sum += gewicht;
            if (roll < sum) return typ;
        }
        return EbenenTyp.Route;
    }

    private static string GeneriereEbenenName(Random rng, EbenenTyp typ, string regionName,
        ref int routeNr, ref int hoehleNr, ref int waldNr,
        ref int stadtNr, ref int gebaeudeNr, ref int inselNr)
    {
        return typ switch
        {
            EbenenTyp.Route    => $"Route {routeNr++}",
            EbenenTyp.Hoehle   => $"{regionName}-{HoehlenNamen[rng.Next(HoehlenNamen.Length)]} {hoehleNr++}",
            EbenenTyp.Wald     => $"{WaldNamen[rng.Next(WaldNamen.Length)]} {waldNr++}",
            EbenenTyp.Stadt    => $"{StadtNamen[rng.Next(StadtNamen.Length)]} {stadtNr++}",
            EbenenTyp.Gebaeude => $"{GebaeudeNamen[rng.Next(GebaeudeNamen.Length)]} {gebaeudeNr++}",
            EbenenTyp.Insel    => $"{InselNamen[rng.Next(InselNamen.Length)]} {inselNr++}",
            _                  => $"Route {routeNr++}"
        };
    }

    /// <summary>Berechnet Grid-Positionen für alle Ebenen (scrollbares 9×9 Raster).</summary>
    private static void BerechneGridPositionen(List<GenerierteEbene> ebenen, Random rng)
    {
        if (!ebenen.Any()) return;

        // Hauptpfad: schlängelt sich durch das Grid
        // Jede Ebene bekommt eine Position im Grid
        // Grid ist 9 Spalten breit, beliebig hoch
        int x = 4; // Startmitte
        int y = 0;
        int dir = 0; // 0=rechts, 1=runter, 2=links, 3=runter

        var belegte = new HashSet<(int, int)>();

        for (int i = 0; i < ebenen.Count; i++)
        {
            // Sicherstellen dass Position frei ist
            while (belegte.Contains((x, y)))
            {
                y++;
            }
            ebenen[i].GridX = x;
            ebenen[i].GridY = y;
            belegte.Add((x, y));

            if (i < ebenen.Count - 1)
            {
                // Nächste Position: hauptsächlich nach unten, leicht zufällig nach links/rechts
                int dx = rng.Next(-1, 2); // -1, 0, oder 1
                int newX = Math.Clamp(x + dx, 0, 8);
                x = newX;
                y++;
            }
        }
    }

    /// <summary>Exportiert eine Karte als kompakter Code (Seed + Regionen).</summary>
    public static string ExportiereKartenCode(GenerierteKarte karte)
    {
        // Format: SEED:REG1,REG2,...
        return $"{karte.SeedCode}:{string.Join(",", karte.RegionsReihenfolge)}";
    }

    /// <summary>Importiert eine Karte aus einem Code.</summary>
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
