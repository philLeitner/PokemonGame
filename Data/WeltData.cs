using MonsterKampf.Models;

namespace MonsterKampf.Data;

/// <summary>
/// Alle 468 Orte aus 9 Regionen – generiert aus pokemon_game_data.json v3
/// </summary>
public static class WeltData
{
    // Farb-Mapping fuer Ort-Typen
    public static readonly Dictionary<string, string> OrtFarben = new()
    {
        { "blue", "#2563eb" },
        { "green", "#16a34a" },
        { "red", "#dc2626" },
        { "yellow", "#ca8a04" },
        { "purple", "#9333ea" },
        { "orange", "#ea580c" },
        { "gray", "#4b5563" },
        { "teal", "#0d9488" },
        { "pink", "#db2777" },
        { "indigo", "#4f46e5" },
        { "cyan", "#0891b2" },
        { "lime", "#65a30d" },
        { "brown", "#78350f" },
        { "slate", "#475569" },
    };

    private static List<ShopItem> StandardMarkt() => new()
    {
        new() { Id="item-001", Name="Heiltrank",   Beschreibung="Heilt 20 KP.",   Preis=300,  Emoji="🧪", Kategorie="Heilung" },
        new() { Id="item-002", Name="Supertrank",  Beschreibung="Heilt 50 KP.",   Preis=700,  Emoji="💊", Kategorie="Heilung" },
        new() { Id="item-003", Name="Hypertrank",  Beschreibung="Heilt 200 KP.",  Preis=1200, Emoji="💉", Kategorie="Heilung" },
        new() { Id="item-004", Name="Monsterball", Beschreibung="Faengt Monster.", Preis=200,  Emoji="⚪", Kategorie="Fangen" },
        new() { Id="item-005", Name="Superball",   Beschreibung="Bessere Chance.", Preis=600,  Emoji="🔵", Kategorie="Fangen" },
        new() { Id="item-006", Name="Hyperball",   Beschreibung="Hohe Chance.",    Preis=1200, Emoji="🟡", Kategorie="Fangen" },
        new() { Id="item-007", Name="Beleber",     Beschreibung="Belebt Monster.", Preis=1500, Emoji="💫", Kategorie="Heilung" },
        new() { Id="item-008", Name="Antidot",     Beschreibung="Heilt Gift.",     Preis=100,  Emoji="🟢", Kategorie="Status" },
    };

    // Globale Item-Datenbank (alle bekannten Items)
    public static List<ShopItem> AlleItems() => new()
    {
        // Heilung
        new() { Id="item-001", Name="Heiltrank",      Beschreibung="Heilt 20 KP.",           Preis=300,   Emoji="🧪", Kategorie="Heilung",  WoBekommt="Alle Märkte" },
        new() { Id="item-002", Name="Supertrank",     Beschreibung="Heilt 50 KP.",           Preis=700,   Emoji="💊", Kategorie="Heilung",  WoBekommt="Märkte ab Arena 3" },
        new() { Id="item-003", Name="Hypertrank",     Beschreibung="Heilt 200 KP.",          Preis=1200,  Emoji="💉", Kategorie="Heilung",  WoBekommt="Märkte ab Arena 5" },
        new() { Id="item-007", Name="Beleber",        Beschreibung="Belebt ohnm. Monster.",  Preis=1500,  Emoji="💫", Kategorie="Heilung",  WoBekommt="Märkte ab Arena 4" },
        new() { Id="item-009", Name="Maxbeleber",     Beschreibung="Belebt mit vollen KP.",  Preis=4000,  Emoji="⭐", Kategorie="Heilung",  WoBekommt="Märkte ab Arena 7" },
        new() { Id="item-010", Name="Maxheiler",      Beschreibung="Heilt alle KP.",         Preis=2500,  Emoji="💥", Kategorie="Heilung",  WoBekommt="Märkte ab Arena 6" },
        // Fangen
        new() { Id="item-004", Name="Monsterball",    Beschreibung="Fängt Monster.",         Preis=200,   Emoji="⚪", Kategorie="Fangen",   WoBekommt="Alle Märkte" },
        new() { Id="item-005", Name="Superball",      Beschreibung="Bessere Fangchance.",    Preis=600,   Emoji="🔵", Kategorie="Fangen",   WoBekommt="Märkte ab Arena 2" },
        new() { Id="item-006", Name="Hyperball",      Beschreibung="Hohe Fangchance.",       Preis=1200,  Emoji="🟡", Kategorie="Fangen",   WoBekommt="Märkte ab Arena 5" },
        new() { Id="item-011", Name="Meisterball",    Beschreibung="Fängt garantiert.",     Preis=0,     Emoji="🟣", Kategorie="Fangen",   WoBekommt="Einmalig – Silphania" },
        // Status
        new() { Id="item-008", Name="Antidot",        Beschreibung="Heilt Gift.",            Preis=100,   Emoji="🟢", Kategorie="Status",   WoBekommt="Alle Märkte" },
        new() { Id="item-012", Name="Auftauen",       Beschreibung="Heilt Einfrieren.",      Preis=250,   Emoji="🔥", Kategorie="Status",   WoBekommt="Märkte ab Arena 3" },
        new() { Id="item-013", Name="Paralyheiler",   Beschreibung="Heilt Lähmung.",        Preis=200,   Emoji="⚡", Kategorie="Status",   WoBekommt="Märkte ab Arena 2" },
        new() { Id="item-014", Name="Schlafheiler",   Beschreibung="Weckt Monster auf.",    Preis=250,   Emoji="👋", Kategorie="Status",   WoBekommt="Märkte ab Arena 2" },
        new() { Id="item-015", Name="Vollheiler",     Beschreibung="Heilt alle Status.",    Preis=600,   Emoji="✨", Kategorie="Status",   WoBekommt="Märkte ab Arena 5" },
        // Kampf-Items
        new() { Id="item-016", Name="X-Angriff",      Beschreibung="Erhöht Angriff.",       Preis=500,   Emoji="🗡️", Kategorie="Kampf",    WoBekommt="Märkte ab Arena 4" },
        new() { Id="item-017", Name="X-Verteidigung", Beschreibung="Erhöht Verteidigung.",  Preis=550,   Emoji="🛡️", Kategorie="Kampf",    WoBekommt="Märkte ab Arena 4" },
        new() { Id="item-018", Name="X-Initiative",   Beschreibung="Erhöht Initiative.",   Preis=350,   Emoji="💨", Kategorie="Kampf",    WoBekommt="Märkte ab Arena 3" },
        // Entwicklungs-Steine
        new() { Id="stein-mond",    Name="Mondstein",    Beschreibung="Löst Entwicklung bei bestimmten Monstern aus.",  Preis=2000, Emoji="🌙", Kategorie="Stein", WoBekommt="Märkte ab Arena 4" },
        new() { Id="stein-feuer",   Name="Feuerstein",   Beschreibung="Löst Entwicklung bei bestimmten Monstern aus.",  Preis=2000, Emoji="🔥", Kategorie="Stein", WoBekommt="Märkte ab Arena 4" },
        new() { Id="stein-wasser",  Name="Wasserstein",  Beschreibung="Löst Entwicklung bei bestimmten Monstern aus.",  Preis=2000, Emoji="💧", Kategorie="Stein", WoBekommt="Märkte ab Arena 4" },
        new() { Id="stein-blatt",   Name="Blattstein",   Beschreibung="Löst Entwicklung bei bestimmten Monstern aus.",  Preis=2000, Emoji="🍃", Kategorie="Stein", WoBekommt="Märkte ab Arena 4" },
        new() { Id="stein-donner",  Name="Donnerstein",  Beschreibung="Löst Entwicklung bei bestimmten Monstern aus.",  Preis=2000, Emoji="⚡", Kategorie="Stein", WoBekommt="Märkte ab Arena 4" },
        new() { Id="stein-sonne",   Name="Sonnenstein",  Beschreibung="Löst Entwicklung bei bestimmten Monstern aus.",  Preis=2500, Emoji="☀️", Kategorie="Stein", WoBekommt="Märkte ab Arena 5" },
        new() { Id="stein-glan",    Name="Glanstein",    Beschreibung="Löst Entwicklung bei bestimmten Monstern aus.",  Preis=2500, Emoji="✨", Kategorie="Stein", WoBekommt="Märkte ab Arena 5" },
        new() { Id="stein-finster", Name="Finsterstein", Beschreibung="Löst Entwicklung bei bestimmten Monstern aus.",  Preis=2500, Emoji="🌑", Kategorie="Stein", WoBekommt="Märkte ab Arena 5" },
        new() { Id="stein-eis",     Name="Eisstein",     Beschreibung="Löst Entwicklung bei bestimmten Monstern aus.",  Preis=2500, Emoji="❄️", Kategorie="Stein", WoBekommt="Märkte ab Arena 5" },
        // Spezial-Items (nicht kaufbar)
        new() { Id="ITEM-KARTE",   Name="Kanto-Karte",   Beschreibung="Zeigt die Kanto-Region.",   Preis=0, Emoji="🗺️", Kategorie="Spezial", WoBekommt="Prof. Eich in Pallet Town" },
        new() { Id="ITEM-SPRUDEL", Name="Sprudelwasser", Beschreibung="Schlüssel für Saffronia.", Preis=0, Emoji="💧", Kategorie="Spezial", WoBekommt="Direktor Hideo in Prismania" },
    };

    public static List<Ort> AlleOrte()
    {
        try
        {
            var json = System.IO.File.ReadAllText(
                System.IO.Path.Combine(AppContext.BaseDirectory, "wwwroot", "data", "weltkarte_import.json"));
            var result = System.Text.Json.JsonSerializer.Deserialize<List<Ort>>(json,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (result?.Any() == true) return result;
        }
        catch { }
        return new List<Ort>();
    }
}
