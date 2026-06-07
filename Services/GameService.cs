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

    // ── Regionen / Prozedurale Karte ─────────────────────────────────────────
    public List<RegionConfig> AlleRegionen { get; private set; } = new();
    public GenerierteKarte? AktuelleGenerierteKarte { get; private set; } = null;
    public bool IstGenerierteKartenModus => AktuelleGenerierteKarte != null;
    // Nach-Arenaleiter-Dialog-Daten
    public Ort? LetzterArenaLeiter { get; private set; } = null;
    public bool NachArenaLeiterLevelA { get; private set; } = true;

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
                    Statuseffekt = a.Statuseffekt,
                    StatuseffektChance = a.StatuseffektChance,
                    Generation = a.Generation,
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
                    TmAttacken = m.TmAttacken ?? new(),
                    EntwickeltZu = m.EntwickeltZu,
                    EntwicklungName = m.EntwicklungName,
                    EntwicklungLevel = m.EntwicklungLevel,
                    Fangrate = m.Fangrate > 0 ? m.Fangrate : 45,
                }).ToList();
            }

            // Regionen laden (enthält TrainerPool + MonsterPool)
            LadeStatus = "Lade Regionen...";
            Notify();
            var regOpts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var regionen = await _http.GetFromJsonAsync<List<RegionConfig>>("data/regionen.json", regOpts);
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
    public void ZuPokédex() { Phase = SpielPhase.Pokédex; Notify(); }
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
        Spieler.ItemHinzufügen("monsterball", "Monsterball", "⚪", 5);
        Spieler.ItemHinzufügen("trank", "Trank", "🧪", 3);
        // Neue Karte mit neuem Seed generieren
        var neuerSeed = KartenGenerator.GeneriereSeedCode();
        var (neueOrte, meta) = KartenGenerator.Generiere(neuerSeed, regionsReihenfolge, AlleRegionen, _originalOrte);
        AktuelleGenerierteKarte = meta;
        AlleOrte = neueOrte;
        if (!string.IsNullOrEmpty(meta.StartOrtId))
            Spieler.AktuellerOrt = meta.StartOrtId;
        Phase = SpielPhase.Weltkarte;
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
        Spieler.ItemHinzufügen("monsterball", "Monsterball", "⚪", 5);
        Spieler.ItemHinzufügen("trank", "Trank", "🧪", 3);
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
        if (!IstGenerierteKartenModus || !NachArenaLeiterLevelA) return basisLevel;
        var spielerLevel = Spieler.Team
            .Where(m => !m.IstOhnmächtig)
            .Select(m => m.Level)
            .DefaultIfEmpty(5)
            .Max();
        return basisLevel + Math.Max(0, spielerLevel - 5);
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

    // GenerierteArenaKampfStarten wird nicht mehr benötigt –
    // der normale ArenaKampfStarten in Weltkarte.razor übernimmt das.

    /// <summary>Wird nach Arenaleiter-Sieg aufgerufen: Orden vergeben, Fog-of-War freischalten, Dialog starten.</summary>
    public void GenerierteArenaGewonnen(Ort arenaOrt)
    {
        if (AktuelleGenerierteKarte == null) return;
        LetzterArenaLeiter = arenaOrt;
        AktuelleGenerierteKarte.BesiegteArenen.Add(arenaOrt.Id);
        // Fog-of-War: sofort freischalten (Orden wird durch KampfBeendet vergeben,
        // zusätzlich beim Betreten der Arena-Stadt erneut geprüft)
        FogOfWarFreischaltenNachOrt(arenaOrt.Id);
        // Nach-Arenaleiter-Dialog
        Phase = SpielPhase.NachArenaLeiter;
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
    public void NachArenaLeiterAbschliessen()
    {
        LetzterArenaLeiter = null;
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
        Spieler.StarterMonsterId = monsterId; // Starter merken für Rivale-Blau-Konter
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

            // Boss-Zugang: braucht N Arenen besiegt UND alle vorherigen Bosse
            if (ziel.Arena != null && karte.BossIds.Contains(zielOrtId))
            {
                int bossNr = karte.BossIds.IndexOf(zielOrtId); // 0-basiert
                int benötigteArenen = (bossNr + 1) * karte.StädteProBoss;
                int besiegteArenen = karte.BesiegteArenen.Count;
                // Vorherige Bosse müssen alle besiegt sein
                int vorherigeBosseBesiegt = karte.BossIds.Take(bossNr).Count(b => karte.BesiegteArenen.Contains(b));
                if (vorherigeBosseBesiegt < bossNr)
                    return $"⛔ Gesperrt! Zuerst Boss {vorherigeBosseBesiegt + 1} besiegen.";
                if (besiegteArenen < benötigteArenen)
                    return $"⛔ Gesperrt! Du brauchst {benötigteArenen} Arenen besiegt – du hast {besiegteArenen}.";
            }

            // Arenen-Reihenfolge: Arena N ist gesperrt wenn Arena N-1 noch nicht besiegt
            if (ziel.Arena != null && karte.StadtIds.Contains(zielOrtId) && !karte.BossIds.Contains(zielOrtId))
            {
                int arenaIdx = karte.StadtIds.IndexOf(zielOrtId); // 0-basiert
                if (arenaIdx > 0)
                {
                    string vorherige = karte.StadtIds[arenaIdx - 1];
                    if (!karte.BesiegteArenen.Contains(vorherige))
                    {
                        var vorOrt = AlleOrte.FirstOrDefault(o => o.Id == vorherige);
                        string vorName = vorOrt?.Name ?? $"Arena {arenaIdx}";
                        return $"⛔ Gesperrt! Zuerst {vorName} besiegen.";
                    }
                }
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
            var namen = string.Join(", ", mussTrainer.Select(t => $"{t.Klasse} {t.Name}"));
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

    // Rivale Blau: Starter-Konter bestimmen (Pflanze→Feuer, Feuer→Wasser, Wasser→Pflanze)
    private string RivaleBlauStarterKonter()
    {
        return Spieler.StarterMonsterId switch
        {
            "PKM-0001" => "PKM-0004", // Spieler hat Bisasam (Pflanze) → Blau nimmt Glumanda (Feuer)
            "PKM-0004" => "PKM-0007", // Spieler hat Glumanda (Feuer) → Blau nimmt Schiggy (Wasser)
            "PKM-0007" => "PKM-0001", // Spieler hat Schiggy (Wasser) → Blau nimmt Bisasam (Pflanze)
            _ => "PKM-0004"           // Fallback
        };
    }

    public void TrainerKampfStarten(TrainerKampf trainer)
    {
        if (Spieler.BesiegteTrainer.Contains(trainer.Id)) return;
        var spielerMonster = Spieler.AktivesMonster;
        if (spielerMonster == null) return;

        var erstesGegnerMonster = trainer.Team.FirstOrDefault();
        if (erstesGegnerMonster == null) return;

        // Rivale Blau: nur den Starter-Konter verwenden (1 Monster, nicht alle 3)
        TrainerKampf effektiverTrainer = trainer;
        if (trainer.Name == "Blau" && trainer.Klasse == "Rivale" && !string.IsNullOrEmpty(Spieler.StarterMonsterId))
        {
            var konterId = RivaleBlauStarterKonter();
            // Erstelle eine Kopie des Trainers mit nur dem Konter-Monster
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

        var ersteMonsterId = effektiverTrainer.Team[0].MonsterId;
        var spezies = AlleMonster.FirstOrDefault(m => m.Id == ersteMonsterId)
                      ?? AlleMonster[_rng.Next(AlleMonster.Count)];
        // Level kommt direkt aus Team-Eintrag (im generierten Modus bereits vom Generator gesetzt)
        var gegner = MonsterInstanz.VonSpezies(spezies, effektiverTrainer.Team[0].Level, AlleAttacken);

        AktuellerKampf = new KampfZustand
        {
            Typ = KampfTyp.Trainer,
            SpielerMonster = spielerMonster,
            GegnerMonster = gegner,
            GegnerName = $"{effektiverTrainer.Klasse} {effektiverTrainer.Name}",
            Phase = KampfPhase.Intro,
            Log = new() { $"💬 {effektiverTrainer.Klasse} {effektiverTrainer.Name}: \"{effektiverTrainer.Dialogvor}\"" },
            TrainerId = effektiverTrainer.Id,
            BelohnungGeld = effektiverTrainer.Belohnung,
            AktuellerTrainer = effektiverTrainer,
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
        // Level kommt direkt aus Arena-Team (im generierten Modus bereits vom Generator gesetzt)
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
            case "fluchtseil":
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
                break;
            case "sprudelwasser":
                AktuellerKampf.Log.Add("❌ Sprudelwasser kann hier nicht verwendet werden!");
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
        var item = Spieler.GetItem(ballId);
        if (item == null || item.Menge <= 0) { AktuellerKampf.Log.Add("❌ Kein Ball mehr!"); Notify(); return; }
        Spieler.ItemVerwenden(ballId);
        AktuellerKampf.Phase = KampfPhase.Fangen;
        var gegner = AktuellerKampf.GegnerMonster;
        string ballEmoji = ballId switch { "superball" => "🔵", "hyperball" => "🟡", "meisterball" => "🟣", _ => "⚪" };
        string ballName  = ballId switch { "superball" => "Superball", "hyperball" => "Hyperball", "meisterball" => "Meisterball", _ => "Monsterball" };
        AktuellerKampf.Log.Add($"{ballEmoji} Du wirfst einen {ballName} nach {gegner.Name}...");
        Notify();
        await Task.Delay(1200);
        // ─ Widerstand berechnen ─
        int widerstand = FangWiderstand(gegner.Fangrate);
        // ─ Ball-Kraft ─
        int ballKraft = ballId switch { "superball" => 2, "hyperball" => 4, "meisterball" => 99, _ => 1 };
        // ─ HP-Schwäche-Bonus ─
        float hpProzent = gegner.MaxKp > 0 ? (float)gegner.AktuelleKp / gegner.MaxKp : 1f;
        int hpBonus = hpProzent < 0.10f ? 3 : hpProzent < 0.25f ? 2 : hpProzent < 0.50f ? 1 : 0;
        // ─ Status-Bonus ─
        int statusBonus = gegner.Status is "eingeschlafen" or "eingefroren" ? 3
                        : gegner.Status is "vergiftet" or "gelähmt" or "verbrannt" ? 1 : 0;
        // ─ Gesamtkraft ─
        int gesamtKraft = ballKraft + hpBonus + statusBonus;
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
        AktuellerKampf.ErfahrungGewonnen = exp;
        spielerMon.ErfahrungsPunkte += exp;
        AktuellerKampf.Log.Add($"+{exp} EP für {spielerMon.AngezeigterName}");

                // Level-Up prüfen
        PrüfeLevelUp(spielerMon);
        // Neue Attacken-Dialog starten falls vorhanden
        if (AktuellerKampf.PendingNeueAttacken.Count > 0)
        {
            AktuellerKampf.LernMonster = spielerMon;
            AktuellerKampf.NeueAttacke = AktuellerKampf.PendingNeueAttacken[0];
        }
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
                // Level kommt direkt aus Team-Eintrag (im generierten Modus bereits vom Generator gesetzt)
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
            AktuellerKampf.NeueAttacke = AktuellerKampf.PendingNeueAttacken[0];
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
                        AktuellerKampf.PendingNeueAttacken.Add(neueAtk);
                }
            }

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

    private void VersuchemStatusEffektDirekt(MonsterInstanz ziel, string neuerStatus, List<string> log)
    {
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
            "verwirrt"      => $"😵 {ziel.AngezeigterName} ist verwirrt!",
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
            BesproacheneNPCs = new List<string>(Spieler.BesproacheneNPCs),
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
