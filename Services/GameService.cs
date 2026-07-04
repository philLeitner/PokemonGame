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
    /// <summary>Letzter Ort wo das Monster-Center genutzt wurde (für Teleport im Trainerkampf)</summary>
    private string? LetztesCenterOrtId { get; set; }
    private const string LS_SAVEGAME = "monsterkampf_save";
    private const string LS_SAVEGAME_EIGENE_MAP = "monsterkampf_save_eigene_map";

    // ── Eigene-Map-Modus ─────────────────────────────────────────────────────
    /// <summary>True wenn der Spieler gerade im Eigene-Map-Modus spielt.</summary>
    public bool IstEigeneMapModus { get; private set; } = false;
    /// <summary>True wenn ein gespeicherter Eigene-Map-Spielstand existiert.</summary>
    public bool HatEigeneMapSpeicherstand { get; private set; } = false;

    // ── Daten ────────────────────────────────────────────────────────────────
    public List<MonsterData> AlleMonster { get; private set; } = new();
    public List<AttackeData> AlleAttacken { get; private set; } = new();
    public Dictionary<string, TypInfo> AlleTypen { get; private set; } = new();
    public List<Ort> AlleOrte { get; private set; } = new();
    public List<ItemDef> AlleItems { get; private set; } = new();
    public ItemDef? GetItemDef(string id) => AlleItems.FirstOrDefault(i => i.Id == id);

    // ── Regionen / Prozedurale Karte ─────────────────────────────────────────
    public List<RegionConfig> AlleRegionen { get; private set; } = new();
    public GenerierteKarte? AktuelleGenerierteKarte { get; private set; } = null;
    public bool IstGenerierteKartenModus => AktuelleGenerierteKarte != null;
    // Nach-Arenaleiter-Dialog-Daten
    public Ort? LetzterArenaLeiter { get; private set; } = null;
    public bool NachArenaLeiterLevelA { get; private set; } = true;
    /// <summary>True wenn der NachArenaLeiter-Dialog nach einem Liga-Sieg (Regionswechsel) gezeigt wird</summary>
    public bool IstNachLigaRegionswechsel { get; private set; } = false;

    // ── Spielzustand ─────────────────────────────────────────────────────────
    public SpielPhase Phase { get; private set; } = SpielPhase.Laden;
    public Spieler Spieler { get; private set; } = new();
    public KampfZustand? AktuellerKampf { get; private set; }
    public Ort? AktuellerOrt => AlleOrte.FirstOrDefault(o => o.Id == Spieler.AktuellerOrt);
    public bool DatenGeladen { get; private set; }
    public string LadeStatus { get; private set; } = "Initialisiere...";
    public bool HatSpeicherstand { get; private set; } = false;
    public SpielEinstellungen Einstellungen { get; private set; } = new();
    /// <summary>Maximale Team-Größe basierend auf aktiven Relikten (Standard: 6)</summary>
    public int MaxTeamGröße
    {
        get
        {
            if (Einstellungen.HatRelikt(ReliktTyp.NurEinMonster)) return 1;
            if (Einstellungen.HatRelikt(ReliktTyp.MaxZweiMonster)) return 2;
            if (Einstellungen.HatRelikt(ReliktTyp.MaxDreiMonster)) return 3;
            if (Einstellungen.HatRelikt(ReliktTyp.MaxVierMonster)) return 4;
            if (Einstellungen.HatRelikt(ReliktTyp.MaxFünfMonster)) return 5;
            int basis = 6;
            // MehrTeamSlots-Upgrade: +2 Team-Slots (max 8)
            if (Spieler.ZähneWallet.HatUpgrade(ZähneUpgrade.MehrTeamSlots)) basis = Math.Min(8, basis + 2);
            // ExtraSlot-Upgrade: +1 Team-Slot (max 7)
            if (Spieler.ZähneWallet.HatUpgrade(ZähneUpgrade.ExtraSlot)) basis = Math.Min(8, basis + 1);
            return basis;
        }
    }
    // Spiel gilt als abgeschlossen wenn der Spieler mindestens 8 Orden hat
    // (kann später auf alle Orden + Elite Vier + Rivale erweitert werden)
    public bool SpielAbgeschlossen => Spieler.Orden.Count >= 8;

    /// <summary>Anzahl abgeschlossener Regionen (0-basiert, 0 = erste Region läuft)</summary>
    public int AbgeschlosseneRegionenAnzahl { get; set; } = 0;

    /// <summary>Maximales Level für die aktuelle Region: Region 1 = 100, Region 2 = 200, usw.</summary>
    public int AktuellesLevelCap => (AbgeschlosseneRegionenAnzahl + 1) * 100;

    /// <summary>Gibt das Level-Cap für eine bestimmte Region zurück (1-basiert).</summary>
    public static int LevelCapFürRegion(int regionNummer) => regionNummer * 100;
    /// <summary>Ob der Einrichtungs-Wizard nach dem Prof.-Dialog bereits abgeschlossen wurde</summary>
    public bool WizardAbgeschlossen { get; private set; } = false;
    public void WizardAbschliessen() { WizardAbgeschlossen = true; Notify(); }
    public void WizardZurücksetzen() { WizardAbgeschlossen = false; }

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
            var typenRaw = await _http.GetFromJsonAsync<Dictionary<string, TypInfoRaw>>("assets/daten/typen.json", typOpts);
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
            var attackenRaw = await _http.GetFromJsonAsync<List<AttackeRaw>>("assets/daten/attacken.json", attackOpts);
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
                    Statuseffekt = a.Statuseffekt,
                    StatuseffektChance = a.StatuseffektChance,
                    Generation = a.Generation,
                }).ToList();
            }

            LadeStatus = "Lade Monster...";
            Notify();
            var monsterOpts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var monsterRaw = await _http.GetFromJsonAsync<List<MonsterRaw>>("assets/daten/monster.json", monsterOpts);
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
                    TmAttacken = m.TmAttacken ?? new(),
                    EntwickeltZu = m.EntwickeltZu,
                    EntwicklungName = m.EntwicklungName,
                    EntwicklungLevel = m.EntwicklungLevel,
                    Fangrate = m.Fangrate > 0 ? m.Fangrate : 45,
                }).ToList();
            }

            // Items laden
            LadeStatus = "Lade Items...";
            Notify();
            var itemOpts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var itemDefs = await _http.GetFromJsonAsync<List<ItemDef>>("assets/daten/items.json", itemOpts);
            if (itemDefs?.Any() == true)
                AlleItems = itemDefs;

            // Regionen laden (enthält TrainerPool + MonsterPool)
            LadeStatus = "Lade Regionen...";
            Notify();
            var regOpts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var regionen = await _http.GetFromJsonAsync<List<RegionConfig>>("assets/daten/regionen.json", regOpts);
            if (regionen?.Any() == true)
                AlleRegionen = regionen;

            DatenGeladen = true;
            LadeStatus = "Fertig!";

            // Speicherstand prüfen
            var saveJson = await _ls.GetItemAsync(LS_SAVEGAME);
            HatSpeicherstand = !string.IsNullOrEmpty(saveJson);
            var eigeneMapJson = await _ls.GetItemAsync(LS_SAVEGAME_EIGENE_MAP);
            HatEigeneMapSpeicherstand = !string.IsNullOrEmpty(eigeneMapJson);

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
    // Map-Editor entfernt - Generator macht alles automatisch
    public void ZuEinstellungen() { Phase = SpielPhase.Einstellungen; Notify(); }

    /// <summary>Wechselt in den Eigene-Map-Modus. Zeigt zuerst den Start-Dialog.</summary>
    public void ZuEigeneMapStart() { Phase = SpielPhase.EigeneMapStart; Notify(); }
    public void ZuAdminPanel() { Phase = SpielPhase.AdminPanel; Notify(); }
    public void ZuPokédex()
    {
        if (Einstellungen.HatRelikt(ReliktTyp.KeinPokédex))
        {
            // Pokédex gesperrt – nicht wechseln, nur Meldung
            return;
        }
        Phase = SpielPhase.Pokédex; Notify();
    }
    public void ZuMonsterEditor() { Phase = SpielPhase.MonsterEditor; Notify(); }
    public void ZuRegionsWahl() { Phase = SpielPhase.RegionsWahl; Notify(); }
    public void ZuNachArenaLeiter() { Phase = SpielPhase.NachArenaLeiter; Notify(); }
    public void ZuStarterWahlNeuRegion() { Phase = SpielPhase.StarterWahlNeuRegion; Notify(); }

    /// <summary>Startet die Eigene Map: entweder Spielstand kopieren oder neu starten.</summary>
    public async Task EigeneMapStarten(bool spielstandKopieren)
    {
        // Aktuellen normalen Spielstand sichern (falls noch nicht gespeichert)
        if (!IstEigeneMapModus && Spieler.Team.Any())
            await SpielstandSpeichern(); // normalen Stand sichern

        if (spielstandKopieren && HatSpeicherstand)
        {
            // Normalen Spielstand als Basis für eigene Map laden
            var json = await _ls.GetItemAsync(LS_SAVEGAME);
            if (!string.IsNullOrEmpty(json))
            {
                // Direkt in eigene Map Slot speichern
                await _ls.SetItemAsync(LS_SAVEGAME_EIGENE_MAP, json);
            }
        }
        else if (!spielstandKopieren)
        {
            // Prüfen ob bereits ein eigener Map-Spielstand existiert
            var eigeneJson = await _ls.GetItemAsync(LS_SAVEGAME_EIGENE_MAP);
            if (string.IsNullOrEmpty(eigeneJson))
            {
                // Kein eigener Spielstand – neuen erstellen (Starter-Wahl)
                IstEigeneMapModus = true;
                Notify();
                Phase = SpielPhase.StarterWahl;
                Notify();
                return;
            }
        }

        // Eigenen Map-Spielstand laden
        IstEigeneMapModus = true;
        await EigeneMapSpielstandLaden();
    }

    /// <summary>Wechselt zurück zum normalen Spielstand (Gen 1-8).</summary>
    public async Task ZumNormalenSpielstandWechseln()
    {
        if (IstEigeneMapModus && Spieler.Team.Any())
            await EigeneMapSpielstandSpeichern(); // eigenen Stand sichern

        IstEigeneMapModus = false;
        await SpielstandLaden();
        if (!HatSpeicherstand)
            Phase = SpielPhase.Hauptmenü;
        Notify();
    }
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
        // LevelBoost-Upgrades: sofort auf Team anwenden
        switch (upgrade)
        {
            case ZähneUpgrade.LevelBoost5:
                foreach (var m in Spieler.Team) { m.Level = Math.Min(m.Level + 5, AktuellesLevelCap); m.ErfahrungsPunkte = m.Level * m.Level * 100; }
                break;
            case ZähneUpgrade.LevelBoost10:
                foreach (var m in Spieler.Team) { m.Level = Math.Min(m.Level + 10, AktuellesLevelCap); m.ErfahrungsPunkte = m.Level * m.Level * 100; }
                break;
            case ZähneUpgrade.LevelBoost20:
                foreach (var m in Spieler.Team) { m.Level = Math.Min(m.Level + 20, AktuellesLevelCap); m.ErfahrungsPunkte = m.Level * m.Level * 100; }
                break;
            case ZähneUpgrade.LevelBoost25:
                if (Spieler.Team.Any()) { var mon = Spieler.Team[0]; mon.Level = Math.Min(mon.Level + 25, AktuellesLevelCap); mon.ErfahrungsPunkte = mon.Level * mon.Level * 100; }
                break;
            case ZähneUpgrade.LevelBoost50:
                if (Spieler.Team.Any()) { var mon = Spieler.Team[0]; mon.Level = Math.Min(mon.Level + 50, AktuellesLevelCap); mon.ErfahrungsPunkte = mon.Level * mon.Level * 100; }
                break;
            case ZähneUpgrade.Meisterball:
                // Meisterball ins Inventar legen
                var mbItem = Spieler.Inventar.FirstOrDefault(i => i.ItemId == "item-011");
                if (mbItem != null) mbItem.Menge++;
                else Spieler.Inventar.Add(new InventarItem { ItemId = "item-011", Menge = 1 });
                break;
            // StatBoost-Upgrades: dauerhafter +10% Bonus (wird bei KP-Berechnung berücksichtigt)
            // Diese Upgrades werden passiv in der Stat-Berechnung ausgewertet
        }
        Notify();
        return $"✅ {info.Emoji} {info.Name} gekauft!";
    }
    // ── Prozedurale Karte ─────────────────────────────────────────────────────

    /// <summary>
    /// Generiert eine neue Karte aus Seed + Regionsauswahl.
    /// Ersetzt AlleOrte durch die generierten Ort-Kopien und setzt den Spieler auf den Startort.
    /// </summary>
    public void KarteGenerieren(string seedCode, List<string> regionsReihenfolge)
    {
        if (!AlleRegionen.Any()) return;

        // Original-Orte sichern (für Rückkehr zum klassischen Modus)
        if (_originalOrte == null)
            _originalOrte = new List<Ort>(AlleOrte);

        var (neueOrte, meta) = KartenGenerator.Generiere(
            seedCode, regionsReihenfolge, AlleRegionen);

        AktuelleGenerierteKarte = meta;

        // AlleOrte durch generierte Orte ersetzen
        AlleOrte = neueOrte;

        // Spieler auf Startort setzen
        if (!string.IsNullOrEmpty(meta.StartOrtId))
            Spieler.AktuellerOrt = meta.StartOrtId;

        // Wizard zurücksetzen (neues Spiel = Wizard muss erneut durchlaufen werden)
        WizardZurücksetzen();

        // Direkt zur Weltkarte – Wizard wird durch Prof.-Dialog ausgelöst
        Phase = SpielPhase.Weltkarte;
        Notify();
    }

    /// <summary>3 zufällige Monster aus dem Gesamt-Pool als Starter (keine Legendären)</summary>
    public List<MonsterData> GetZufälligeStarterOptionen()
    {
        var pool = AlleMonster
            .Where(m => m.Fangrate >= 10)
            .OrderBy(_ => _rng.Next())
            .Take(3)
            .ToList();
        return pool;
    }

    /// <summary>3 zufällige Monster aus der aktuellen Region als Starter</summary>
    public List<MonsterData> GetZufälligeStarterNurRegion(string regionId)
    {
        var region = AlleRegionen.FirstOrDefault(r => r.Id == regionId);
        if (region == null) return GetZufälligeStarterOptionen();
        // Alle Monster der Region aus den Gebieten sammeln
        var regionMonsterIds = AlleOrte
            .Where(o => o.Id.StartsWith(regionId, StringComparison.OrdinalIgnoreCase))
            .SelectMany(o => o.WildMonster.Select(w => w.MonsterId))
            .Distinct().ToHashSet();
        var pool = AlleMonster
            .Where(m => regionMonsterIds.Contains(m.Id) && m.Fangrate >= 10)
            .OrderBy(_ => _rng.Next())
            .Take(3)
            .ToList();
        if (pool.Count < 3)
            pool = GetZufälligeStarterOptionen();
        return pool;
    }

    /// <summary>3 echte Starter-Pokémon aus allen Regionen zufällig</summary>
    public List<MonsterData> GetZufälligeEchteStarter()
    {
        var alleStarter = AlleRegionen
            .SelectMany(r => r.Starter)
            .Distinct()
            .Select(id => AlleMonster.FirstOrDefault(m => m.Id == id || m.Id == $"PKM-{id.TrimStart('#')}"))
            .Where(m => m != null).Cast<MonsterData>()
            .OrderBy(_ => _rng.Next())
            .Take(3)
            .ToList();
        if (alleStarter.Count < 3)
            alleStarter = GetZufälligeStarterOptionen();
        return alleStarter;
    }

    /// <summary>Starter wählen im Wizard (ohne Phasenwechsel)</summary>
    public void StarterWählenImWizard(string monsterId)
    {
        var spezies = AlleMonster.FirstOrDefault(m => m.Id == monsterId);
        if (spezies == null) return;
        Spieler.Team.Clear();
        var starter = MonsterInstanz.VonSpezies(spezies, 5, AlleAttacken);
        Spieler.Team.Add(starter);
        Spieler.StarterMonsterId = monsterId;
        Notify();
    }

    // Original-Orte für Rückkehr zum klassischen Modus
    private List<Ort>? _originalOrte = null;

    /// <summary>Gleiche Regionen, neuer Seed – Spieler-Fortschritt wird zurückgesetzt.</summary>
    public void NeuGenerieren()
    {
        if (AktuelleGenerierteKarte == null || _originalOrte == null) return;
        // Regionen aus der aktuellen Karte merken
        var regionsReihenfolge = AktuelleGenerierteKarte.RegionsReihenfolge;
        // Spieler-Fortschritt zurücksetzen (Team behalten, Orden/Fortschritt weg)
        var spielerName = Spieler.Name;
        var team = Spieler.Team;
        var starter = Spieler.StarterMonsterId;
        Spieler = new Spieler { Name = spielerName, StarterMonsterId = starter };
        Spieler.Team.AddRange(team);
        Spieler.ItemHinzufügen("monsterball", "Monsterball", "⚪", "Pokébälle", 5);
        Spieler.ItemHinzufügen("trank", "Trank", "🧪", "Heilitems", 3);
        // Neue Karte mit neuem Seed generieren
        var neuerSeed = KartenGenerator.GeneriereSeedCode();
        var (neueOrte, meta) = KartenGenerator.Generiere(neuerSeed, regionsReihenfolge, AlleRegionen, _originalOrte);
        AktuelleGenerierteKarte = meta;
        AlleOrte = neueOrte;
        if (!string.IsNullOrEmpty(meta.StartOrtId))
            Spieler.AktuellerOrt = meta.StartOrtId;
        // Wenn kein Team mehr → StarterWahl anzeigen
        Phase = Spieler.Team.Count == 0 ? SpielPhase.StarterWahl : SpielPhase.Weltkarte;
        Notify();
    }

    /// <summary>Stellt die Original-Weltkarte wieder her (nach generierter Karte).</summary>
    public void GenerierteKarteBeenden()
    {
        if (_originalOrte != null)
        {
            AlleOrte = _originalOrte;
            _originalOrte = null;
        }
        AktuelleGenerierteKarte = null;
        LetzterArenaLeiter = null;
        Phase = SpielPhase.Hauptmenü;
        Notify();
    }

    /// <summary>Startet ein neues Spiel im Regionen-Modus (geht zur Regionsauswahl).</summary>
    public void GenerierteKarteSpielStarten(string spielerName)
    {
        Spieler = new Spieler { Name = spielerName };
        Spieler.ItemHinzufügen("monsterball", "Monsterball", "⚪", "Pokébälle", 5);
        Spieler.ItemHinzufügen("trank", "Trank", "🧪", "Heilitems", 3);
        Phase = SpielPhase.RegionsWahl;
        Notify();
    }

    /// <summary>Navigiert den Spieler zu einem Ort der generierten Karte (Fog-of-War beachten).</summary>
    public void GenerierteOrtBetreten(string ortId)
    {
        if (AktuelleGenerierteKarte == null) return;
        // Fog-of-War: nur freigeschaltete Orte betreten
        if (!AktuelleGenerierteKarte.FreigeschalteteOrte.Contains(ortId)) return;
        Spieler.AktuellerOrt = ortId;
        // Fog-of-War: Betreten einer Arena-Stadt MIT Orden → nächste Orte freischalten
        var ort = AlleOrte.FirstOrDefault(o => o.Id == ortId);
        if (ort?.Arena != null && Spieler.Orden.Contains(ort.Arena.OrdenName))
            FogOfWarFreischaltenNachOrt(ortId);
        Notify();
    }

    /// <summary>Schaltet alle Orte bis zur nächsten Arena frei (Fog-of-War nach Orden).</summary>
    private void FogOfWarFreischaltenNachOrt(string arenaOrtId)
    {
        if (AktuelleGenerierteKarte == null) return;
        var reihenfolge = AktuelleGenerierteKarte.OrtReihenfolge;
        int aktIdx = reihenfolge.IndexOf(arenaOrtId);
        if (aktIdx < 0) return;
        int naechsteArenaIdx = -1;
        for (int i = aktIdx + 1; i < reihenfolge.Count; i++)
        {
            var o = AlleOrte.FirstOrDefault(x => x.Id == reihenfolge[i]);
            if (o?.Arena != null) { naechsteArenaIdx = i; break; }
        }
        int bisIdx = naechsteArenaIdx >= 0 ? naechsteArenaIdx : reihenfolge.Count - 1;
        for (int i = 0; i <= bisIdx; i++)
            AktuelleGenerierteKarte.FreigeschalteteOrte.Add(reihenfolge[i]);
        AktuelleGenerierteKarte.FreigeschalteBisIndex = bisIdx;
    }

    /// <summary>Berechnet Gegner-Level basierend auf Spieler-Level (Option A: Skalierung).</summary>
    public int GegnerLevelBerechnen(int basisLevel)
    {
        int level = basisLevel;
        if (IstGenerierteKartenModus && NachArenaLeiterLevelA)
        {
            var spielerLevel = Spieler.Team
                .Where(m => !m.IstOhnmächtig)
                .Select(m => m.Level)
                .DefaultIfEmpty(5)
                .Max();
            level = basisLevel + Math.Max(0, spielerLevel - 5);
        }
        // HöhereLevel-Relikt: alle Gegner +5 Level
        if (Einstellungen.HatRelikt(ReliktTyp.HöhereLevel)) level += 5;
        return level;
    }

    /// <summary>
    /// Gibt das Level für einen Ort basierend auf seiner Position in der generierten Reihenfolge zurück.
    /// Position 0 = Level 2-3, jede Position +1 Level, Arenen +3 Bonus.
    /// Im klassischen Modus: gibt (0,0) zurück = kein Override.
    /// </summary>
    public (int Min, int Max) EbenenLevelBerechnen(string ortId, bool istArena = false)
    {
        if (!IstGenerierteKartenModus || AktuelleGenerierteKarte == null)
            return (0, 0);
        // Distanz vom Startort (Anzahl Schritte) = Level-Basis
        if (!AktuelleGenerierteKarte.OrtDistanzen.TryGetValue(ortId, out int distanz))
        {
            // Fallback: Index in Reihenfolge
            distanz = AktuelleGenerierteKarte.OrtReihenfolge.IndexOf(ortId);
            if (distanz < 0) return (0, 0);
        }
        int basis = distanz + 1; // Distanz 0 = Level 1
        int min = basis;
        int max = basis + 1;
        if (istArena) { min += 2; max += 3; }
        return (min, max);
    }

    /// <summary>
    /// Liefert das korrekte, nach Ebene skalierte Level für ein Trainer-/Arena-Monster.
    /// Genau wie bei wilden Monstern wird das Level aus der Ebenen-Distanz des aktuellen Orts abgeleitet.
    /// Wenn das im Team gespeicherte Level grob unplausibel ist (0, oder weit von der Ebene entfernt),
    /// wird es durch das Ebenen-Level ersetzt. So skalieren ALLE Trainer zuverlässig – auch wenn
    /// der Generator einen Ort nicht erfasst hat. Im klassischen Modus bleibt das Original-Level erhalten.
    /// </summary>
    public int TrainerLevelKorrigieren(string? ortId, int eintragLevel, bool istArena = false)
    {
        // Klassischer Modus: keine Änderung
        if (!IstGenerierteKartenModus || AktuelleGenerierteKarte == null)
            return Math.Max(2, eintragLevel);
        if (string.IsNullOrEmpty(ortId))
            return Math.Max(2, eintragLevel);

        // Distanz vom Startort ermitteln (gleiche Quelle wie der Generator)
        if (!AktuelleGenerierteKarte.OrtDistanzen.TryGetValue(ortId, out int distanz))
        {
            distanz = AktuelleGenerierteKarte.OrtReihenfolge.IndexOf(ortId);
            if (distanz < 0) return Math.Max(2, eintragLevel); // Ort unbekannt – Original
        }

        // EXAKT die gleiche Formel wie wilde Monster (KartenGenerator):
        //   baseLvl = 2 + dist*1.8 ; Trainer = baseLvl + 1 ; Arena = baseLvl + 2
        int baseLvl = Math.Max(2, 2 + (int)(distanz * 1.8));
        int zielLevel = istArena ? baseLvl + 2 : baseLvl + 1;

        // HöhereLevel-Relikt: Trainer ebenfalls anheben (konsistent mit Wild-Logik)
        if (Einstellungen.HatRelikt(ReliktTyp.HöhereLevel)) zielLevel += 5;

        return Math.Max(2, zielLevel);
    }

    // GenerierteArenaKampfStarten wird nicht mehr benötigt –
    // der normale ArenaKampfStarten in Weltkarte.razor übernimmt das.

    /// <summary>Wird nach Arenaleiter-Sieg aufgerufen: Orden vergeben, Fog-of-War freischalten, Dialog starten.</summary>
    public void GenerierteArenaGewonnen(Ort arenaOrt)
    {
        if (AktuelleGenerierteKarte == null) return;
        LetzterArenaLeiter = arenaOrt;
        AktuelleGenerierteKarte.BesiegteArenen.Add(arenaOrt.Id);
        FogOfWarFreischaltenNachOrt(arenaOrt.Id);

        // Prüfen ob das der LETZTE Boss der LETZTEN Region ist
        bool istLetzterBossAllerRegionen = AktuelleGenerierteKarte.BossIds.Count > 0
            && arenaOrt.Id == AktuelleGenerierteKarte.BossIds.Last();

        if (istLetzterBossAllerRegionen)
        {
            // Liga-Abschluss: Glückwunsch-Screen
            Phase = SpielPhase.LigaAbschluss;
        }
        else
        {
            // Normaler Arena-Sieg: NachArenaLeiter-Dialog
            Phase = SpielPhase.NachArenaLeiter;
        }
        Notify();
    }

    /// <summary>Prüft ob es noch eine weitere Region nach der aktuellen gibt.</summary>
    public bool HatNächsteRegion()
    {
        if (AktuelleGenerierteKarte == null) return false;
        var regionen = AktuelleGenerierteKarte.RegionsReihenfolge;
        // Finde die Region des letzten besiegten Bosses
        var letzterBoss = LetzterArenaLeiter;
        if (letzterBoss == null) return false;
        var prefix = letzterBoss.Id.Split('-')[0].ToUpper();
        int idx = regionen.IndexOf(prefix);
        return idx >= 0 && idx < regionen.Count - 1;
    }

    /// <summary>Gibt die nächste RegionConfig zurück (nach der aktuellen Liga).</summary>
    public RegionConfig? GetNächsteRegion()
    {
        if (AktuelleGenerierteKarte == null || LetzterArenaLeiter == null) return null;
        var regionen = AktuelleGenerierteKarte.RegionsReihenfolge;
        var prefix = LetzterArenaLeiter.Id.Split('-')[0].ToUpper();
        int idx = regionen.IndexOf(prefix);
        if (idx < 0 || idx >= regionen.Count - 1) return null;
        var nextId = regionen[idx + 1];
        return AlleRegionen.FirstOrDefault(r => r.Id.Equals(nextId, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>Liga-Abschluss: zur nächsten Region wechseln (NachArenaLeiter-Wizard).</summary>
    public void LigaAbschlussNächsteRegion()
    {
        // Zähne für die abgeschlossene Region vergeben
        int zähne = Einstellungen.GetGesamtZähne();
        if (zähne > 0)
        {
            Spieler.ZähneWallet.Verdienen(zähne);
        }
        AbgeschlosseneRegionenAnzahl++;
        IstNachLigaRegionswechsel = true;  // Level-Option Popup zeigen
        Phase = SpielPhase.NachArenaLeiter;
        Notify();
    }

    /// <summary>Liga-Abschluss: gleiche Map neu starten.</summary>
    /// <summary>Liga-Abschluss: Neustart mit ausgewählten Monstern in Box.</summary>
    public void LigaAbschlussNeustartMitAuswahl(List<MonsterInstanz> mitnehmen)
    {
        // Alle Monster sammeln
        var alleMonster = Spieler.Team.Concat(Spieler.Box).ToList();
        Spieler.Team.Clear();
        Spieler.Box.Clear();
        // Nur ausgewählte in die Box
        foreach (var mon in alleMonster)
        {
            if (mitnehmen.Contains(mon))
                Spieler.Box.Add(mon);
            // Nicht ausgewählte werden freigelassen (nicht hinzugefügt)
        }
        // Rest des Resets
        LigaAbschlussNeustart(true); // true = mitnehmen (Box ist schon befüllt)
    }

    public void LigaAbschlussNeustart(bool monsterMitnehmen)
    {
        if (!monsterMitnehmen)
        {
            Spieler.Team.Clear();
            Spieler.Box.Clear();
        }
        else
        {
            // Alle Monster in die Box
            foreach (var m in Spieler.Team)
                Spieler.Box.Add(m);
            Spieler.Team.Clear();
        }
        Spieler.Orden.Clear();
        Spieler.BesiegteTrainer.Clear();
        Spieler.Geld = 3000; // Startgeld zurücksetzen
        // Inventar zurücksetzen (Karte entfernen, aber Items behalten wenn Monster mitgenommen)
        var karteItem = Spieler.Inventar.FirstOrDefault(i => i.ItemId == "ITEM-KARTE-GEN");
        if (karteItem != null) Spieler.Inventar.Remove(karteItem);
        // Prof. Eich Dialog zurücksetzen
        WizardZurücksetzen();
        // NPC-Gespräche zurücksetzen (damit Prof. Eich wieder angesprochen werden kann)
        var startOrtId = AktuelleGenerierteKarte?.StartOrtId;
        if (startOrtId != null)
        {
            var startOrt2 = AlleOrte.FirstOrDefault(o => o.Id == startOrtId);
            if (startOrt2 != null)
            {
                foreach (var npc in startOrt2.NPCs)
                    Spieler.BesproacheneNPCs.Remove(npc.Id);
            }
        }
        AktuelleGenerierteKarte?.BesiegteArenen.Clear();
        AktuelleGenerierteKarte?.FreigeschalteteOrte.Clear();
        // Startort zurücksetzen
        if (AktuelleGenerierteKarte != null)
        {
            Spieler.AktuellerOrt = AktuelleGenerierteKarte.StartOrtId;
            // Startpunkt + Nachbarn freischalten
            var startOrt = AlleOrte.FirstOrDefault(o => o.Id == AktuelleGenerierteKarte.StartOrtId);
            if (startOrt != null)
            {
                AktuelleGenerierteKarte.FreigeschalteteOrte.Add(startOrt.Id);
                foreach (var nId in startOrt.Verbindungen)
                    AktuelleGenerierteKarte.FreigeschalteteOrte.Add(nId);
            }
        }
        LetzterArenaLeiter = null;
        AbgeschlosseneRegionenAnzahl = 0; // Regionen-Zähler zurücksetzen
        Phase = SpielPhase.Weltkarte;
        Notify();
    }

    /// <summary>Liga-Abschluss: zum Hauptmenü zurück.</summary>
    public async Task LigaAbschlussHauptmenü(bool spielstandSpeichern)
    {
        // Zähne für die letzte Region vergeben (falls noch nicht passiert)
        int zähne = Einstellungen.GetGesamtZähne();
        if (zähne > 0 && Spieler.ZähneWallet.GesamtZähne == 0)
        {
            Spieler.ZähneWallet.Verdienen(zähne);
        }
        if (spielstandSpeichern)
            await SpielstandSpeichern();
        Phase = SpielPhase.Hauptmenü;
        Notify();
    }

    /// <summary>Nach-Arenaleiter-Dialog: Level-Option A (Skalierung) oder B (Reset auf 5) wählen.</summary>
    public void NachArenaLeiterLevelOptionWählen(bool optionA)
    {
        NachArenaLeiterLevelA = optionA;
        if (!optionA)
        {
            // Option B: alle Monster auf Level 5 zurücksetzen
            foreach (var mon in Spieler.Team)
            {
                var spezies = AlleMonster.FirstOrDefault(m => m.Id == mon.SpeziesId);
                if (spezies != null)
                {
                    var neu = MonsterInstanz.VonSpezies(spezies, 5, AlleAttacken);
                    mon.Level = 5;
                    mon.MaxKp = neu.MaxKp;
                    mon.AktuelleKp = neu.MaxKp;
                    mon.Angriff = neu.Angriff;
                    mon.Verteidigung = neu.Verteidigung;
                    mon.SpezialAngriff = neu.SpezialAngriff;
                    mon.SpezialVerteidigung = neu.SpezialVerteidigung;
                    mon.Initiative = neu.Initiative;
                }
            }
        }
        Notify();
    }

    /// <summary>Neuen Starter für die nächste Region wählen.</summary>
    public void NeuenStarterWählen(string monsterId)
    {
        var spezies = AlleMonster.FirstOrDefault(m => m.Id == monsterId);
        if (spezies == null) return;
        // Level = schwächstes Monster im Team (oder 5)
        int neuesLevel = Spieler.Team
            .Where(m => !m.IstOhnmächtig)
            .Select(m => m.Level)
            .DefaultIfEmpty(5)
            .Min();
        var starter = MonsterInstanz.VonSpezies(spezies, neuesLevel, AlleAttacken);
        Spieler.Team.Add(starter);
        Phase = SpielPhase.Weltkarte;
        Notify();
    }

    /// <summary>Nach-Arenaleiter-Dialog abschließen und zur Weltkarte zurückkehren.</summary>
    public void NachArenaLeiterAbschließen()
    {
        LetzterArenaLeiter = null;
        IstNachLigaRegionswechsel = false;  // zurücksetzen
        Phase = SpielPhase.Weltkarte;
        Notify();
    }

    /// <summary>Gibt die RegionConfig für einen Ort zurück (anhand ID-Prefix).</summary>
    public RegionConfig? GetRegionFürOrt(Ort ort)
    {
        var prefix = ort.Id.Split('-')[0].ToUpper();
        return AlleRegionen.FirstOrDefault(r => r.Id.Equals(prefix, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>Gibt die Starter-Optionen für eine bestimmte Region zurück.</summary>
    public List<MonsterData> GetStarterFürRegion(string regionId)
    {
        var region = AlleRegionen.FirstOrDefault(r => r.Id == regionId);
        if (region == null) return new();
        return region.Starter
            .Select(id => AlleMonster.FirstOrDefault(m => m.Id == id || m.Id == $"PKM-{id.TrimStart('#')}"))
            .Where(m => m != null).Cast<MonsterData>().ToList();
    }

    // ── Spiel starten ────────────────────────────────────────────────────────
    public void SpielStarten(string spielerName)
    {
        Spieler = new Spieler { Name = spielerName };
        // Starter-Items mitgeben
        Spieler.ItemHinzufügen("monsterball", "Monsterball", "⚪", "Pokébälle", 5);
        Spieler.ItemHinzufügen("trank", "Trank", "🧪", "Heilitems", 3);
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
        // ZufälligerStarter-Relikt: Starter wird zufällig aus dem Pool gewählt
        if (Einstellungen.HatRelikt(ReliktTyp.ZufälligerStarter))
        {
            var pool = GetStarterOptionen();
            if (pool.Any())
                monsterId = pool[_rng.Next(pool.Count)].Id;
        }
        var spezies = AlleMonster.FirstOrDefault(m => m.Id == monsterId);
        if (spezies == null) return;
        var starter = MonsterInstanz.VonSpezies(spezies, 5, AlleAttacken);
        Spieler.Team.Add(starter);
        Spieler.StarterMonsterId = monsterId;
        Spieler.GefangeneMonster.Add(monsterId);  // Starter dauerhaft als gefangen markieren
        Spieler.GeseheneMonster.Add(monsterId);
        // ZufälligesTeam-Relikt: 2 weitere zufällige Monster ins Team
        if (Einstellungen.HatRelikt(ReliktTyp.ZufälligesTeam))
        {
            var extraPool = AlleMonster.Where(m => m.Id != monsterId && m.Fangrate >= 10)
                                       .OrderBy(_ => _rng.Next()).Take(2).ToList();
            foreach (var extra in extraPool)
            {
                var extraMon = MonsterInstanz.VonSpezies(extra, 5, AlleAttacken);
                Spieler.Team.Add(extraMon);
                Spieler.GefangeneMonster.Add(extra.Id);
                Spieler.GeseheneMonster.Add(extra.Id);
            }
        }
        Spieler.AktuellerOrt = "KAN-0001";
        Phase = SpielPhase.Weltkarte;
        Notify();
    }

    // ── Ort betreten ─────────────────────────────────────────────────────────
    // SPERREN-LOGIK (neu geschrieben, einfach):
    // Jede Verbindung hat ein MinOrden-Feld (NordMinOrden, SuedMinOrden, OstMinOrden, WestMinOrden).
    // Wenn MinOrden > 0: Spieler braucht mindestens so viele Orden.
    // Prüfung läuft in BEIDE Richtungen: Ausgangsort UND Zielort werden geprüft.

    // Schritt 1: Wie viele Orden braucht man für diese Richtung?
    private int MinOrdenFürRichtung(Ort ort, string richtung) => richtung switch
    {
        "Nord" => ort.NordMinOrden,
        "Sued" => ort.SuedMinOrden,
        "Ost"  => ort.OstMinOrden,
        "West" => ort.WestMinOrden,
        _      => 0
    };

    // Schritt 2: Gegenrichtung (Nord↔Sued, Ost↔West)
    private static string? Gegenrichtung(string richtung) => richtung switch
    {
        "Nord" => "Sued",
        "Sued" => "Nord",
        "Ost"  => "West",
        "West" => "Ost",
        _      => null
    };

    // Schritt 3: Richtung vom aktuellen Ort zum Zielort bestimmen
    private static string? RichtungZumZiel(Ort von, string zielId)
    {
        if (von.Nord == zielId) return "Nord";
        if (von.Sued == zielId) return "Sued";
        if (von.Ost  == zielId) return "Ost";
        if (von.West == zielId) return "West";
        return null;
    }

    // Schritt 4: Hauptprüfung – darf der Spieler zum Zielort?
    // Gibt null zurück wenn erlaubt, sonst eine Fehlermeldung.
    public string? ZugangPrüfen(string zielOrtId)
    {
        var ziel   = AlleOrte.FirstOrDefault(o => o.Id == zielOrtId);
        var aktOrt = AlleOrte.FirstOrDefault(o => o.Id == Spieler.AktuellerOrt);
        if (ziel == null || aktOrt == null) return null;

        // Liga-Zugang: alle 8 Kanto-Orden nötig
        if (ziel.LigaZugang)
        {
            var kantoOrden = AlleOrte
                .Where(o => o.Arena != null && o.Id.StartsWith("KAN-") && o.Arena.OrdenNr >= 1 && o.Arena.OrdenNr <= 8)
                .Select(o => o.Arena!.OrdenName).Distinct().ToList();
            int fehlend = kantoOrden.Count(o => !Spieler.Orden.Contains(o));
            if (fehlend > 0)
                return $"⛔ Liga-Zugang gesperrt! Du brauchst alle 8 Orden – es fehlen noch {fehlend}.";
        }

        // ─── Generierter Modus: Boss-Zugang und Arenen-Reihenfolge prüfen ───
        if (IstGenerierteKartenModus && AktuelleGenerierteKarte != null)
        {
            var karte = AktuelleGenerierteKarte;

            // Boss-Zugang: braucht alle Arenen besiegt die VOR diesem Boss liegen
            if (ziel.Arena != null && karte.BossIds.Contains(zielOrtId))
            {
                int bossNr = karte.BossIds.IndexOf(zielOrtId); // 0-basiert
                // Alle vorherigen Arenen müssen besiegt sein (Reihenfolge)
                int nochNichtBesiegt = karte.BossIds.Take(bossNr).Count(id => !karte.BesiegteArenen.Contains(id));
                if (nochNichtBesiegt > 0)
                    return $"⛔ Gesperrt! Du brauchst noch {nochNichtBesiegt} Arena(en) besiegen.";
            }


        }

        // Richtung vom aktuellen Ort zum Ziel bestimmen
        string? richtung = RichtungZumZiel(aktOrt, zielOrtId);
        if (richtung == null) return null; // kein direkter Nachbar → keine Richtungssperre

        // ─── Stadt-Sperren (generierter Modus: SperrNord/Sued/Ost/West mit Hinweis) ───
        var richtungsSperre = richtung switch
        {
            "Nord" => aktOrt.SperrNord,
            "Sued" => aktOrt.SperrSued,
            "Ost"  => aktOrt.SperrOst,
            "West" => aktOrt.SperrWest,
            _      => null
        };
        if (richtungsSperre != null)
        {
            // Orden-Sperre: benötigt bestimmten Orden (Arena besiegt)
            if (!string.IsNullOrEmpty(richtungsSperre.BenötigtOrdenName))
            {
                if (!Spieler.Orden.Contains(richtungsSperre.BenötigtOrdenName))
                    return $"⛔ Gesperrt! Du brauchst den Orden: {richtungsSperre.BenötigtOrdenName}";
            }
            // Hinweis-Sperre (Fallback: alte Logik)
            else if (!string.IsNullOrEmpty(richtungsSperre.Hinweis) && richtungsSperre.BenötigtOrdenName == null)
            {
                string benötigteName = richtungsSperre.Hinweis
                    .Replace("Benötigt Orden: ", "")
                    .Replace("Benötigt: ", "").Trim();
                var benötigtOrt = AlleOrte.FirstOrDefault(o => o.Name == benötigteName);
                bool gefunden = benötigtOrt != null &&
                    (benötigtOrt.Arena != null && Spieler.Orden.Contains(benötigtOrt.Arena.OrdenName));
                if (!gefunden)
                    return $"⛔ Gesperrt! Zuerst Arena besiegen: {benötigteName}";
            }
            // Item-Sperre
            if (!string.IsNullOrEmpty(richtungsSperre.ItemId))
            {
                bool hatItem = Spieler.Inventar.Any(i => i.ItemId == richtungsSperre.ItemId);
                if (!hatItem)
                    return $"⛔ Gesperrt! Du brauchst: {richtungsSperre.ItemName ?? richtungsSperre.ItemId}";
            }
            // MinOrden-Sperre
            if (richtungsSperre.MinOrden > 0 && Spieler.Orden.Count < richtungsSperre.MinOrden)
                return $"⛔ Gesperrt! Du brauchst {richtungsSperre.MinOrden} Orden – du hast {Spieler.Orden.Count}.";
        }

        // Sperre auf dem AUSGANGSORT prüfen (z.B. Route 3 hat NordMinOrden=1)
        int minOrden1 = MinOrdenFürRichtung(aktOrt, richtung);
        if (minOrden1 > 0 && Spieler.Orden.Count < minOrden1)
            return $"⛔ Gesperrt! Du brauchst {minOrden1} Orden – du hast {Spieler.Orden.Count}.";

        // Auch Zielort-Richtungssperre prüfen (Gegenrichtung)
        string? gegen2 = Gegenrichtung(richtung);
        if (gegen2 != null)
        {
            var zielSperre = gegen2 switch
            {
                "Nord" => ziel.SperrNord,
                "Sued" => ziel.SperrSued,
                "Ost"  => ziel.SperrOst,
                "West" => ziel.SperrWest,
                _      => null
            };
            if (zielSperre != null)
            {
                if (!string.IsNullOrEmpty(zielSperre.BenötigtOrdenName))
                {
                    if (!Spieler.Orden.Contains(zielSperre.BenötigtOrdenName))
                        return $"⛔ Gesperrt! Du brauchst den Orden: {zielSperre.BenötigtOrdenName}";
                }
                else if (!string.IsNullOrEmpty(zielSperre.Hinweis))
                {
                    string benötigteName2 = zielSperre.Hinweis
                        .Replace("Benötigt Orden: ", "")
                        .Replace("Benötigt: ", "").Trim();
                    var benötigtOrt2 = AlleOrte.FirstOrDefault(o => o.Name == benötigteName2);
                    bool gefunden2 = benötigtOrt2 != null &&
                        (benötigtOrt2.Arena != null && Spieler.Orden.Contains(benötigtOrt2.Arena.OrdenName));
                    if (!gefunden2)
                        return $"⛔ Gesperrt! Zuerst Arena besiegen: {benötigteName2}";
                }
            }
        }

        // Sperre auf dem ZIELORT in Gegenrichtung prüfen (z.B. Orania hat WestMinOrden=3)
        string? gegen = Gegenrichtung(richtung);
        if (gegen != null)
        {
            int minOrden2 = MinOrdenFürRichtung(ziel, gegen);
            if (minOrden2 > 0 && Spieler.Orden.Count < minOrden2)
                return $"⛔ Gesperrt! Du brauchst {minOrden2} Orden – du hast {Spieler.Orden.Count}.";
        }

        // Muss-Kampf Trainer auf dem aktuellen Ort prüfen
        // Wenn SperrtRichtung gesetzt ist: nur diese Richtung blockieren (Rückweg bleibt frei)
        // Wenn SperrtRichtung null/leer: alle Richtungen blockieren
        // Sonderfall: Wenn der Ort eine Arena hat und der Orden bereits gewonnen wurde,
        //             gelten alle Trainer dieses Ortes als besiegt (auch ohne BesiegteTrainer-Eintrag)
        bool ordenBereitsGewonnen = aktOrt.Arena != null && Spieler.Orden.Contains(aktOrt.Arena.OrdenName);
        var mussTrainer = aktOrt.Trainer
            .Where(t => t.MussBesiegt
                     && !ordenBereitsGewonnen
                     && !Spieler.BesiegteTrainer.Contains(t.Id)
                     && !Spieler.BesiegteTrainer.Contains($"arena_{aktOrt.Id}")
                     && (string.IsNullOrEmpty(t.SperrtRichtung) || t.SperrtRichtung == richtung))
            .ToList();
        if (mussTrainer.Any())
        {
            var namen = string.Join(", ", mussTrainer.Select(t => t.Name));
            return $"⚔️ Zuerst besiegen: {namen}";
        }

        return null; // alles ok!
    }

    // Alte Methoden als Wrapper (damit MapEditor/Weltkarte.razor nicht kaputt gehen)
    public string? OrtZugangsPrüfung(Ort ort) => ZugangPrüfen(ort.Id);
    public string? RichtungsZugangsPrüfung(Ort vonOrt, string richtung)
    {
        int min = MinOrdenFürRichtung(vonOrt, richtung);
        if (min > 0 && Spieler.Orden.Count < min)
            return $"⛔ Gesperrt! Du brauchst {min} Orden – du hast {Spieler.Orden.Count}.";
        return null;
    }

    // Ort betreten: Sperre prüfen, dann reisen
    public string? OrtBetreten(string ortId, string? vonRichtung = null)
    {
        var fehler = ZugangPrüfen(ortId);
        if (fehler != null) return fehler;
        Spieler.AktuellerOrt = ortId;
        Notify();
        return null;
    }

    // ── Kampf starten ────────────────────────────────────────────────────────
    public void WildkampfStarten(WildBegegnung begegnung)
    {
        var spezies = AlleMonster.FirstOrDefault(m => m.Id == begegnung.MonsterId);
        if (spezies == null) return;
        // Level kommt direkt aus begegnung (im generierten Modus bereits vom Generator gesetzt)
        int level = _rng.Next(begegnung.MinLevel, begegnung.MaxLevel + 1);
        var gegner = MonsterInstanz.VonSpezies(spezies, level, AlleAttacken);
        var spielerMonster = Spieler.AktivesMonster;
        if (spielerMonster == null) return;

        // NuzlockeLeicht / NuzlockeEinsFangen: nur 1 Monster pro Route fangen
        // Wenn auf dieser Route schon ein Monster gefangen wurde → Fangen sperren
        bool kannFangen = true;
        if (Einstellungen.HatRelikt(ReliktTyp.NuzlockeLeicht) || Einstellungen.HatRelikt(ReliktTyp.NuzlockeEinsFangen))
        {
            string ortId = Spieler.AktuellerOrt ?? "";
            if (Spieler.GefangeneMonsterProOrt.Contains(ortId))
                kannFangen = false;
        }

        // Monster als gesehen markieren
        Spieler.GeseheneMonster.Add(spezies.Id);
        // Attacken des Gegners als gesehen speichern
        if (!Spieler.GeseheneAttacken.ContainsKey(spezies.Id))
            Spieler.GeseheneAttacken[spezies.Id] = new List<string>();
        foreach (var atk in gegner.Attacken)
        {
            if (!Spieler.GeseheneAttacken[spezies.Id].Contains(atk.Id))
                Spieler.GeseheneAttacken[spezies.Id].Add(atk.Id);
        }

        AktuellerKampf = new KampfZustand
        {
            Typ = KampfTyp.Wild,
            SpielerMonster = spielerMonster,
            GegnerMonster = gegner,
            GegnerName = $"Wildes {gegner.Name}",
            Phase = KampfPhase.Intro,
            Log = new() { $"Ein wildes {gegner.Name} (Lv.{level}) erscheint!" },
            NuzlockeFangenGesperrt = !kannFangen,
            ZeitlimitAktiv = Einstellungen.HatRelikt(ReliktTyp.ZeitlimitProKampf)
                          || Einstellungen.HatRelikt(ReliktTyp.ZeitlimitProKampf2Min)
                          || Einstellungen.HatRelikt(ReliktTyp.ZeitlimitProKampf1Min),
            ZeitlimitSekunden = Einstellungen.HatRelikt(ReliktTyp.ZeitlimitProKampf1Min) ? 60
                              : Einstellungen.HatRelikt(ReliktTyp.ZeitlimitProKampf2Min) ? 120
                              : 180,
        };
        Phase = SpielPhase.Kampf;
        Notify();
    }

    // Rivale Blau: Starter-Konter bestimmen (Pflanze→Feuer, Feuer→Wasser, Wasser→Pflanze)
    private string? RivaleStarterKonter(string starterId)
    {
        // Starter-Konter-Mapping für alle Regionen
        // Feuer → Wasser, Wasser → Pflanze, Pflanze → Feuer
        return starterId switch
        {
            // Kanto
            "PKM-0001" => "PKM-0004", // Bisasam (Pflanze) → Glumanda (Feuer)
            "PKM-0004" => "PKM-0007", // Glumanda (Feuer) → Schiggy (Wasser)
            "PKM-0007" => "PKM-0001", // Schiggy (Wasser) → Bisasam (Pflanze)
            // Johto
            "PKM-0152" => "PKM-0155", // Endivie (Pflanze) → Feurigel (Feuer)
            "PKM-0155" => "PKM-0158", // Feurigel (Feuer) → Karnimani (Wasser)
            "PKM-0158" => "PKM-0152", // Karnimani (Wasser) → Endivie (Pflanze)
            // Hoenn
            "PKM-0252" => "PKM-0255", // Geckarbor (Pflanze) → Flemmli (Feuer)
            "PKM-0255" => "PKM-0258", // Flemmli (Feuer) → Hydropi (Wasser)
            "PKM-0258" => "PKM-0252", // Hydropi (Wasser) → Geckarbor (Pflanze)
            // Sinnoh
            "PKM-0387" => "PKM-0390", // Chelast (Pflanze) → Panflam (Feuer)
            "PKM-0390" => "PKM-0393", // Panflam (Feuer) → Plinfa (Wasser)
            "PKM-0393" => "PKM-0387", // Plinfa (Wasser) → Chelast (Pflanze)
            // Einall
            "PKM-0495" => "PKM-0498", // Serpifeu (Pflanze) → Floink (Feuer)
            "PKM-0498" => "PKM-0501", // Floink (Feuer) → Ottaro (Wasser)
            "PKM-0501" => "PKM-0495", // Ottaro (Wasser) → Serpifeu (Pflanze)
            // Kalos
            "PKM-0650" => "PKM-0653", // Igamaro (Pflanze) → Fynx (Feuer)
            "PKM-0653" => "PKM-0656", // Fynx (Feuer) → Froxy (Wasser)
            "PKM-0656" => "PKM-0650", // Froxy (Wasser) → Igamaro (Pflanze)
            // Alola
            "PKM-0722" => "PKM-0725", // Bauz (Pflanze) → Flamiau (Feuer)
            "PKM-0725" => "PKM-0728", // Flamiau (Feuer) → Robball (Wasser)
            "PKM-0728" => "PKM-0722", // Robball (Wasser) → Bauz (Pflanze)
            // Galar
            "PKM-0810" => "PKM-0813", // Chimpep (Pflanze) → Hopplo (Feuer)
            "PKM-0813" => "PKM-0816", // Hopplo (Feuer) → Memmeon (Wasser)
            "PKM-0816" => "PKM-0810", // Memmeon (Wasser) → Chimpep (Pflanze)
            // Paldea
            "PKM-0906" => "PKM-0909", // Felori (Pflanze) → Krokel (Feuer)
            "PKM-0909" => "PKM-0912", // Krokel (Feuer) → Kwaks (Wasser)
            "PKM-0912" => "PKM-0906", // Kwaks (Wasser) → Felori (Pflanze)
            _ => null // Kein Konter bekannt
        };
    }

    public void TrainerKampfStarten(TrainerKampf trainer)
    {
        if (Spieler.BesiegteTrainer.Contains(trainer.Id)) return;
        var spielerMonster = Spieler.AktivesMonster;
        if (spielerMonster == null) return;

        var erstesGegnerMonster = trainer.Team.FirstOrDefault();
        if (erstesGegnerMonster == null) return;

        // Rivale: Starter-Konter-Logik für alle Regionen
        TrainerKampf effektiverTrainer = trainer;
        if (trainer.Klasse == "Rivale" && !string.IsNullOrEmpty(Spieler.StarterMonsterId))
        {
            // Wenn Rival nur 1 Monster hat und es ein bekannter Starter ist, Konter-Logik anwenden
            var konterId = RivaleStarterKonter(Spieler.StarterMonsterId);
            if (konterId != null && trainer.Team.Count == 1)
            {
                effektiverTrainer = new TrainerKampf
                {
                    Id = trainer.Id,
                    Name = trainer.Name,
                    Klasse = trainer.Klasse,
                    Belohnung = trainer.Belohnung,
                    Dialogvor = trainer.Dialogvor,
                    DialogNach = trainer.DialogNach,
                    MussBesiegt = trainer.MussBesiegt,
                    SperrtRichtung = trainer.SperrtRichtung,
                    Team = new List<MonsterTeamEintrag>
                    {
                        new MonsterTeamEintrag { MonsterId = konterId, Level = erstesGegnerMonster.Level }
                    }
                };
            }
        }

        // Ort des Trainers ermitteln (Trainer stehen auf Routen, nicht zwingend am Spieler-Ort)
        var trainerOrt = AlleOrte.FirstOrDefault(o =>
            o.Trainer.Any(tr => tr.Id == effektiverTrainer.Id))
            ?? AktuellerOrt;
        var trainerOrtId = trainerOrt?.Id;

        // Trainer-Monster-Level nach Ebene korrigieren (zuverlässige Skalierung wie bei Wilden)
        foreach (var m in effektiverTrainer.Team)
            m.Level = TrainerLevelKorrigieren(trainerOrtId, m.Level, istArena: false);

        var ersteMonsterId = effektiverTrainer.Team[0].MonsterId;
        var spezies = TrainerMonsterSpeziesWählen(ersteMonsterId, effektiverTrainer.Team[0]);
        var gegner = MonsterInstanz.VonSpezies(spezies, effektiverTrainer.Team[0].Level, AlleAttacken);

        // Monster als gesehen markieren
        MonsterAlsGesehenMarkieren(spezies.Id, gegner);

        AktuellerKampf = new KampfZustand
        {
            Typ = KampfTyp.Trainer,
            SpielerMonster = spielerMonster,
            GegnerMonster = gegner,
            GegnerName = effektiverTrainer.Name,
            Phase = KampfPhase.Intro,
            Log = new() { $"💬 {effektiverTrainer.Name}: \"{effektiverTrainer.Dialogvor}\"" },
            TrainerId = effektiverTrainer.Id,
            BelohnungGeld = effektiverTrainer.Belohnung,
            AktuellerTrainer = effektiverTrainer,
            TrainerMonsterIndex = 0,
            ZeitlimitAktiv = Einstellungen.HatRelikt(ReliktTyp.ZeitlimitProKampf)
                          || Einstellungen.HatRelikt(ReliktTyp.ZeitlimitProKampf2Min)
                          || Einstellungen.HatRelikt(ReliktTyp.ZeitlimitProKampf1Min),
            ZeitlimitSekunden = Einstellungen.HatRelikt(ReliktTyp.ZeitlimitProKampf1Min) ? 60
                              : Einstellungen.HatRelikt(ReliktTyp.ZeitlimitProKampf2Min) ? 120
                              : 180,
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

        // Arena-Monster-Level nach Ebene korrigieren (Arena-Bonus, zuverlässige Skalierung)
        foreach (var m in ort.Arena.Team)
            m.Level = TrainerLevelKorrigieren(ort.Id, m.Level, istArena: true);

        var spezies = TrainerMonsterSpeziesWählen(erstesGegnerMonster.MonsterId, erstesGegnerMonster);
        var gegner = MonsterInstanz.VonSpezies(spezies, erstesGegnerMonster.Level, AlleAttacken);

        // Arena als Trainer-Kampf mit Orden-Belohnung
        var arenaTrainer = new TrainerKampf
        {
            Id = $"arena_{ort.Id}",
            Name = ort.Arena.Leiter,
            Klasse = "Arena-Leiter",
            Belohnung = 500 + ort.Arena.OrdenNr * 200,
            Team = ort.Arena.Team,
            Dialogvor = $"Ich bin {ort.Arena.Leiter}, Meister des {ort.Arena.TypSpezialisierung}-Typs!",
            DialogNach = ort.Arena.OrdenName + " erhalten!",
        };

        AktuellerKampf = new KampfZustand
        {
            Typ = KampfTyp.Arena,
            SpielerMonster = spielerMonster,
            GegnerMonster = gegner,
            GegnerName = ort.Arena.Leiter,
            Phase = KampfPhase.Intro,
            Log = new() { $"🏆 {ort.Arena.Leiter} fordert dich heraus!" },
            OrtId = ort.Id,
            BelohnungGeld = arenaTrainer.Belohnung,
            AktuellerTrainer = arenaTrainer,
            TrainerMonsterIndex = 0,
            ZeitlimitAktiv = Einstellungen.HatRelikt(ReliktTyp.ZeitlimitProKampf)
                          || Einstellungen.HatRelikt(ReliktTyp.ZeitlimitProKampf2Min)
                          || Einstellungen.HatRelikt(ReliktTyp.ZeitlimitProKampf1Min),
            ZeitlimitSekunden = Einstellungen.HatRelikt(ReliktTyp.ZeitlimitProKampf1Min) ? 60
                              : Einstellungen.HatRelikt(ReliktTyp.ZeitlimitProKampf2Min) ? 120
                              : 180,
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

        // Initiative-Vergleich: wer schneller ist greift zuerst an
        // ImmerErstangriff-Relikt überschreibt dies (Spieler immer zuerst)
        bool spielerZuerst = true;
        if (!Einstellungen.HatRelikt(ReliktTyp.ImmerErstangriff))
        {
            int spielerInit = MonsterInstanz.MitStatStufe(spielerMon.Initiative, spielerMon.StatStufeInitiative);
            int gegnerInit  = MonsterInstanz.MitStatStufe(gegnerMon.Initiative,  gegnerMon.StatStufeInitiative);
            if (gegnerInit > spielerInit)
            {
                spielerZuerst = false;
                AktuellerKampf.Log.Add($"⚡ {gegnerMon.AngezeigterName} ist schneller (Initiative {gegnerInit} vs {spielerInit})!");
            }
            else if (gegnerInit == spielerInit)
            {
                spielerZuerst = _rng.Next(2) == 0;
                if (!spielerZuerst)
                    AktuellerKampf.Log.Add($"⚡ {gegnerMon.AngezeigterName} greift zuerst an (gleiche Initiative, Zufall)!");
            }
        }

        // Wenn Gegner schneller: erst Gegner angreifen, dann Spieler
        if (!spielerZuerst)
        {
            // Gegner greift zuerst an
            Notify();
            await Task.Delay(400);
            await GegnerZugOhneRunde(); // Gegner greift an ohne Runden-Ende
            if (AktuellerKampf == null) return; // Kampf vorzeitig beendet
            if (spielerMon.IstOhnmächtig)
            {
                // Spieler-Monster ohnmächtig nach Gegner-Erstangriff
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
        }

        // ZufälligeAttacke-Relikt: zufällige Attacke mit AP > 0 wählen
        if (Einstellungen.HatRelikt(ReliktTyp.ZufälligeAttacke))
        {
            var verfügbar = spielerMon.Attacken.Where(a => a.AktuelleAp > 0).ToList();
            if (verfügbar.Any())
            {
                attacke = verfügbar[_rng.Next(verfügbar.Count)];
                AktuellerKampf.Log.Add($"🎲 Relikt: {spielerMon.AngezeigterName} setzt zufällig {attacke.Name} ein!");
            }
        }

        // Status-Effekt vor Angriff prüfen (Lähmung, Schlaf, Eingefroren)
        if (!StatusErlaubtAngriff(spielerMon, AktuellerKampf.Log))
        {
            Notify();
            await Task.Delay(800);
            // Wenn Gegner schon zuerst angegriffen hat, nur Runden-Ende
            if (!spielerZuerst)
                await RundenEnde();
            else
                await GegnerZug();
            return;
        }

        // NurEigenTyp-Relikt: Attacke muss zum eigenen Typ passen
        if (Einstellungen.HatRelikt(ReliktTyp.NurEigenTyp) && !spielerMon.Typen.Contains(attacke.Typ))
        {
            AktuellerKampf.Log.Add($"❌ Relikt: {spielerMon.AngezeigterName} kann nur Attacken des eigenen Typs einsetzen!");
            AktuellerKampf.Phase = KampfPhase.SpielerZug;
            Notify();
            return;
        }

        if (attacke.AktuelleAp > 0)
        {
            attacke.AktuelleAp--;
            int schaden = SchadenBerechnen(spielerMon, gegnerMon, attacke);
            float multi = TypeChart.GetVerteidigungsMultiplikator(attacke.Typ, gegnerMon.Typen);
            gegnerMon.AktuelleKp = Math.Max(0, gegnerMon.AktuelleKp - schaden);

            // Sondereffekte (Stat-Boosts, Flucht, etc.) für Status-Attacken
            bool hatSondereffekt = AttackeSondereffektAusführen(attacke, spielerMon, gegnerMon, AktuellerKampf.Log, AktuellerKampf.IstTrainerKampf, attacke.Name);

            if (schaden > 0)
            {
                string trefferText = multi >= 2f ? " — sehr effektiv!" : multi <= 0f ? " — keine Wirkung" : multi < 1f ? " — nicht sehr effektiv" : "";
                AktuellerKampf.Log.Add($"⚔️ {spielerMon.AngezeigterName} setzt {attacke.Name} ein! {gegnerMon.AngezeigterName} erleidet {schaden} Schaden{trefferText}.");
            }
            else if (!hatSondereffekt)
            {
                AktuellerKampf.Log.Add($"⚔️ {spielerMon.AngezeigterName} setzt {attacke.Name} ein!");
            }

            // Status-Effekt durch Attacke (aus Attacken-Daten)
            if (schaden > 0 && !string.IsNullOrEmpty(attacke.Statuseffekt))
            {
                int chance = attacke.StatuseffektChance ?? 30;
                if (_rng.Next(100) < chance)
                    VersuchemStatusEffektDirekt(gegnerMon, attacke.Statuseffekt, AktuellerKampf.Log);
            }
            else if (schaden == 0 && !string.IsNullOrEmpty(attacke.Statuseffekt))
            {
                // Status-Attacken (kein Schaden) treffen direkt
                int chance = attacke.StatuseffektChance ?? 100;
                if (_rng.Next(100) < chance)
                    VersuchemStatusEffektDirekt(gegnerMon, attacke.Statuseffekt, AktuellerKampf.Log);
            }

            // Teleport: Spieler flieht (Wildkampf) oder teleportiert zum letzten Center (Trainerkampf)
            if (spielerMon.IstTeleportFlucht)
            {
                spielerMon.IstTeleportFlucht = false;
                // KampfTyp VOR KampfBeenden() speichern, da AktuellerKampf danach null ist
                var kampfTyp = AktuellerKampf.Typ;
                Notify();
                await Task.Delay(800);
                if (kampfTyp == KampfTyp.Wild)
                {
                    KampfBeenden();
                }
                else
                {
                    // Im Trainerkampf: Kampf beenden + zum letzten Monster-Center teleportieren
                    KampfBeenden();
                    // Letztes Monster-Center finden und teleportieren
                    var letztesCenterOrt = AlleOrte.FirstOrDefault(o => o.HatMonsterCenter && o.Id == LetztesCenterOrtId)
                        ?? AlleOrte.FirstOrDefault(o => o.HatMonsterCenter);
                    if (letztesCenterOrt != null)
                    {
                        Spieler.AktuellerOrt = letztesCenterOrt.Id;
                        Notify();
                    }
                }
                return;
            }
            // Roar/Brüller: Gegner flieht (nur wilder Kampf)
            if (gegnerMon.IstRoarFlucht)
            {
                gegnerMon.IstRoarFlucht = false;
                Notify();
                await Task.Delay(800);
                KampfBeenden();
                return;
            }
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

        // Wenn Gegner bereits zuerst angegriffen hat (Initiative), darf er NICHT nochmal angreifen.
        // Nur Runden-Ende (Status-Schaden) durchführen.
        if (!spielerZuerst)
        {
            await RundenEnde();
            return;
        }

        await GegnerZug();
    }

    /// <summary>Runden-Ende: Status-Schaden, Ohnmacht-Prüfung, nächste Runde.</summary>
    private async Task RundenEnde()
    {
        if (AktuellerKampf == null) return;
        var spielerMon = AktuellerKampf.SpielerMonster;
        var gegnerMon = AktuellerKampf.GegnerMonster;

        // Status-Schaden am Ende der Runde (Vergiftung, Verbrennung)
        StatusSchadenRunde(spielerMon, AktuellerKampf.Log);
        StatusSchadenRunde(gegnerMon, AktuellerKampf.Log);

        // Halteitem-Effekte: LeechHeal (Seegesang)
        HalteItemRundenEffekt(spielerMon, AktuellerKampf.Log);
        HalteItemRundenEffekt(gegnerMon, AktuellerKampf.Log);

        Notify();
        await Task.Delay(800);

        if (gegnerMon.IstOhnmächtig)
        {
            await KampfGewonnen();
            return;
        }

        if (spielerMon.IstOhnmächtig)
        {
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
                bool gegnerHatSonder = AttackeSondereffektAusführen(gegnerAttacke, gegnerMon, spielerMon, AktuellerKampf.Log, AktuellerKampf.IstTrainerKampf, gegnerAttacke.Name);
                if (gegnerSchaden > 0)
                {
                    string gTrefferText = gegnerMulti >= 2f ? " — sehr effektiv!" : gegnerMulti <= 0f ? " — keine Wirkung" : gegnerMulti < 1f ? " — nicht sehr effektiv" : "";
                    AktuellerKampf.Log.Add($"💢 {gegnerMon.AngezeigterName} setzt {gegnerAttacke.Name} ein! {spielerMon.AngezeigterName} erleidet {gegnerSchaden} Schaden{gTrefferText}.");
                }
                else if (!gegnerHatSonder)
                {
                    AktuellerKampf.Log.Add($"💢 {gegnerMon.AngezeigterName} setzt {gegnerAttacke.Name} ein!");
                }
                // Status-Effekt durch Gegner-Attacke
                if (gegnerSchaden > 0 && !string.IsNullOrEmpty(gegnerAttacke.Statuseffekt))
                {
                    int chance = gegnerAttacke.StatuseffektChance ?? 30;
                    if (_rng.Next(100) < chance)
                        VersuchemStatusEffektDirekt(spielerMon, gegnerAttacke.Statuseffekt, AktuellerKampf.Log);
                }
                else if (gegnerSchaden == 0 && !string.IsNullOrEmpty(gegnerAttacke.Statuseffekt))
                {
                    int chance = gegnerAttacke.StatuseffektChance ?? 100;
                    if (_rng.Next(100) < chance)
                        VersuchemStatusEffektDirekt(spielerMon, gegnerAttacke.Statuseffekt, AktuellerKampf.Log);
                }
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


    // ── Gegner greift zuerst an (Initiative-Vorteil) ─────────────────────────
    // Nur der Angriff des Gegners, KEIN Runden-Ende (Status-Schaden etc.)
    private async Task GegnerZugOhneRunde()
    {
        if (AktuellerKampf == null) return;
        var spielerMon = AktuellerKampf.SpielerMonster;
        var gegnerMon  = AktuellerKampf.GegnerMonster;

        // Status-Effekt des Gegners prüfen (Schläft, gelähmt etc.)
        if (!StatusErlaubtAngriff(gegnerMon, AktuellerKampf.Log))
        {
            Notify();
            await Task.Delay(400);
            return;
        }

        var gegnerAttacke = GegnerAttackeWählen(gegnerMon);
        if (gegnerAttacke != null)
        {
            int gegnerSchaden = SchadenBerechnen(gegnerMon, spielerMon, gegnerAttacke);
            float gegnerMulti = TypeChart.GetVerteidigungsMultiplikator(gegnerAttacke.Typ, spielerMon.Typen);
            spielerMon.AktuelleKp = Math.Max(0, spielerMon.AktuelleKp - gegnerSchaden);
            bool gegnerHatSonder = AttackeSondereffektAusführen(gegnerAttacke, gegnerMon, spielerMon, AktuellerKampf.Log, AktuellerKampf.IstTrainerKampf, gegnerAttacke.Name);
            if (gegnerSchaden > 0)
            {
                string gTrefferText = gegnerMulti >= 2f ? " — sehr effektiv!" : gegnerMulti <= 0f ? " — keine Wirkung" : gegnerMulti < 1f ? " — nicht sehr effektiv" : "";
                AktuellerKampf.Log.Add($"💢 {gegnerMon.AngezeigterName} setzt {gegnerAttacke.Name} ein! {spielerMon.AngezeigterName} erleidet {gegnerSchaden} Schaden{gTrefferText}.");
            }
            else if (!gegnerHatSonder)
            {
                AktuellerKampf.Log.Add($"💢 {gegnerMon.AngezeigterName} setzt {gegnerAttacke.Name} ein!");
            }
            if (gegnerSchaden > 0 && gegnerAttacke.Statuseffekt != null)
            {
                int chance = gegnerAttacke.StatuseffektChance ?? 30;
                if (_rng.Next(100) < chance)
                    VersuchemStatusEffektDirekt(spielerMon, gegnerAttacke.Statuseffekt, AktuellerKampf.Log);
            }
            else if (gegnerSchaden == 0 && gegnerAttacke.Statuseffekt != null)
            {
                int chance = gegnerAttacke.StatuseffektChance ?? 100;
                if (_rng.Next(100) < chance)
                    VersuchemStatusEffektDirekt(spielerMon, gegnerAttacke.Statuseffekt, AktuellerKampf.Log);
            }
        }
        Notify();
        await Task.Delay(600);
    }

    // ── Monster wechseln ─────────────────────────────────────────────────────
    public async Task MonsterWechseln(MonsterInstanz neuesMonster)
    {
        if (AktuellerKampf == null) return;
        if (neuesMonster.IstOhnmächtig) return;

        // KeinMonsterWechsel-Relikt: kein freiwilliger Wechsel erlaubt
        // Erzwungener Wechsel nach Ohnmacht (Phase == MonsterWechsel) ist immer erlaubt
        if (Einstellungen.HatRelikt(ReliktTyp.KeinMonsterWechsel) && AktuellerKampf.Phase != KampfPhase.MonsterWechsel)
        {
            AktuellerKampf.Log.Add("❌ Relikt: Kein freiwilliger Monster-Wechsel erlaubt!");
            Notify();
            return;
        }

        var altesMonster = AktuellerKampf.SpielerMonster;
        AktuellerKampf.SpielerMonster = neuesMonster;
        Spieler.AktivesMonsterIndex = Spieler.Team.IndexOf(neuesMonster);
        AktuellerKampf.Log.Add($"🔄 {altesMonster.AngezeigterName} zurück! {neuesMonster.AngezeigterName} kämpft weiter!");

        // XP-Tracking: altes Monster als "eingewechselt" markieren (für XP-Teiler-Logik)
        if (!AktuellerKampf.EingewechselteMonster.Contains(altesMonster))
            AktuellerKampf.EingewechselteMonster.Add(altesMonster);

        bool warMonsterWechselPhase = AktuellerKampf.Phase == KampfPhase.MonsterWechsel;

        if (warMonsterWechselPhase)
        {
            // Erzwungener Wechsel nach Ohnmacht: KEIN Gegner-Angriff!
            // Das neue Monster bekommt sofort seinen Zug (freier Wechsel).
            AktuellerKampf.Log.Add($"⚡ {neuesMonster.AngezeigterName} ist bereit!");
            await Task.Delay(400);
            AktuellerKampf.Phase = KampfPhase.SpielerZug;
            Notify();
        }
        else
        {
            // Freiwilliger Wechsel im eigenen Zug: Gegner greift danach an
            AktuellerKampf.Phase = KampfPhase.GegnerZug;
            Notify();
            await Task.Delay(600);
            await GegnerZug();
        }
    }

    // ── Item im Kampf verwenden ───────────────────────────────────────────────
    public async Task ItemImKampfVerwenden(string itemId, MonsterInstanz ziel)
    {
        if (AktuellerKampf == null) return;
        if (AktuellerKampf.Phase != KampfPhase.SpielerZug) return;

        // KeineItemsImKampf-Relikt
        if (Einstellungen.HatRelikt(ReliktTyp.KeineItemsImKampf))
        {
            AktuellerKampf.Log.Add("❌ Relikt: Items können im Kampf nicht benutzt werden!");
            Notify();
            return;
        }

        var item = Spieler.GetItem(itemId);
        if (item == null || item.Menge <= 0)
        {
            AktuellerKampf.Log.Add("❌ Kein Item vorhanden!");
            Notify();
            return;
        }

        // Fluchtseil separat behandeln
        var itemDef = GetItemDef(itemId);
        if (itemDef?.Name == "Fluchtseil" || itemDef?.Effekt.Typ == "Flucht")
        {
            if (AktuellerKampf.Typ == KampfTyp.Wild)
            {
                AktuellerKampf.Log.Add("🧵 Fluchtseil verwendet! Du bist geflohen!");
                Spieler.ItemVerwenden(itemId);
                AktuellerKampf.SpielerGewonnen = false;
                AktuellerKampf.Phase = KampfPhase.Beendet;
                Notify();
                return;
            }
            AktuellerKampf.Log.Add("❌ Fluchtseil funktioniert nicht in Trainer-Kämpfen!");
            Notify();
            return;
        }
        // Bälle gehören nicht hierher (eigene MonsterFangen-Methode)
        if (itemDef?.Effekt.Typ == "Fangen")
        {
            AktuellerKampf.Log.Add("❌ Dieses Item kann jetzt nicht verwendet werden!");
            Notify();
            return;
        }
        // Alle anderen Items über ItemAnwenden-Logik verarbeiten
        bool verwendet = false;
        string ergebnis = "";
        if (itemDef != null)
        {
            var effekt = itemDef.Effekt;
            switch (effekt.Typ)
            {
                case "HeilKP":
                    if (!ziel.IstOhnmächtig && ziel.AktuelleKp < ziel.MaxKp)
                    {
                        int heilung = Math.Min(effekt.Wert, ziel.MaxKp - ziel.AktuelleKp);
                        ziel.AktuelleKp += heilung;
                        AktuellerKampf.Log.Add($"{itemDef.Emoji} {itemDef.Name} verwendet! {ziel.AngezeigterName} erhält {heilung} KP.");
                        verwendet = true;
                    }
                    break;
                case "HeilKPVoll":
                    if (!ziel.IstOhnmächtig && ziel.AktuelleKp < ziel.MaxKp)
                    {
                        int heilung = ziel.MaxKp - ziel.AktuelleKp;
                        ziel.AktuelleKp = ziel.MaxKp;
                        AktuellerKampf.Log.Add($"{itemDef.Emoji} {itemDef.Name} verwendet! {ziel.AngezeigterName} vollständig geheilt (+{heilung} KP).");
                        verwendet = true;
                    }
                    break;
                case "Beleben":
                    if (ziel.IstOhnmächtig)
                    {
                        ziel.AktuelleKp = Math.Max(1, ziel.MaxKp * effekt.Wert / 100);
                        AktuellerKampf.Log.Add($"{itemDef.Emoji} {ziel.AngezeigterName} wurde belebt!");
                        verwendet = true;
                    }
                    break;
                case "BelebenVoll":
                    if (ziel.IstOhnmächtig)
                    {
                        ziel.AktuelleKp = ziel.MaxKp;
                        AktuellerKampf.Log.Add($"{itemDef.Emoji} {ziel.AngezeigterName} wurde vollständig belebt!");
                        verwendet = true;
                    }
                    break;
                case "HeilAP":
                    foreach (var atk in ziel.Attacken) atk.AktuelleAp = Math.Min(atk.MaxAp, atk.AktuelleAp + effekt.Wert);
                    AktuellerKampf.Log.Add($"{itemDef.Emoji} AP von {ziel.AngezeigterName} aufgefüllt!");
                    verwendet = true;
                    break;
                case "HeilAPVoll":
                    foreach (var atk in ziel.Attacken) atk.AktuelleAp = atk.MaxAp;
                    AktuellerKampf.Log.Add($"{itemDef.Emoji} Alle AP von {ziel.AngezeigterName} vollständig aufgefüllt!");
                    verwendet = true;
                    break;
                case "HeilStatus":
                    if (ziel.Status != null && ziel.Status != "none")
                    {
                        string altStatus = ziel.Status;
                        ziel.Status = "none";
                        AktuellerKampf.Log.Add($"{itemDef.Emoji} {ziel.AngezeigterName} ist nicht mehr {altStatus}!");
                        verwendet = true;
                    }
                    break;
                case "HeilAlles":
                    ziel.Status = "none";
                    ziel.AktuelleKp = ziel.MaxKp;
                    foreach (var atk in ziel.Attacken) atk.AktuelleAp = atk.MaxAp;
                    AktuellerKampf.Log.Add($"{itemDef.Emoji} {ziel.AngezeigterName} vollständig geheilt!");
                    verwendet = true;
                    break;
                case "KampfStatBoost":
                    if (!ziel.IstOhnmächtig)
                    {
                        int stufen = effekt.Stufen > 0 ? effekt.Stufen : 1;
                        switch (effekt.Stat)
                        {
                            case "Angriff":            ziel.StatStufeAngriff            = Math.Min(6, ziel.StatStufeAngriff + stufen); break;
                            case "Verteidigung":       ziel.StatStufeVerteidigung       = Math.Min(6, ziel.StatStufeVerteidigung + stufen); break;
                            case "SpezialAngriff":     ziel.StatStufeSpAngriff          = Math.Min(6, ziel.StatStufeSpAngriff + stufen); break;
                            case "SpezialVerteidigung":ziel.StatStufeSpVerteidigung     = Math.Min(6, ziel.StatStufeSpVerteidigung + stufen); break;
                            case "Initiative":         ziel.StatStufeInitiative         = Math.Min(6, ziel.StatStufeInitiative + stufen); break;
                            case "Genauigkeit":        ziel.StatStufeGenauigkeit        = Math.Min(6, ziel.StatStufeGenauigkeit + stufen); break;
                        }
                        AktuellerKampf.Log.Add($"{itemDef.Emoji} {itemDef.Name} verwendet! {ziel.AngezeigterName}'s {effekt.Stat} steigt!");
                        verwendet = true;
                    }
                    break;
                default:
                    AktuellerKampf.Log.Add($"❌ {itemDef.Name} kann hier nicht verwendet werden!");
                    break;
            }
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

    // ── Monster fangen (eigenes Widerstandssystem) ──────────────────────────
    // Widerstand: 1–10 (aus Fangrate: Fangrate 255=1, Fangrate 3=10)
    // Ball-Kraft:  Monsterball=1, Superball=2, Hyperball=4, Meisterball=99
    // HP-Bonus:    <50%=+1, <25%=+2, <10%=+3
    // Status-Bonus: Schlaf/Einfrieren=+3, Gift/Lähmung/Verbrennung=+1
    // Fang: Ball-Kraft + Boni > Schutzwurf (0..Widerstand-1) → gefangen!
    private int FangWiderstand(int fangrate)
    {
        // Fangrate 255 → Widerstand 1 (sehr leicht zu fangen)
        // Fangrate 45  → Widerstand 5 (normal)
        // Fangrate 3   → Widerstand 10 (sehr selten)
        int w = (int)Math.Round(10.0 - (fangrate / 255.0) * 9.0);
        return Math.Clamp(w, 1, 10);
    }
    public async Task MonsterFangen(string ballId)
    {
        if (AktuellerKampf == null) return;
        if (AktuellerKampf.Phase != KampfPhase.SpielerZug) return;
        if (!AktuellerKampf.KannFangen) { AktuellerKampf.Log.Add("❌ Vor einem Trainer-Kampf kann man nicht fangen!"); Notify(); return; }
        // KeinFangen-Relikt
        if (Einstellungen.HatRelikt(ReliktTyp.KeinFangen))
        {
            AktuellerKampf.Log.Add("❌ Relikt: Fangen ist verboten!");
            Notify(); return;
        }
        // NurPokeball-Relikt: nur einfacher Ball erlaubt
        if (Einstellungen.HatRelikt(ReliktTyp.NurPokeball) && ballId != "monsterball" && ballId != "ITEM-011")
        {
            AktuellerKampf.Log.Add("❌ Relikt: Nur einfache Monsterbälle erlaubt!");
            Notify(); return;
        }
        var item = Spieler.GetItem(ballId);
        if (item == null || item.Menge <= 0) { AktuellerKampf.Log.Add("❌ Kein Ball mehr!"); Notify(); return; }
        Spieler.ItemVerwenden(ballId);
        AktuellerKampf.Phase = KampfPhase.Fangen;
        var gegner = AktuellerKampf.GegnerMonster;
        string ballEmoji = ballId switch { "ITEM-012" => "🔵", "ITEM-013" => "🔴", "ITEM-014" => "🟣", _ => "⚪" };
        string ballName  = ballId switch { "ITEM-012" => "Monsterball UR", "ITEM-013" => "Monsterball S", "ITEM-014" => "Monsterball SR", _ => "Monsterball R" };
        AktuellerKampf.Log.Add($"{ballEmoji} Du wirfst einen {ballName} nach {gegner.Name}...");
        Notify();
        await Task.Delay(1200);
        // ─ Widerstand berechnen ─
        int widerstand = FangWiderstand(gegner.Fangrate);
        // ─ Ball-Kraft ─
        int ballKraft = ballId switch { "ITEM-012" => 2, "ITEM-013" => 4, "ITEM-014" => 99, _ => 1 };
        // BessereBallle-Upgrade: Monsterball wirkt wie Superball (+1)
        if (Spieler.ZähneWallet.HatUpgrade(ZähneUpgrade.BessereBallle) && ballKraft == 1) ballKraft = 2;
        // ProfiCatcher-Upgrade: Superball wirkt wie Hyperball (+2 statt +2)
        if (Spieler.ZähneWallet.HatUpgrade(ZähneUpgrade.ProfiCatcher) && ballKraft == 2) ballKraft = 4;
        // BessereKugeln-Upgrade: alle Bälle +2 Fangkraft
        if (Spieler.ZähneWallet.HatUpgrade(ZähneUpgrade.BessereKugeln)) ballKraft += 2;
        // ─ HP-Schwäche-Bonus ─
        float hpProzent = gegner.MaxKp > 0 ? (float)gegner.AktuelleKp / gegner.MaxKp : 1f;
        int hpBonus = hpProzent < 0.10f ? 3 : hpProzent < 0.25f ? 2 : hpProzent < 0.50f ? 1 : 0;
        // ─ Status-Bonus ─
        int statusBonus = gegner.Status is "eingeschlafen" or "eingefroren" ? 3
                        : gegner.Status is "vergiftet" or "gelähmt" or "verbrannt" ? 1 : 0;
        // LegendärBoost-Upgrade: +10% oder +20% Fangchance bei legendären Monstern (Fangrate <= 3)
        int legendärBonus = 0;
        if (gegner.Fangrate <= 3)
        {
            if (Spieler.ZähneWallet.HatUpgrade(ZähneUpgrade.LegendärBoost20)) legendärBonus = 3;
            else if (Spieler.ZähneWallet.HatUpgrade(ZähneUpgrade.LegendärBoost10)) legendärBonus = 2;
        }
        // ─ Gesamtkraft ─
        int gesamtKraft = ballKraft + hpBonus + statusBonus + legendärBonus;
        // ─ Schutzwurf des Monsters (0 bis Widerstand-1) ─
        int schutzwurf = _rng.Next(0, widerstand);
        bool gefangen = gesamtKraft > schutzwurf;
        // ─ Feedback-Log ─
        string hpInfo = hpProzent < 0.10f ? "(sehr schwach)" : hpProzent < 0.25f ? "(schwach)" : hpProzent < 0.50f ? "(angeschlagen)" : "";
        string statusInfo = statusBonus > 0 ? $"({gegner.Status})" : "";
        AktuellerKampf.Log.Add($"📦 Kraft: {gesamtKraft} vs. Schutz: {schutzwurf} (Widerstand {widerstand}) {hpInfo} {statusInfo}".Trim());
        if (gefangen)
        {
            AktuellerKampf.Log.Add($"🎉 {gegner.Name} wurde gefangen!");
            // Dauerhaft als gefangen im Pokédex markieren
            Spieler.GefangeneMonster.Add(gegner.SpeziesId);
            Spieler.GeseheneMonster.Add(gegner.SpeziesId);
            // Nuzlocke: Ort als "bereits gefangen" markieren
            if (!string.IsNullOrEmpty(Spieler.AktuellerOrt))
                Spieler.GefangeneMonsterProOrt.Add(Spieler.AktuellerOrt);
            // StarterOnly-Relikt: nur Starter im Team, alles andere in Box
            if (Einstellungen.HatRelikt(ReliktTyp.StarterOnly))
            {
                Spieler.Box.Add(gegner);
                AktuellerKampf.Log.Add($"📦 {gegner.Name} wurde in die Box geschickt (Relikt: Starter-Only).");
            }
            else if (Spieler.Team.Count < MaxTeamGröße)
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
            int differenz = gesamtKraft - schutzwurf;
            int wackler = differenz == 0 ? 3 : differenz >= -1 ? 2 : 1;
            string wacklerText = wackler switch
            {
                3 => $"🟡 {gegner.Name} wackelt dreimal... und entwischt knapp!",
                2 => $"🟠 {gegner.Name} wackelt zweimal... und bricht aus!",
                _ => $"🔴 {gegner.Name} bricht sofort aus!"
            };
            AktuellerKampf.Log.Add(wacklerText);
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
        // KeinFliehen-Relikt
        if (Einstellungen.HatRelikt(ReliktTyp.KeinFliehen))
        {
            AktuellerKampf.Log.Add("❌ Relikt: Du kannst nicht fliehen!");
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
        bool warArenaKampf = AktuellerKampf?.Typ == KampfTyp.Arena;
        bool hatGewonnen   = AktuellerKampf?.SpielerGewonnen == true;
        string? arenaOrtId = AktuellerKampf?.OrtId;
        AktuellerKampf = null;

        // Generierte Karte: nach Arena-Sieg → Fog-of-War freischalten + NachArenaLeiter-Dialog
        if (IstGenerierteKartenModus && warArenaKampf && hatGewonnen && arenaOrtId != null)
        {
            var arenaOrt = AlleOrte.FirstOrDefault(o => o.Id == arenaOrtId);
            if (arenaOrt?.Arena != null)
            {
                GenerierteArenaGewonnen(arenaOrt);
                return;
            }
        }

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
        // WenigerXp-Relikt: 50% weniger XP
        if (Einstellungen.HatRelikt(ReliktTyp.WenigerXp)) exp = Math.Max(1, exp / 2);
        // ZähneUpgrade XpBoost: +50% XP
        if (Spieler.ZähneWallet.HatUpgrade(ZähneUpgrade.XpBoost)) exp = (int)(exp * 1.5f);
        // ZähneUpgrade MehrXp: +25% XP
        if (Spieler.ZähneWallet.HatUpgrade(ZähneUpgrade.MehrXp)) exp = (int)(exp * 1.25f);
        AktuellerKampf.ErfahrungGewonnen = exp;

        // ── XP-Verteilung: eingewechselte Monster ───────────────────────────────────
        // KeinXpTeiler-Relikt: nur das besiegende Monster bekommt 100% XP
        // Ohne Relikt: 50% für das besiegende Monster, 50% geteilt durch alle eingewechselten
        var eingewechselte = AktuellerKampf.EingewechselteMonster
            .Where(m => m != spielerMon && !m.IstOhnmächtig)
            .Distinct()
            .ToList();

        bool hatKeinXpTeilerRelikt = Einstellungen.HatRelikt(ReliktTyp.KeinXpTeiler);

        if (!hatKeinXpTeilerRelikt && eingewechselte.Count > 0)
        {
            // 50% für das besiegende Monster
            int expBesieger = exp / 2;
            // 50% aufgeteilt durch alle eingewechselten
            int expTeilerGesamt = exp - expBesieger;
            int expProEingewechseltes = Math.Max(1, expTeilerGesamt / eingewechselte.Count);

            spielerMon.ErfahrungsPunkte += expBesieger;
            AktuellerKampf.Log.Add($"+{expBesieger} EP für {spielerMon.AngezeigterName} (hat gesiegt)");

            foreach (var einMon in eingewechselte)
            {
                einMon.ErfahrungsPunkte += expProEingewechseltes;
                PrüfeLevelUp(einMon);
                AktuellerKampf.Log.Add($"+{expProEingewechseltes} EP für {einMon.AngezeigterName} (eingewechselt)");
            }
        }
        else
        {
            // Kein XP-Teiler-Relikt aktiv ODER keine eingewechselten Monster: 100% für das besiegende Monster
            spielerMon.ErfahrungsPunkte += exp;
            AktuellerKampf.Log.Add($"+{exp} EP für {spielerMon.AngezeigterName}");
        }

        // ZähneUpgrade XpTeiler: alle anderen Team-Monster bekommen halbe XP (nur wenn KeinXpTeiler-Relikt nicht aktiv)
        if (!hatKeinXpTeilerRelikt &&
            (Spieler.ZähneWallet.HatUpgrade(ZähneUpgrade.XpTeiler) || Spieler.ZähneWallet.HatUpgrade(ZähneUpgrade.VollerXpTeiler)))
        {
            int teamExp = Spieler.ZähneWallet.HatUpgrade(ZähneUpgrade.VollerXpTeiler) ? exp : exp / 2;
            foreach (var teamMon in Spieler.Team.Where(m => m != spielerMon && !m.IstOhnmächtig && !eingewechselte.Contains(m)))
            {
                teamMon.ErfahrungsPunkte += teamExp;
                PrüfeLevelUp(teamMon);
                AktuellerKampf.Log.Add($"+{teamExp} EP für {teamMon.AngezeigterName} (XP-Teiler)");
            }
        }

                // Level-Up prüfen
        PrüfeLevelUp(spielerMon);
        // Neue Attacken-Dialog starten falls vorhanden
        if (AktuellerKampf.PendingNeueAttacken.Count > 0)
        {
            // LernMonster aus Tupel lesen – so lernt immer das richtige Monster die Attacke
            AktuellerKampf.LernMonster = AktuellerKampf.PendingNeueAttacken[0].Monster;
            AktuellerKampf.NeueAttacke = AktuellerKampf.PendingNeueAttacken[0].Attacke;
        }
        // Geld
        if (AktuellerKampf.BelohnungGeld > 0)
        {
            int geld = AktuellerKampf.BelohnungGeld;
            // KeinGeldNachKampf-Relikt: kein Geld nach Kampf
            if (Einstellungen.HatRelikt(ReliktTyp.KeinGeldNachKampf))
            {
                geld = 0;
                AktuellerKampf.Log.Add($"🚷 Relikt: Kein Geld nach Kampf!");
            }
            else
            {
                // WenigerGeld-Relikt: 50% weniger Geld
                if (Einstellungen.HatRelikt(ReliktTyp.WenigerGeld)) geld = Math.Max(1, geld / 2);
                // ZähneUpgrade GeldBoost: +50% Geld
                if (Spieler.ZähneWallet.HatUpgrade(ZähneUpgrade.GeldBoost)) geld = (int)(geld * 1.5f);
                // ZähneUpgrade MehrGeld: +25% Geld
                if (Spieler.ZähneWallet.HatUpgrade(ZähneUpgrade.MehrGeld)) geld = (int)(geld * 1.25f);
            }
            if (geld > 0)
            {
                Spieler.Geld += geld;
                AktuellerKampf.Log.Add($"💰 +{geld} Münzen!");
            }
        }

        // Trainer: nächstes Monster?
        if ((AktuellerKampf.Typ == KampfTyp.Trainer || AktuellerKampf.Typ == KampfTyp.Arena)
            && AktuellerKampf.AktuellerTrainer != null)
        {
            AktuellerKampf.TrainerMonsterIndex++;
            if (AktuellerKampf.TrainerMonsterIndex < AktuellerKampf.AktuellerTrainer.Team.Count)
            {
                var nächsterEintrag = AktuellerKampf.AktuellerTrainer.Team[AktuellerKampf.TrainerMonsterIndex];
                var nächsteSpezies = TrainerMonsterSpeziesWählen(nächsterEintrag.MonsterId, nächsterEintrag);
                // Level kommt direkt aus Team-Eintrag (im generierten Modus bereits vom Generator gesetzt)
                var nächsterGegner = MonsterInstanz.VonSpezies(nächsteSpezies, nächsterEintrag.Level, AlleAttacken);
                MonsterAlsGesehenMarkieren(nächsteSpezies.Id, nächsterGegner);
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
        Notify();
        await Task.Delay(500);
        // Attacken-Lern-Dialog zuerst, dann Evolution
        if (AktuellerKampf.PendingNeueAttacken.Count > 0)
        {
            AktuellerKampf.Phase = KampfPhase.AttackeLernen;
            Notify();
        }
        else
        {
            await PrüfeEvolution(spielerMon);
        }
    }

    /// <summary>Wird vom Kampf.razor aufgerufen wenn der Spieler eine Attacke lernen/ablehnen möchte.</summary>
    public async Task AttackeLernenAbschliessen(AttackeInstanz? zuErsetzen)
    {
        if (AktuellerKampf == null) return;
        var mon = AktuellerKampf.LernMonster;
        var neueAtk = AktuellerKampf.NeueAttacke;
        if (mon == null || neueAtk == null) return;

        if (zuErsetzen != null)
        {
            // Alte Attacke ersetzen
            int idx = mon.Attacken.IndexOf(zuErsetzen);
            if (idx >= 0)
            {
                mon.Attacken[idx] = new AttackeInstanz
                {
                    Id = neueAtk.Id,
                    Name = neueAtk.Name,
                    Typ = neueAtk.Typ,
                    Kategorie = neueAtk.Kategorie,
                    Staerke = neueAtk.Staerke,
                    Genauigkeit = neueAtk.Genauigkeit,
                    MaxAp = neueAtk.Ap ?? 10,
                    AktuelleAp = neueAtk.Ap ?? 10,
                    Statuseffekt = neueAtk.Statuseffekt,
                    StatuseffektChance = neueAtk.StatuseffektChance,
                };
                AktuellerKampf.Log.Add($"✨ {mon.AngezeigterName} hat {neueAtk.Name} gelernt!");
            }
        }
        else
        {
            // Wenn noch Platz (< 4 Attacken), automatisch hinzufügen
            if (mon.Attacken.Count < 4)
            {
                mon.Attacken.Add(new AttackeInstanz
                {
                    Id = neueAtk.Id,
                    Name = neueAtk.Name,
                    Typ = neueAtk.Typ,
                    Kategorie = neueAtk.Kategorie,
                    Staerke = neueAtk.Staerke,
                    Genauigkeit = neueAtk.Genauigkeit,
                    MaxAp = neueAtk.Ap ?? 10,
                    AktuelleAp = neueAtk.Ap ?? 10,
                    Statuseffekt = neueAtk.Statuseffekt,
                    StatuseffektChance = neueAtk.StatuseffektChance,
                });
                AktuellerKampf.Log.Add($"✨ {mon.AngezeigterName} hat {neueAtk.Name} gelernt!");
            }
            else
            {
                AktuellerKampf.Log.Add($"❌ {mon.AngezeigterName} hat {neueAtk.Name} nicht gelernt.");
            }
        }

        // Nächste pending Attacke oder weiter
        AktuellerKampf.PendingNeueAttacken.RemoveAt(0);
        if (AktuellerKampf.PendingNeueAttacken.Count > 0)
        {
            // Nächstes Tupel: richtiges Monster UND richtige Attacke setzen
            AktuellerKampf.LernMonster = AktuellerKampf.PendingNeueAttacken[0].Monster;
            AktuellerKampf.NeueAttacke  = AktuellerKampf.PendingNeueAttacken[0].Attacke;
            Notify();
        }
        else
        {
            AktuellerKampf.LernMonster = null;
            AktuellerKampf.NeueAttacke = null;
            AktuellerKampf.Phase = KampfPhase.Beendet;
            Notify();
            await Task.Delay(300);
            await PrüfeEvolution(mon);
        }
    }

    private void KampfVerloren()
    {
        if (AktuellerKampf == null) return;
        AktuellerKampf.SpielerGewonnen = false;
        AktuellerKampf.Log.Add("💀 Alle Monster sind ohnmächtig...");

        // NuzlockeHart: ohnmächtige Monster werden dauerhaft aus dem Team entfernt
        if (Einstellungen.HatRelikt(ReliktTyp.NuzlockeHart) || Einstellungen.HatRelikt(ReliktTyp.Nuzlocke))
        {
            var ohnmächtige = Spieler.Team.Where(m => m.IstOhnmächtig).ToList();
            foreach (var mon in ohnmächtige)
            {
                Spieler.Team.Remove(mon);
                AktuellerKampf.Log.Add($"☠️ Nuzlocke: {mon.AngezeigterName} ist für immer verloren!");
            }
            if (!Spieler.Team.Any())
            {
                AktuellerKampf.Log.Add("🛑 Nuzlocke: Kein Monster mehr übrig! Spiel vorbei.");
                AktuellerKampf.Phase = KampfPhase.Beendet;
                Notify();
                return;
            }
        }
        else
        {
            // Normale Niederlage: alle heilen
            AktuellerKampf.Log.Add("Du wurdest besiegt und zum Monster Center gebracht.");
            int verlust = Math.Min(Spieler.Geld / 2, 500);
            Spieler.Geld -= verlust;
            if (verlust > 0) AktuellerKampf.Log.Add($"-{verlust} Münzen verloren.");
            foreach (var mon in Spieler.Team)
            {
                mon.AktuelleKp = mon.MaxKp;
                mon.Status = "none";
                foreach (var atk in mon.Attacken) atk.AktuelleAp = atk.MaxAp;
            }
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
        // KeineEntwicklung-Relikt: Entwicklung blockieren
        if (Einstellungen.HatRelikt(ReliktTyp.KeineEntwicklung))
        {
            AktuellerKampf.Log.Add($"🚫 Relikt: {mon.AngezeigterName} kann sich nicht entwickeln!");
            AktuellerKampf.Phase = KampfPhase.Beendet; Notify(); return;
        }
        // KeineEntwickeltenMonster-Relikt: nach Evolution in Box verschieben
        if (Einstellungen.HatRelikt(ReliktTyp.KeineEntwickeltenMonster))
        {
            AktuellerKampf.Log.Add($"🐣 Relikt: {mon.AngezeigterName} wird nach der Entwicklung in die Box verschoben!");
        }

        var neueSpezies = AlleMonster.FirstOrDefault(m => m.Id == mon.EntwickeltZu);
        if (neueSpezies == null) { AktuellerKampf.Phase = KampfPhase.Beendet; Notify(); return; }

        AktuellerKampf.EntwickeltSichMonster = mon;
        AktuellerKampf.EntwickeltSichZuName = neueSpezies.Name;
        AktuellerKampf.Phase = KampfPhase.Evolution;
        AktuellerKampf.Log.Add($"✨ {mon.AngezeigterName} entwickelt sich zu {neueSpezies.Name}!");
        Notify();
        await Task.Delay(2000);

        // Evolution durchführen
        string alteSpeziesId = mon.SpeziesId; // Alte ID merken für Pokédex
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
        // Pokédex: alte Vorstufe bleibt dauerhaft als gefangen, neue Stufe wird als gefangen markiert
        Spieler.GefangeneMonster.Add(alteSpeziesId);
        Spieler.GefangeneMonster.Add(neueSpezies.Id);
        Spieler.GeseheneMonster.Add(alteSpeziesId);
        Spieler.GeseheneMonster.Add(neueSpezies.Id);

        // KeineEntwickeltenMonster-Relikt: entwickeltes Monster in Box verschieben
        if (Einstellungen.HatRelikt(ReliktTyp.KeineEntwickeltenMonster))
        {
            Spieler.Team.Remove(mon);
            Spieler.Box.Add(mon);
            AktuellerKampf.Log.Add($"📦 {mon.AngezeigterName} wurde in die Box verschoben (Relikt: Keine Entwicklungen im Team).");
            // Wenn kein Monster mehr im Team: Kampf verloren
            if (!Spieler.Team.Any(m => !m.IstOhnmächtig))
                AktuellerKampf.Log.Add("🚨 Kein Monster mehr im Team!");
        }

        AktuellerKampf.EntwickeltSichMonster = null;
        AktuellerKampf.EntwickeltSichZuName = null;
        AktuellerKampf.Phase = KampfPhase.Beendet;
        Notify();
    }

    private void MonsterAlsGesehenMarkieren(string speziesId, MonsterInstanz instanz)
    {
        Spieler.GeseheneMonster.Add(speziesId);
        if (!Spieler.GeseheneAttacken.ContainsKey(speziesId))
            Spieler.GeseheneAttacken[speziesId] = new List<string>();
        foreach (var atk in instanz.Attacken)
        {
            if (!Spieler.GeseheneAttacken[speziesId].Contains(atk.Id))
                Spieler.GeseheneAttacken[speziesId].Add(atk.Id);
        }
    }

    private void PrüfeLevelUp(MonsterInstanz mon)
    {
        // Level-Cap: Region 1 = max 100, Region 2 = max 200, usw.
        // Relikt-LevelKappe hat Vorrang
        int maxLevel = AktuellesLevelCap;
        if (Einstellungen.HatRelikt(ReliktTyp.LevelKappe20)) maxLevel = Math.Min(maxLevel, 20);
        else if (Einstellungen.HatRelikt(ReliktTyp.LevelKappe40)) maxLevel = Math.Min(maxLevel, 40);
        else if (Einstellungen.HatRelikt(ReliktTyp.LevelKappe60)) maxLevel = Math.Min(maxLevel, 60);

        // WenigerXp-Relikt: 50% weniger Erfahrung
        if (Einstellungen.HatRelikt(ReliktTyp.WenigerXp))
            mon.ErfahrungsPunkte = (int)(mon.ErfahrungsPunkte * 0.5f);

        int nächstesLevel = mon.Level + 1;
        int benötigteExp = nächstesLevel * nächstesLevel * 10;
        while (mon.ErfahrungsPunkte >= benötigteExp && mon.Level < maxLevel)
        {
            mon.Level++;
            mon.ErfahrungsPunkte -= benötigteExp;
            mon.MaxKp += 5; mon.AktuelleKp = Math.Min(mon.AktuelleKp + 5, mon.MaxKp);
            mon.Angriff += 2; mon.Verteidigung += 2; mon.SpezialAngriff += 2;
            mon.SpezialVerteidigung += 2; mon.Initiative += 2;
            AktuellerKampf?.Log.Add($"🎉 {mon.AngezeigterName} ist auf Level {mon.Level} aufgestiegen!");

            // Neue Attacken prüfen die genau auf diesem Level gelernt werden
            var spezies = AlleMonster.FirstOrDefault(m => m.Id == mon.SpeziesId);
            if (spezies != null && AktuellerKampf != null)
            {
                var neueAttackenAufDiesemLevel = spezies.Attacken
                    .Where(a => a.Level == mon.Level)
                    .Select(a => AlleAttacken.FirstOrDefault(ad => ad.Id == a.AttackeId))
                    .Where(ad => ad != null && !mon.Attacken.Any(vorh => vorh.Id == ad!.Id))
                    .ToList();

                foreach (var neueAtk in neueAttackenAufDiesemLevel)
                {
                    if (neueAtk != null)
                        AktuellerKampf.PendingNeueAttacken.Add((mon, neueAtk));
                }
            }

            nächstesLevel = mon.Level + 1;
            benötigteExp = nächstesLevel * nächstesLevel * 10;
        }
    }

    // ── Status-Effekte ────────────────────────────────────────────────────────
    private bool StatusErlaubtAngriff(MonsterInstanz mon, List<string> log)
    {
        // Verliebt: 50% Chance nicht anzugreifen
        if (mon.IstVerliebt && _rng.Next(2) == 0)
        {
            log.Add($"💕 {mon.AngezeigterName} ist zu verliebt um anzugreifen!");
            return false;
        }
        // Verwirrt: 33% Chance sich selbst zu treffen
        if (mon.IstVerwirrt && _rng.Next(3) == 0)
        {
            int selbstSchaden = Math.Max(1, mon.MaxKp / 10);
            mon.AktuelleKp = Math.Max(0, mon.AktuelleKp - selbstSchaden);
            log.Add($"😵 {mon.AngezeigterName} verletzt sich in der Verwirrung! (-{selbstSchaden} KP)");
            return false;
        }
        switch (mon.Status)
        {
            case "eingeschlafen":
                mon.StatusZähler--;
                if (mon.StatusZähler <= 0) { mon.Status = "none"; mon.HatAlbtraum = false; log.Add($"☀️ {mon.AngezeigterName} ist aufgewacht!"); return true; }
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
            case "eingeschlafen":
                // Albtraum: Schaden während Schlaf
                if (mon.HatAlbtraum)
                {
                    int albtraumSchaden = Math.Max(1, mon.MaxKp / 4);
                    mon.AktuelleKp = Math.Max(0, mon.AktuelleKp - albtraumSchaden);
                    log.Add($"👻 {mon.AngezeigterName} leidet unter Albträumen! (-{albtraumSchaden} KP)");
                }
                break;
        }
        // Egelsamen: KP-Drain
        if (mon.HatEgelsamen && mon.AktuelleKp > 0)
        {
            int egelSchaden = Math.Max(1, mon.MaxKp / 8);
            mon.AktuelleKp = Math.Max(0, mon.AktuelleKp - egelSchaden);
            log.Add($"🌱 {mon.AngezeigterName} verliert KP durch Egelsamen! (-{egelSchaden} KP)");
        }
        // Gähnen: nächste Runde einschlafen
        if (mon.GähnenAktiv)
        {
            mon.GähnenAktiv = false;
            if (mon.Status == "none")
            {
                mon.Status = "eingeschlafen";
                mon.StatusZähler = _rng.Next(2, 5);
                log.Add($"💤 {mon.AngezeigterName} ist vor Müdigkeit eingeschlafen!");
            }
        }
        // Verwirrt: Zähler reduzieren
        if (mon.IstVerwirrt)
        {
            mon.VerwirrtZähler--;
            if (mon.VerwirrtZähler <= 0)
                log.Add($"✨ {mon.AngezeigterName} ist nicht mehr verwirrt!");
        }
    }

    private void VersuchemStatusEffektDirekt(MonsterInstanz ziel, string neuerStatus, List<string> log)
    {
        // Spezial-Status die nicht den Haupt-Status belegen
        if (neuerStatus == "egelsamen")
        {
            if (ziel.HatEgelsamen) { log.Add($"🌱 {ziel.AngezeigterName} hat bereits Egelsamen!"); return; }
            if (ziel.Typen.Contains("Pflanze")) { log.Add($"🌱 {ziel.AngezeigterName} ist immun gegen Egelsamen!"); return; }
            ziel.HatEgelsamen = true;
            log.Add($"🌱 {ziel.AngezeigterName} wurde mit Egelsamen bepflanzt!");
            return;
        }
        if (neuerStatus == "albtraum")
        {
            if (ziel.Status != "eingeschlafen") { log.Add($"👻 Albtraum wirkt nur bei schlafenden Monstern!"); return; }
            ziel.HatAlbtraum = true;
            log.Add($"👻 {ziel.AngezeigterName} hat Albträume!");
            return;
        }
        if (neuerStatus == "verwirrt")
        {
            if (ziel.IstVerwirrt) { log.Add($"😵 {ziel.AngezeigterName} ist bereits verwirrt!"); return; }
            ziel.VerwirrtZähler = _rng.Next(2, 6);
            log.Add($"😵 {ziel.AngezeigterName} ist verwirrt!");
            return;
        }
        if (neuerStatus == "verliebt")
        {
            if (ziel.IstVerliebt) { log.Add($"💕 {ziel.AngezeigterName} ist bereits verliebt!"); return; }
            ziel.IstVerliebt = true;
            log.Add($"💕 {ziel.AngezeigterName} ist verliebt!");
            return;
        }
        if (neuerStatus == "gähnen")
        {
            if (ziel.GähnenAktiv) { log.Add($"💤 {ziel.AngezeigterName} gähnt bereits!"); return; }
            ziel.GähnenAktiv = true;
            log.Add($"💤 {ziel.AngezeigterName} gähnt... Es wird müde!");
            return;
        }
        // Haupt-Status (vergiftet, gelähmt, verbrannt, eingeschlafen, eingefroren)
        if (ziel.Status != "none") return;
        // Immunität: Feuer-Typen können nicht verbrennen, Eis-Typen nicht einfrieren, Gift-Typen nicht vergiftet werden
        if (neuerStatus == "verbrannt" && ziel.Typen.Contains("Brennen")) return;
        if (neuerStatus == "eingefroren" && ziel.Typen.Contains("Eis")) return;
        if (neuerStatus == "vergiftet" && (ziel.Typen.Contains("Gift") || ziel.Typen.Contains("Stahl"))) return;
        ziel.Status = neuerStatus;
        if (neuerStatus == "eingeschlafen") ziel.StatusZähler = _rng.Next(2, 5);
        log.Add(neuerStatus switch
        {
            "verbrannt"     => $"🔥 {ziel.AngezeigterName} wurde verbrannt!",
            "eingefroren"   => $"🧊 {ziel.AngezeigterName} wurde eingefroren!",
            "gelähmt"       => $"⚡ {ziel.AngezeigterName} wurde gelähmt!",
            "vergiftet"     => $"☠️ {ziel.AngezeigterName} wurde vergiftet!",
            "eingeschlafen" => $"💤 {ziel.AngezeigterName} ist eingeschlafen!",
            _ => ""
        });
    }
    // Legacy-Methode (nicht mehr verwendet, bleibt für Kompatibilität)
    private void VersuchemStatusEffekt(MonsterInstanz ziel, string attackeTyp, List<string> log)
    {
        string? neuerStatus = attackeTyp switch
        {
            "Brennen" => "verbrannt",
            "Eis"     => "eingefroren",
            "Blitz"   => "gelähmt",
            "Gift"    => "vergiftet",
            _         => null
        };
        if (neuerStatus != null) VersuchemStatusEffektDirekt(ziel, neuerStatus, log);
    }

    /// <summary>Führt Sondereffekte von Status-Attacken aus (Stat-Boosts, Flucht, etc.). Gibt true zurück wenn ein Effekt ausgeführt wurde.</summary>
    private bool AttackeSondereffektAusführen(AttackeInstanz attacke, MonsterInstanz angreifer, MonsterInstanz ziel, List<string> log, bool istTrainerKampf, string? attackeName = null)
    {
        switch (attacke.Id)
        {
            // ─── ANGREIFER STAT +2 ────────────────────────────────────────────
            case "MOV-0014": // Schwerttanz: Angriff +2
                StatStufeAnpassen(angreifer, "angriff", 2, log, attacke.Name); return true;
            case "MOV-0097": // Agilität: Initiative +2
                StatStufeAnpassen(angreifer, "initiative", 2, log, attacke.Name); return true;
            case "MOV-0133": // Amnesie: SpVerteidigung +2
                StatStufeAnpassen(angreifer, "spverteidigung", 2, log, attacke.Name); return true;
            case "MOV-0417": // Ränkeschmied (Nasty Plot): SpAngriff +2
                StatStufeAnpassen(angreifer, "spangriff", 2, log, attacke.Name); return true;
            case "MOV-0367": // Klinge schärfen: Angriff +2
                StatStufeAnpassen(angreifer, "angriff", 2, log, attacke.Name); return true;
            case "MOV-0349": // Eisenabwehr: Verteidigung +2
                StatStufeAnpassen(angreifer, "verteidigung", 2, log, attacke.Name); return true;
            case "MOV-0334": // Eisenpanzer: Verteidigung +2
                StatStufeAnpassen(angreifer, "verteidigung", 2, log, attacke.Name); return true;
            case "MOV-0322": // Kosmik-Kraft: SpVerteidigung+SpAngriff +1
                StatStufeAnpassen(angreifer, "spverteidigung", 1, log, attacke.Name);
                StatStufeAnpassen(angreifer, "spangriff", 1, log, attacke.Name); return true;

            // ─── ANGREIFER STAT +1 ────────────────────────────────────────────
            case "MOV-0074": // Wachstum: Angriff+SpAngriff +1
                StatStufeAnpassen(angreifer, "angriff", 1, log, attacke.Name);
                StatStufeAnpassen(angreifer, "spangriff", 1, log, attacke.Name); return true;
            case "MOV-0096": // Meditation: Angriff +1
                StatStufeAnpassen(angreifer, "angriff", 1, log, attacke.Name); return true;
            case "MOV-0106": // Härtner: Verteidigung +1
                StatStufeAnpassen(angreifer, "verteidigung", 1, log, attacke.Name); return true;
            case "MOV-0110": // Verhärtung: Verteidigung +1
                StatStufeAnpassen(angreifer, "verteidigung", 1, log, attacke.Name); return true;
            case "MOV-0339": // Protzer (Bulk Up): Angriff+Verteidigung +1
                StatStufeAnpassen(angreifer, "angriff", 1, log, attacke.Name);
                StatStufeAnpassen(angreifer, "verteidigung", 1, log, attacke.Name); return true;
            case "MOV-0347": // Gedankengut (Calm Mind): SpAngriff+SpVerteidigung +1
                StatStufeAnpassen(angreifer, "spangriff", 1, log, attacke.Name);
                StatStufeAnpassen(angreifer, "spverteidigung", 1, log, attacke.Name); return true;
            case "MOV-0526": // Kraftschub (Coil): Angriff+Verteidigung+Genauigkeit +1
                StatStufeAnpassen(angreifer, "angriff", 1, log, attacke.Name);
                StatStufeAnpassen(angreifer, "verteidigung", 1, log, attacke.Name);
                StatStufeAnpassen(angreifer, "genauigkeit", 1, log, attacke.Name); return true;

            // ─── ANGREIFER STAT gemischt ──────────────────────────────────────
            case "MOV-0174": // Fluch: Angriff+Verteidigung +1, Initiative -1
                StatStufeAnpassen(angreifer, "angriff", 1, log, attacke.Name);
                StatStufeAnpassen(angreifer, "verteidigung", 1, log, attacke.Name);
                StatStufeAnpassen(angreifer, "initiative", -1, log, attacke.Name); return true;

            // ─── GEGNER STAT -1 ───────────────────────────────────────────────
            case "MOV-0028": // Sandwirbel: Genauigkeit Gegner -1
                StatStufeAnpassen(ziel, "genauigkeit", -1, log, attacke.Name); return true;
            case "MOV-0039": // Rutenschlag (Tail Whip): Verteidigung Gegner -1
                StatStufeAnpassen(ziel, "verteidigung", -1, log, attacke.Name); return true;
            case "MOV-0045": // Heuler (Growl): Angriff Gegner -1
                StatStufeAnpassen(ziel, "angriff", -1, log, attacke.Name); return true;
            case "MOV-0103": // Kreideschrei (Screech): Verteidigung Gegner -2
                StatStufeAnpassen(ziel, "verteidigung", -2, log, attacke.Name); return true;
            case "MOV-0104": // Doppelteam: Ausweichen Gegner -1
                StatStufeAnpassen(ziel, "genauigkeit", -1, log, attacke.Name); return true;
            case "MOV-0111": // Schrei: Angriff Gegner -1
                StatStufeAnpassen(ziel, "angriff", -1, log, attacke.Name); return true;
            case "MOV-0141": // Blutsauger (Leer): Verteidigung Gegner -1
                StatStufeAnpassen(ziel, "verteidigung", -1, log, attacke.Name); return true;
            case "MOV-0186": // Nasenschleim: SpAngriff Gegner -1
                StatStufeAnpassen(ziel, "spangriff", -1, log, attacke.Name); return true;
            case "MOV-0187": // Schleimschleuder: SpAngriff Gegner -1
                StatStufeAnpassen(ziel, "spangriff", -1, log, attacke.Name); return true;
            case "MOV-0204": // Charme: Angriff Gegner -2
                StatStufeAnpassen(ziel, "angriff", -2, log, attacke.Name); return true;
            case "MOV-0252": // Mogelhieb (Fake Tears): SpVerteidigung Gegner -2
                StatStufeAnpassen(ziel, "spverteidigung", -2, log, attacke.Name); return true;
            case "MOV-0260": // Schmeichler (Flatter): SpAngriff Gegner +1 (aber verwirrt)
                StatStufeAnpassen(ziel, "spangriff", 1, log, attacke.Name); return true;
            case "MOV-0498": // Zermueben (Snarl): SpAngriff Gegner -1
                StatStufeAnpassen(ziel, "spangriff", -1, log, attacke.Name); return true;
            case "MOV-0493": // Wankelstrahl (Parting Shot): Angriff+SpAngriff Gegner -1
                StatStufeAnpassen(ziel, "angriff", -1, log, attacke.Name);
                StatStufeAnpassen(ziel, "spangriff", -1, log, attacke.Name); return true;

            // ─── HEILUNG (50% max KP) ────────────────────────────────────────────
            case "MOV-0156": // Erholung (Recover): 50% max KP
            case "MOV-0135": // Weichei (Soft-Boiled): 50% max KP
            case "MOV-0235": // Synthese: 50% max KP
            case "MOV-0236": // Mondschein: 50% max KP
            case "MOV-0355": // Ruheort: 50% max KP
            {
                int heilung = Math.Max(1, angreifer.MaxKp / 2);
                heilung = Math.Min(heilung, angreifer.MaxKp - angreifer.AktuelleKp);
                if (heilung > 0)
                {
                    angreifer.AktuelleKp += heilung;
                    log.Add($"💚 {angreifer.AngezeigterName} erholt sich! (+{heilung} KP)");
                }
                else
                    log.Add($"💚 {angreifer.AngezeigterName} hat bereits volle KP!");
                return true;
            }

            // ─── TELEPORT (Flucht aus wildem Kampf / Teleport im Trainerkampf) ──
            case "MOV-0100": // Teleport
                log.Add($"🌀 {angreifer.AngezeigterName} setzt Teleport ein!");
                angreifer.IstTeleportFlucht = true; // Signal für Flucht/Teleport
                return true;

            // ─── ROAR / BRÜLLER (Gegner flieht / wechselt) ─────────────────────
            case "MOV-0046": // Brüller (Roar)
                if (!istTrainerKampf)
                {
                    log.Add($"📢 {ziel.Name} ist geflohen!");
                    ziel.IstRoarFlucht = true; // Signal für Gegner-Flucht
                }
                else
                    log.Add($"📢 {ziel.Name} wird zurückgerufen! (Trainer wechselt zum nächsten)");
                return true;

            // ─── ANGREIFER STAT +2 (weitere) ─────────────────────────────────────
            case "MOV-0397": // Steinpolitur: Initiative +2
                StatStufeAnpassen(angreifer, "initiative", 2, log, attacke.Name); return true;
            case "MOV-0159": // Schärfer: Angriff +2
                StatStufeAnpassen(angreifer, "angriff", 2, log, attacke.Name); return true;
            case "MOV-0151": // Säurepanzer: SpAngriff +2
                StatStufeAnpassen(angreifer, "spangriff", 2, log, attacke.Name); return true;
            case "MOV-0112": // Barriere: Verteidigung +2
                StatStufeAnpassen(angreifer, "verteidigung", 2, log, attacke.Name); return true;
            case "MOV-0842": // Shelter: Verteidigung +2
                StatStufeAnpassen(angreifer, "verteidigung", 2, log, attacke.Name); return true;

            case "MOV-0837": // Victory Dance: Angriff+Verteidigung+Initiative +1
                StatStufeAnpassen(angreifer, "angriff", 1, log, attacke.Name);
                StatStufeAnpassen(angreifer, "verteidigung", 1, log, attacke.Name);
                StatStufeAnpassen(angreifer, "initiative", 1, log, attacke.Name); return true;
            case "MOV-0868": // Fillet Away: Angriff+SpAngriff+Initiative +2, KP -50%
            {
                int verlust = Math.Max(1, angreifer.MaxKp / 2);
                angreifer.AktuelleKp = Math.Max(1, angreifer.AktuelleKp - verlust);
                StatStufeAnpassen(angreifer, "angriff", 2, log, attacke.Name);
                StatStufeAnpassen(angreifer, "spangriff", 2, log, attacke.Name);
                StatStufeAnpassen(angreifer, "initiative", 2, log, attacke.Name);
                log.Add($"💔 {angreifer.AngezeigterName} verliert {verlust} KP!");
                return true;
            }

            // ─── ANGREIFER STAT +1 (weitere) ─────────────────────────────────────
            case "MOV-0107": // Komprimator: Verteidigung +1
                StatStufeAnpassen(angreifer, "verteidigung", 1, log, attacke.Name); return true;
            case "MOV-0116": // Energiefokus: Genauigkeit +1
                StatStufeAnpassen(angreifer, "genauigkeit", 1, log, attacke.Name); return true;
            case "MOV-0113": // Lichtschild: SpVerteidigung +1
                StatStufeAnpassen(angreifer, "spverteidigung", 1, log, attacke.Name); return true;
            case "MOV-0115": // Reflektor: Verteidigung +1
                StatStufeAnpassen(angreifer, "verteidigung", 1, log, attacke.Name); return true;
            case "MOV-0673": // Konzentration: Angriff+SpAngriff +1
                StatStufeAnpassen(angreifer, "angriff", 1, log, attacke.Name);
                StatStufeAnpassen(angreifer, "spangriff", 1, log, attacke.Name); return true;
            case "MOV-0811": // Coaching: Angriff+Verteidigung +1
                StatStufeAnpassen(angreifer, "angriff", 1, log, attacke.Name);
                StatStufeAnpassen(angreifer, "verteidigung", 1, log, attacke.Name); return true;
            case "MOV-0775": // Seelentanz: SpAngriff +1
                StatStufeAnpassen(angreifer, "spangriff", 1, log, attacke.Name); return true;

            // ─── GEGNER STAT -1 (weitere) ─────────────────────────────────────────
            case "MOV-0081": // Fadenschuss: Initiative Gegner -1
                StatStufeAnpassen(ziel, "initiative", -1, log, attacke.Name); return true;
            case "MOV-0108": // Rauchwolke: Genauigkeit Gegner -1
                StatStufeAnpassen(ziel, "genauigkeit", -1, log, attacke.Name); return true;
            case "MOV-0043": // Silberblick: Genauigkeit Gegner -1
                StatStufeAnpassen(ziel, "genauigkeit", -1, log, attacke.Name); return true;

            case "MOV-0749": // Teerschuss: Initiative Gegner -2
                StatStufeAnpassen(ziel, "initiative", -2, log, attacke.Name); return true;
            case "MOV-0715": // Tränendrüse: SpAngriff+SpVerteidigung Gegner -1
                StatStufeAnpassen(ziel, "spangriff", -1, log, attacke.Name);
                StatStufeAnpassen(ziel, "spverteidigung", -1, log, attacke.Name); return true;
            case "MOV-0608": // Kulleraugen: Verteidigung Gegner -1
                StatStufeAnpassen(ziel, "verteidigung", -1, log, attacke.Name); return true;
            case "MOV-0858": // Spicy Extract: SpAngriff Gegner +2, SpVerteidigung Gegner -2
                StatStufeAnpassen(ziel, "spangriff", 2, log, attacke.Name);
                StatStufeAnpassen(ziel, "spverteidigung", -2, log, attacke.Name); return true;
            case "MOV-0913": // Drachenschrei: Initiative Gegner -1
                StatStufeAnpassen(ziel, "initiative", -1, log, attacke.Name); return true;

            // ─── HEILUNG (weitere) ────────────────────────────────────────────────
            case "MOV-0105": // Genesung: 50% max KP
            case "MOV-0816": // Dschungelheilung: 50% max KP
            case "MOV-0849": // Lunar Blessing: 50% max KP
            {
                int heilung2 = Math.Max(1, angreifer.MaxKp / 2);
                heilung2 = Math.Min(heilung2, angreifer.MaxKp - angreifer.AktuelleKp);
                if (heilung2 > 0)
                {
                    angreifer.AktuelleKp += heilung2;
                    log.Add($"💚 {angreifer.AngezeigterName} erholt sich! (+{heilung2} KP)");
                }
                else
                    log.Add($"💚 {angreifer.AngezeigterName} hat bereits volle KP!");
                return true;
            }
            case "MOV-0791": // Lebenstropfen: 25% max KP
            {
                int heilung3 = Math.Max(1, angreifer.MaxKp / 4);
                heilung3 = Math.Min(heilung3, angreifer.MaxKp - angreifer.AktuelleKp);
                if (heilung3 > 0)
                {
                    angreifer.AktuelleKp += heilung3;
                    log.Add($"💚 {angreifer.AngezeigterName} erholt sich ein wenig! (+{heilung3} KP)");
                }
                else
                    log.Add($"💚 {angreifer.AngezeigterName} hat bereits volle KP!");
                return true;
            }

            // ─── GEDULD (Physisch, 0 Stärke → zählt als Status) ─────────────────
            case "MOV-0117": // Geduld: Angriff+SpAngriff +1
                StatStufeAnpassen(angreifer, "angriff", 1, log, attacke.Name);
                StatStufeAnpassen(angreifer, "spangriff", 1, log, attacke.Name); return true;

            // ─── WIRBELWIND (Flucht wild) ─────────────────────────────────────────
            case "MOV-0018": // Wirbelwind
                if (!istTrainerKampf)
                {
                    log.Add($"💨 {ziel.Name} wurde weggeblasen!");
                    ziel.IstRoarFlucht = true;
                }
                else
                    log.Add($"💨 Wirbelwind hat im Trainerkampf keine Wirkung!");
                return true;

            // ─── SONSTIGE (kein Effekt implementiert, aber Log-Text) ─────────────
            case "MOV-0054": // Weißnebel: verhindert Stat-Senkung (vereinfacht: nichts)
                log.Add($"🌫️ {angreifer.AngezeigterName} ist von Weißnebel umhüllt!"); return true;
            case "MOV-0102": // Mimikry: keine Wirkung
                log.Add($"🪞 {angreifer.AngezeigterName} setzt Mimikry ein!"); return true;

            case "MOV-0114": // Dunkelnebel: Genauigkeit Gegner -1
                StatStufeAnpassen(ziel, "genauigkeit", -1, log, attacke.Name); return true;
            case "MOV-0119": // Spiegeltrick: Verteidigung +1
                StatStufeAnpassen(angreifer, "verteidigung", 1, log, attacke.Name); return true;
            case "MOV-0134": // Psykraft: SpAngriff +1
                StatStufeAnpassen(angreifer, "spangriff", 1, log, attacke.Name); return true;
            case "MOV-0148": // Blitz (Status): Genauigkeit Gegner -1
                StatStufeAnpassen(ziel, "genauigkeit", -1, log, attacke.Name); return true;
            case "MOV-0150": // Platscher: Verteidigung +1
                StatStufeAnpassen(angreifer, "verteidigung", 1, log, attacke.Name); return true;
            case "MOV-0169": // Spinnennetz: Initiative Gegner -1
                StatStufeAnpassen(ziel, "initiative", -1, log, attacke.Name); return true;
            case "MOV-0659": // Sandsammler: SpVerteidigung +1
                StatStufeAnpassen(angreifer, "spverteidigung", 1, log, attacke.Name); return true;
        }
        return false;
    }

    private void StatStufeAnpassen(MonsterInstanz mon, string stat, int delta, List<string> log, string? attackeName = null)
    {
        string statName = stat switch {
            "angriff" => "Angriff", "verteidigung" => "Verteidigung",
            "spangriff" => "Sp.Angriff", "spverteidigung" => "Sp.Verteidigung",
            "initiative" => "Initiative", "genauigkeit" => "Genauigkeit", _ => stat
        };
        string richtung = delta > 0 ? "stieg" : "sank";
        string stufe = Math.Abs(delta) >= 2 ? " stark" : "";
        switch (stat)
        {
            case "angriff": mon.StatStufeAngriff = Math.Clamp(mon.StatStufeAngriff + delta, -6, 6); break;
            case "verteidigung": mon.StatStufeVerteidigung = Math.Clamp(mon.StatStufeVerteidigung + delta, -6, 6); break;
            case "spangriff": mon.StatStufeSpAngriff = Math.Clamp(mon.StatStufeSpAngriff + delta, -6, 6); break;
            case "spverteidigung": mon.StatStufeSpVerteidigung = Math.Clamp(mon.StatStufeSpVerteidigung + delta, -6, 6); break;
            case "initiative": mon.StatStufeInitiative = Math.Clamp(mon.StatStufeInitiative + delta, -6, 6); break;
            case "genauigkeit": mon.StatStufeGenauigkeit = Math.Clamp(mon.StatStufeGenauigkeit + delta, -6, 6); break;
        }
        string attackeZusatz = attackeName != null ? $" ({attackeName})" : "";
        log.Add($"✨ {mon.AngezeigterName}s {statName} {richtung}{stufe}{attackeZusatz}!");
    }

    private int SchadenBerechnen(MonsterInstanz angreifer, MonsterInstanz verteidiger, AttackeInstanz attacke)
    {
        if (attacke.Staerke == null || attacke.Staerke == 0) return 0;
        int atkBasis = attacke.Kategorie == "Physisch" ? angreifer.Angriff : angreifer.SpezialAngriff;
        int defBasis = attacke.Kategorie == "Physisch" ? verteidiger.Verteidigung : verteidiger.SpezialVerteidigung;
        int atkStufe = attacke.Kategorie == "Physisch" ? angreifer.StatStufeAngriff : angreifer.StatStufeSpAngriff;
        int defStufe = attacke.Kategorie == "Physisch" ? verteidiger.StatStufeVerteidigung : verteidiger.StatStufeSpVerteidigung;
        int atk = MonsterInstanz.MitStatStufe(atkBasis, atkStufe);
        int def = MonsterInstanz.MitStatStufe(defBasis, defStufe);
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
        int ergebnis2 = Math.Max(1, (int)(schaden * multi * stab * zufall));
        // DoppelterSchaden-Relikt: Spieler erhält doppelten Schaden
        if (Einstellungen.HatRelikt(ReliktTyp.DoppelterSchaden) && AktuellerKampf != null && verteidiger == AktuellerKampf.SpielerMonster)
            ergebnis2 *= 2;
        return ergebnis2;
    }

    private AttackeInstanz? GegnerAttackeWählen(MonsterInstanz gegner)
    {
        var verfügbar = gegner.Attacken.Where(a => a.HatAp).ToList();
        if (!verfügbar.Any()) return null;
        return verfügbar[_rng.Next(verfügbar.Count)];
    }

        // ── Wilde Begegnung ───────────────────────────────────────────────────────
    public bool ZufallsBegegnungPrüfen(Ort ort) => ort.WildMonster.Any() ||
        Einstellungen.WildModus == WildMonsterModus.ZufälligNurRegion ||
        Einstellungen.WildModus == WildMonsterModus.Zufällig ||
        Einstellungen.WildModus == WildMonsterModus.ZufälligMitLegär;
    public WildBegegnung? ZufälligesWildMonster(Ort ort)
    {
        var modus = Einstellungen.WildModus;

        // Zufällig nur diese Region
        if (modus == WildMonsterModus.ZufälligNurRegion)
        {
            var regionPrefix = Spieler.AktuellerOrt?.Length >= 3 ? Spieler.AktuellerOrt[..3] : "KAN";
            var regionMonsterIds = AlleOrte
                .Where(o => o.Id.StartsWith(regionPrefix, StringComparison.OrdinalIgnoreCase))
                .SelectMany(o => o.WildMonster.Select(w => w.MonsterId))
                .Distinct().ToHashSet();
            var regionPool = AlleMonster.Where(m => regionMonsterIds.Contains(m.Id) && m.Fangrate > 3).ToList();
            if (!regionPool.Any()) regionPool = AlleMonster.Where(m => m.Fangrate > 3).ToList();
            var spezies = regionPool[_rng.Next(regionPool.Count)];
            int minLvl = ort.WildMonster.Any() ? ort.WildMonster.Min(w => w.MinLevel) : 2;
            int maxLvl = ort.WildMonster.Any() ? ort.WildMonster.Max(w => w.MaxLevel) : 60;
            return new WildBegegnung { MonsterId = spezies.Id, MinLevel = minLvl, MaxLevel = maxLvl, Chance = 100 };
        }

        // Zufällig (alle Generationen) oder Zufällig + Legendäre
        if (modus == WildMonsterModus.Zufällig || modus == WildMonsterModus.ZufälligMitLegär)
        {
            // Pool: alle Monster, bei Zufällig ohne Legendäre (fangrate > 3)
            var pool = modus == WildMonsterModus.ZufälligMitLegär
                ? AlleMonster
                : AlleMonster.Where(m => m.Fangrate > 3).ToList();

            if (!pool.Any()) return null;

            var zufälligeSpezies = pool[_rng.Next(pool.Count)];

            // Level aus Ort-Bereich ableiten (falls vorhanden), sonst 2-60
            int minLvl = ort.WildMonster.Any() ? ort.WildMonster.Min(w => w.MinLevel) : 2;
            int maxLvl = ort.WildMonster.Any() ? ort.WildMonster.Max(w => w.MaxLevel) : 60;
            // Legendäre: etwas höheres Level
            if (zufälligeSpezies.Fangrate <= 3)
            {
                minLvl = Math.Max(minLvl, 40);
                maxLvl = Math.Max(maxLvl, 70);
            }

            return new WildBegegnung
            {
                MonsterId = zufälligeSpezies.Id,
                MinLevel = minLvl,
                MaxLevel = maxLvl,
                Chance = 100
            };
        }

        // RouteGenau: Original-Logik
        if (!ort.WildMonster.Any()) return null;

        // Legendäre/Einzigartige Monster (Fangrate ≤ 3) haben im Route-genau-Modus 100% Begegnungsgarantie
        // Sie kommen nur auf einer einzigen Route vor, also wenn man dort ist, trifft man sie garantiert
        var legendärAufRoute = ort.WildMonster
            .Where(w => AlleMonster.FirstOrDefault(m => m.Id == w.MonsterId)?.Fangrate <= 3)
            .ToList();
        if (legendärAufRoute.Any())
        {
            // 100% Chance: immer das Legendäre Monster als Begegnung wählen
            return legendärAufRoute[_rng.Next(legendärAufRoute.Count)];
        }

        // Normale gewichtete Auswahl für nicht-legendäre Monster
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

    /// <summary>Wählt die Spezies für ein Trainer-Monster basierend auf dem TrainerModus.</summary>
    private MonsterData TrainerMonsterSpeziesWählen(string monsterId, MonsterTeamEintrag eintrag)
    {
        var modus = Einstellungen.TrainerModus;

        // Region des aktuellen Ortes ermitteln
        var regionPrefix = Spieler.AktuellerOrt?.Length >= 3 ? Spieler.AktuellerOrt[..3] : "KAN";
        var regionMonsterIds = AlleOrte
            .Where(o => o.Id.StartsWith(regionPrefix, StringComparison.OrdinalIgnoreCase))
            .SelectMany(o => o.WildMonster.Select(w => w.MonsterId))
            .Distinct().ToHashSet();
        var regionPool = AlleMonster.Where(m => regionMonsterIds.Contains(m.Id) && m.Fangrate > 3).ToList();
        if (!regionPool.Any()) regionPool = AlleMonster.Where(m => m.Fangrate > 3).ToList();

        if (modus == TrainerMonsterModus.Zufällig)
        {
            // Zufälliges Monster aus der aktuellen Region
            return regionPool.Any() ? regionPool[_rng.Next(regionPool.Count)] : AlleMonster[_rng.Next(AlleMonster.Count)];
        }

        if (modus == TrainerMonsterModus.ZufälligAlleRegionen)
        {
            // Zufälliges Monster aus allen Regionen
            var pool = AlleMonster.Where(m => m.Fangrate > 3).ToList();
            return pool.Any() ? pool[_rng.Next(pool.Count)] : AlleMonster[_rng.Next(AlleMonster.Count)];
        }

        if (modus == TrainerMonsterModus.ZufälligMitTypen)
        {
            // Zufälliges Monster mit gleichem Typ aus der Region
            var original = AlleMonster.FirstOrDefault(m => m.Id == monsterId);
            if (original != null && original.Typen.Any())
            {
                var ersterTyp = original.Typen[0];
                var pool = regionPool.Where(m => m.Typen.Contains(ersterTyp)).ToList();
                if (!pool.Any()) pool = regionPool;
                if (pool.Any()) return pool[_rng.Next(pool.Count)];
            }
            return regionPool.Any() ? regionPool[_rng.Next(regionPool.Count)] : AlleMonster[_rng.Next(AlleMonster.Count)];
        }

        if (modus == TrainerMonsterModus.ZufälligMitTypenAlleRegionen)
        {
            // Zufälliges Monster mit gleichem Typ aus allen Regionen
            var original = AlleMonster.FirstOrDefault(m => m.Id == monsterId);
            if (original != null && original.Typen.Any())
            {
                var ersterTyp = original.Typen[0];
                var pool = AlleMonster.Where(m => m.Fangrate > 3 && m.Typen.Contains(ersterTyp)).ToList();
                if (pool.Any()) return pool[_rng.Next(pool.Count)];
            }
            var fallback = AlleMonster.Where(m => m.Fangrate > 3).ToList();
            return fallback.Any() ? fallback[_rng.Next(fallback.Count)] : AlleMonster[_rng.Next(AlleMonster.Count)];
        }

        if (modus == TrainerMonsterModus.WildeNurRegion)
        {
            // Wilde Monster nur aus der aktuellen Region (wie ZufälligNurRegion bei Wildnis)
            return regionPool.Any() ? regionPool[_rng.Next(regionPool.Count)] : AlleMonster[_rng.Next(AlleMonster.Count)];
        }

        if (modus == TrainerMonsterModus.WildeAlleGenerationen)
        {
            // Wilde Monster aus allen Generationen (wie Zufällig bei Wildnis)
            var pool = AlleMonster.Where(m => m.Fangrate > 3).ToList();
            return pool.Any() ? pool[_rng.Next(pool.Count)] : AlleMonster[_rng.Next(AlleMonster.Count)];
        }

        // Genau: definiertes Monster
        return AlleMonster.FirstOrDefault(m => m.Id == monsterId) ?? AlleMonster[_rng.Next(AlleMonster.Count)];
    }

    // ── Monster Center ────────────────────────────────────────────────────────
    public string MonsterZentrumHeilen()
    {
        // KeinHeilen / KeinMonsterCenter Relikt
        if (Einstellungen.HatRelikt(ReliktTyp.KeinHeilen) || Einstellungen.HatRelikt(ReliktTyp.KeinMonsterCenter))
            return "❌ Relikt: Monster-Center ist gesperrt! Du kannst hier nicht heilen. ";

        // CenterLimit-Relikt: maximale Nutzungsanzahl prüfen
        int centerMax = -1;
        if (Einstellungen.HatRelikt(ReliktTyp.CenterLimit5))  centerMax = 5;
        else if (Einstellungen.HatRelikt(ReliktTyp.CenterLimit10)) centerMax = 10;
        else if (Einstellungen.HatRelikt(ReliktTyp.CenterLimit15)) centerMax = 15;
        else if (Einstellungen.HatRelikt(ReliktTyp.CenterLimit20)) centerMax = 20;

        if (centerMax >= 0 && Spieler.CenterNutzungen >= centerMax)
            return $"❌ Relikt: Monster-Center-Limit erreicht! Du hast das Center bereits {Spieler.CenterNutzungen}x benutzt (Limit: {centerMax})";

        if (!Spieler.Team.Any()) return "Dein Team ist leer.";
        // Letzten Center-Ort merken für Teleport
        LetztesCenterOrtId = Spieler.AktuellerOrt;
        foreach (var mon in Spieler.Team)
        {
            mon.AktuelleKp = mon.MaxKp;
            mon.Status = "none";
            mon.StatusZähler = 0;
            foreach (var atk in mon.Attacken) atk.AktuelleAp = atk.MaxAp;
        }

        // CenterLimit-Zähler erhöhen
        if (centerMax >= 0)
        {
            Spieler.CenterNutzungen++;
            int verbleibend = centerMax - Spieler.CenterNutzungen;
            Notify();
            return $"✅ Alle {Spieler.Team.Count} Monster wurden geheilt! (Center noch {verbleibend}x nutzbar)";
        }

        Notify();
        return $"✅ Alle {Spieler.Team.Count} Monster wurden vollständig geheilt!";
    }

    // ── Markt ─────────────────────────────────────────────────────────────────
    // ── NPC Gespräch ─────────────────────────────────────────────────────
    public (string dialog, string? itemName, string? itemEmoji) NpcSprechen(GesprächsNPC npc)
    {
        bool bereitsGesprochen = Spieler.BesproacheneNPCs.Contains(npc.Id);
        if (!bereitsGesprochen)
            Spieler.BesproacheneNPCs.Add(npc.Id);

        string? itemName = null;
        string? itemEmoji = null;

        // Item schenken (nur beim ersten Gespräch)
        if (!bereitsGesprochen && npc.GibtItemId != null && npc.GibtItemName != null)
        {
            Spieler.ItemHinzufügen(npc.GibtItemId, npc.GibtItemName, npc.GibtItemEmoji ?? "🎁");
            itemName = npc.GibtItemName;
            itemEmoji = npc.GibtItemEmoji;
            Notify();
        }

        // Professor gibt Karte nach Wizard-Abschluss (beim zweiten Gespräch)
        bool karteSchonErhalten = Spieler.Inventar.Any(i => i.ItemId == "ITEM-KARTE-GEN");
        if (npc.IstProfessor && WizardAbgeschlossen && bereitsGesprochen && !karteSchonErhalten)
        {
            Spieler.ItemHinzufügen("ITEM-KARTE-GEN", "Abenteuer-Karte", "🗺️");
            itemName = "Abenteuer-Karte";
            itemEmoji = "🗺️";
            Notify();
        }

        string dialog;
        if (npc.IstProfessor && WizardAbgeschlossen && bereitsGesprochen && itemName != null)
        {
            // Professor übergibt die Karte
            dialog = $"Ah, da bist du ja wieder! Hier, nimm diese Karte – sie zeigt dir alle Orte deiner Reise. " +
                     $"Viel Erfolg auf deinem Abenteuer! Dieses Spiel wurde von Phil Leitner mit viel Herzblut entwickelt. Ich drücke dir die Daumen! 🍀";
        }
        else if (npc.IstProfessor && WizardAbgeschlossen && karteSchonErhalten)
        {
            // Professor nach Karten-Übergabe – normaler Dialog
            dialog = $"Schön, dich zu sehen! Schau auf deine Karte, wenn du die Übersicht verlierst. Viel Erfolg!";
        }
        else if (bereitsGesprochen && npc.DialogNachGeschenk != null)
        {
            dialog = npc.DialogNachGeschenk;
        }
        else
        {
            dialog = npc.Dialog;
        }

        return (dialog, itemName, itemEmoji);
    }

    // ── Markt ─────────────────────────────────────────────────────
    // Legacy-Überladung für ShopItem (rückwärtskompatibel)
    public string ItemKaufen(ShopItem item)
    {
        int preis = Einstellungen.HatRelikt(ReliktTyp.DoppelteMarktpreise) ? item.Preis * 2 : item.Preis;
        if (Spieler.ZähneWallet.HatUpgrade(ZähneUpgrade.GünstigerMarkt)) preis = (int)(preis * 0.8f);
        if (Spieler.Geld < preis)
            return $"❌ Nicht genug Geld! Du brauchst {preis} Münzen, hast aber nur {Spieler.Geld}.";
        Spieler.Geld -= preis;
        Spieler.ItemHinzufügen(item.Id, item.Name, item.Emoji, item.Kategorie);
        Notify();
        return $"✅ {item.Emoji} {item.Name} gekauft! Verbleibendes Geld: {Spieler.Geld} Münzen.";
    }
    public string ItemKaufen(ItemDef item, int menge = 1)
    {
        int gesamtPreis = item.KaufPreis * menge;
        if (Einstellungen.HatRelikt(ReliktTyp.DoppelteMarktpreise)) gesamtPreis *= 2;
        if (Spieler.ZähneWallet.HatUpgrade(ZähneUpgrade.GünstigerMarkt)) gesamtPreis = (int)(gesamtPreis * 0.8f);
        if (Spieler.Geld < gesamtPreis)
            return $"❌ Nicht genug Geld! Du brauchst {gesamtPreis} Münzen, hast aber nur {Spieler.Geld}.";
        Spieler.Geld -= gesamtPreis;
        Spieler.ItemHinzufügen(item.Id, item.Name, item.Emoji, item.Kategorie, menge);
        Notify();
        return $"✅ {item.Emoji} {item.Name} ×{menge} gekauft! Verbleibendes Geld: {Spieler.Geld} Münzen.";
    }
    public string ItemVerkaufen(string itemId, int menge = 1)
    {
        var invItem = Spieler.GetItem(itemId);
        if (invItem == null || invItem.Menge < menge)
            return $"❌ Du hast nicht genug davon.";
        var def = GetItemDef(itemId);
        if (def == null) return "❌ Unbekanntes Item.";
        if (def.VerkaufPreis <= 0) return "❌ Dieses Item kann nicht verkauft werden.";
        int einnahmen = def.VerkaufPreis * menge;
        invItem.Menge -= menge;
        if (invItem.Menge <= 0) Spieler.Inventar.Remove(invItem);
        Spieler.Geld += einnahmen;
        Notify();
        return $"✅ {def.Emoji} {def.Name} ×{menge} verkauft! +{einnahmen} Münzen. Geld: {Spieler.Geld} Münzen.";
    }
    public string ItemAnwenden(string itemId, MonsterInstanz ziel)
    {
        var def = GetItemDef(itemId);
        if (def == null) return "❌ Unbekanntes Item.";
        var invItem = Spieler.GetItem(itemId);
        if (invItem == null || invItem.Menge <= 0) return "❌ Du hast dieses Item nicht.";
        var effekt = def.Effekt;
        string ergebnis = effekt.Typ switch
        {
            "HeilKP" => HeilKP(ziel, effekt.Wert),
            "HeilKPVoll" => HeilKP(ziel, ziel.MaxKp),
            "Beleben" => Beleben(ziel, effekt.Wert),
            "BelebenVoll" => Beleben(ziel, 100),
            "HeilAP" => HeilAP(ziel, effekt.Wert),
            "HeilAPVoll" => HeilAP(ziel, -1),
            "HeilStatus" => HeilStatus(ziel, effekt.StatusTyp),
            "HeilAlleStatus" => HeilAlleStatus(ziel),
            "HeilTeamAlles" => HeilTeamAlles(),
            "Entwicklungsstein" => SteinAnwenden(ziel, effekt.StatusTyp ?? ""),
            "Vitamin" => VitaminAnwenden(ziel, effekt.Stat ?? "", def.Name, def.Emoji),
            "StatBoost" => StatBoostAnwenden(ziel, effekt.Stat ?? "", effekt.Wert, def.Name, def.Emoji),
            "Repellent" => RepellentAnwenden(effekt.Wert, def.Name, def.Emoji),
            _ => $"❓ {def.Name} hat keinen direkten Effekt."
        };
        if (!ergebnis.StartsWith("❌"))
            Spieler.ItemVerwenden(itemId);
        Notify();
        return ergebnis;
    }
    private string HeilKP(MonsterInstanz m, int menge)
    {
        if (m.IstOhnmächtig) return $"❌ {m.AngezeigterName} ist ohnmächtig!";
        if (m.AktuelleKp >= m.MaxKp) return $"❌ {m.AngezeigterName} hat schon volle KP!";
        // BessereHeilung-Upgrade: +25% Heilung
        if (Spieler.ZähneWallet.HatUpgrade(ZähneUpgrade.BessereHeilung)) menge = (int)(menge * 1.25f);
        int vorher = m.AktuelleKp;
        m.AktuelleKp = Math.Min(m.MaxKp, m.AktuelleKp + menge);
        return $"✅ {m.AngezeigterName} hat {m.AktuelleKp - vorher} KP erhalten. ({m.AktuelleKp}/{m.MaxKp})";
    }
    private string Beleben(MonsterInstanz m, int prozent)
    {
        if (!m.IstOhnmächtig) return $"❌ {m.AngezeigterName} ist nicht ohnmächtig!";
        m.AktuelleKp = prozent >= 100 ? m.MaxKp : Math.Max(1, (int)(m.MaxKp * prozent / 100f));
        m.Status = "none";
        return $"✅ {m.AngezeigterName} wurde belebt! ({m.AktuelleKp}/{m.MaxKp} KP)";
    }
    private string HeilAP(MonsterInstanz m, int menge)
    {
        foreach (var a in m.Attacken)
        {
            if (menge < 0) a.AktuelleAp = a.MaxAp;
            else a.AktuelleAp = Math.Min(a.MaxAp, a.AktuelleAp + menge);
        }
        return $"✅ AP von {m.AngezeigterName} wiederhergestellt.";
    }
    private string HeilStatus(MonsterInstanz m, string? statusTyp)
    {
        if (m.Status == "none") return $"❌ {m.AngezeigterName} hat kein Statusproblem.";
        if (statusTyp != null)
        {
            bool passt = statusTyp.ToLower() switch
            {
                "vergiftet" => m.Status == "vergiftet",
                "verbrannt" => m.Status == "verbrannt",
                "eingefroren" => m.Status == "eingefroren",
                "gelähmt" => m.Status == "gelähmt",
                "schläft" => m.Status == "eingeschlafen",
                _ => true
            };
            if (!passt) return $"❌ {m.AngezeigterName} hat nicht dieses Statusproblem.";
        }
        string alterStatus = m.Status;
        m.Status = "none";
        m.StatusZähler = 0;
        return $"✅ {m.AngezeigterName} ist nicht mehr {alterStatus}.";
    }
    private string HeilAlleStatus(MonsterInstanz m)
    {
        m.Status = "none";
        m.StatusZähler = 0;
        return $"✅ Alle Statusprobleme von {m.AngezeigterName} geheilt.";
    }
    private string HeilTeamAlles()
    {
        foreach (var m in Spieler.Team)
        {
            if (!m.IstOhnmächtig) m.AktuelleKp = m.MaxKp;
            m.Status = "none";
            m.StatusZähler = 0;
            foreach (var a in m.Attacken) a.AktuelleAp = a.MaxAp;
        }
        return "✅ Das gesamte Team wurde vollständig geheilt!";
    }
    private string SteinAnwenden(MonsterInstanz mon, string steinTyp)
    {
        if (string.IsNullOrEmpty(mon.EntwickeltZu))
            return $"❌ {mon.AngezeigterName} kann sich nicht entwickeln.";
        // Prüfen ob der Stein zum Trigger passt
        var trigger = mon.EntwicklungTrigger ?? "";
        if (!trigger.Equals(steinTyp, StringComparison.OrdinalIgnoreCase))
        {
            var steinName = steinTyp switch {
                "mondstein" => "Mondstein", "feuerstein" => "Feuerstein", "wasserstein" => "Wasserstein",
                "blattstein" => "Blattstein", "donnerstein" => "Donnerstein", "sonnenstein" => "Sonnenstein",
                "glanstein" => "Glanstein", "finsterstein" => "Finsterstein", "eisstein" => "Eisstein",
                _ => steinTyp
            };
            return $"❌ {mon.AngezeigterName} reagiert nicht auf den {steinName}.";
        }
        // Relikt-Checks
        if (Einstellungen.HatRelikt(ReliktTyp.KeineEntwicklung))
            return $"🚫 Relikt: {mon.AngezeigterName} kann sich nicht entwickeln!";
        var neueSpezies = AlleMonster.FirstOrDefault(m => m.Id == mon.EntwickeltZu);
        if (neueSpezies == null) return $"❌ Entwicklungs-Daten nicht gefunden.";
        // Evolution durchführen
        string alteSpeziesId = mon.SpeziesId;
        mon.SpeziesId = neueSpezies.Id;
        mon.Name = neueSpezies.Name;
        mon.Typen = new List<string>(neueSpezies.Typen);
        mon.Bild = neueSpezies.Bild;
        mon.EntwickeltZu = neueSpezies.EntwickeltZu;
        mon.EntwicklungName = neueSpezies.EntwicklungName;
        mon.EntwicklungLevel = neueSpezies.EntwicklungLevel;
        mon.EntwicklungTrigger = neueSpezies.EntwicklungTrigger;
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
        // Pokédex aktualisieren
        Spieler.GefangeneMonster.Add(alteSpeziesId);
        Spieler.GefangeneMonster.Add(neueSpezies.Id);
        Spieler.GeseheneMonster.Add(alteSpeziesId);
        Spieler.GeseheneMonster.Add(neueSpezies.Id);
        // KeineEntwickeltenMonster-Relikt: in Box verschieben
        if (Einstellungen.HatRelikt(ReliktTyp.KeineEntwickeltenMonster))
        {
            Spieler.Team.Remove(mon);
            Spieler.Box.Add(mon);
        }
        Notify();
        return $"✨ {mon.AngezeigterName} hat sich zu {neueSpezies.Name} entwickelt!";
    }

    // ─── Neue Item-Hilfsmethoden ─────────────────────────────────────────────────

    /// <summary>Vitamin anwenden: erhöht Basis-Stat dauerhaft (+10 EV-Punkte, max 10x pro Stat)</summary>
    private string VitaminAnwenden(MonsterInstanz m, string stat, string name, string emoji)
    {
        if (m.IstOhnmächtig) return $"❌ {m.AngezeigterName} ist ohnmächtig!";
        if (string.IsNullOrEmpty(stat)) return $"❌ Unbekannter Stat.";
        if (!Spieler.VitaminZähler.ContainsKey(m.SpeziesId))
            Spieler.VitaminZähler[m.SpeziesId] = new Dictionary<string, int>();
        var zähler = Spieler.VitaminZähler[m.SpeziesId];
        if (!zähler.ContainsKey(stat)) zähler[stat] = 0;
        if (zähler[stat] >= 10) return $"❌ {m.AngezeigterName}'s {stat} kann nicht weiter gesteigert werden (max 10×)!";
        zähler[stat]++;
        // Stat dauerhaft erhöhen
        switch (stat)
        {
            case "KP":                 m.MaxKp += 4; m.AktuelleKp = Math.Min(m.AktuelleKp + 4, m.MaxKp); break;
            case "Angriff":            m.Angriff += 4; break;
            case "Verteidigung":       m.Verteidigung += 4; break;
            case "SpezialAngriff":     m.SpezialAngriff += 4; break;
            case "SpezialVerteidigung":m.SpezialVerteidigung += 4; break;
            case "Initiative":         m.Initiative += 4; break;
        }
        return $"✅ {emoji} {name} verwendet! {m.AngezeigterName}'s {stat} steigt dauerhaft! ({zähler[stat]}/10)";
    }

    /// <summary>StatBoost außerhalb Kampf: temporäre Stat-Erhöhung für nächsten Kampf (nicht implementiert als dauerhaft)</summary>
    private string StatBoostAnwenden(MonsterInstanz m, string stat, int wert, string name, string emoji)
    {
        if (m.IstOhnmächtig) return $"❌ {m.AngezeigterName} ist ohnmächtig!";
        switch (stat)
        {
            case "Angriff":            m.Angriff = Math.Min(999, m.Angriff + wert); break;
            case "Verteidigung":       m.Verteidigung = Math.Min(999, m.Verteidigung + wert); break;
            case "SpezialAngriff":     m.SpezialAngriff = Math.Min(999, m.SpezialAngriff + wert); break;
            case "SpezialVerteidigung":m.SpezialVerteidigung = Math.Min(999, m.SpezialVerteidigung + wert); break;
            case "Initiative":         m.Initiative = Math.Min(999, m.Initiative + wert); break;
            default: return $"❓ Unbekannter Stat: {stat}";
        }
        return $"✅ {emoji} {name} verwendet! {m.AngezeigterName}'s {stat} steigt um {wert}!";
    }

    /// <summary>Repellent anwenden: verhindert Wildkämpfe für X Schritte</summary>
    private string RepellentAnwenden(int schritte, string name, string emoji)
    {
        int s = schritte > 0 ? schritte : 100;
        Spieler.RepellentSchritte = Math.Max(Spieler.RepellentSchritte, s);
        return $"✅ {emoji} {name} verwendet! Wilde Monster meiden dich für {s} Schritte.";
    }

    /// <summary>Halteitem-Effekte pro Runde (LeechHeal, Giftstachel etc.)</summary>
    private void HalteItemRundenEffekt(MonsterInstanz mon, List<string> log)
    {
        if (mon.GetrageneItemId == null) return;
        var def = GetItemDef(mon.GetrageneItemId);
        if (def?.Effekt == null) return;
        switch (def.Effekt.Typ)
        {
            case "LeechHeal":
                if (!mon.IstOhnmächtig && mon.AktuelleKp < mon.MaxKp)
                {
                    int heilung = Math.Max(1, mon.MaxKp / 16);
                    mon.AktuelleKp = Math.Min(mon.MaxKp, mon.AktuelleKp + heilung);
                    log.Add($"🌿 {mon.AngezeigterName}'s {def.Emoji} {def.Name} stellt {heilung} KP wieder her!");
                }
                break;
            case "GiftHalte":
                if (!mon.IstOhnmächtig && (mon.Status == null || mon.Status == "none"))
                {
                    mon.Status = "vergiftet";
                    log.Add($"☠️ {mon.AngezeigterName} wurde durch {def.Emoji} {def.Name} vergiftet!");
                }
                break;
        }
    }

    /// <summary>TypVerstärker-Bonus für Halteitem (wird in SchadenBerechnen aufgerufen)</summary>
    public float GetHalteItemSchadenBonus(MonsterInstanz angreifer, string attackeTyp)
    {
        if (angreifer.GetrageneItemId == null) return 1f;
        var def = GetItemDef(angreifer.GetrageneItemId);
        if (def?.Effekt == null) return 1f;
        if (def.Effekt.Typ == "TypVerstärker" && def.Effekt.StatusTyp == attackeTyp)
            return 1.2f; // +20% Schaden für passenden Typ
        if (def.Effekt.Typ == "AllTypVerstärker")
            return 1.1f; // +10% für alle Typen
        return 1f;
    }

        public string ItemAusrüsten(MonsterInstanz monster, string? itemId)
    {
        if (itemId == null)
        {
            if (monster.GetrageneItemId == null) return "❌ Dieses Monster trägt kein Item.";
            var altDef = GetItemDef(monster.GetrageneItemId);
            monster.GetrageneItemId = null;
            Notify();
            return $"✅ {altDef?.Emoji ?? ""} {altDef?.Name ?? "Item"} wurde abgenommen.";
        }
        var def = GetItemDef(itemId);
        if (def == null) return "❌ Unbekanntes Item.";
        if (def.Kategorie != "Ausrüstung") return "❌ Nur Ausrüstungsitems können getragen werden.";
        var invItem = Spieler.GetItem(itemId);
        if (invItem == null || invItem.Menge <= 0) return "❌ Du hast dieses Item nicht.";
        if (monster.GetrageneItemId != null)
        {
            var altDef = GetItemDef(monster.GetrageneItemId);
            if (altDef != null)
                Spieler.ItemHinzufügen(altDef.Id, altDef.Name, altDef.Emoji, altDef.Kategorie);
        }
        Spieler.ItemVerwenden(itemId);
        monster.GetrageneItemId = itemId;
        Notify();
        return $"✅ {def.Emoji} {def.Name} wurde {monster.AngezeigterName} gegeben.";
    }

    // ── Spielstand speichern ──────────────────────────────────────────────────
    public bool ManuellSpeichernErlaubt => !Einstellungen.HatRelikt(ReliktTyp.KeinManuelesSpeichern);

    /// <summary>Wird vom Kampf-UI aufgerufen wenn das Zeitlimit abgelaufen ist.</summary>
    public async Task ZeitlimitAbgelaufen()
    {
        if (AktuellerKampf == null) return;
        AktuellerKampf.ZeitlimitAbgelaufen = true;
        AktuellerKampf.Log.Add("⏱️ Zeit abgelaufen! Du hast den Kampf verloren!");
        Notify();
        await Task.Delay(1000);
        KampfVerloren();
    }

    public async Task SpielstandSpeichern()
    {
        // GefangeneMonster aus aktuellem Team/Box synchronisieren (für ältere Spielstände)
        foreach (var m in Spieler.Team.Concat(Spieler.Box))
            Spieler.GefangeneMonster.Add(m.SpeziesId);

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
            BesproacheneNPCs = new List<string>(Spieler.BesproacheneNPCs),
            GeseheneMonster = Spieler.GeseheneMonster.ToList(),
            GefangeneMonster = Spieler.GefangeneMonster.ToList(),
            GeseheneAttacken = Spieler.GeseheneAttacken
                .ToDictionary(kv => kv.Key, kv => new List<string>(kv.Value)),
            CenterNutzungen = Spieler.CenterNutzungen,
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
                BesproacheneNPCs = save.BesproacheneNPCs ?? new(),
                Inventar = save.Inventar?.Select(i => new InventarItem
                {
                    ItemId = i.ItemId, Name = i.Name, Emoji = i.Emoji, Menge = i.Menge
                }).ToList() ?? new(),
                GeseheneMonster = save.GeseheneMonster != null
                    ? new HashSet<string>(save.GeseheneMonster) : new(),
                GefangeneMonster = save.GefangeneMonster != null
                    ? new HashSet<string>(save.GefangeneMonster) : new(),
                GeseheneAttacken = save.GeseheneAttacken != null
                    ? save.GeseheneAttacken.ToDictionary(kv => kv.Key, kv => kv.Value) : new(),
                CenterNutzungen = save.CenterNutzungen,
            };

            Spieler.Team.AddRange(save.Team?.Select(GespeichertesZuMonster) ?? []);
            Spieler.Box.AddRange(save.Box?.Select(GespeichertesZuMonster) ?? []);

            // Rückwärtskompatibilität: aktuelle Team/Box-Monster immer als gefangen markieren
            foreach (var m in Spieler.Team.Concat(Spieler.Box))
            {
                Spieler.GefangeneMonster.Add(m.SpeziesId);
                Spieler.GeseheneMonster.Add(m.SpeziesId);
            }

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

    // ── Eigene Map: Spielstand speichern / laden / löschen ────────────────────────────
    /// <summary>Speichert den aktuellen Spielstand in den Eigene-Map-Slot.</summary>
    public async Task EigeneMapSpielstandSpeichern()
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
            BesproacheneNPCs = new List<string>(Spieler.BesproacheneNPCs),
        };
        var json = JsonSerializer.Serialize(save);
        await _ls.SetItemAsync(LS_SAVEGAME_EIGENE_MAP, json);
        HatEigeneMapSpeicherstand = true;
        Notify();
    }

    /// <summary>Lädt den Eigene-Map-Spielstand und wechselt zur Weltkarte.</summary>
    public async Task EigeneMapSpielstandLaden()
    {
        var json = await _ls.GetItemAsync(LS_SAVEGAME_EIGENE_MAP);
        if (string.IsNullOrEmpty(json)) { Phase = SpielPhase.StarterWahl; Notify(); return; }
        try
        {
            var save = JsonSerializer.Deserialize<SpielstandDaten>(json);
            if (save == null) { Phase = SpielPhase.StarterWahl; Notify(); return; }
            Spieler = new Spieler
            {
                Name = save.SpielerName,
                Geld = save.Geld,
                AktuellerOrt = save.AktuellerOrt,
                Orden = save.Orden ?? new(),
                BesiegteTrainer = save.BesiegteTrainer ?? new(),
                BesproacheneNPCs = save.BesproacheneNPCs ?? new(),
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
        catch { Phase = SpielPhase.StarterWahl; Notify(); }
    }

    /// <summary>Löscht den Eigene-Map-Spielstand.</summary>
    public async Task EigeneMapSpielstandLöschen()
    {
        await _ls.RemoveItemAsync(LS_SAVEGAME_EIGENE_MAP);
        HatEigeneMapSpeicherstand = false;
        Notify();
    }

    /// <summary>Speichert im richtigen Slot je nach Modus (normal oder eigene Map).</summary>
    public async Task AktuellenSpielstandSpeichern()
    {
        if (IstEigeneMapModus)
            await EigeneMapSpielstandSpeichern();
        else
            await SpielstandSpeichern();
    }

    /// <summary>Gibt zurück ob Zähne/Boni im aktuellen Modus gelten.</summary>
    public bool ZähneAktiv => !IstEigeneMapModus;
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
    [JsonPropertyName("statuseffekt")] public string? Statuseffekt { get; set; }
    [JsonPropertyName("statuseffektChance")] public int? StatuseffektChance { get; set; }
    [JsonPropertyName("generation")] public int? Generation { get; set; }
}

public class MonsterRaw
{
    [JsonPropertyName("id")] public string Id { get; set; } = "";
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("typen")] public List<string>? Typen { get; set; }
    [JsonPropertyName("bild")] public string Bild { get; set; } = "";
    [JsonPropertyName("stats")] public Dictionary<string, int>? Stats { get; set; }
    [JsonPropertyName("attacken")] public List<AttackenLernEintragRaw>? Attacken { get; set; }
    [JsonPropertyName("tm_attacken")] public List<string>? TmAttacken { get; set; }
    [JsonPropertyName("entwickeltZu")] public string? EntwickeltZu { get; set; }
    [JsonPropertyName("entwicklungName")] public string? EntwicklungName { get; set; }
    [JsonPropertyName("entwicklungLevel")] public int? EntwicklungLevel { get; set; }
    [JsonPropertyName("fangrate")] public int Fangrate { get; set; } = 45;
}

public class AttackenLernEintragRaw
{
    [JsonPropertyName("AttackeId")] public string AttackeId { get; set; } = "";
    [JsonPropertyName("level")] public int Level { get; set; }
}