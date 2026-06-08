using System.Globalization;
using System.Text;
using System.Text.Json;

/// <summary>
/// Clase base compartida por todas las herramientas MCP de análisis.
/// Centraliza la configuración común: cliente HTTP, servicio de IA, URL base y serialización JSON.
/// </summary>
internal abstract class AnalysisToolBase
{
    protected readonly HttpClient _httpClient;
    protected readonly IAIService _aiService;

    protected const string BaseApiUrl = "https://localhost:44384/GetRawData";

    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    // Alias en lenguaje natural → identificador canónico del espacio
    private static readonly (string Alias, string Id)[] _espacioAliases =
    [
        ("PlayaArenal_1",    "PlayaArenal_1"),
        ("Playa del Arenal", "PlayaArenal_1"),
        ("playa arenal",     "PlayaArenal_1"),
        ("arenal",           "PlayaArenal_1"),

        ("PlayaGrava_2",     "PlayaGrava_2"),
        ("Playa La Grava",   "PlayaGrava_2"),
        ("la grava",         "PlayaGrava_2"),
        ("grava",            "PlayaGrava_2"),
        ("centro historico", "PlayaGrava_2"),

        ("PuertoJavea_3",    "PuertoJavea_3"),
        ("Puerto Nautico",   "PuertoJavea_3"),
        ("puerto nautico",   "PuertoJavea_3"),
        ("puerto",           "PuertoJavea_3"),
        ("puerto de javea",  "PuertoJavea_3"),
        ("puerto javea",     "PuertoJavea_3"),
    ];

    protected AnalysisToolBase(HttpClient httpClient, IAIService aiService)
    {
        _httpClient = httpClient;
        _aiService = aiService;
    }

    /// <summary>
    /// Traduce un nombre de espacio (canónico o en lenguaje natural) al identificador canónico.
    /// Si no hay coincidencia exacta, intenta coincidencia parcial para frases descriptivas.
    /// Devuelve el valor original si no hay ninguna coincidencia, dejando que la API lo gestione.
    /// </summary>
    protected static string ResolveIdEspacio(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;

        var normalized = NormalizeText(input.Trim());

        // 1. Coincidencia exacta (sin tildes ni mayúsculas)
        foreach (var (alias, id) in _espacioAliases)
        {
            if (NormalizeText(alias) == normalized)
                return id;
        }

        // 2. El input contiene un alias conocido (para frases largas del tipo "zona del puerto náutico")
        foreach (var (alias, id) in _espacioAliases)
        {
            var normalizedAlias = NormalizeText(alias);
            if (normalizedAlias.Length > 4 && normalized.Contains(normalizedAlias))
                return id;
        }

        return input;
    }

    // Elimina tildes y convierte a minúsculas para comparación normalizada
    private static string NormalizeText(string s)
    {
        var decomposed = s.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(decomposed.Length);
        foreach (var c in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        return sb.ToString().ToLowerInvariant();
    }

    protected static string SerializeSuccess(object result) =>
        JsonSerializer.Serialize(result, JsonOptions);

    protected static string SerializeError(string idEspacio, string error, string message) =>
        JsonSerializer.Serialize(new
        {
            success = false,
            timestamp = DateTime.UtcNow,
            idEspacio,
            error,
            message
        }, JsonOptions);
}
