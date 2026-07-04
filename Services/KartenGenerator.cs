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
    public List<string> StadtIds { get; set; } = new();   // Arenen in Reihenfolge
    public List<string> BossIds { get; set; } = new();
    public List<string> StartIds { get; set; } = new();
    public int StädteProBoss { get; set; } = 3;
    // Anzahl besiegter Arenen (Orden erhalten) – für Boss-Zugang und Reihenfolge
    public int BesiegteArenenAnzahl => BesiegteArenen.Count;
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
            int arenaCount = regCfg.Arenaleiter.Count > 0 ? regCfg.Arenaleiter.Count : 8;
            // +2: vorletzter Boss = Top 4, letzter Boss = Champion (extra nach den Arenen)
            int bossCount = arenaCount + 2;
            int städteProBoss = bossCount;
            int totalCitiesWanted = 0;

            // Trainer-Pool direkt aus regionen.json (tiefe Kopie damit Level-Änderungen nicht den Pool mutieren)
            var trainerPool = regCfg.TrainerPool.Select(t => new TrainerKampf
            {
                Id = t.Id,
                Name = t.Name,
                Klasse = t.Klasse,
                Belohnung = t.Belohnung,
                Team = t.Team.Select(m => new MonsterTeamEintrag { MonsterId = m.MonsterId, Level = m.Level }).ToList()
            }).ToList();
            var wildPool = regCfg.MonsterPool;
            // Legendäre/Einzigartige Monster (Chance <= 3) separieren – werden später einzeln verteilt
            var legendärePool = wildPool.Where(e => e.Chance <= 3).ToList();
            var normalePool = wildPool.Where(e => e.Chance > 3).ToList();
            // Tracking: welche Legendären bereits auf einer Route platziert wurden
            var platzierteLegendäre = new HashSet<string>();

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
                    // Letzter Boss = Champion, vorletzter = Top 4, davor = normale Arenen
                    bool istLetzterBoss  = bossZähler == bossCount;     // Champion
                    bool istVorletzter   = bossZähler == bossCount - 1; // Top 4
                    bool istNormaleArena = !istLetzterBoss && !istVorletzter;
                    // leiterCfg nur für normale Arenen (bossZähler 1..arenaCount)
                    var leiterCfg = istNormaleArena && bossZähler <= arenaleiter.Count ? arenaleiter[bossZähler - 1] : null;

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
                        name = "Monster-Liga";
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
                    hatMarkt = true; // Jede Arena hat einen Markt

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
                // IstStadt ohne IstBoss gibt es nicht mehr – Arenen sind die Städte
                else
                {
                    ebeneZähler++;
                    name = $"Ebene {ebeneZähler}";
                    typ = "route";
                    farbe = eb.IstSackgasse ? "gray" : "green";
                    int trainerAnz = eb.IstSackgasse ? 1 : rng.Next(1, 4);
                    trainer = HoleTrainerFürLevel(trainerPool, dist, rng, trainerAnz, trainerAnz);
                    wildMonster = HoleWildMonsterFürLevel(normalePool.Any() ? normalePool : wildPool, dist, rng, 2, 5);
                    // Legendäre Monster: genau 1 pro Region auf einer einzigen Route (spätere Ebenen bevorzugt)
                    if (legendärePool.Any() && dist >= 8)
                    {
                        var verfügbar = legendärePool.Where(l => !platzierteLegendäre.Contains(l.Id)).ToList();
                        if (verfügbar.Any() && rng.Next(100) < 40) // 40% Chance pro passende Route
                        {
                            var legend = verfügbar[rng.Next(verfügbar.Count)];
                            platzierteLegendäre.Add(legend.Id);
                            int targetLevel = 2 + (int)(dist * 1.8);
                            wildMonster.Add(new WildBegegnung
                            {
                                MonsterId = legend.Id,
                                MinLevel = Math.Max(targetLevel, 40),
                                MaxLevel = Math.Max(targetLevel + 5, 50),
                                Chance = 255 // 100% Begegnung im Route-genau-Modus (wird später speziell behandelt)
                            });
                        }
                    }
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

            // Noch nicht platzierte Legendäre auf späte Routen verteilen (garantiert alle vorkommen)
            var nichtPlatziert = legendärePool.Where(l => !platzierteLegendäre.Contains(l.Id)).ToList();
            if (nichtPlatziert.Any())
            {
                // Routen mit Wild-Monstern als Kandidaten, sortiert nach Distanz (höchste zuerst)
                var routenOrte = ergebnis.Where(o => o.Typ == "route" && o.WildMonster.Count > 0).ToList();
                if (routenOrte.Count > 0)
                {
                    // Sortiere nach Distanz absteigend (späteste Routen zuerst)
                    var ortZuDistanz = new Dictionary<string, int>();
                    foreach (var eb2 in ebenen)
                    {
                        if (ebeneZuOrtId.TryGetValue(eb2.Id, out var oId) && distanzen.TryGetValue(eb2.Id, out int d))
                            ortZuDistanz[oId] = d;
                    }
                    routenOrte = routenOrte.OrderByDescending(o => ortZuDistanz.GetValueOrDefault(o.Id, 0)).ToList();

                    int idx = 0;
                    foreach (var legend in nichtPlatziert)
                    {
                        if (idx >= routenOrte.Count) idx = 0;
                        var zielOrt = routenOrte[idx];
                        int targetLevel = 40;
                        if (zielOrt.WildMonster.Count > 0)
                            targetLevel = Math.Max(40, zielOrt.WildMonster.Max(w => w.MaxLevel));
                        zielOrt.WildMonster.Add(new WildBegegnung
                        {
                            MonsterId = legend.Id,
                            MinLevel = targetLevel,
                            MaxLevel = targetLevel + 10,
                            Chance = 255
                        });
                        platzierteLegendäre.Add(legend.Id);
                        idx++;
                    }
                }
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
                if (eb.IstBoss)  { meta.StadtIds.Add(ortId); meta.BossIds.Add(ortId); }
                if (eb.IstStart) meta.StartIds.Add(ortId);
            }
            meta.StädteProBoss = städteProBoss;

            // Level in Ort-Objekte schreiben
            foreach (var ort in ergebnis.Where(o => o.Id.StartsWith(regId + "-GEN-")))
            {
                if (!meta.OrtDistanzen.TryGetValue(ort.Id, out int dist2)) continue;
                // Startgebiet: Lv.2-5, dann +1.8 pro Schritt
                int baseLvl = Math.Max(2, 2 + (int)(dist2 * 1.8));
                int minLvl = baseLvl;
                int maxLvl = baseLvl + 3;
                bool istArena = ort.Arena != null;
                if (istArena) { minLvl += 3; maxLvl += 5; }

                foreach (var w in ort.WildMonster)
                { w.MinLevel = minLvl; w.MaxLevel = maxLvl; }
                foreach (var t in ort.Trainer)
                    foreach (var m in t.Team)
                        m.Level = baseLvl + 1;  // gleiche Formel wie wilde Pokémon
                if (ort.Arena?.Team != null)
                    foreach (var m in ort.Arena.Team)
                        m.Level = baseLvl + 2;  // Arena-Leiter: baseLvl + 2 (nicht maxLvl + 2!)
            }

            // Fog-of-War: Startpunkt + direkte Nachbarn
            var startOrtObj = ebeneZuOrt[startEbene.Id];
            meta.FreigeschalteteOrte.Add(startOrtObj.Id);
            foreach (var nachbarId in startEbene.Exits.Values)
                if (ebeneZuOrtId.TryGetValue(nachbarId, out var nId))
                    meta.FreigeschalteteOrte.Add(nId);

            // Nächste Region: Startpunkt direkt nach dem LETZTEN Boss (Liga) dieser Region
            // Wird als Verbindung gesetzt wenn es eine nächste Region gibt
            int nextRegIdx = regionsReihenfolge.IndexOf(regId) + 1;
            if (nextRegIdx < regionsReihenfolge.Count)
            {
                // Den letzten Boss dieser Region finden
                var letzterBoss = ebenen.LastOrDefault(e => e.IstBoss);
                if (letzterBoss != null)
                {
                    // Merken: nächste Region soll ihren Startpunkt mit diesem Boss verbinden
                    // Dies geschieht in der nächsten Iterations-Runde
                    // Wir speichern die OrtId des letzten Bosses für die Verknüpfung
                    meta.OrtDistanzen[$"__LAST_BOSS_{regId}"] = ebeneZuOrtId.TryGetValue(letzterBoss.Id, out var lbId) ? meta.OrtDistanzen.GetValueOrDefault(lbId, 0) : 0;
                    // Speichere OrtId des letzten Bosses als Verbindungspunkt
                    if (ebeneZuOrtId.TryGetValue(letzterBoss.Id, out var lastBossOrtId))
                        meta.OrtDistanzen[$"__LAST_BOSS_ID_{regId}"] = 0; // Marker
                }
            }

            globalOrdenOffset += bossCount;

            // ─── Rivale-Stadt nach dem Champion generieren ───────────────────
            var rivaleTrainer = trainerPool
                .Where(t => t.Klasse == "Rivale")
                .OrderByDescending(t => t.Team.Any() ? t.Team.Max(m => m.Level) : 0)
                .FirstOrDefault();
            if (rivaleTrainer != null)
            {
                // Champion-Ort finden (letzter Boss)
                var championOrt = ergebnis
                    .Where(o => o.Id.StartsWith(regId + "-GEN-BOSS") && o.Arena != null)
                    .OrderByDescending(o => o.Arena!.OrdenNr)
                    .FirstOrDefault();
                if (championOrt != null)
                {
                    // Rivale-Stadt erstellen
                    var rivaleOrtId = $"{regId}-GEN-RIVALE";
                    int championDist = meta.OrtDistanzen.TryGetValue(championOrt.Id, out int cd) ? cd : 0;
                    int rivaleLvl = Math.Max(2, 2 + (int)(championDist * 1.8)) + 3;
                    var rivaleTeamKopie = new TrainerKampf
                    {
                        Id = rivaleTrainer.Id,
                        Name = rivaleTrainer.Name,
                        Klasse = "Rivale",
                        Belohnung = rivaleTrainer.Belohnung,
                        Team = rivaleTrainer.Team.Select(m => new MonsterTeamEintrag { MonsterId = m.MonsterId, Level = rivaleLvl }).ToList()
                    };
                    var rivaleOrt = new Ort
                    {
                        Id = rivaleOrtId,
                        Name = $"Rivale: {rivaleTrainer.Name}",
                        Typ = "stadt",
                        Farbe = "#1a6b3a",
                        GridX = championOrt.GridX + 1,
                        GridY = championOrt.GridY,
                        Arena = new Arena
                        {
                            OrdenName = "Rivale besiegt",
                            OrdenNr = globalOrdenOffset + 1,
                            Leiter = rivaleTrainer.Name,
                            TypSpezialisierung = "",
                            Team = rivaleTeamKopie.Team.Select(m => new MonsterTeamEintrag { MonsterId = m.MonsterId, Level = m.Level }).ToList()
                        },
                        Trainer = new List<TrainerKampf> { rivaleTeamKopie },
                        WildMonster = new List<WildBegegnung>(),
                        HatMonsterCenter = true,
                        HatMarkt = true,
                        NPCs = new List<GesprächsNPC>(),
                        IstStartOrt = false,
                        IstRivaleStadt = true,
                        Verbindungen = new List<string>()
                    };
                    // Verbindung Champion → Rivale
                    if (string.IsNullOrEmpty(championOrt.Ost))
                    {
                        championOrt.Ost = rivaleOrtId;
                        rivaleOrt.West = championOrt.Id;
                    }
                    else
                    {
                        championOrt.Nord = rivaleOrtId;
                        rivaleOrt.Sued = championOrt.Id;
                    }
                    championOrt.Verbindungen.Add(rivaleOrtId);
                    rivaleOrt.Verbindungen.Add(championOrt.Id);
                    ergebnis.Add(rivaleOrt);
                    meta.OrtReihenfolge.Add(rivaleOrtId);
                    meta.OrtKoordinaten[rivaleOrtId] = (rivaleOrt.GridX, rivaleOrt.GridY);
                    meta.OrtDistanzen[rivaleOrtId] = championDist + 1;
                    meta.StadtIds.Add(rivaleOrtId);
                }
            }
        }

        // Regionen verbinden: Startpunkt der Region N+1 direkt nach Liga von Region N
        for (int ri = 0; ri < regionsReihenfolge.Count - 1; ri++)
        {
            string thisReg = regionsReihenfolge[ri];
            string nextReg = regionsReihenfolge[ri + 1];

            // Letzter Boss dieser Region
            var letzterBossOrt = ergebnis
                .Where(o => o.Id.StartsWith(thisReg + "-GEN-BOSS") && o.Arena != null)
                .OrderByDescending(o => o.Arena!.OrdenNr)
                .FirstOrDefault();

            // Erster Startpunkt der nächsten Region
            var nächsterStart = ergebnis
                .FirstOrDefault(o => o.Id.StartsWith(nextReg + "-GEN-START01"));

            if (letzterBossOrt != null && nächsterStart != null)
            {
                // Verbindung: Liga → Start der nächsten Region (Ost-Richtung)
                if (string.IsNullOrEmpty(letzterBossOrt.Ost))
                {
                    letzterBossOrt.Ost = nächsterStart.Id;
                    nächsterStart.West = letzterBossOrt.Id;
                    letzterBossOrt.Verbindungen.Add(nächsterStart.Id);
                    nächsterStart.Verbindungen.Add(letzterBossOrt.Id);
                }
                else if (string.IsNullOrEmpty(letzterBossOrt.Sued))
                {
                    letzterBossOrt.Sued = nächsterStart.Id;
                    nächsterStart.Nord = letzterBossOrt.Id;
                    letzterBossOrt.Verbindungen.Add(nächsterStart.Id);
                    nächsterStart.Verbindungen.Add(letzterBossOrt.Id);
                }
                else
                {
                    letzterBossOrt.Nord = nächsterStart.Id;
                    nächsterStart.Sued = letzterBossOrt.Id;
                    letzterBossOrt.Verbindungen.Add(nächsterStart.Id);
                    nächsterStart.Verbindungen.Add(letzterBossOrt.Id);
                }

                // Start der nächsten Region im Fog-of-War sperren bis Liga besiegt
                // (wird durch Boss-Zugang-Prüfung in ZugangPrüfen gehandhabt)
            }
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

        // Bosse platzieren (Arenen = Städte, keine separaten Städte mehr)
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
        foreach (var e in ebenen) { e.IstBoss = false; e.IstStadt = false; }
        if (count <= 0) return;
        var dist = CalcDistances(ebenen);
        var chosen = new List<int>();

        // Gesamtdistanz des Graphen
        int maxDist = dist.Values.Where(d => d < int.MaxValue).DefaultIfEmpty(1).Max();

        // Kein Boss darf direkter Nachbar eines anderen Bosses sein
        bool KeinBossNachbar(int id) =>
            !ebenen[id].Exits.Values.Any(n => ebenen[n].IstBoss);

        // Kein Boss direkt nach Startpunkt (Ebene 0)
        bool KeinStartNachbar(int id) =>
            !ebenen[id].Exits.Values.Any(n => n == 0) && id != 0;

        // Hat Nachbar mit größerer Distanz (für Vorwärts-Bewegung)
        bool HasForwardNeighbor(int id)
        {
            int d = dist.TryGetValue(id, out int dd) ? dd : 0;
            return ebenen[id].Exits.Values.Any(n =>
                dist.TryGetValue(n, out int nd) && nd > d && !ebenen[n].IstBoss && n != 0);
        }

        // Bosse gleichmäßig über den Graphen verteilen
        for (int i = 1; i <= count; i++)
        {
            // Ziel-Distanz für diesen Boss: gleichmäßig verteilt
            double targetFraction = (double)i / (count + 1);
            int targetDist = (int)(maxDist * targetFraction);

            // Kandidaten: nicht Start, nicht Boss, nicht direkter Nachbar von Boss/Start
            var candidates = ebenen
                .Where(e => e.Id != 0 && !e.IstBoss
                    && dist.TryGetValue(e.Id, out int dd) && dd < int.MaxValue
                    && KeinBossNachbar(e.Id)
                    && KeinStartNachbar(e.Id))
                .ToList();

            if (i < count)
                candidates = candidates.Where(e => HasForwardNeighbor(e.Id)).ToList();

            // Nächsten Kandidaten zur Ziel-Distanz wählen
            candidates.Sort((a, b) =>
                Math.Abs((dist.TryGetValue(a.Id, out int da) ? da : 0) - targetDist)
                .CompareTo(Math.Abs((dist.TryGetValue(b.Id, out int db) ? db : 0) - targetDist)));

            var pick = candidates.FirstOrDefault()
                ?? ebenen.FirstOrDefault(e => e.Id != 0 && !e.IstBoss && KeinBossNachbar(e.Id))
                ?? ebenen.FirstOrDefault(e => e.Id != 0 && !e.IstBoss);

            if (pick != null)
            {
                pick.IstBoss = true;
                pick.IstStadt = true; // Arenen sind die Städte
                chosen.Add(pick.Id);
            }
        }
    }

    private static void PlaceStartPoints(List<Ebene> ebenen)
    {
        // Alle Starts zurücksetzen
        foreach (var e in ebenen) e.IstStart = false;

        // Startpunkt 1: immer Ebene 0 (Anfang der Region, wo der Professor ist)
        if (ebenen.Count > 0) ebenen[0].IstStart = true;

        // Weitere Startpunkte: NUR nach dem LETZTEN Boss (Liga/Champ) der Region
        // Diese Methode wird pro Region aufgerufen, also gibt es maximal 1 weiteren Start
        // Der weitere Start wird von außen gesetzt (nach der Generierung der Region)
        // Hier: nichts weiter tun – der nächste Regions-Start wird in Generiere() gesetzt
    }

    private static void AddLocksAfterCities(List<Ebene> ebenen, int lockChance, Random rng)
    {
        foreach (var e in ebenen) e.Locks.Clear();
        var cityIds = ebenen.Where(e => e.IstBoss).Select(e => e.Id).ToList(); // Arenen sind die Städte
        if (cityIds.Count == 0) return;
        var dist = CalcDistances(ebenen);
        var seen = new HashSet<string>();

        // Alle Kanten sammeln und nach Tiefe sortieren (wie HTML v16)
        var edges = new List<(int a, int b, string dir, int depth)>();
        foreach (var a in ebenen)
        {
            foreach (var (dir, b) in a.Exits)
            {
                string k = a.Id < b ? $"{a.Id}-{b}" : $"{b}-{a.Id}";
                if (seen.Contains(k)) continue;
                seen.Add(k);
                // Hauptpfad frei lassen
                if (ebenen[a.Id].IstHauptpfad && ebenen[b].IstHauptpfad && Math.Abs(a.Id - b) == 1) continue;
                int depth = Math.Max(dist.TryGetValue(a.Id, out int da) ? da : 0, dist.TryGetValue(b, out int db) ? db : 0);
                edges.Add((a.Id, b, dir, depth));
            }
        }
        edges.Sort((e1, e2) => e1.depth.CompareTo(e2.depth));

        foreach (var (a, b, dir, depth) in edges)
        {
            if (!Chance(lockChance, rng)) continue;
            var possible = cityIds.Where(c => (dist.TryGetValue(c, out int dc) ? dc : 0) < depth - 1).ToList();
            if (possible.Count == 0) continue;
            int lockCity = possible[rng.Next(possible.Count)];
            ebenen[a].Locks[dir] = lockCity;
            ebenen[b].Locks[Opposite[dir]] = lockCity;
        }

        // Jede Arena soll einen Sinn haben: ungenutzte Arenen bekommen nachträglich eine Sperre
        var used = new HashSet<int>();
        foreach (var e in ebenen)
            foreach (var lk in e.Locks.Values)
                used.Add(lk);

        foreach (var city in cityIds)
        {
            if (used.Contains(city)) continue;
            int cityDist = dist.TryGetValue(city, out int cd) ? cd : 0;
            // Spätere Kante finden die noch keine Sperre hat
            var later = edges.Where(e =>
                e.depth > cityDist + 2 &&
                !ebenen[e.a].Locks.ContainsKey(e.dir)).ToList();
            if (later.Count == 0) continue;
            var pick = later[rng.Next(later.Count)];
            ebenen[pick.a].Locks[pick.dir] = city;
            ebenen[pick.b].Locks[Opposite[pick.dir]] = city;
            used.Add(city);
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
        int targetLevel = Math.Max(2, 3 + (int)(dist * 1.8));

        // Alle Trainer aus dem Pool nehmen (Klasse Trainer oder Zwischenboss)
        var kandidaten = pool
            .Where(t => (t.Klasse == "Trainer" || t.Klasse == "Zwischenboss") && t.Team.Any())
            .ToList();
        if (kandidaten.Count == 0)
            kandidaten = pool.Where(t => t.Team.Any()).ToList();

        int anzahl = min == max ? min : rng.Next(min, max + 1);
        var ausgewählt = kandidaten.OrderBy(_ => rng.Next()).Take(anzahl).ToList();

        // Level ALLER Trainer-Monster auf targetLevel setzen (mit kleiner Variation)
        foreach (var trainer in ausgewählt)
        {
            for (int i = 0; i < trainer.Team.Count; i++)
            {
                // Erstes Monster: genau targetLevel, weitere: +0 bis +2
                int variation = i == 0 ? 0 : rng.Next(0, 3);
                trainer.Team[i].Level = Math.Max(2, targetLevel + variation);
            }
        }

        return ausgewählt;
    }

    private static List<WildBegegnung> HoleWildMonsterFürLevel(
        List<MonsterPoolEintrag> pool, int dist, Random rng, int min, int max)
    {
        int targetLevel = 2 + (int)(dist * 1.8);
        var passend = pool
            .Where(e => e.MinLevel <= targetLevel + 5 && e.MaxLevel >= targetLevel - 5)
            .ToList();
        if (passend.Count == 0) passend = pool.ToList();
        int anzahl = rng.Next(min, max + 1);
        // Gewichtete Auswahl: Monster mit höherer Chance werden öfter ausgewählt
        // Chance 255 = häufig (Rattfratz), Chance 3 = selten (Garados)
        var gewichtet = new List<MonsterPoolEintrag>();
        foreach (var e in passend)
        {
            int gewicht = Math.Max(1, e.Chance / 30); // 255→3 Einträge, 3→1 Eintrag
            for (int i = 0; i < gewicht; i++) gewichtet.Add(e);
        }
        var ausgewählt = gewichtet.OrderBy(_ => rng.Next())
            .DistinctBy(e => e.Id).Take(anzahl).ToList();
        return ausgewählt
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
