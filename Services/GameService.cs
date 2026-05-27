using MonsterKampf.Data;
using MonsterKampf.Models;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonsterKampf.Services;

public class GameService
{
    private readonly HttpClient _http;
    private readonly LocalStorageService _ls;
    private readonly Random _rng = new();
    private const string LS_SAVEGAME = "monsterkampf_save";

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
    public bool HatSpeicherstand { get; private set; } = false;
    public SpielEinstellungen Einstellungen { get; private set; } = new();
    // Spiel gilt als abgeschlossen wenn der Spieler mindestens 8 Orden hat
    // (kann später auf alle Orden + Elite Vier + Rivale erweitert werden)
    public bool SpielAbgeschlossen => Spieler.Orden.Count >= 8;

    // ── Events ───────────────────────────────────────────────────────────────────────────
    public event Action? OnChange;
    public void Notify() => OnChange?.Invoke();

    public GameService(HttpClient http, LocalStorageService ls)
    {
        _http = http;
        _ls = ls;
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
            var monsterOpts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var monsterRaw = await _http.GetFromJsonAsync<List<MonsterRaw>>("data/monster.json", monsterOpts);
            if (monsterRaw != null)
            {
                AlleMonster = monsterRaw.Select(m => new MonsterData
                {
                    Id = m.Id,
                    Name = m.Name,
                    Typen = m.Typen ?? new(),
                    Bild = m.Bild,
                    Stats = m.Stats ?? new(),
                    Attacken = m.Attacken?.Select(a => new AttackenLernEintrag
                    {
                        AttackeId = a.AttackeId,
                        Level = a.Level
                    }).ToList() ?? new(),
                    EntwickeltZu = m.EntwickeltZu,
                    EntwicklungName = m.EntwicklungName,
                    EntwicklungLevel = m.EntwicklungLevel,
                    Fangrate = m.Fangrate > 0 ? m.Fangrate : 45,
                }).ToList();
            }

            AlleOrte = WeltData.AlleOrte();
            DatenGeladen = true;
            LadeStatus = "Fertig!";

            // Speicherstand prüfen
            var saveJson = await _ls.GetItemAsync(LS_SAVEGAME);
            HatSpeicherstand = !string.IsNullOrEmpty(saveJson);

            Phase = SpielPhase.Hauptmenü;
        }
        catch (Exception ex)
        {
            LadeStatus = $"Fehler beim Laden: {ex.Message}";
        }
        Notify();
    }

    // ── Navigation ───────────────────────────────────────────────────────────
    public void ZuHauptmenü() { Phase = SpielPhase.Hauptmenü; Notify(); }
    public void ZuStarterWahl() { Phase = SpielPhase.StarterWahl; Notify(); }
    public void ZuWeltkarte() { Phase = SpielPhase.Weltkarte; Notify(); }
    public void ZuMapEditor() { Phase = SpielPhase.MapEditor; Notify(); }
    public void ZuEinstellungen() { Phase = SpielPhase.Einstellungen; Notify(); }
    public void ZuZähneShop() { Phase = SpielPhase.ZähneShop; Notify(); }
    public void EinstellungenSpeichern() { Notify(); }
    public string UpgradeKaufen(ZähneUpgrade upgrade)
    {
        var info = MonsterKampf.Data.ZähneUpgradeDaten.Get(upgrade);
        if (info == null) return "❌ Upgrade nicht gefunden.";
        if (Spieler.ZähneWallet.HatUpgrade(upgrade)) return "⚠️ Upgrade bereits gekauft.";
        if (!Spieler.ZähneWallet.KannBezahlen(info.Kosten))
            return $"❌ Nicht genug Zähne! Kosten: {info.Kosten}, Verfügbar: {Spieler.ZähneWallet.VerfügbareZähne}";
        Spieler.ZähneWallet.Ausgeben(info.Kosten);
        Spieler.ZähneWallet.GekaufteUpgrades.Add(upgrade);
        Notify();
        return $"✅ {info.Emoji} {info.Name} gekauft!";
    }
    // ── Spiel starten ────────────────────────────────────────────────────────
    public void SpielStarten(string spielerName)
    {
        Spieler = new Spieler { Name = spielerName };
        // Starter-Items mitgeben
        Spieler.ItemHinzufügen("monsterball", "Monsterball", "⚪", 5);
        Spieler.ItemHinzufügen("trank", "Trank", "🧪", 3);
        Phase = SpielPhase.StarterWahl;
        Notify();
    }

    public List<MonsterData> GetStarterOptionen()
    {
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
        Spieler.AktuellerOrt = "KAN-0001";
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
            Log = new() { $"Ein wildes {gegner.Name} (Lv.{level}) erscheint!" }
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
        var spezies = AlleMonster.FirstOrDefault(m => m.Id == erstesGegnerMonster.MonsterId)
                      ?? AlleMonster[_rng.Next(AlleMonster.Count)];
        var gegner = MonsterInstanz.VonSpezies(spezies, erstesGegnerMonster.Level, AlleAttacken);

        AktuellerKampf = new KampfZustand
        {
            Typ = KampfTyp.Trainer,
            SpielerMonster = spielerMonster,
            GegnerMonster = gegner,
            GegnerName = $"{trainer.Klasse} {trainer.Name}",
            Phase = KampfPhase.Intro,
            Log = new() { $"💬 {trainer.Klasse} {trainer.Name}: \"{trainer.Dialogvor}\"" },
            TrainerId = trainer.Id,
            BelohnungGeld = trainer.Belohnung,
            AktuellerTrainer = trainer,
            TrainerMonsterIndex = 0,
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
        var spezies = AlleMonster.FirstOrDefault(m => m.Id == erstesGegnerMonster.MonsterId)
                      ?? AlleMonster[_rng.Next(AlleMonster.Count)];
        var gegner = MonsterInstanz.VonSpezies(spezies, erstesGegnerMonster.Level, AlleAttacken);

        // Arena als Trainer-Kampf mit Orden-Belohnung
        var arenaTrainer = new TrainerKampf
        {
            Id = $"arena_{ort.Id}",
            Name = ort.Arena.Leiter,
            Klasse = "Arena-Leiter",
            Belohnung = 2000 + ort.Arena.OrdenNr * 1000,
            Team = ort.Arena.Team,
            Dialogvor = $"Ich bin {ort.Arena.Leiter}, Meister des {ort.Arena.TypSpezialisierung}-Typs!",
            DialogNach = ort.Arena.OrdenName + " erhalten!",
        };

        AktuellerKampf = new KampfZustand
        {
            Typ = KampfTyp.Arena,
            SpielerMonster = spielerMonster,
            GegnerMonster = gegner,
            GegnerName = $"Arena-Leiter {ort.Arena.Leiter}",
            Phase = KampfPhase.Intro,
            Log = new() { $"🏆 Arena-Leiter {ort.Arena.Leiter} fordert dich heraus!" },
            OrtId = ort.Id,
            BelohnungGeld = arenaTrainer.Belohnung,
            AktuellerTrainer = arenaTrainer,
            TrainerMonsterIndex = 0,
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

        var spielerMon = AktuellerKampf.SpielerMonster;
        var gegnerMon = AktuellerKampf.GegnerMonster;

        // Status-Effekt vor Angriff prüfen (Lähmung, Schlaf, Eingefroren)
        if (!StatusErlaubtAngriff(spielerMon, AktuellerKampf.Log))
        {
            Notify();
            await Task.Delay(800);
            await GegnerZug();
            return;
        }

        if (attacke.AktuelleAp > 0)
        {
            attacke.AktuelleAp--;
            int schaden = SchadenBerechnen(spielerMon, gegnerMon, attacke);
            float multi = TypeChart.GetVerteidigungsMultiplikator(attacke.Typ, gegnerMon.Typen);
            gegnerMon.AktuelleKp = Math.Max(0, gegnerMon.AktuelleKp - schaden);
            AktuellerKampf.Log.Add($"⚔️ {spielerMon.AngezeigterName} setzt {attacke.Name} ein! ({schaden} Schaden)");
            if (multi >= 2f) AktuellerKampf.Log.Add("💥 Das ist sehr effektiv!");
            else if (multi <= 0f) AktuellerKampf.Log.Add("🛡️ Das hat keine Wirkung...");
            else if (multi < 1f) AktuellerKampf.Log.Add("😐 Das ist nicht sehr effektiv...");

            // Status-Effekt durch Attacke (vereinfacht: 10% Chance)
            if (schaden > 0 && _rng.Next(10) == 0)
                VersuchemStatusEffekt(gegnerMon, attacke.Typ, AktuellerKampf.Log);
        }
        else
        {
            AktuellerKampf.Log.Add($"❌ {attacke.Name} hat keine AP mehr!");
        }

        Notify();
        await Task.Delay(800);

        if (gegnerMon.IstOhnmächtig)
        {
            await KampfGewonnen();
            return;
        }

        await GegnerZug();
    }

    private async Task GegnerZug()
    {
        if (AktuellerKampf == null) return;
        var spielerMon = AktuellerKampf.SpielerMonster;
        var gegnerMon = AktuellerKampf.GegnerMonster;

        // Status-Effekt des Gegners prüfen
        if (!StatusErlaubtAngriff(gegnerMon, AktuellerKampf.Log))
        {
            Notify();
            await Task.Delay(600);
        }
        else
        {
            var gegnerAttacke = GegnerAttackeWählen(gegnerMon);
            if (gegnerAttacke != null)
            {
                int gegnerSchaden = SchadenBerechnen(gegnerMon, spielerMon, gegnerAttacke);
                float gegnerMulti = TypeChart.GetVerteidigungsMultiplikator(gegnerAttacke.Typ, spielerMon.Typen);
                spielerMon.AktuelleKp = Math.Max(0, spielerMon.AktuelleKp - gegnerSchaden);
                AktuellerKampf.Log.Add($"💢 {gegnerMon.Name} setzt {gegnerAttacke.Name} ein! ({gegnerSchaden} Schaden)");
                if (gegnerMulti >= 2f) AktuellerKampf.Log.Add("💥 Das ist sehr effektiv!");
                else if (gegnerMulti <= 0f) AktuellerKampf.Log.Add("🛡️ Das hat keine Wirkung...");
                else if (gegnerMulti < 1f) AktuellerKampf.Log.Add("😐 Das ist nicht sehr effektiv...");
            }
        }

        // Status-Schaden am Ende der Runde (Vergiftung, Verbrennung)
        StatusSchadenRunde(spielerMon, AktuellerKampf.Log);
        StatusSchadenRunde(gegnerMon, AktuellerKampf.Log);

        Notify();
        await Task.Delay(800);

        // Zuerst prüfen ob Gegner durch Status-Schaden besiegt wurde → Sieg!
        if (gegnerMon.IstOhnmächtig)
        {
            await KampfGewonnen();
            return;
        }

        if (spielerMon.IstOhnmächtig)
        {
            // Prüfen ob noch andere Monster im Team verfügbar
            var nächstes = Spieler.Team.FirstOrDefault(m => m != spielerMon && !m.IstOhnmächtig);
            if (nächstes != null)
            {
                AktuellerKampf.Log.Add($"💀 {spielerMon.AngezeigterName} ist ohnmächtig!");
                AktuellerKampf.Log.Add("Wähle dein nächstes Monster!");
                AktuellerKampf.Phase = KampfPhase.MonsterWechsel;
                Notify();
            }
            else
            {
                KampfVerloren();
            }
            return;
        }

        AktuellerKampf.Phase = KampfPhase.SpielerZug;
        Notify();
    }

    // ── Monster wechseln ─────────────────────────────────────────────────────
    public async Task MonsterWechseln(MonsterInstanz neuesMonster)
    {
        if (AktuellerKampf == null) return;
        if (neuesMonster.IstOhnmächtig) return;

        var altesMonster = AktuellerKampf.SpielerMonster;
        AktuellerKampf.SpielerMonster = neuesMonster;
        Spieler.AktivesMonsterIndex = Spieler.Team.IndexOf(neuesMonster);
        AktuellerKampf.Log.Add($"🔄 {altesMonster.AngezeigterName} zurück! {neuesMonster.AngezeigterName} kämpft weiter!");

        bool warMonsterWechselPhase = AktuellerKampf.Phase == KampfPhase.MonsterWechsel;
        AktuellerKampf.Phase = KampfPhase.GegnerZug;
        Notify();

        // Wenn erzwungener Wechsel (Monster ohnmächtig), kein Gegner-Angriff
        if (!warMonsterWechselPhase)
        {
            await Task.Delay(600);
            await GegnerZug();
        }
        else
        {
            // Erzwungener Wechsel: Gegner greift trotzdem an
            await Task.Delay(600);
            await GegnerZug();
        }
    }

    // ── Item im Kampf verwenden ───────────────────────────────────────────────
    public async Task ItemImKampfVerwenden(string itemId, MonsterInstanz ziel)
    {
        if (AktuellerKampf == null) return;
        if (AktuellerKampf.Phase != KampfPhase.SpielerZug) return;

        var item = Spieler.GetItem(itemId);
        if (item == null || item.Menge <= 0)
        {
            AktuellerKampf.Log.Add("❌ Kein Item vorhanden!");
            Notify();
            return;
        }

        bool verwendet = false;
        switch (itemId)
        {
            case "trank":
                if (!ziel.IstOhnmächtig && ziel.AktuelleKp < ziel.MaxKp)
                {
                    int heilung = 20;
                    ziel.AktuelleKp = Math.Min(ziel.MaxKp, ziel.AktuelleKp + heilung);
                    AktuellerKampf.Log.Add($"💊 Trank verwendet! {ziel.AngezeigterName} erhält {heilung} KP.");
                    verwendet = true;
                }
                break;
            case "supertrank":
                if (!ziel.IstOhnmächtig && ziel.AktuelleKp < ziel.MaxKp)
                {
                    int heilung = 50;
                    ziel.AktuelleKp = Math.Min(ziel.MaxKp, ziel.AktuelleKp + heilung);
                    AktuellerKampf.Log.Add($"💊 Supertrank verwendet! {ziel.AngezeigterName} erhält {heilung} KP.");
                    verwendet = true;
                }
                break;
            case "hypertrank":
                if (!ziel.IstOhnmächtig && ziel.AktuelleKp < ziel.MaxKp)
                {
                    int heilung = 200;
                    ziel.AktuelleKp = Math.Min(ziel.MaxKp, ziel.AktuelleKp + heilung);
                    AktuellerKampf.Log.Add($"💊 Hypertrank verwendet! {ziel.AngezeigterName} erhält {heilung} KP.");
                    verwendet = true;
                }
                break;
            case "antidot":
                if (ziel.Status == "vergiftet") { ziel.Status = "none"; AktuellerKampf.Log.Add($"✅ {ziel.AngezeigterName} ist nicht mehr vergiftet!"); verwendet = true; }
                break;
            case "paralysheiler":
                if (ziel.Status == "gelähmt") { ziel.Status = "none"; AktuellerKampf.Log.Add($"✅ {ziel.AngezeigterName} ist nicht mehr gelähmt!"); verwendet = true; }
                break;
            case "brandheiler":
                if (ziel.Status == "verbrannt") { ziel.Status = "none"; AktuellerKampf.Log.Add($"✅ {ziel.AngezeigterName} ist nicht mehr verbrannt!"); verwendet = true; }
                break;
            case "weckpille":
                if (ziel.Status == "eingeschlafen") { ziel.Status = "none"; AktuellerKampf.Log.Add($"✅ {ziel.AngezeigterName} ist aufgewacht!"); verwendet = true; }
                break;
            case "auftaupille":
                if (ziel.Status == "eingefroren") { ziel.Status = "none"; AktuellerKampf.Log.Add($"✅ {ziel.AngezeigterName} ist aufgetaut!"); verwendet = true; }
                break;
            case "vollheiler":
                if (!ziel.IstOhnmächtig) { ziel.Status = "none"; ziel.AktuelleKp = ziel.MaxKp; AktuellerKampf.Log.Add($"✅ {ziel.AngezeigterName} vollständig geheilt!"); verwendet = true; }
                break;
            case "belebung":
                if (ziel.IstOhnmächtig) { ziel.AktuelleKp = ziel.MaxKp / 2; AktuellerKampf.Log.Add($"✅ {ziel.AngezeigterName} wurde belebt!"); verwendet = true; }
                break;
            case "ap-trank":
                foreach (var atk in ziel.Attacken) atk.AktuelleAp = Math.Min(atk.MaxAp, atk.AktuelleAp + 10);
                AktuellerKampf.Log.Add($"✅ AP von {ziel.AngezeigterName} aufgefüllt!"); verwendet = true;
                break;
        }

        if (verwendet)
        {
            Spieler.ItemVerwenden(itemId);
            AktuellerKampf.Phase = KampfPhase.GegnerZug;
            Notify();
            await Task.Delay(600);
            await GegnerZug();
        }
        else
        {
            AktuellerKampf.Log.Add("❌ Item kann hier nicht verwendet werden!");
            Notify();
        }
    }

    // ── Monster fangen ────────────────────────────────────────────────────────
    public async Task MonsterFangen(string ballId)
    {
        if (AktuellerKampf == null) return;
        if (AktuellerKampf.Phase != KampfPhase.SpielerZug) return;
        if (!AktuellerKampf.KannFangen) { AktuellerKampf.Log.Add("❌ Vor einem Trainer-Kampf kann man nicht fangen!"); Notify(); return; }

        var item = Spieler.GetItem(ballId);
        if (item == null || item.Menge <= 0) { AktuellerKampf.Log.Add("❌ Kein Monsterball mehr!"); Notify(); return; }

        Spieler.ItemVerwenden(ballId);
        AktuellerKampf.Phase = KampfPhase.Fangen;
        var gegner = AktuellerKampf.GegnerMonster;
        AktuellerKampf.Log.Add($"⚪ Du wirfst einen Monsterball nach {gegner.Name}...");
        Notify();
        await Task.Delay(1200);

        // Fang-Formel (vereinfacht nach Pokémon-Formel)
        float ballBonus = ballId switch { "superball" => 1.5f, "hyperball" => 2f, "meisterball" => 255f, _ => 1f };
        float hpFaktor = (float)(3 * gegner.MaxKp - 2 * gegner.AktuelleKp) / (3f * gegner.MaxKp);
        float statusBonus = gegner.Status is "eingeschlafen" or "eingefroren" ? 2f : gegner.Status == "none" ? 1f : 1.5f;
        float fangChance = (gegner.Fangrate * hpFaktor * statusBonus * ballBonus) / 255f;
        fangChance = Math.Clamp(fangChance, 0.01f, 0.99f);

        bool gefangen = _rng.NextDouble() < fangChance;

        if (gefangen)
        {
            AktuellerKampf.Log.Add($"🎉 {gegner.Name} wurde gefangen!");
            if (Spieler.Team.Count < 6)
                Spieler.Team.Add(gegner);
            else
            {
                Spieler.Box.Add(gegner);
                AktuellerKampf.Log.Add($"📦 {gegner.Name} wurde in die Box geschickt (Team voll).");
            }
            AktuellerKampf.SpielerGewonnen = true;
            AktuellerKampf.Phase = KampfPhase.Beendet;
        }
        else
        {
            int wackler = _rng.Next(1, 4);
            AktuellerKampf.Log.Add(wackler switch
            {
                1 => "💨 Oh! Fast gefangen!",
                2 => "💨 Ärgerlich! Knapp daneben!",
                _ => "💨 Entkommen!"
            });
            AktuellerKampf.Phase = KampfPhase.GegnerZug;
            Notify();
            await Task.Delay(800);
            await GegnerZug();
            return;
        }

        Notify();
    }

    // ── Fliehen ───────────────────────────────────────────────────────────────
    public async Task KampfFliehen()
    {
        if (AktuellerKampf == null) return;
        if (AktuellerKampf.Typ != KampfTyp.Wild)
        {
            AktuellerKampf.Log.Add("❌ Vor einem Trainer-Kampf kann man nicht fliehen!");
            Notify();
            return;
        }

        AktuellerKampf.FluchtVersuche++;
        var spielerMon = AktuellerKampf.SpielerMonster;
        var gegnerMon = AktuellerKampf.GegnerMonster;

        // Flucht-Formel
        int fluchtWert = (spielerMon.Initiative * 32 / Math.Max(1, gegnerMon.Initiative / 4)) + 30 * AktuellerKampf.FluchtVersuche;
        bool erfolgreich = fluchtWert >= 255 || _rng.Next(256) < fluchtWert;

        if (erfolgreich)
        {
            AktuellerKampf.Log.Add("🏃 Du bist erfolgreich geflohen!");
            AktuellerKampf.SpielerGewonnen = false;
            AktuellerKampf.Phase = KampfPhase.Beendet;
            Notify();
        }
        else
        {
            AktuellerKampf.Log.Add("❌ Flucht fehlgeschlagen!");
            AktuellerKampf.Phase = KampfPhase.GegnerZug;
            Notify();
            await Task.Delay(600);
            await GegnerZug();
        }
    }

    public void KampfBeenden()
    {
        AktuellerKampf = null;
        Phase = SpielPhase.Weltkarte;
        Notify();
    }

    private async Task KampfGewonnen()
    {
        if (AktuellerKampf == null) return;
        var gegner = AktuellerKampf.GegnerMonster;
        var spielerMon = AktuellerKampf.SpielerMonster;

        AktuellerKampf.Log.Add($"⭐ {gegner.Name} wurde besiegt!");

        // EXP berechnen
        int exp = gegner.Level * 50;
        AktuellerKampf.ErfahrungGewonnen = exp;
        spielerMon.ErfahrungsPunkte += exp;
        AktuellerKampf.Log.Add($"+{exp} EP für {spielerMon.AngezeigterName}");

        // Level-Up prüfen
        PrüfeLevelUp(spielerMon);

        // Geld
        if (AktuellerKampf.BelohnungGeld > 0)
        {
            Spieler.Geld += AktuellerKampf.BelohnungGeld;
            AktuellerKampf.Log.Add($"💰 +{AktuellerKampf.BelohnungGeld} Münzen!");
        }

        // Trainer: nächstes Monster?
        if ((AktuellerKampf.Typ == KampfTyp.Trainer || AktuellerKampf.Typ == KampfTyp.Arena)
            && AktuellerKampf.AktuellerTrainer != null)
        {
            AktuellerKampf.TrainerMonsterIndex++;
            if (AktuellerKampf.TrainerMonsterIndex < AktuellerKampf.AktuellerTrainer.Team.Count)
            {
                var nächsterEintrag = AktuellerKampf.AktuellerTrainer.Team[AktuellerKampf.TrainerMonsterIndex];
                var nächsteSpezies = AlleMonster.FirstOrDefault(m => m.Id == nächsterEintrag.MonsterId)
                                     ?? AlleMonster[_rng.Next(AlleMonster.Count)];
                var nächsterGegner = MonsterInstanz.VonSpezies(nächsteSpezies, nächsterEintrag.Level, AlleAttacken);
                AktuellerKampf.GegnerMonster = nächsterGegner;
                AktuellerKampf.Log.Add($"🔄 {AktuellerKampf.GegnerName} schickt {nächsterGegner.Name} (Lv.{nächsterGegner.Level})!");
                AktuellerKampf.Phase = KampfPhase.SpielerZug;
                Notify();
                return;
            }
            else
            {
                // Trainer besiegt
                if (AktuellerKampf.TrainerId != null)
                    Spieler.BesiegteTrainer.Add(AktuellerKampf.TrainerId);

                // Orden für Arena
                if (AktuellerKampf.Typ == KampfTyp.Arena && AktuellerKampf.OrtId != null)
                {
                    var ort = AlleOrte.FirstOrDefault(o => o.Id == AktuellerKampf.OrtId);
                    if (ort?.Arena != null && !Spieler.Orden.Contains(ort.Arena.OrdenName))
                    {
                        Spieler.Orden.Add(ort.Arena.OrdenName);
                        AktuellerKampf.Log.Add($"🏅 {ort.Arena.OrdenName} erhalten!");
                    }
                }

                if (!string.IsNullOrEmpty(AktuellerKampf.AktuellerTrainer.DialogNach))
                    AktuellerKampf.Log.Add($"💬 {AktuellerKampf.GegnerName}: \"{AktuellerKampf.AktuellerTrainer.DialogNach}\"");
            }
        }

        // Sieg markieren
        AktuellerKampf.SpielerGewonnen = true;
        // Evolution prüfen
        Notify();
        await Task.Delay(500);
        await PrüfeEvolution(spielerMon);
    }

    private void KampfVerloren()
    {
        if (AktuellerKampf == null) return;
        AktuellerKampf.SpielerGewonnen = false;
        AktuellerKampf.Log.Add("💀 Alle Monster sind ohnmächtig...");
        AktuellerKampf.Log.Add("Du wurdest besiegt und zum Monster Center gebracht.");
        int verlust = Math.Min(Spieler.Geld / 2, 500);
        Spieler.Geld -= verlust;
        if (verlust > 0) AktuellerKampf.Log.Add($"-{verlust} Münzen verloren.");
        // Alle Monster heilen
        foreach (var mon in Spieler.Team)
        {
            mon.AktuelleKp = mon.MaxKp;
            mon.Status = "none";
            foreach (var atk in mon.Attacken) atk.AktuelleAp = atk.MaxAp;
        }
        AktuellerKampf.Phase = KampfPhase.Beendet;
        Notify();
    }

    // ── Evolution ─────────────────────────────────────────────────────────────
    private async Task PrüfeEvolution(MonsterInstanz mon)
    {
        if (AktuellerKampf == null) return;
        if (string.IsNullOrEmpty(mon.EntwickeltZu)) { AktuellerKampf.Phase = KampfPhase.Beendet; Notify(); return; }
        if (!mon.EntwicklungLevel.HasValue || mon.Level < mon.EntwicklungLevel.Value) { AktuellerKampf.Phase = KampfPhase.Beendet; Notify(); return; }

        var neueSpezies = AlleMonster.FirstOrDefault(m => m.Id == mon.EntwickeltZu);
        if (neueSpezies == null) { AktuellerKampf.Phase = KampfPhase.Beendet; Notify(); return; }

        AktuellerKampf.EntwickeltSichMonster = mon;
        AktuellerKampf.EntwickeltSichZuName = neueSpezies.Name;
        AktuellerKampf.Phase = KampfPhase.Evolution;
        AktuellerKampf.Log.Add($"✨ {mon.AngezeigterName} entwickelt sich zu {neueSpezies.Name}!");
        Notify();
        await Task.Delay(2000);

        // Evolution durchführen
        mon.SpeziesId = neueSpezies.Id;
        mon.Name = neueSpezies.Name;
        mon.Typen = new List<string>(neueSpezies.Typen);
        mon.Bild = neueSpezies.Bild;
        mon.EntwickeltZu = neueSpezies.EntwickeltZu;
        mon.EntwicklungName = neueSpezies.EntwicklungName;
        mon.EntwicklungLevel = neueSpezies.EntwicklungLevel;
        mon.Fangrate = neueSpezies.Fangrate;
        // Stats verbessern
        mon.MaxKp += 15; mon.AktuelleKp = Math.Min(mon.AktuelleKp + 15, mon.MaxKp);
        mon.Angriff += 5; mon.Verteidigung += 5; mon.SpezialAngriff += 5;
        mon.SpezialVerteidigung += 5; mon.Initiative += 5;
        // Neue Attacken lernen
        var neueAttacken = neueSpezies.Attacken.Where(a => a.Level <= mon.Level).OrderByDescending(a => a.Level).Take(4).ToList();
        foreach (var eintrag in neueAttacken)
        {
            var atk = AlleAttacken.FirstOrDefault(a => a.Id == eintrag.AttackeId);
            if (atk != null && !mon.Attacken.Any(a => a.Id == atk.Id) && mon.Attacken.Count < 4)
                mon.Attacken.Add(new AttackeInstanz { Id = atk.Id, Name = atk.Name, Typ = atk.Typ, Kategorie = atk.Kategorie, Staerke = atk.Staerke, Genauigkeit = atk.Genauigkeit, MaxAp = atk.Ap ?? 10, AktuelleAp = atk.Ap ?? 10 });
        }

        AktuellerKampf.EntwickeltSichMonster = null;
        AktuellerKampf.EntwickeltSichZuName = null;
        AktuellerKampf.Phase = KampfPhase.Beendet;
        Notify();
    }

    private void PrüfeLevelUp(MonsterInstanz mon)
    {
        int nächstesLevel = mon.Level + 1;
        int benötigteExp = nächstesLevel * nächstesLevel * 10;
        while (mon.ErfahrungsPunkte >= benötigteExp && mon.Level < 100)
        {
            mon.Level++;
            mon.ErfahrungsPunkte -= benötigteExp;
            mon.MaxKp += 5; mon.AktuelleKp = Math.Min(mon.AktuelleKp + 5, mon.MaxKp);
            mon.Angriff += 2; mon.Verteidigung += 2; mon.SpezialAngriff += 2;
            mon.SpezialVerteidigung += 2; mon.Initiative += 2;
            AktuellerKampf?.Log.Add($"🎉 {mon.AngezeigterName} ist auf Level {mon.Level} aufgestiegen!");
            nächstesLevel = mon.Level + 1;
            benötigteExp = nächstesLevel * nächstesLevel * 10;
        }
    }

    // ── Status-Effekte ────────────────────────────────────────────────────────
    private bool StatusErlaubtAngriff(MonsterInstanz mon, List<string> log)
    {
        switch (mon.Status)
        {
            case "eingeschlafen":
                mon.StatusZähler--;
                if (mon.StatusZähler <= 0) { mon.Status = "none"; log.Add($"☀️ {mon.AngezeigterName} ist aufgewacht!"); return true; }
                log.Add($"💤 {mon.AngezeigterName} schläft..."); return false;
            case "eingefroren":
                if (_rng.Next(5) == 0) { mon.Status = "none"; log.Add($"🔥 {mon.AngezeigterName} ist aufgetaut!"); return true; }
                log.Add($"🧊 {mon.AngezeigterName} ist eingefroren!"); return false;
            case "gelähmt":
                if (_rng.Next(4) == 0) { log.Add($"⚡ {mon.AngezeigterName} ist gelähmt und kann sich nicht bewegen!"); return false; }
                return true;
            default: return true;
        }
    }

    private void StatusSchadenRunde(MonsterInstanz mon, List<string> log)
    {
        switch (mon.Status)
        {
            case "vergiftet":
                int giftSchaden = Math.Max(1, mon.MaxKp / 8);
                mon.AktuelleKp = Math.Max(0, mon.AktuelleKp - giftSchaden);
                log.Add($"☠️ {mon.AngezeigterName} leidet unter Vergiftung! (-{giftSchaden} KP)");
                break;
            case "verbrannt":
                int brandSchaden = Math.Max(1, mon.MaxKp / 16);
                mon.AktuelleKp = Math.Max(0, mon.AktuelleKp - brandSchaden);
                log.Add($"🔥 {mon.AngezeigterName} leidet unter Verbrennung! (-{brandSchaden} KP)");
                break;
        }
    }

    private void VersuchemStatusEffekt(MonsterInstanz ziel, string attackeTyp, List<string> log)
    {
        if (ziel.Status != "none") return;
        string? neuerStatus = attackeTyp switch
        {
            "Brennen" => "verbrannt",
            "Eis" => "eingefroren",
            "Blitz" => "gelähmt",
            "Gift" => "vergiftet",
            _ => null
        };
        if (neuerStatus == null) return;
        ziel.Status = neuerStatus;
        if (neuerStatus == "eingeschlafen") ziel.StatusZähler = _rng.Next(2, 5);
        log.Add(neuerStatus switch
        {
            "verbrannt" => $"🔥 {ziel.Name} wurde verbrannt!",
            "eingefroren" => $"🧊 {ziel.Name} wurde eingefroren!",
            "gelähmt" => $"⚡ {ziel.Name} wurde gelähmt!",
            "vergiftet" => $"☠️ {ziel.Name} wurde vergiftet!",
            _ => ""
        });
    }

    private int SchadenBerechnen(MonsterInstanz angreifer, MonsterInstanz verteidiger, AttackeInstanz attacke)
    {
        if (attacke.Staerke == null || attacke.Staerke == 0) return 0;
        int atk = attacke.Kategorie == "Physisch" ? angreifer.Angriff : angreifer.SpezialAngriff;
        int def = attacke.Kategorie == "Physisch" ? verteidiger.Verteidigung : verteidiger.SpezialVerteidigung;
        // Verbrennung: Angriff halbiert
        if (angreifer.Status == "verbrannt" && attacke.Kategorie == "Physisch") atk /= 2;
        float multi = TypeChart.GetVerteidigungsMultiplikator(attacke.Typ, verteidiger.Typen);
        float stab = angreifer.Typen.Contains(attacke.Typ) ? 1.5f : 1f;
        float zufall = (_rng.Next(85, 101)) / 100f;
        if (attacke.Genauigkeit.HasValue && _rng.Next(1, 101) > attacke.Genauigkeit.Value)
        {
            AktuellerKampf?.Log.Add($"💨 {angreifer.AngezeigterName} hat nicht getroffen!");
            return 0;
        }
        float schaden = ((2f * angreifer.Level / 5f + 2f) * attacke.Staerke.Value * atk / Math.Max(1, def)) / 50f + 2f;
        return Math.Max(1, (int)(schaden * multi * stab * zufall));
    }

    private AttackeInstanz? GegnerAttackeWählen(MonsterInstanz gegner)
    {
        var verfügbar = gegner.Attacken.Where(a => a.HatAp).ToList();
        if (!verfügbar.Any()) return null;
        return verfügbar[_rng.Next(verfügbar.Count)];
    }

    // ── Wilde Begegnung ───────────────────────────────────────────────────────
    public bool ZufallsBegegnungPrüfen(Ort ort) => ort.WildMonster.Any();

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

    // ── Monster Center ────────────────────────────────────────────────────────
    public string MonsterZentrumHeilen()
    {
        if (!Spieler.Team.Any()) return "Dein Team ist leer.";
        foreach (var mon in Spieler.Team)
        {
            mon.AktuelleKp = mon.MaxKp;
            mon.Status = "none";
            mon.StatusZähler = 0;
            foreach (var atk in mon.Attacken) atk.AktuelleAp = atk.MaxAp;
        }
        Notify();
        return $"✅ Alle {Spieler.Team.Count} Monster wurden vollständig geheilt!";
    }

    // ── Markt ─────────────────────────────────────────────────────────────────
    public string ItemKaufen(ShopItem item)
    {
        if (Spieler.Geld < item.Preis)
            return $"❌ Nicht genug Geld! Du brauchst {item.Preis} Münzen, hast aber nur {Spieler.Geld}.";
        Spieler.Geld -= item.Preis;
        Spieler.ItemHinzufügen(item.Id, item.Name, item.Emoji);
        Notify();
        return $"✅ {item.Emoji} {item.Name} gekauft! Verbleibendes Geld: {Spieler.Geld} Münzen.";
    }

    // ── Spielstand speichern ──────────────────────────────────────────────────
    public async Task SpielstandSpeichern()
    {
        var save = new SpielstandDaten
        {
            SpielerName = Spieler.Name,
            Geld = Spieler.Geld,
            AktuellerOrt = Spieler.AktuellerOrt,
            Orden = new List<string>(Spieler.Orden),
            BesiegteTrainer = new List<string>(Spieler.BesiegteTrainer),
            Team = Spieler.Team.Select(MonsterZuSpeichern).ToList(),
            Box = Spieler.Box.Select(MonsterZuSpeichern).ToList(),
            Inventar = Spieler.Inventar.Select(i => new GespeichertesItem
            {
                ItemId = i.ItemId, Name = i.Name, Emoji = i.Emoji, Menge = i.Menge
            }).ToList(),
        };
        var json = JsonSerializer.Serialize(save);
        await _ls.SetItemAsync(LS_SAVEGAME, json);
        HatSpeicherstand = true;
        Notify();
    }

    private GespeichertesMonster MonsterZuSpeichern(MonsterInstanz m) => new()
    {
        SpeziesId = m.SpeziesId, Name = m.Name, Spitzname = m.Spitzname,
        Level = m.Level, AktuelleKp = m.AktuelleKp, MaxKp = m.MaxKp,
        ErfahrungsPunkte = m.ErfahrungsPunkte, Status = m.Status,
        Angriff = m.Angriff, Verteidigung = m.Verteidigung,
        SpezialAngriff = m.SpezialAngriff, SpezialVerteidigung = m.SpezialVerteidigung,
        Initiative = m.Initiative,
        Attacken = m.Attacken.Select(a => new GespeicherteAttacke
        {
            Id = a.Id, Name = a.Name, AktuelleAp = a.AktuelleAp, MaxAp = a.MaxAp
        }).ToList(),
    };

    // ── Spielstand laden ──────────────────────────────────────────────────────
    public async Task SpielstandLaden()
    {
        var json = await _ls.GetItemAsync(LS_SAVEGAME);
        if (string.IsNullOrEmpty(json)) return;
        try
        {
            var save = JsonSerializer.Deserialize<SpielstandDaten>(json);
            if (save == null) return;

            Spieler = new Spieler
            {
                Name = save.SpielerName,
                Geld = save.Geld,
                AktuellerOrt = save.AktuellerOrt,
                Orden = save.Orden ?? new(),
                BesiegteTrainer = save.BesiegteTrainer ?? new(),
                Inventar = save.Inventar?.Select(i => new InventarItem
                {
                    ItemId = i.ItemId, Name = i.Name, Emoji = i.Emoji, Menge = i.Menge
                }).ToList() ?? new(),
            };

            Spieler.Team.AddRange(save.Team?.Select(GespeichertesZuMonster) ?? []);
            Spieler.Box.AddRange(save.Box?.Select(GespeichertesZuMonster) ?? []);
            Phase = SpielPhase.Weltkarte;
            Notify();
        }
        catch { /* Ignorieren */ }
    }

    private MonsterInstanz GespeichertesZuMonster(GespeichertesMonster s)
    {
        var spezies = AlleMonster.FirstOrDefault(m => m.Id == s.SpeziesId);
        var mon = spezies != null
            ? MonsterInstanz.VonSpezies(spezies, s.Level, AlleAttacken)
            : new MonsterInstanz { SpeziesId = s.SpeziesId, Name = s.Name, Level = s.Level };
        mon.Spitzname = s.Spitzname;
        mon.AktuelleKp = s.AktuelleKp;
        mon.MaxKp = s.MaxKp;
        mon.ErfahrungsPunkte = s.ErfahrungsPunkte;
        mon.Status = s.Status ?? "none";
        mon.Angriff = s.Angriff; mon.Verteidigung = s.Verteidigung;
        mon.SpezialAngriff = s.SpezialAngriff; mon.SpezialVerteidigung = s.SpezialVerteidigung;
        mon.Initiative = s.Initiative;
        if (s.Attacken?.Count > 0)
            mon.Attacken = s.Attacken.Select(a => new AttackeInstanz
            {
                Id = a.Id, Name = a.Name, AktuelleAp = a.AktuelleAp, MaxAp = a.MaxAp,
                Typ = AlleAttacken.FirstOrDefault(x => x.Id == a.Id)?.Typ ?? "Normal",
                Kategorie = AlleAttacken.FirstOrDefault(x => x.Id == a.Id)?.Kategorie ?? "Physisch",
                Staerke = AlleAttacken.FirstOrDefault(x => x.Id == a.Id)?.Staerke,
                Genauigkeit = AlleAttacken.FirstOrDefault(x => x.Id == a.Id)?.Genauigkeit,
            }).ToList();
        return mon;
    }

    public async Task SpielstandLöschen()
    {
        await _ls.RemoveItemAsync(LS_SAVEGAME);
        HatSpeicherstand = false;
        Notify();
    }
}

// ── JSON-Hilfsklassen ─────────────────────────────────────────────────────────
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
    [JsonPropertyName("fangrate")] public int Fangrate { get; set; } = 45;
}

public class AttackenLernEintragRaw
{
    [JsonPropertyName("attacke_id")] public string AttackeId { get; set; } = "";
    [JsonPropertyName("level")] public int Level { get; set; }
}
