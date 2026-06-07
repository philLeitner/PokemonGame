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
    public List<MonsterPoolEintrag> MonsterPool { get; set; } = new();
    public List<TrainerPoolEintrag> TrainerPool { get; set; } = new();
}
public class ArenaLeiterConfig
{
    public string Name { get; set; } = "";
    public string Typ { get; set; } = "";
    public string Orden { get; set; } = "";
}
public class MonsterPoolEintrag
{
    public string Id { get; set; } = "";
    public int MinLevel { get; set; } = 3;
    public int MaxLevel { get; set; } = 60;
    public int Chance { get; set; } = 10;
}
public class TrainerPoolEintrag
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Klasse { get; set; } = "Trainer";
    public int Belohnung { get; set; } = 100;
    public List<MonsterTeamEintrag> Team { get; set; } = new();
}

// ─── Interne Ebenen-Struktur (wie HTML-Generator) ────────────────────────────
internal class Ebene
{
    public int Id { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public Dictionary<string, int> Exits { get; set; } = new();  // "left"/"right"/"up"/"down" → Ebenen-ID
    public Dictionary<string, int> Locks { get; set; } = new();  // Richtung → Stadt-Ebenen-ID
    public bool IstStadt { get; set; }
    public bool IstBoss { get; set; }
    public bool IstStart { get; set; }
    public bool IstHauptpfad { get; set; }
    public bool IstSackgasse { get; set; }
    public int? NachBoss { get; set; }  // Startpunkt: nach welchem Boss-Index
    public bool Besucht { get; set; }
}

// ─── Generierte Karte (Metadaten für Speichern/Laden + Fog-of-War) ───────────
public class GenerierteKarte
{
    public string SeedCode { get; set; } = "";
    public List<string> RegionsReihenfolge { get; set; } = new();
    public string StartOrtId { get; set; } = "";
    public List<string> OrtReihenfolge { get; set; } = new();
    public int FreigeschalteBisIndex { get; set; } = 5;
    public HashSet<string> FreigeschalteteOrte { get; set; } = new();
    public HashSet<string> BesiegteArenen { get; set; } = new();
    public Dictionary<string, (int X, int Y)> OrtKoordinaten { get; set; } = new();
    public Dictionary<string, int> OrtDistanzen { get; set; } = new();
    public List<string> StadtIds { get; set; } = new();
    public List<string> BossIds { get; set; } = new();
    public List<string> StartIds { get; set; } = new();
    public int StädteProBoss { get; set; } = 3;
}

// ─── Generator ───────────────────────────────────────────────────────────────
public class KartenGenerator
{
    private static readonly char[] SeedChars =
        "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    private static readonly string[] Dirs = { "left", "right", "up", "down" };
    private static readonly Dictionary<string, string> Opposite = new()
    {
        {"left","right"},{"right","left"},{"up","down"},{"down","up"}
    };
    private static readonly Dictionary<string, (int dx, int dy)> MovePos = new()
    {
        {"left",(-1,0)},{"right",(1,0)},{"up",(0,-1)},{"down",(0,1)}
    };

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

    // ─── Haupt-Generierungsmethode ────────────────────────────────────────────
    public static (List<Ort> Orte, GenerierteKarte Meta) Generiere(
        string seedCode,
        List<string> regionsReihenfolge,
        List<RegionConfig> alleRegionen,
        List<Ort>? alleOrte = null)
    {
        var rng = new Random(SeedZuInt(seedCode));
        var ergebnis = new List<Ort>();
        var meta = new GenerierteKarte
        {
            SeedCode = seedCode,
            RegionsReihenfolge = regionsReihenfolge
        };

        int globalOrdenOffset = 0;

        foreach (var regId in regionsReihenfolge)
        {
            var regCfg = alleRegionen.FirstOrDefault(r => r.Id == regId);
            if (regCfg == null) continue;

            int total = Math.Max(20, regCfg.BasisEbenen);
            int bossCount = regCfg.Arenaleiter.Count > 0 ? regCfg.Arenaleiter.Count : 8;
            int städteProBoss = regCfg.Arenaleiter.Count > 0 ? regCfg.Arenaleiter.Count : 8;
            int totalCitiesWanted = Math.Min(Math.Max(0, total - 2 - bossCount), städteProBoss * bossCount);

            // Trainer-Pool direkt aus regionen.json
            var trainerPool = regCfg.TrainerPool.Select(t => new TrainerKampf
            {
                Id = t.Id,
                Name = t.Name,
                Klasse = t.Klasse,
                Belohnung = t.Belohnung,
                Team = t.Team
            }).ToList();
            var wildPool = regCfg.MonsterPool;

            // ─── HTML-Algorithmus: Ebenen generieren ─────────────────────────
            var ebenen = GeneriereEbenenStruktur(rng, total, bossCount, städteProBoss, totalCitiesWanted);
            var distanzen = CalcDistances(ebenen);

            // ─── Ebenen → Ort-Objekte ─────────────────────────────────────────
            int stadtZähler = 0, bossZähler = 0, startZähler = 0, ebeneZähler = 0;
            var ebeneZuOrtId = new Dictionary<int, string>();

            // Erst IDs vergeben
            foreach (var eb in ebenen)
            {
                string ortId;
                if (eb.IstStart)
                {
                    startZähler++;
                    ortId = $"{regId}-GEN-START{startZähler:D2}";
                }
                else if (eb.IstBoss)
                {
                    bossZähler++;
                    ortId = $"{regId}-GEN-BOSS{bossZähler:D2}";
                }
                else if (eb.IstStadt)
                {
                    stadtZähler++;
                    ortId = $"{regId}-GEN-STADT{stadtZähler:D2}";
                }
                else
                {
                    ebeneZähler++;
                    ortId = $"{regId}-GEN-EBENE{ebeneZähler:D3}";
                }
                ebeneZuOrtId[eb.Id] = ortId;
            }

            // Zähler zurücksetzen für zweiten Durchlauf
            stadtZähler = 0; bossZähler = 0; startZähler = 0; ebeneZähler = 0;
            var ebeneZuOrt = new Dictionary<int, Ort>();
            var arenaleiter = regCfg.Arenaleiter.ToList();

            foreach (var eb in ebenen)
            {
                string ortId = ebeneZuOrtId[eb.Id];
                string name, typ, farbe;
                bool istStartOrt = false;
                Arena? arena = null;
                var trainer = new List<TrainerKampf>();
                var wildMonster = new List<WildBegegnung>();
                bool hatMonsterCenter = false, hatMarkt = false;
                var npcs = new List<GesprächsNPC>();
                int dist = distanzen.TryGetValue(eb.Id, out int dd) ? (dd == int.MaxValue ? 0 : dd) : 0;

                if (eb.IstStart)
                {
                    startZähler++;
                    name = startZähler == 1 ? "Startpunkt" : $"Start {startZähler}";
                    typ = "ort"; farbe = "blue";
                    istStartOrt = startZähler == 1;
                    hatMonsterCenter = true;
                    hatMarkt = true;

                    if (startZähler == 1)
                    {
                        // Professor-NPC beim ersten Startpunkt
                        npcs.Add(new GesprächsNPC
                        {
                            Id = $"{regId}-PROF",
                            Name = regCfg.Professor,
                            Emoji = regCfg.ProfessorEmoji,
                            Dialog = $"Herzlich willkommen! Ich bin {regCfg.Professor}. " +
                                     $"Dieses Spiel wurde mit viel Leidenschaft entwickelt – es ist eine Liebeserklärung an die Welt der Monster-Abenteuer. " +
                                     $"Deine Reise beginnt hier. Erkunde die Welt, besiege Bosse und werde zum Champion!",
                            GibtItemId = null, // Karte erst nach Wizard
                            GibtItemName = null,
                            GibtItemEmoji = null,
                            DialogNachGeschenk = null,
                            IstProfessor = true
                        });
                    }
                    else
                    {
                        hatMonsterCenter = true;
                        npcs.Add(new GesprächsNPC
                        {
                            Id = $"{regId}-NPC-START{startZähler}",
                            Name = "Assistent",
                            Emoji = "🧑‍🔬",
                            Dialog = $"Du hast Boss {startZähler - 1} besiegt! Weiter geht's – das nächste Kapitel wartet auf dich!"
                        });
                    }
                }
                else if (eb.IstBoss)
                {
                    bossZähler++;
                    var leiterCfg = bossZähler <= arenaleiter.Count ? arenaleiter[bossZähler - 1] : null;

                    // Letzter Boss = Pokémon-Liga (Champion), vorletzter = Top 4
                    bool istLetzterBoss  = bossZähler == bossCount;
                    bool istVorletzter   = bossZähler == bossCount - 1;

                    // Trainer aus Pool nach Klasse auswählen
                    TrainerKampf? arenaTrainer;
                    if (istLetzterBoss)
                    {
                        // Champion (Endgegner)
                        arenaTrainer = trainerPool
                            .Where(t => t.Klasse == "Endgegner")
                            .FirstOrDefault()
                            ?? trainerPool
                                .Where(t => t.Klasse == "Arena" || t.Klasse == "Hauptboss")
                                .OrderByDescending(t => t.Team.Any() ? t.Team.Max(m => m.Level) : 0)
                                .FirstOrDefault();
                        name = "Pokémon-Liga";
                        farbe = "#6a0dad";
                    }
                    else if (istVorletzter)
                    {
                        // Top 4 (Hauptbosse)
                        arenaTrainer = trainerPool
                            .Where(t => t.Klasse == "Hauptboss")
                            .OrderBy(t => t.Team.Any() ? t.Team.Max(m => m.Level) : 0)
                            .FirstOrDefault()
                            ?? trainerPool
                                .Where(t => t.Klasse == "Arena")
                                .OrderByDescending(t => t.Team.Any() ? t.Team.Max(m => m.Level) : 0)
                                .FirstOrDefault();
                        name = "Top 4";
                        farbe = "#8b0000";
                    }
                    else
                    {
                        // Normaler Arenaleiter
                        arenaTrainer = trainerPool
                            .Where(t => t.Klasse == "Arena")
                            .OrderBy(t => t.Team.Any() ? t.Team.Max(m => m.Level) : 0)
                            .Skip(bossZähler - 1)
                            .FirstOrDefault();
                        string leiterName = leiterCfg?.Name ?? arenaTrainer?.Name ?? $"Arena {bossZähler}";
                        name = $"Arena: {leiterName}";
                        farbe = "black";
                    }

                    typ = "stadt";
                    hatMonsterCenter = true;

                    // Zwischenboss-Trainer (Vortrainer) nur bei normalen Arenen
                    if (!istLetzterBoss && !istVorletzter)
                    {
                        var zwischen = trainerPool
                            .Where(t => t.Klasse == "Zwischenboss")
                            .OrderBy(t => t.Team.Any() ? t.Team.Max(m => m.Level) : 0)
                            .Skip((bossZähler - 1) * 2)
                            .Take(2)
                            .ToList();
                        trainer.AddRange(zwischen);
                    }
                    else if (istVorletzter)
                    {
                        // Top 4: alle 4 Hauptboss-Trainer als Pflicht-Kämpfe
                        var top4 = trainerPool
                            .Where(t => t.Klasse == "Hauptboss")
                            .OrderBy(t => t.Team.Any() ? t.Team.Max(m => m.Level) : 0)
                            .ToList();
                        trainer.AddRange(top4);
                    }

                    if (arenaTrainer != null)
                    {
                        arena = new Arena
                        {
                            OrdenName = istLetzterBoss ? "Champion-Titel" :
                                        istVorletzter  ? "Top-4-Sieg" :
                                        (leiterCfg?.Orden ?? $"Orden {bossZähler + globalOrdenOffset}"),
                            OrdenNr = bossZähler + globalOrdenOffset,
                            Leiter = istLetzterBoss ? (arenaTrainer.Name) :
                                     istVorletzter  ? "Top 4" :
                                     (leiterCfg?.Name ?? arenaTrainer.Name),
                            TypSpezialisierung = leiterCfg?.Typ ?? "",
                            Team = arenaTrainer.Team.Select(m => new MonsterTeamEintrag
                            {
                                MonsterId = m.MonsterId, Level = m.Level
                            }).ToList()
                        };
                    }
                    else
                    {
                        arena = new Arena
                        {
                            OrdenName = istLetzterBoss ? "Champion-Titel" :
                                        istVorletzter  ? "Top-4-Sieg" :
                                        (leiterCfg?.Orden ?? $"Orden {bossZähler + globalOrdenOffset}"),
                            OrdenNr = bossZähler + globalOrdenOffset,
                            Leiter = istLetzterBoss ? "Champion" :
                                     istVorletzter  ? "Top 4" :
                                     (leiterCfg?.Name ?? $"Arena {bossZähler}"),
                            TypSpezialisierung = leiterCfg?.Typ ?? "",
                            Team = new List<MonsterTeamEintrag>()
                        };
                    }
                }
                else if (eb.IstStadt)
                {
                    stadtZähler++;
                    name = $"Stadt {stadtZähler}";
                    typ = "stadt"; farbe = "red";
                    hatMonsterCenter = true;
                    hatMarkt = stadtZähler % 2 == 0;
                    trainer = HoleTrainerFürLevel(trainerPool, dist, rng, 1, 3);
                    wildMonster = HoleWildMonsterFürLevel(wildPool, dist, rng, 2, 4);
                }
                else
                {
                    ebeneZähler++;
                    name = $"Ebene {ebeneZähler}";
                    typ = "route";
                    farbe = eb.IstSackgasse ? "gray" : "green";
                    int trainerAnz = eb.IstSackgasse ? 1 : rng.Next(1, 4);
                    trainer = HoleTrainerFürLevel(trainerPool, dist, rng, trainerAnz, trainerAnz);
                    wildMonster = HoleWildMonsterFürLevel(wildPool, dist, rng, 2, 5);
                }

                var ort = new Ort
                {
                    Id = ortId,
                    Name = name,
                    Typ = typ,
                    Farbe = farbe,
                    GridX = eb.X,
                    GridY = eb.Y,
                    Arena = arena,
                    Trainer = trainer,
                    WildMonster = wildMonster,
                    HatMonsterCenter = hatMonsterCenter,
                    HatMarkt = hatMarkt,
                    NPCs = npcs,
                    IstStartOrt = istStartOrt,
                    Verbindungen = new List<string>()
                };

                ebeneZuOrt[eb.Id] = ort;
                ergebnis.Add(ort);
            }

            // Verbindungen setzen (Nord/Süd/Ost/West)
            foreach (var eb in ebenen)
            {
                var ort = ebeneZuOrt[eb.Id];
                foreach (var (dir, nachbarEbId) in eb.Exits)
                {
                    if (!ebeneZuOrtId.TryGetValue(nachbarEbId, out var nachbarOrtId)) continue;
                    switch (dir)
                    {
                        case "up":    ort.Nord = nachbarOrtId; break;
                        case "down":  ort.Sued = nachbarOrtId; break;
                        case "right": ort.Ost  = nachbarOrtId; break;
                        case "left":  ort.West = nachbarOrtId; break;
                    }
                    ort.Verbindungen.Add(nachbarOrtId);
                }
                // Sperren aus Locks (zufällige Seitenwege)
                foreach (var (dir, lockEbId) in eb.Locks)
                {
                    if (!ebeneZuOrt.TryGetValue(lockEbId, out var lockOrt)) continue;
                    // Sperre benötigt Orden der Arena (nicht nur Stadtbesuch)
                    string? ordenName = lockOrt.Arena?.OrdenName;
                    var sperre = ordenName != null
                        ? new RichtungsSperre { BenötigtOrdenName = ordenName, Hinweis = $"Benötigt Orden: {ordenName}" }
                        : new RichtungsSperre { Hinweis = $"Benötigt: {lockOrt.Name}" };
                    switch (dir)
                    {
                        case "up":    ort.SperrNord = sperre; break;
                        case "down":  ort.SperrSued = sperre; break;
                        case "right": ort.SperrOst  = sperre; break;
                        case "left":  ort.SperrWest = sperre; break;
                    }
                }
            }

            // Meta-Daten befüllen
            var startEbene = ebenen.First(e => e.IstStart);
            if (string.IsNullOrEmpty(meta.StartOrtId))
                meta.StartOrtId = ebeneZuOrtId[startEbene.Id];

            foreach (var eb in ebenen)
            {
                var ortId = ebeneZuOrtId[eb.Id];
                meta.OrtReihenfolge.Add(ortId);
                meta.OrtKoordinaten[ortId] = (eb.X, eb.Y);
                int d = distanzen.TryGetValue(eb.Id, out int dv) ? (dv == int.MaxValue ? 999 : dv) : 0;
                meta.OrtDistanzen[ortId] = d;
                if (eb.IstStadt) meta.StadtIds.Add(ortId);
                if (eb.IstBoss)  meta.BossIds.Add(ortId);
                if (eb.IstStart) meta.StartIds.Add(ortId);
            }
            meta.StädteProBoss = städteProBoss;

            // Level in Ort-Objekte schreiben
            foreach (var ort in ergebnis.Where(o => o.Id.StartsWith(regId + "-GEN-")))
            {
                if (!meta.OrtDistanzen.TryGetValue(ort.Id, out int dist2)) continue;
                int minLvl = Math.Max(3, 3 + (int)(dist2 * 1.8));
                int maxLvl = minLvl + 3;
                bool istArena = ort.Arena != null;
                if (istArena) { minLvl += 3; maxLvl += 5; }

                foreach (var w in ort.WildMonster)
                { w.MinLevel = minLvl; w.MaxLevel = maxLvl; }
                foreach (var t in ort.Trainer)
                    foreach (var m in t.Team)
                        m.Level = Math.Max(m.Level, minLvl + 1);
                if (ort.Arena?.Team != null)
                    foreach (var m in ort.Arena.Team)
                        m.Level = Math.Max(m.Level, maxLvl + 2);
            }

            // Fog-of-War: Startpunkt + direkte Nachbarn
            var startOrtObj = ebeneZuOrt[startEbene.Id];
            meta.FreigeschalteteOrte.Add(startOrtObj.Id);
            foreach (var nachbarId in startEbene.Exits.Values)
                if (ebeneZuOrtId.TryGetValue(nachbarId, out var nId))
                    meta.FreigeschalteteOrte.Add(nId);

            globalOrdenOffset += bossCount;
        }

        return (ergebnis, meta);
    }

    // ─── HTML-Algorithmus: Ebenen-Struktur generieren ────────────────────────
    private static List<Ebene> GeneriereEbenenStruktur(
        Random rng, int total, int bossCount, int städteProBoss, int totalCitiesWanted)
    {
        var ebenen = Enumerable.Range(0, total).Select(i => new Ebene { Id = i }).ToList();
        var occupied = new Dictionary<(int, int), int>();

        void MarkOccupied(int id) => occupied[(ebenen[id].X, ebenen[id].Y)] = id;
        bool FreeAt(int x, int y) => !occupied.ContainsKey((x, y));
        int ExitCount(int id) => ebenen[id].Exits.Count;

        void Connect(int a, string dir, int b, int? lockCity = null)
        {
            ebenen[a].Exits[dir] = b;
            ebenen[b].Exits[Opposite[dir]] = a;
            if (lockCity.HasValue)
            {
                ebenen[a].Locks[dir] = lockCity.Value;
                ebenen[b].Locks[Opposite[dir]] = lockCity.Value;
            }
        }

        bool PlaceAndConnect(int a, int b, int? lockCity = null)
        {
            foreach (var dir in Shuffled(Dirs, rng))
            {
                var (dx, dy) = MovePos[dir];
                int nx = ebenen[a].X + dx, ny = ebenen[a].Y + dy;
                if (!ebenen[a].Exits.ContainsKey(dir) &&
                    !ebenen[b].Exits.ContainsKey(Opposite[dir]) &&
                    ExitCount(a) < 4 && ExitCount(b) < 4 &&
                    FreeAt(nx, ny))
                {
                    ebenen[b].X = nx; ebenen[b].Y = ny;
                    Connect(a, dir, b, lockCity);
                    MarkOccupied(b);
                    return true;
                }
            }
            return false;
        }

        // Hauptpfad (Schlangenmuster wie HTML)
        const int cols = 26;
        ebenen[0].X = 0; ebenen[0].Y = 0; ebenen[0].IstHauptpfad = true; MarkOccupied(0);
        int mainEnd = Math.Min(total - 1, Math.Max(10, (int)Math.Ceiling(total * 0.45)));
        mainEnd = Math.Min(total - 1, Math.Max(mainEnd, totalCitiesWanted + bossCount + 4));

        for (int i = 1; i <= mainEnd; i++)
        {
            int row = i / cols, pos = i % cols;
            bool even = row % 2 == 0;
            ebenen[i].X = even ? pos : cols - 1 - pos;
            ebenen[i].Y = row * 2;
            ebenen[i].IstHauptpfad = true;
            MarkOccupied(i);
            int dx = ebenen[i].X - ebenen[i - 1].X, dy = ebenen[i].Y - ebenen[i - 1].Y;
            string dir = dx == 1 ? "right" : dx == -1 ? "left" : dy == 1 ? "down" : "up";
            Connect(i - 1, dir, i);
        }

        // Seitenwege
        int nextId = mainEnd + 1;
        int AddBranch(int anchor, int start, int len, bool markDead)
        {
            int prev = anchor, cur = start, made = 0;
            for (int s = 0; s < len && cur < total; s++)
            {
                if (!PlaceAndConnect(prev, cur)) break;
                prev = cur; cur++; made++;
            }
            if (made > 0 && markDead) ebenen[prev].IstSackgasse = true;
            return cur;
        }

        for (int anchor = 1; anchor <= mainEnd && nextId < total; anchor++)
        {
            if (Chance(32, rng) && nextId < total)
                nextId = AddBranch(anchor, nextId, rng.Next(2, 7), !Chance(35, rng));
            if (Chance(13, rng) && nextId < total)
                nextId = AddBranch(anchor, nextId, rng.Next(1, 4), true);
        }

        // Verbleibende Ebenen anbinden
        int safety = 0;
        while (nextId < total && safety++ < total * 50)
        {
            var anchors = Enumerable.Range(0, nextId).Where(i => ExitCount(i) < 4).ToList();
            int a = anchors.Count > 0 ? anchors[rng.Next(anchors.Count)] : rng.Next(0, mainEnd + 1);
            int old = nextId;
            nextId = AddBranch(a, nextId, rng.Next(1, 4), Chance(75, rng));
            if (nextId == old && nextId < total)
            {
                ebenen[nextId].X = ebenen[a].X;
                ebenen[nextId].Y = ebenen[a].Y + 3 + rng.Next(0, 10);
                while (!FreeAt(ebenen[nextId].X, ebenen[nextId].Y)) ebenen[nextId].Y++;
                MarkOccupied(nextId);
                var freeDir = Shuffled(Dirs, rng).FirstOrDefault(d => !ebenen[a].Exits.ContainsKey(d)) ?? "down";
                ebenen[a].Exits[freeDir] = nextId;
                ebenen[nextId].Exits[Opposite[freeDir]] = a;
                ebenen[nextId].IstSackgasse = true;
                nextId++;
            }
        }

        // Umnummerieren nach Karten-Distanz (wie HTML)
        ebenen = RenumberByMapDistance(ebenen, mainEnd, rng);

        // Städte platzieren
        PlaceCities(ebenen, totalCitiesWanted, rng);

        // Bosse platzieren
        PlaceBosses(ebenen, bossCount, städteProBoss, rng);

        // Startpunkte setzen
        PlaceStartPoints(ebenen);

        // Sperren hinzufügen
        AddLocksAfterCities(ebenen, 28, rng);

        ebenen[0].Besucht = true;
        return ebenen;
    }

    private static void PlaceCities(List<Ebene> ebenen, int count, Random rng)
    {
        foreach (var e in ebenen) e.IstStadt = false;
        if (count <= 0) return;
        var cityIds = new List<int>();
        var dist = CalcDistances(ebenen);

        // Stadt 1 immer auf Ebene 4 (wie HTML)
        if (ebenen.Count > 4)
        {
            ebenen[4].IstStadt = true;
            cityIds.Add(4);
        }

        bool CityDistOk(int id, int minD)
        {
            foreach (var o in cityIds)
            {
                int dx = Math.Abs(ebenen[id].X - ebenen[o].X) + Math.Abs(ebenen[id].Y - ebenen[o].Y);
                if (dx < minD) return false;
            }
            return true;
        }

        var main = ebenen.Where(e => e.Id >= 5 && !e.IstStadt && e.IstHauptpfad).Select(e => e.Id).ToList();
        var side = ebenen.Where(e => e.Id >= 5 && !e.IstStadt && !e.IstHauptpfad && !e.IstSackgasse).Select(e => e.Id).ToList();
        var dead = ebenen.Where(e => e.Id >= 5 && !e.IstStadt && e.IstSackgasse).Select(e => e.Id).ToList();

        int tMain = (int)Math.Round(count * 0.40);
        int tSide = (int)Math.Round(count * 0.40);
        int tDead = count - tMain - tSide;

        void PickFrom(List<int> arr, ref int amount, int minDist)
        {
            foreach (var id in Shuffled(arr.ToArray(), rng))
            {
                if (cityIds.Count >= count || amount <= 0) break;
                if (!ebenen[id].IstStadt && CityDistOk(id, minDist))
                { ebenen[id].IstStadt = true; cityIds.Add(id); amount--; }
            }
        }

        foreach (int minDist in new[] { 5, 4, 3, 2 })
        {
            PickFrom(main, ref tMain, minDist);
            PickFrom(side, ref tSide, minDist);
            PickFrom(dead, ref tDead, minDist);
            if (cityIds.Count >= count) break;
            int rem = count - cityIds.Count;
            PickFrom(main.Concat(side).Concat(dead).ToList(), ref rem, minDist);
            if (cityIds.Count >= count) break;
        }
    }

    private static void PlaceBosses(List<Ebene> ebenen, int count, int citiesPerBoss, Random rng)
    {
        foreach (var e in ebenen) e.IstBoss = false;
        if (count <= 0) return;
        var cityIds = ebenen.Where(e => e.IstStadt).Select(e => e.Id).ToList();
        var dist = CalcDistances(ebenen);
        var chosen = new List<int>();

        bool HasForwardNeighbor(int id)
        {
            int d = dist.TryGetValue(id, out int dd) ? dd : 0;
            return ebenen[id].Exits.Values.Any(n =>
                dist.TryGetValue(n, out int nd) && nd > d && !ebenen[n].IstStadt && !ebenen[n].IstBoss && n != 0);
        }

        // Kein Boss darf direkter Nachbar eines anderen Bosses sein
        bool KeinBossNachbar(int id)
        {
            return !ebenen[id].Exits.Values.Any(n => ebenen[n].IstBoss);
        }

        bool CityDistOk(int id, int minD)
        {
            foreach (var o in chosen)
            {
                int dx = Math.Abs(ebenen[id].X - ebenen[o].X) + Math.Abs(ebenen[id].Y - ebenen[o].Y);
                if (dx < minD) return false;
            }
            return true;
        }

        for (int i = 1; i <= count; i++)
        {
            int needCities = Math.Min(i * citiesPerBoss, cityIds.Count);
            int cityForBoss = needCities > 0 ? cityIds[Math.Max(0, needCities - 1)] : 0;
            int minD = dist.TryGetValue(cityForBoss, out int cd) ? cd : 0;

            List<Ebene> candidates;
            if (i < count)
            {
                candidates = ebenen.Where(e => e.Id != 0 && !e.IstStadt && !e.IstBoss
                    && dist.TryGetValue(e.Id, out int dd) && dd >= minD
                    && CityDistOk(e.Id, 4) && HasForwardNeighbor(e.Id) && KeinBossNachbar(e.Id)).ToList();
                candidates.Sort((a, b) =>
                    Math.Abs((dist.TryGetValue(a.Id, out int da) ? da : 0) - minD)
                    .CompareTo(Math.Abs((dist.TryGetValue(b.Id, out int db) ? db : 0) - minD)));
            }
            else
            {
                candidates = ebenen.Where(e => e.Id != 0 && !e.IstStadt && !e.IstBoss
                    && dist.TryGetValue(e.Id, out int dd) && dd < int.MaxValue
                    && KeinBossNachbar(e.Id)).ToList();
                candidates.Sort((a, b) =>
                    (dist.TryGetValue(b.Id, out int db) ? db : 0)
                    .CompareTo(dist.TryGetValue(a.Id, out int da) ? da : 0));
            }

            var pick = candidates.FirstOrDefault()
                ?? ebenen.FirstOrDefault(e => e.Id != 0 && !e.IstStadt && !e.IstBoss && HasForwardNeighbor(e.Id) && KeinBossNachbar(e.Id))
                ?? ebenen.FirstOrDefault(e => e.Id != 0 && !e.IstStadt && !e.IstBoss && KeinBossNachbar(e.Id))
                ?? ebenen.FirstOrDefault(e => e.Id != 0 && !e.IstStadt && !e.IstBoss);
            if (pick != null) { pick.IstBoss = true; chosen.Add(pick.Id); }
        }
    }

    private static void PlaceStartPoints(List<Ebene> ebenen)
    {
        foreach (var e in ebenen) e.IstStart = false;
        if (ebenen.Count > 0) ebenen[0].IstStart = true;
        var bossIds = ebenen.Where(e => e.IstBoss).Select(e => e.Id).ToList();
        if (bossIds.Count == 0) return;
        var dist = CalcDistances(ebenen);

        for (int i = 0; i < bossIds.Count - 1; i++)
        {
            int boss = bossIds[i];
            int bossDist = dist.TryGetValue(boss, out int bd) ? bd : 0;

            // Start N muss DIREKT nach Boss N sein (direkter Nachbar mit größerer Distanz)
            var direkteNachbarn = ebenen[boss].Exits.Values
                .Where(n => !ebenen[n].IstStadt && !ebenen[n].IstBoss && !ebenen[n].IstStart
                    && dist.TryGetValue(n, out int nd) && nd > bossDist)
                .ToList();

            // Bevorzuge Nachbarn die noch nicht Start sind und weiter vorne liegen
            direkteNachbarn.Sort((a, b) =>
                (dist.TryGetValue(b, out int db) ? db : 0)
                .CompareTo(dist.TryGetValue(a, out int da) ? da : 0));

            var pick = direkteNachbarn.FirstOrDefault();

            // Fallback: irgendeinen Nachbar des Bosses nehmen
            if (pick == default)
            {
                pick = ebenen[boss].Exits.Values
                    .FirstOrDefault(n => !ebenen[n].IstBoss && !ebenen[n].IstStart);
            }

            if (pick != default) { ebenen[pick].IstStart = true; ebenen[pick].NachBoss = boss; }
        }
    }

    private static void AddLocksAfterCities(List<Ebene> ebenen, int lockChance, Random rng)
    {
        foreach (var e in ebenen) e.Locks.Clear();
        var cityIds = ebenen.Where(e => e.IstStadt).Select(e => e.Id).ToList();
        if (cityIds.Count == 0) return;
        var dist = CalcDistances(ebenen);
        var seen = new HashSet<string>();

        foreach (var a in ebenen)
        {
            foreach (var (dir, b) in a.Exits)
            {
                string k = a.Id < b ? $"{a.Id}-{b}" : $"{b}-{a.Id}";
                if (seen.Contains(k)) continue;
                seen.Add(k);
                // Hauptpfad frei lassen
                if (ebenen[a.Id].IstHauptpfad && ebenen[b].IstHauptpfad && Math.Abs(a.Id - b) == 1) continue;
                if (!Chance(lockChance, rng)) continue;
                int depth = Math.Max(dist.TryGetValue(a.Id, out int da) ? da : 0, dist.TryGetValue(b, out int db) ? db : 0);
                var possible = cityIds.Where(c => (dist.TryGetValue(c, out int dc) ? dc : 0) < depth - 1).ToList();
                if (possible.Count == 0) continue;
                int lockCity = possible[rng.Next(possible.Count)];
                ebenen[a.Id].Locks[dir] = lockCity;
                ebenen[b].Locks[Opposite[dir]] = lockCity;
            }
        }
    }

    private static List<Ebene> RenumberByMapDistance(List<Ebene> old, int mainEnd, Random rng)
    {
        var order = new List<int>();
        var seen = new HashSet<int>();

        void Add(int id) { if (!seen.Contains(id)) { seen.Add(id); order.Add(id); } }

        double NeighborScore(int from, int n) =>
            Math.Abs(old[from].X - old[n].X) + Math.Abs(old[from].Y - old[n].Y)
            + (old[n].IstSackgasse ? 0.25 : 0) + n / 10000.0;

        void TraverseSide(int id, int parent)
        {
            if (seen.Contains(id)) return;
            Add(id);
            var ns = old[id].Exits.Values.Where(n => n != parent && !seen.Contains(n))
                .OrderBy(n => NeighborScore(id, n)).ToList();
            foreach (var n in ns) TraverseSide(n, id);
        }

        int prefixEnd = Math.Min(4, Math.Min(mainEnd, old.Count - 1));
        for (int i = 0; i <= prefixEnd; i++) Add(i);
        for (int i = 1; i <= prefixEnd; i++)
        {
            var branches = old[i].Exits.Values.Where(n => !seen.Contains(n) &&
                !(old[i].IstHauptpfad && old[n].IstHauptpfad && Math.Abs(i - n) == 1))
                .OrderBy(n => NeighborScore(i, n)).ToList();
            foreach (var b in branches) TraverseSide(b, i);
        }
        for (int i = prefixEnd + 1; i <= mainEnd && i < old.Count; i++)
        {
            Add(i);
            var branches = old[i].Exits.Values.Where(n => !seen.Contains(n) &&
                !(old[i].IstHauptpfad && old[n].IstHauptpfad && Math.Abs(i - n) == 1))
                .OrderBy(n => NeighborScore(i, n)).ToList();
            foreach (var b in branches) TraverseSide(b, i);
        }
        for (int i = 0; i < old.Count; i++) Add(i);

        var mapOldToNew = new Dictionary<int, int>();
        for (int newId = 0; newId < order.Count; newId++) mapOldToNew[order[newId]] = newId;

        var newEbenen = Enumerable.Range(0, old.Count).Select(newId =>
        {
            int oldId = order[newId];
            var o = old[oldId];
            return new Ebene
            {
                Id = newId, X = o.X, Y = o.Y,
                IstStadt = o.IstStadt, IstBoss = o.IstBoss, IstStart = o.IstStart,
                IstHauptpfad = o.IstHauptpfad, IstSackgasse = o.IstSackgasse
            };
        }).ToList();

        for (int newId = 0; newId < newEbenen.Count; newId++)
        {
            int oldId = order[newId];
            foreach (var (dir, nb) in old[oldId].Exits)
                if (mapOldToNew.TryGetValue(nb, out int newNb)) newEbenen[newId].Exits[dir] = newNb;
            foreach (var (dir, lk) in old[oldId].Locks)
                if (mapOldToNew.TryGetValue(lk, out int newLk)) newEbenen[newId].Locks[dir] = newLk;
        }

        return newEbenen;
    }

    private static Dictionary<int, int> CalcDistances(List<Ebene> ebenen)
    {
        var dist = new Dictionary<int, int>();
        for (int i = 0; i < ebenen.Count; i++) dist[i] = int.MaxValue;
        if (ebenen.Count == 0) return dist;
        dist[0] = 0;
        var q = new Queue<int>();
        q.Enqueue(0);
        while (q.Count > 0)
        {
            int id = q.Dequeue();
            foreach (var n in ebenen[id].Exits.Values)
                if (dist[n] == int.MaxValue) { dist[n] = dist[id] + 1; q.Enqueue(n); }
        }
        return dist;
    }

    // ─── Trainer-Zuweisung (level-basiert) ───────────────────────────────────
    private static List<TrainerKampf> HoleTrainerFürLevel(
        List<TrainerKampf> pool, int dist, Random rng, int min, int max)
    {
        int targetLevel = 3 + (int)(dist * 1.8);
        int tolerance = 8;
        var passend = pool
            .Where(t => (t.Klasse == "Trainer" || t.Klasse == "Zwischenboss") && t.Team.Any())
            .Where(t => Math.Abs(t.Team.Max(m => m.Level) - targetLevel) <= tolerance)
            .ToList();
        if (passend.Count == 0)
            passend = pool.Where(t => t.Klasse == "Trainer" && t.Team.Any()).ToList();
        int anzahl = min == max ? min : rng.Next(min, max + 1);
        return passend.OrderBy(_ => rng.Next()).Take(anzahl).ToList();
    }

    private static List<WildBegegnung> HoleWildMonsterFürLevel(
        List<MonsterPoolEintrag> pool, int dist, Random rng, int min, int max)
    {
        int targetLevel = 3 + (int)(dist * 1.8);
        var passend = pool
            .Where(e => e.MinLevel <= targetLevel + 5 && e.MaxLevel >= targetLevel - 5)
            .ToList();
        if (passend.Count == 0) passend = pool.ToList();
        int anzahl = rng.Next(min, max + 1);
        return passend.OrderBy(_ => rng.Next()).Take(anzahl)
            .Select(e => new WildBegegnung
            {
                MonsterId = e.Id,
                MinLevel = Math.Max(1, targetLevel - 2),
                MaxLevel = targetLevel + 2,
                Chance = e.Chance
            }).ToList();
    }

    // ─── Hilfsmethoden ───────────────────────────────────────────────────────
    private static bool Chance(int percent, Random rng) => rng.Next(100) < percent;

    private static T[] Shuffled<T>(T[] arr, Random rng)
    {
        var copy = arr.ToArray();
        for (int i = copy.Length - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (copy[i], copy[j]) = (copy[j], copy[i]);
        }
        return copy;
    }

    private static List<T> Shuffled<T>(IEnumerable<T> source, Random rng)
        => Shuffled(source.ToArray(), rng).ToList();

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
