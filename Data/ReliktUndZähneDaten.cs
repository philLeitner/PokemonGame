using MonsterKampf.Models;

namespace MonsterKampf.Data;

// Positional record für ZähneUpgrade-Einträge
public record ZähneUpgradeInfo(ZähneUpgrade Typ, string Kategorie, string Emoji, string Name, string Beschreibung, int Kosten);

// ═══════════════════════════════════════════════════════════════════════════
// RELIKT-DATEN – alle verfügbaren Handicaps mit Zähne-Belohnung
// ═══════════════════════════════════════════════════════════════════════════
public static class ReliktDaten
{
    private static readonly List<ReliktInfo> Alle = new()
    {
        // ── Kampf-Handicaps ──────────────────────────────────────────────
        new(ReliktTyp.NurEinMonster,    "Kampf", "☠️", "Nur ein Monster",
            "Du darfst nur 1 Monster im Team haben.", 3),
        new(ReliktTyp.KeinHeilen,       "Kampf", "🚫", "Kein Heilen im Kampf",
            "Items dürfen im Kampf nicht verwendet werden.", 2),
        new(ReliktTyp.KeinFliehen,      "Kampf", "🔒", "Kein Fliehen",
            "Flucht aus Kämpfen ist nicht möglich.", 2),
        new(ReliktTyp.KeinFangen,       "Kampf", "🎯", "Kein Fangen",
            "Monster können nicht gefangen werden.", 2),
        new(ReliktTyp.NurEigenTyp,      "Kampf", "🎭", "Nur eigener Typ",
            "Nur Attacken des eigenen Typs dürfen eingesetzt werden.", 3),

        // ── Team-Handicaps ───────────────────────────────────────────────
        new(ReliktTyp.MaxZweiMonster,   "Team",  "👥", "Max. 2 Monster",
            "Du darfst maximal 2 Monster im Team haben.", 2),
        new(ReliktTyp.MaxDreiMonster,   "Team",  "👥", "Max. 3 Monster",
            "Du darfst maximal 3 Monster im Team haben.", 1),
        new(ReliktTyp.MaxVierMonster,   "Team",  "👥", "Max. 4 Monster",
            "Du darfst maximal 4 Monster im Team haben.", 1),
        new(ReliktTyp.MaxFünfMonster,   "Team",  "👥", "Max. 5 Monster",
            "Du darfst maximal 5 Monster im Team haben.", 1),
        new(ReliktTyp.KeineEntwicklung, "Team",  "🚷", "Keine Entwicklung",
            "Monster dürfen sich nicht entwickeln.", 3),

        // ── Welt-Handicaps ───────────────────────────────────────────────
        new(ReliktTyp.KeinMonsterCenter,"Welt",  "🏥", "Kein Monster Center",
            "Monster Center darf nicht benutzt werden.", 3),
        new(ReliktTyp.KeinMarkt,        "Welt",  "🏪", "Kein Markt",
            "Markt darf nicht benutzt werden.", 2),
        new(ReliktTyp.WenigerGeld,      "Welt",  "💸", "Weniger Geld",
            "Du erhältst nur 50% des normalen Geldes.", 1),
        new(ReliktTyp.HöhereLevel,      "Welt",  "📈", "Stärkere Gegner",
            "Alle Gegner sind 5 Level höher.", 2),

        // ── Nuzlocke-Varianten ───────────────────────────────────────────
        new(ReliktTyp.Nuzlocke,         "Nuzlocke", "💀", "Nuzlocke",
            "Ohnmächtige Monster sind permanent verloren.", 5),
        new(ReliktTyp.NuzlockeEinsFangen,"Nuzlocke","🎯", "Nuzlocke: Nur Erstes",
            "Pro Route darf nur das erste Monster gefangen werden.", 3),
        // ── Neue Kampf-Handicaps ───────────────────────────────────────
        new(ReliktTyp.ZufälligeAttacke,   "Kampf", "🎲", "Zufällige Attacke",
            "Dein Monster wählt jede Runde automatisch eine zufällige Attacke.", 4),
        new(ReliktTyp.KeineItemsImKampf,  "Kampf", "🚧", "Keine Items im Kampf",
            "Tränke, Beeren und alle anderen Items können im Kampf nicht benutzt werden.", 3),
        new(ReliktTyp.KeinMonsterWechsel, "Kampf", "🔒", "Kein Wechsel im Kampf",
            "Kein freiwilliger Monster-Wechsel während eines Kampfes möglich.", 2),
        new(ReliktTyp.DoppelterSchaden,   "Kampf", "💥", "Doppelter Schaden",
            "Du erleidest immer doppelten Schaden von Gegnern.", 5),
        new(ReliktTyp.ImmerErstangriff,   "Kampf", "⚡", "Immer Erstangriff",
            "Dein Monster greift immer zuerst an – Gegner-Statusattacken wirken aber sofort.", 2),
        // ── Neue Team-Handicaps ───────────────────────────────────────
        new(ReliktTyp.StarterOnly,         "Team",  "🔒", "Starter-Only",
            "Nur dein Starter darf im Team bleiben. Alle anderen müssen in die Box.", 4),
        new(ReliktTyp.KeineEntwickeltenMonster, "Team", "🐣", "Keine Entwicklungen",
            "Nur Basis-Stufe-Monster dürfen im Team sein. Entwicklungen müssen in die Box.", 3),
        new(ReliktTyp.ZufälligerStarter,   "Team",  "🎰", "Zufälliger Starter",
            "Dein Starter wird zufällig ausgewählt – keine manuelle Wahl möglich.", 2),
        // ── Neue Welt-Handicaps ───────────────────────────────────────
        new(ReliktTyp.KeinPokédex,         "Welt",  "📕", "Kein Pokédex",
            "Die Monsterübersicht ist gesperrt. Du siehst keine Infos zu Monstern.", 2),
        new(ReliktTyp.DoppelteMarktpreise, "Welt",  "💸", "Doppelte Marktpreise",
            "Alle Items im Markt kosten doppelt so viel.", 3),
        new(ReliktTyp.KeinManuelesSpeichern, "Welt", "🚷", "Kein manuelles Speichern",
            "Manuelles Speichern ist gesperrt. Nur Auto-Save nach Arena-Siegen.", 2),
        new(ReliktTyp.ZeitlimitProKampf,   "Kampf", "⏱️", "Zeitlimit pro Kampf",
            "Jeder Kampf hat ein Zeitlimit von 3 Minuten. Läuft die Zeit ab, gilt der Kampf als verloren.", 4),
        // Weitere Handicaps
        new(ReliktTyp.NurWildeMonster,     "Team",  "🌿", "Nur wilde Monster",
            "Monster können nur in Wildkampf-Begegnungen gefangen werden (Trainer-Monster nicht fangbar).", 2),
        new(ReliktTyp.ZufälligesTeam,      "Team",  "🎲", "Zufälliges Team",
            "Du startest mit 3 zufälligen Monstern statt nur dem Starter.", 3),
        new(ReliktTyp.KeinXpTeiler,        "XP",    "❌", "Kein XP-Teiler",
            "Der XP-Teiler ist deaktiviert. Nur das aktive Monster erhält Erfahrungspunkte.", 1),
    };

    public static IEnumerable<ReliktInfo> GetNachKategorie(string kategorie) =>
        Alle.Where(r => r.Kategorie == kategorie);

    public static IEnumerable<string> GetKategorien() =>
        Alle.Select(r => r.Kategorie).Distinct();

    public static ReliktInfo? Get(ReliktTyp typ) =>
        Alle.FirstOrDefault(r => r.Typ == typ);
}

// ═══════════════════════════════════════════════════════════════════════════
// ZÄHNE-UPGRADE-DATEN – alle kaufbaren Upgrades
// ═══════════════════════════════════════════════════════════════════════════
public static class ZähneUpgradeDaten
{
    private static readonly List<ZähneUpgradeInfo> Alle = new()
    {
        // ── Level ────────────────────────────────────────────────────────
        new(ZähneUpgrade.LevelBoost5,   "Level", "⬆️", "Level +5",
            "Alle Monster im Team erhalten +5 Level.", 3),
        new(ZähneUpgrade.LevelBoost10,  "Level", "⬆️⬆️", "Level +10",
            "Alle Monster im Team erhalten +10 Level.", 6),
        new(ZähneUpgrade.LevelBoost20,  "Level", "🚀", "Level +20",
            "Alle Monster im Team erhalten +20 Level.", 10),

        // ── Stats ────────────────────────────────────────────────────────
        new(ZähneUpgrade.KpBoost,       "Stats", "❤️", "KP +10%",
            "Alle Monster erhalten dauerhaft +10% KP.", 3),
        new(ZähneUpgrade.AngriffBoost,  "Stats", "⚔️", "Angriff +10%",
            "Alle Monster erhalten dauerhaft +10% Angriff.", 3),
        new(ZähneUpgrade.VertBoost,     "Stats", "🛡️", "Verteidigung +10%",
            "Alle Monster erhalten dauerhaft +10% Verteidigung.", 3),
        new(ZähneUpgrade.SpeedBoost,    "Stats", "💨", "Speed +10%",
            "Alle Monster erhalten dauerhaft +10% Speed.", 3),
        new(ZähneUpgrade.AlleStatsBoost,"Stats", "💎", "Alle Stats +10%",
            "Alle Monster erhalten dauerhaft +10% auf alle Stats.", 8),

        // ── XP & Geld ────────────────────────────────────────────────────
        new(ZähneUpgrade.XpBoost,       "XP & Geld", "✨", "XP +50%",
            "Alle Monster erhalten 50% mehr Erfahrungspunkte.", 4),
        new(ZähneUpgrade.XpTeiler,      "XP & Geld", "🔀", "XP-Teiler",
            "Alle Monster im Team erhalten XP, nicht nur das aktive.", 5),
        new(ZähneUpgrade.GeldBoost,     "XP & Geld", "💰", "Geld +50%",
            "Du erhältst 50% mehr Geld nach Kämpfen.", 3),
        new(ZähneUpgrade.GünstigerMarkt,"XP & Geld", "🏷️", "Günstigerer Markt",
            "Alle Items im Markt kosten 30% weniger.", 4),

        // ── Fangen ───────────────────────────────────────────────────────
        new(ZähneUpgrade.BessereKugeln, "Fangen", "🎯", "Bessere Fangkugeln",
            "Alle Fangkugeln haben 20% höhere Fangrate.", 3),
        new(ZähneUpgrade.MehrTeamSlots, "Fangen", "➕", "Mehr Team-Slots",
            "Dein Team kann 2 zusätzliche Monster aufnehmen (max. 8).", 5),

        // ── Bonus-Upgrades ────────────────────────────────────────────────
        new(ZähneUpgrade.BonusStatUpgrade, "Bonus", "🎁", "Bonus-Stat-Upgrade",
            "Wähle einen Stat eines Monsters und erhöhe ihn dauerhaft.", 2),
    };

    public static IEnumerable<ZähneUpgradeInfo> GetNachKategorie(string kategorie) =>
        Alle.Where(u => u.Kategorie == kategorie);

    public static IEnumerable<string> GetKategorien() =>
        Alle.Select(u => u.Kategorie).Distinct();

    public static ZähneUpgradeInfo? Get(ZähneUpgrade typ) =>
        Alle.FirstOrDefault(u => u.Typ == typ);
}
