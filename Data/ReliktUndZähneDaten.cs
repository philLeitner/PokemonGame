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
            "Nur dein Starter darf im Team bleiben. Pro Region erhältst du 1 neuen Starter. Alle anderen Monster müssen in die Box.", 4),
        new(ReliktTyp.KeineEntwickeltenMonster, "Team", "🐣", "Keine Entwicklungen",
            "Nur Basis-Stufe-Monster dürfen im Team sein. Entwicklungen müssen in die Box.", 3),
        // ── Neue Welt-Handicaps ───────────────────────────────────────
        new(ReliktTyp.KeinPokédex,         "Welt",  "📕", "Kein Pokédex",
            "Die Monsterübersicht ist gesperrt. Du siehst keine Infos zu Monstern.", 2),
        new(ReliktTyp.DoppelteMarktpreise, "Welt",  "💸", "Doppelte Marktpreise",
            "Alle Items im Markt kosten doppelt so viel.", 3),
        new(ReliktTyp.KeinManuelesSpeichern, "Welt", "🚷", "Kein manuelles Speichern",
            "Manuelles Speichern ist gesperrt. Nur Auto-Save nach Arena-Siegen.", 2),
        new(ReliktTyp.ZeitlimitProKampf,   "Kampf", "⏱️", "Zeitlimit: 3 Minuten",
            "Jeder Kampf hat ein Zeitlimit von 3 Minuten. Läuft die Zeit ab, gilt der Kampf als verloren.", 4),
        new(ReliktTyp.ZeitlimitProKampf2Min, "Kampf", "⏱️", "Zeitlimit: 2 Minuten",
            "Jeder Kampf hat ein Zeitlimit von 2 Minuten. Läuft die Zeit ab, gilt der Kampf als verloren.", 5),
        new(ReliktTyp.ZeitlimitProKampf1Min, "Kampf", "⏱️", "Zeitlimit: 1 Minute",
            "Jeder Kampf hat ein Zeitlimit von 1 Minute. Läuft die Zeit ab, gilt der Kampf als verloren.", 7),
        // ── Center-Limit-Handicaps ────────────────────────────────────
        new(ReliktTyp.CenterLimit20,    "Welt",  "🏥", "Center-Limit: 20 Nutzungen",
            "Das Monster Center darf insgesamt nur 20 Mal benutzt werden.", 2),
        new(ReliktTyp.CenterLimit15,    "Welt",  "🏥", "Center-Limit: 15 Nutzungen",
            "Das Monster Center darf insgesamt nur 15 Mal benutzt werden.", 3),
        new(ReliktTyp.CenterLimit10,    "Welt",  "🏥", "Center-Limit: 10 Nutzungen",
            "Das Monster Center darf insgesamt nur 10 Mal benutzt werden.", 4),
        new(ReliktTyp.CenterLimit5,     "Welt",  "🏥", "Center-Limit: 5 Nutzungen",
            "Das Monster Center darf insgesamt nur 5 Mal benutzt werden.", 6),
        new(ReliktTyp.KeinXpTeiler,        "XP",    "❌", "Kein XP-Teiler",
            "Eingewechselte Monster erhalten keine XP. Nur das Monster, das den Gegner besiegt hat, erhält die vollen Erfahrungspunkte.", 1),
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
        new(ZähneUpgrade.LevelBoost5,   "Level", "⬆️", "Level +5 (Team)",
            "Alle Monster im Team erhalten +5 Level.", 3),
        new(ZähneUpgrade.LevelBoost10,  "Level", "⬆️⬆️", "Level +10 (Team)",
            "Alle Monster im Team erhalten +10 Level.", 6),
        new(ZähneUpgrade.LevelBoost20,  "Level", "🚀", "Level +20 (Team)",
            "Alle Monster im Team erhalten +20 Level.", 10),
        new(ZähneUpgrade.LevelBoost25,  "Level", "⚡", "Level +25 (Starter)",
            "Dein erstes Monster erhält +25 Level.", 5),
        new(ZähneUpgrade.LevelBoost50,  "Level", "💥", "Level +50 (Starter)",
            "Dein erstes Monster erhält +50 Level.", 10),

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
        new(ZähneUpgrade.MehrXp,        "XP & Geld", "💫", "XP +25%",
            "Alle Monster erhalten 25% mehr Erfahrungspunkte.", 2),
        new(ZähneUpgrade.XpTeiler,      "XP & Geld", "🔀", "XP-Teiler",
            "Alle Monster im Team erhalten XP, nicht nur das aktive.", 5),
        new(ZähneUpgrade.VollerXpTeiler,"XP & Geld", "🔁", "Voller XP-Teiler",
            "Alle Monster im Team erhalten die vollen XP nach jedem Kampf.", 8),
        new(ZähneUpgrade.GeldBoost,     "XP & Geld", "💰", "Geld +50%",
            "Du erhältst 50% mehr Geld nach Kämpfen.", 3),
        new(ZähneUpgrade.MehrGeld,      "XP & Geld", "💵", "Geld +25%",
            "Du erhältst 25% mehr Geld nach Kämpfen.", 2),
        new(ZähneUpgrade.GünstigerMarkt,"XP & Geld", "🏷️", "Günstigerer Markt",
            "Alle Items im Markt kosten 20% weniger.", 4),
        new(ZähneUpgrade.BessereHeilung,"XP & Geld", "💊", "Bessere Heilung",
            "Tränke heilen 25% mehr KP.", 3),

        // ── Fangen ────────────────────────────────────────────────────────
        new(ZähneUpgrade.BessereBallle, "Fangen", "⚪→🔵", "Besserer Monsterball",
            "Monsterball wirkt wie ein Superball.", 2),
        new(ZähneUpgrade.ProfiCatcher,  "Fangen", "🔵→🟡", "Profi-Catcher",
            "Superball wirkt wie ein Hyperball.", 3),
        new(ZähneUpgrade.LegendärBoost10, "Fangen", "⭐", "Legendär-Boost",
            "Legendäre Monster sind 2x leichter zu fangen.", 3),
        new(ZähneUpgrade.LegendärBoost20, "Fangen", "🌟", "Legendär-Boost+",
            "Legendäre Monster sind 3x leichter zu fangen.", 5),
        new(ZähneUpgrade.Meisterball,   "Fangen", "🟣", "1 Meisterball",
            "Du erhältst einen Meisterball, der garantiert fängt.", 5),
        new(ZähneUpgrade.BessereKugeln, "Fangen", "🎯", "Bessere Fangkugeln",
            "Alle Fangkugeln haben +2 Fangkraft.", 3),
        new(ZähneUpgrade.MehrTeamSlots, "Fangen", "➕➕", "Mehr Team-Slots (+2)",
            "Dein Team kann 2 zusätzliche Monster aufnehmen (max. 8).", 5),
        new(ZähneUpgrade.ExtraSlot,     "Fangen", "➕", "Extra Team-Slot (+1)",
            "Dein Team kann 1 zusätzliches Monster aufnehmen.", 3),

        // ── Bonus-Upgrades ──────────────────────────────────────────────────
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
