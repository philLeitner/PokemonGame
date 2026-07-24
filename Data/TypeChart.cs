namespace MonsterKampf.Data;

/// <summary>
/// Typ-Effektivitäts-Tabelle – 18 Typen (Gen 6+)
/// Angreifer (Zeile) → Verteidiger (Spalte)
/// Werte: 0 = keine Wirkung, 0.5 = nicht sehr effektiv, 1 = normal, 2 = sehr effektiv
/// </summary>
public static class TypeChart
{
    public static readonly string[] AlleTypen =
    {
        "Normal","Brennen","Tropfen","Blitz","Blatt","Eis",
        "Kampf","Gift","Boden","Flug","Psycho","Käfer",
        "Stein","Geist","Drache","Dunkel","Stahl","Fee"
    };

    public static readonly Dictionary<string, string> TypFarben = new()
    {
        { "Normal",  "#A8A878" }, { "Brennen", "#F08030" }, { "Tropfen", "#6890F0" },
        { "Blitz",   "#F8D030" }, { "Blatt",   "#78C850" }, { "Eis",     "#98D8D8" },
        { "Kampf",   "#C03028" }, { "Gift",    "#A040A0" }, { "Boden",   "#E0C068" },
        { "Flug",    "#A890F0" }, { "Psycho",  "#F85888" }, { "Käfer",   "#A8B820" },
        { "Stein",   "#B8A038" }, { "Geist",   "#705898" }, { "Drache",  "#7038F8" },
        { "Dunkel",  "#705848" }, { "Stahl",   "#B8B8D0" }, { "Fee",     "#EE99AC" },
    };

    public static readonly Dictionary<string, string> TypEmojis = new()
    {
        { "Normal",  "⬜" }, { "Brennen", "🔥" }, { "Tropfen", "💧" },
        { "Blitz",   "⚡" }, { "Blatt",   "🌿" }, { "Eis",     "❄️" },
        { "Kampf",   "🥊" }, { "Gift",    "☠️" }, { "Boden",   "🌍" },
        { "Flug",    "🌬️" }, { "Psycho",  "🔮" }, { "Käfer",   "🐛" },
        { "Stein",   "🪨" }, { "Geist",   "👻" }, { "Drache",  "🐉" },
        { "Dunkel",  "🌑" }, { "Stahl",   "⚙️" }, { "Fee",     "✨" },
    };

    // ─── ELEMENT-EFFEKTIVITÄTS-TABELLE ───
    // Bestimmt wie viel Schaden ein Element-Typ gegen einen anderen macht.
    // Beispiel: Feuer (Brennen) gegen Blatt = 2x Schaden (sehr effektiv)
    //           Feuer (Brennen) gegen Wasser (Tropfen) = 0.5x Schaden (nicht sehr effektiv)
    // [Angreifer][Verteidiger] = Multiplikator
    // 0 = keine Wirkung, 0.5 = halber Schaden, 1 = normal, 2 = doppelter Schaden
    private static readonly Dictionary<string, Dictionary<string, float>> Matrix = new()
    {
        ["Normal"] = new() {
            ["Stein"]=0.5f, ["Geist"]=0f, ["Stahl"]=0.5f
        },
        ["Brennen"] = new() {
            ["Brennen"]=0.5f, ["Tropfen"]=0.5f, ["Stein"]=0.5f, ["Drache"]=0.5f,
            ["Blatt"]=2f, ["Eis"]=2f, ["Käfer"]=2f, ["Stahl"]=2f
        },
        ["Tropfen"] = new() {
            ["Tropfen"]=0.5f, ["Blatt"]=0.5f, ["Drache"]=0.5f,
            ["Brennen"]=2f, ["Boden"]=2f, ["Stein"]=2f
        },
        ["Blitz"] = new() {
            ["Tropfen"]=2f, ["Flug"]=2f,
            ["Blitz"]=0.5f, ["Blatt"]=0.5f, ["Drache"]=0.5f, ["Boden"]=0f
        },
        ["Blatt"] = new() {
            ["Tropfen"]=2f, ["Boden"]=2f, ["Stein"]=2f,
            ["Brennen"]=0.5f, ["Blatt"]=0.5f, ["Gift"]=0.5f, ["Flug"]=0.5f,
            ["Käfer"]=0.5f, ["Drache"]=0.5f, ["Stahl"]=0.5f
        },
        ["Eis"] = new() {
            ["Blatt"]=2f, ["Boden"]=2f, ["Flug"]=2f, ["Drache"]=2f,
            ["Tropfen"]=0.5f, ["Eis"]=0.5f, ["Stahl"]=0.5f
        },
        ["Kampf"] = new() {
            ["Normal"]=2f, ["Eis"]=2f, ["Stein"]=2f, ["Dunkel"]=2f, ["Stahl"]=2f,
            ["Gift"]=0.5f, ["Flug"]=0.5f, ["Psycho"]=0.5f, ["Käfer"]=0.5f, ["Fee"]=0.5f,
            ["Geist"]=0f
        },
        ["Gift"] = new() {
            ["Blatt"]=2f, ["Fee"]=2f,
            ["Gift"]=0.5f, ["Boden"]=0.5f, ["Stein"]=0.5f, ["Geist"]=0.5f, ["Stahl"]=0f
        },
        ["Boden"] = new() {
            ["Brennen"]=2f, ["Blitz"]=2f, ["Gift"]=2f, ["Stein"]=2f, ["Stahl"]=2f,
            ["Blatt"]=0.5f, ["Käfer"]=0.5f, ["Flug"]=0f
        },
        ["Flug"] = new() {
            ["Blatt"]=2f, ["Kampf"]=2f, ["Käfer"]=2f,
            ["Blitz"]=0.5f, ["Stein"]=0.5f, ["Stahl"]=0.5f
        },
        ["Psycho"] = new() {
            ["Kampf"]=2f, ["Gift"]=2f,
            ["Psycho"]=0.5f, ["Stahl"]=0.5f, ["Dunkel"]=0f
        },
        ["Käfer"] = new() {
            ["Blatt"]=2f, ["Psycho"]=2f, ["Dunkel"]=2f,
            ["Brennen"]=0.5f, ["Kampf"]=0.5f, ["Flug"]=0.5f, ["Geist"]=0.5f,
            ["Stahl"]=0.5f, ["Fee"]=0.5f
        },
        ["Stein"] = new() {
            ["Brennen"]=2f, ["Eis"]=2f, ["Flug"]=2f, ["Käfer"]=2f,
            ["Kampf"]=0.5f, ["Boden"]=0.5f, ["Stahl"]=0.5f
        },
        ["Geist"] = new() {
            ["Geist"]=2f, ["Psycho"]=2f,
            ["Dunkel"]=0.5f, ["Normal"]=0f
        },
        ["Drache"] = new() {
            ["Drache"]=2f,
            ["Stahl"]=0.5f, ["Fee"]=0f
        },
        ["Dunkel"] = new() {
            ["Geist"]=2f, ["Psycho"]=2f,
            ["Kampf"]=0.5f, ["Dunkel"]=0.5f, ["Fee"]=0.5f
        },
        ["Stahl"] = new() {
            ["Eis"]=2f, ["Stein"]=2f, ["Fee"]=2f,
            ["Brennen"]=0.5f, ["Tropfen"]=0.5f, ["Blitz"]=0.5f, ["Stahl"]=0.5f
        },
        ["Fee"] = new() {
            ["Kampf"]=2f, ["Drache"]=2f, ["Dunkel"]=2f,
            ["Brennen"]=0.5f, ["Gift"]=0.5f, ["Stahl"]=0.5f
        },
    };

    /// <summary>Gibt den Multiplikator zurück: Angreifer-Typ vs. Verteidiger-Typ</summary>
    public static float GetMultiplikator(string angreifer, string verteidiger)
    {
        if (Matrix.TryGetValue(angreifer, out var row) && row.TryGetValue(verteidiger, out var val))
            return val;
        return 1f;
    }

    // Wenn ein Monster zwei Typen hat (z.B. Wasser+Boden), werden beide Multiplikatoren multipliziert.
    // Beispiel: Blitz gegen Wasser+Boden = 2x * 0x = 0 (keine Wirkung)
    /// <summary>Kombinierter Multiplikator gegen einen Dual-Typen Verteidiger</summary>
    public static float GetVerteidigungsMultiplikator(string angreifer, List<string> verteidigerTypen)
    {
        float result = 1f;
        foreach (var vt in verteidigerTypen)
            result *= GetMultiplikator(angreifer, vt); // Jeden Typ einzeln prüfen und multiplizieren
        return result;
    }

    public static string GetEffektivitätsText(float multi) => multi switch
    {
        0f   => "Keine Wirkung!",
        0.5f => "Nicht sehr effektiv...",
        2f   => "Sehr effektiv!",
        4f   => "Extrem effektiv!",
        _    => ""
    };
}
