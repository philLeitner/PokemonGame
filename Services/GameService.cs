using MonsterKampf.Models;
using MonsterKampf.Data;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonsterKampf.Services;

public class GameService
{
    private readonly HttpClient _http;
    private readonly Random _rng = new();

    // ── Daten ────────────────────────────────────────────────────────────────
    public List<MonsterData> AlleMonster { get; private set; } = new();
    public List<AttackeData> AlleAttacken { get; private set; } = new();
    public Dictionary<string, TypInfo> AlleTypen { get; private set; } = new();
    public List<Ort> AlleOrte { get; private set; } = new();

    // ── Spielzustand ─────────────────────────────────────────────────────────
    public SpielPhase Phase { get; private set; } = SpielPhase.Laden;
    public Spieler Spieler { get; private set; } = new();
    public KampfZustand? AktuellerKampf { get; private set; }
    public Ort? AktuellerOrt => AlleOrte.FirstOrDefault(o => o.Id == Spieler.AktuellerOrt);
    public bool DatenGeladen { get; private set; }
    public string LadeStatus { get; private set; } = "Initialisiere...";

    // ── Events ───────────────────────────────────────────────────────────────
    public event Action? OnChange;
    private void Notify() => OnChange?.Invoke();

    public GameService(HttpClient http)
    {
        _http = http;
    }

    // ── Daten laden ──────────────────────────────────────────────────────────
    public async Task DatenLadenAsync()
    {
        try
        {
            LadeStatus = "Lade Typen...";
            Notify();
            var typOpts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var typenRaw = await _http.GetFromJsonAsync<Dictionary<string, TypInfoRaw>>("data/typen.json", typOpts);
            if (typenRaw != null)
            {
                foreach (var kv in typenRaw)
                {
                    AlleTypen[kv.Key] = new TypInfo
                    {
                        Id = kv.Value.Id,
                        Name = kv.Value.Name,
                        X2Gegen = kv.Value.X2Gegen ?? new(),
                        X05Gegen = kv.Value.X05Gegen ?? new(),
                        X0Gegen = kv.Value.X0Gegen ?? new(),
                        SchwachGegen = kv.Value.SchwachGegen ?? new(),
                        ResistentGegen = kv.Value.ResistentGegen ?? new(),
                        ImmunGegen = kv.Value.ImmunGegen ?? new(),
                    };
                }
            }

            LadeStatus = "Lade Attacken...";
            Notify();
            var attackOpts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var attackenRaw = await _http.GetFromJsonAsync<List<AttackeRaw>>("data/attacken.json", attackOpts);
            if (attackenRaw != null)
            {
                AlleAttacken = attackenRaw.Select(a => new AttackeData
                {
                    Id = a.Id,
                    Name = a.Name,
                    Typ = a.Typ,
                    Kategorie = a.Kategorie,
                    Staerke = a.Staerke,
                    Genauigkeit = a.Genauigkeit,
                    Ap = a.Ap,
                }).ToList();
            }

            LadeStatus = "Lade Monster...";
            Notify();
            var monOpts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var monsterRaw = await _http.GetFromJsonAsync<List<MonsterRaw>>("data/monster.json", monOpts);
            if (monsterRaw != null)
            {
                AlleMonster = monsterRaw.Select(m => new MonsterData
                {
                    Id = m.Id,
                    Name = m.Name,
                    Typen = m.Typen ?? new(),
                    Bild = m.Bild,
                    Stats = m.Stats ?? new(),
                    Attacken = (m.Attacken ?? new()).Select(a => new AttackenLernEintrag
                    {
                        AttackeId = a.AttackeId,
                        Level = a.Level
                    }).ToList(),
                    EntwickeltZu = m.EntwickeltZu,
                    EntwicklungName = m.EntwicklungName,
                    EntwicklungLevel = m.EntwicklungLevel,
                }).ToList();
            }

            AlleOrte = WeltData.AlleOrte();
            DatenGeladen = true;
            Phase = SpielPhase.Hauptmenü;
            LadeStatus = "Fertig!";
        }
        catch (Exception ex)
        {
            LadeStatus = $"Fehler: {ex.Message}";
        }
        Notify();
    }

    // ── Navigation ───────────────────────────────────────────────────────────
    public void ZuHauptmenü() { Phase = SpielPhase.Hauptmenü; Notify(); }
    public void ZuStarterWahl() { Phase = SpielPhase.StarterWahl; Notify(); }
    public void ZuWeltkarte() { Phase = SpielPhase.Weltkarte; Notify(); }
    public void ZuMapEditor() { Phase = SpielPhase.MapEditor; Notify(); }

    // ── Spiel starten ────────────────────────────────────────────────────────
    public void SpielStarten(string spielerName)
    {
        Spieler = new Spieler { Name = spielerName };
        Phase = SpielPhase.StarterWahl;
        Notify();
    }

    public List<MonsterData> GetStarterOptionen()
    {
        // Starter: PKM-0001 (Blatt/Gift), PKM-0004 (Brennen), PKM-0007 (Tropfen)
        var ids = new[] { "PKM-0001", "PKM-0004", "PKM-0007" };
        return ids.Select(id => AlleMonster.FirstOrDefault(m => m.Id == id))
                  .Where(m => m != null).Cast<MonsterData>().ToList();
    }

    public void StarterWählen(string monsterId)
    {
        var spezies = AlleMonster.FirstOrDefault(m => m.Id == monsterId);
        if (spezies == null) return;
        var starter = MonsterInstanz.VonSpezies(spezies, 5, AlleAttacken);
        Spieler.Team.Add(starter);
        Spieler.AktuellerOrt = "startstadt";
        Phase = SpielPhase.Weltkarte;
        Notify();
    }

    // ── Ort betreten ─────────────────────────────────────────────────────────
    public void OrtBetreten(string ortId)
    {
        Spieler.AktuellerOrt = ortId;
        Notify();
    }

    // ── Kampf starten ────────────────────────────────────────────────────────
    public void WildkampfStarten(WildBegegnung begegnung)
    {
        var spezies = AlleMonster.FirstOrDefault(m => m.Id == begegnung.MonsterId);
        if (spezies == null) return;
        int level = _rng.Next(begegnung.MinLevel, begegnung.MaxLevel + 1);
        var gegner = MonsterInstanz.VonSpezies(spezies, level, AlleAttacken);
        var spielerMonster = Spieler.AktivesMonster;
        if (spielerMonster == null) return;

        AktuellerKampf = new KampfZustand
        {
            Typ = KampfTyp.Wild,
            SpielerMonster = spielerMonster,
            GegnerMonster = gegner,
            GegnerName = $"Wildes {gegner.Name}",
            Phase = KampfPhase.Intro,
            Log = new() { $"Ein wildes {gegner.Name} erscheint!" }
        };
        Phase = SpielPhase.Kampf;
        Notify();
    }

    public void TrainerKampfStarten(TrainerKampf trainer)
    {
        if (Spieler.BesiegteTrainer.Contains(trainer.Id)) return;
        var spielerMonster = Spieler.AktivesMonster;
        if (spielerMonster == null) return;

        var erstesGegnerMonster = trainer.Team.FirstOrDefault();
        if (erstesGegnerMonster == null) return;
        var spezies = AlleMonster.FirstOrDefault(m => m.Id == erstesGegnerMonster.MonsterId);
        if (spezies == null) return;
        var gegner = MonsterInstanz.VonSpezies(spezies, erstesGegnerMonster.Level, AlleAttacken);

        AktuellerKampf = new KampfZustand
        {
            Typ = KampfTyp.Trainer,
            SpielerMonster = spielerMonster,
            GegnerMonster = gegner,
            GegnerName = $"{trainer.Klasse} {trainer.Name}",
            Phase = KampfPhase.Intro,
            Log = new() { trainer.Dialogvor },
            TrainerId = trainer.Id,
            BelohnungGeld = trainer.Belohnung,
        };
        Phase = SpielPhase.Kampf;
        Notify();
    }

    public void ArenaKampfStarten(Ort ort)
    {
        if (ort.Arena == null) return;
        if (Spieler.Orden.Contains(ort.Arena.OrdenName)) return;
        var spielerMonster = Spieler.AktivesMonster;
        if (spielerMonster == null) return;

        var erstesGegnerMonster = ort.Arena.Team.FirstOrDefault();
        if (erstesGegnerMonster == null) return;
        var spezies = AlleMonster.FirstOrDefault(m => m.Id == erstesGegnerMonster.MonsterId);
        if (spezies == null) return;
        var gegner = MonsterInstanz.VonSpezies(spezies, erstesGegnerMonster.Level, AlleAttacken);

        AktuellerKampf = new KampfZustand
        {
            Typ = KampfTyp.Arena,
            SpielerMonster = spielerMonster,
            GegnerMonster = gegner,
            GegnerName = $"Arena-Leiter {ort.Arena.Leiter}",
            Phase = KampfPhase.SpielerZug,
            Log = new() { $"Arena-Leiter {ort.Arena.Leiter} fordert dich heraus!" },
            OrtId = ort.Id,
            BelohnungGeld = 2000 + ort.Arena.OrdenNr * 1000,
        };
        Phase = SpielPhase.Kampf;
        Notify();
    }

    // ── Kampf-Logik ──────────────────────────────────────────────────────────
    public async Task AttackeAusführen(AttackeInstanz attacke)
    {
        if (AktuellerKampf == null) return;
        if (AktuellerKampf.Phase != KampfPhase.SpielerZug) return;

        AktuellerKampf.Phase = KampfPhase.GegnerZug;

        // Spieler greift an
        var spielerMon = AktuellerKampf.SpielerMonster;
        var gegnerMon = AktuellerKampf.GegnerMonster;

        if (attacke.AktuelleAp > 0)
        {
            attacke.AktuelleAp--;
            int schaden = SchadenBerechnen(spielerMon, gegnerMon, attacke);
            float multi = TypeChart.GetVerteidigungsMultiplikator(attacke.Typ, gegnerMon.Typen);
            gegnerMon.AktuelleKp = Math.Max(0, gegnerMon.AktuelleKp - schaden);
            AktuellerKampf.Log.Add($"{spielerMon.Name} setzt {attacke.Name} ein! ({schaden} Schaden)");
            if (multi != 1f) AktuellerKampf.Log.Add(TypeChart.GetEffektivitätsText(multi));
        }
        else
        {
            AktuellerKampf.Log.Add($"{attacke.Name} hat keine AP mehr!");
        }

        Notify();
        await Task.Delay(800);

        // Prüfen ob Gegner besiegt
        if (gegnerMon.IstOhnmächtig)
        {
            KampfGewonnen();
            return;
        }

        // Gegner greift an
        var gegnerAttacke = GegnerAttackeWählen(gegnerMon);
        if (gegnerAttacke != null)
        {
            int gegnerSchaden = SchadenBerechnen(gegnerMon, spielerMon, gegnerAttacke);
            float gegnerMulti = TypeChart.GetVerteidigungsMultiplikator(gegnerAttacke.Typ, spielerMon.Typen);
            spielerMon.AktuelleKp = Math.Max(0, spielerMon.AktuelleKp - gegnerSchaden);
            AktuellerKampf.Log.Add($"{gegnerMon.Name} setzt {gegnerAttacke.Name} ein! ({gegnerSchaden} Schaden)");
            if (gegnerMulti != 1f) AktuellerKampf.Log.Add(TypeChart.GetEffektivitätsText(gegnerMulti));
        }

        Notify();
        await Task.Delay(800);

        // Prüfen ob Spieler besiegt
        if (spielerMon.IstOhnmächtig)
        {
            KampfVerloren();
            return;
        }

        AktuellerKampf.Phase = KampfPhase.SpielerZug;
        Notify();
    }

    public void KampfFliehen()
    {
        if (AktuellerKampf == null) return;
        if (AktuellerKampf.Typ != KampfTyp.Wild)
        {
            AktuellerKampf.Log.Add("Vor einem Trainer-Kampf kann man nicht fliehen!");
            Notify();
            return;
        }
        AktuellerKampf.Log.Add("Du bist geflohen!");
        AktuellerKampf.Phase = KampfPhase.Beendet;
        Notify();
    }

    public void KampfBeenden()
    {
        AktuellerKampf = null;
        Phase = SpielPhase.Weltkarte;
        Notify();
    }

    private void KampfGewonnen()
    {
        if (AktuellerKampf == null) return;
        int exp = AktuellerKampf.GegnerMonster.Level * 50;
        AktuellerKampf.ErfahrungGewonnen = exp;
        AktuellerKampf.SpielerGewonnen = true;
        AktuellerKampf.SpielerMonster.ErfahrungsPunkte += exp;
        AktuellerKampf.Log.Add($"{AktuellerKampf.SpielerMonster.Name} hat gewonnen! +{exp} EP");

        if (AktuellerKampf.BelohnungGeld > 0)
        {
            Spieler.Geld += AktuellerKampf.BelohnungGeld;
            AktuellerKampf.Log.Add($"+{AktuellerKampf.BelohnungGeld} Münzen erhalten!");
        }

        if (AktuellerKampf.TrainerId != null)
            Spieler.BesiegteTrainer.Add(AktuellerKampf.TrainerId);

        if (AktuellerKampf.Typ == KampfTyp.Arena && AktuellerKampf.OrtId != null)
        {
            var ort = AlleOrte.FirstOrDefault(o => o.Id == AktuellerKampf.OrtId);
            if (ort?.Arena != null && !Spieler.Orden.Contains(ort.Arena.OrdenName))
            {
                Spieler.Orden.Add(ort.Arena.OrdenName);
                AktuellerKampf.Log.Add($"🏅 {ort.Arena.OrdenName} erhalten!");
            }
        }

        // Level-Up prüfen
        PrüfeLevelUp(AktuellerKampf.SpielerMonster);
        AktuellerKampf.Phase = KampfPhase.Beendet;
        Notify();
    }

    private void KampfVerloren()
    {
        if (AktuellerKampf == null) return;
        AktuellerKampf.SpielerGewonnen = false;
        AktuellerKampf.Log.Add($"{AktuellerKampf.SpielerMonster.Name} ist ohnmächtig!");
        AktuellerKampf.Log.Add("Du wurdest besiegt...");
        // Spieler verliert Geld
        int verlust = Math.Min(Spieler.Geld / 2, 500);
        Spieler.Geld -= verlust;
        if (verlust > 0) AktuellerKampf.Log.Add($"-{verlust} Münzen verloren.");
        // Alle Monster heilen (Pokémon Center)
        foreach (var mon in Spieler.Team)
        {
            mon.AktuelleKp = mon.MaxKp;
            mon.Status = "none";
            foreach (var atk in mon.Attacken) atk.AktuelleAp = atk.MaxAp;
        }
        AktuellerKampf.Log.Add("Deine Monster wurden geheilt.");
        AktuellerKampf.Phase = KampfPhase.Beendet;
        Notify();
    }

    private void PrüfeLevelUp(MonsterInstanz mon)
    {
        int nächstesLevel = mon.Level + 1;
        int benötigteExp = nächstesLevel * nächstesLevel * 10;
        if (mon.ErfahrungsPunkte >= benötigteExp && mon.Level < 100)
        {
            mon.Level++;
            mon.ErfahrungsPunkte = 0;
            // Stats erhöhen
            mon.MaxKp += 5;
            mon.AktuelleKp = Math.Min(mon.AktuelleKp + 5, mon.MaxKp);
            mon.Angriff += 2;
            mon.Verteidigung += 2;
            mon.SpezialAngriff += 2;
            mon.SpezialVerteidigung += 2;
            mon.Initiative += 2;
            AktuellerKampf?.Log.Add($"🎉 {mon.Name} ist auf Level {mon.Level} aufgestiegen!");
        }
    }

    private int SchadenBerechnen(MonsterInstanz angreifer, MonsterInstanz verteidiger, AttackeInstanz attacke)
    {
        if (attacke.Staerke == null || attacke.Staerke == 0) return 0;
        int atk = attacke.Kategorie == "Physisch" ? angreifer.Angriff : angreifer.SpezialAngriff;
        int def = attacke.Kategorie == "Physisch" ? verteidiger.Verteidigung : verteidiger.SpezialVerteidigung;
        float multi = TypeChart.GetVerteidigungsMultiplikator(attacke.Typ, verteidiger.Typen);
        // STAB (Same Type Attack Bonus)
        float stab = angreifer.Typen.Contains(attacke.Typ) ? 1.5f : 1f;
        // Zufallsfaktor
        float zufall = (_rng.Next(85, 101)) / 100f;
        // Trefferchance
        if (attacke.Genauigkeit.HasValue && _rng.Next(1, 101) > attacke.Genauigkeit.Value)
        {
            AktuellerKampf?.Log.Add($"{angreifer.Name} hat nicht getroffen!");
            return 0;
        }
        float schaden = ((2f * angreifer.Level / 5f + 2f) * attacke.Staerke.Value * atk / def) / 50f + 2f;
        return Math.Max(1, (int)(schaden * multi * stab * zufall));
    }

    private AttackeInstanz? GegnerAttackeWählen(MonsterInstanz gegner)
    {
        var verfügbar = gegner.Attacken.Where(a => a.HatAp).ToList();
        if (!verfügbar.Any()) return null;
        return verfügbar[_rng.Next(verfügbar.Count)];
    }

    // ── Zufällige Begegnung prüfen ────────────────────────────────────────────
    public bool ZufallsBegegnungPrüfen(Ort ort)
    {
        if (!ort.WildMonster.Any()) return false;
        return _rng.Next(1, 101) <= 15; // 15% Chance
    }

    public WildBegegnung? ZufälligesWildMonster(Ort ort)
    {
        if (!ort.WildMonster.Any()) return null;
        int gesamt = ort.WildMonster.Sum(w => w.Chance);
        int würfel = _rng.Next(1, gesamt + 1);
        int kumuliert = 0;
        foreach (var w in ort.WildMonster)
        {
            kumuliert += w.Chance;
            if (würfel <= kumuliert) return w;
        }
        return ort.WildMonster.Last();
    }
}

// ── JSON-Hilfsklassen (für Deserialisierung) ─────────────────────────────────
public class TypInfoRaw
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("x2_gegen")] public List<string>? X2Gegen { get; set; }
    [JsonPropertyName("x05_gegen")] public List<string>? X05Gegen { get; set; }
    [JsonPropertyName("x0_gegen")] public List<string>? X0Gegen { get; set; }
    [JsonPropertyName("schwach_gegen")] public List<string>? SchwachGegen { get; set; }
    [JsonPropertyName("resistent_gegen")] public List<string>? ResistentGegen { get; set; }
    [JsonPropertyName("immun_gegen")] public List<string>? ImmunGegen { get; set; }
}

public class AttackeRaw
{
    [JsonPropertyName("id")] public string Id { get; set; } = "";
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("typ")] public string Typ { get; set; } = "";
    [JsonPropertyName("kategorie")] public string Kategorie { get; set; } = "";
    [JsonPropertyName("staerke")] public int? Staerke { get; set; }
    [JsonPropertyName("genauigkeit")] public int? Genauigkeit { get; set; }
    [JsonPropertyName("ap")] public int? Ap { get; set; }
}

public class MonsterRaw
{
    [JsonPropertyName("id")] public string Id { get; set; } = "";
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("typen")] public List<string>? Typen { get; set; }
    [JsonPropertyName("bild")] public string Bild { get; set; } = "";
    [JsonPropertyName("stats")] public Dictionary<string, int>? Stats { get; set; }
    [JsonPropertyName("attacken")] public List<AttackenLernEintragRaw>? Attacken { get; set; }
    [JsonPropertyName("entwickelt_zu")] public string? EntwickeltZu { get; set; }
    [JsonPropertyName("entwicklung_name")] public string? EntwicklungName { get; set; }
    [JsonPropertyName("entwicklung_level")] public int? EntwicklungLevel { get; set; }
}

public class AttackenLernEintragRaw
{
    [JsonPropertyName("attacke_id")] public string AttackeId { get; set; } = "";
    [JsonPropertyName("level")] public int Level { get; set; }
}
