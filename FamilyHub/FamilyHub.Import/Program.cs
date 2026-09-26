using System.Net.Http.Json;

const string apiBaseUrl = "http://localhost:5005";

if (args.Length == 0)
{
    Console.WriteLine("Bitte eine CSV-Datei angeben.");
    Console.WriteLine();
    Console.WriteLine("Beispiel:");
    Console.WriteLine(@"dotnet run --project FamilyHub.Import -- gerichte.csv");

    return;
}

var csvPath = args[0];

if (!File.Exists(csvPath))
{
    Console.WriteLine($"Datei wurde nicht gefunden: {csvPath}");
    return;
}

using var client = new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl)
};

Console.WriteLine("Vorhandene Gerichte werden geladen...");

var existingMeals =
    await client.GetFromJsonAsync<List<MealDto>>("/api/meals")
    ?? new List<MealDto>();

var existingNames = existingMeals
    .Select(x => x.Name)
    .ToHashSet(StringComparer.OrdinalIgnoreCase);

var lines = await File.ReadAllLinesAsync(csvPath);

if (lines.Length == 0)
{
    Console.WriteLine("Die CSV-Datei ist leer.");
    return;
}

var imported = 0;
var skipped = 0;
var failed = 0;

foreach (var rawLine in lines.Skip(1))
{
    var name = rawLine.Trim();

    if (string.IsNullOrWhiteSpace(name))
    {
        continue;
    }

    if (existingNames.Contains(name))
    {
        Console.WriteLine($"Übersprungen: {name} existiert bereits.");
        skipped++;
        continue;
    }

    try
    {
        var response = await client.PostAsJsonAsync(
            "/api/meals",
            new
            {
                Name = name
            });

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();

            Console.WriteLine(
                $"FEHLER: {name} - HTTP {(int)response.StatusCode} {error}");

            failed++;
            continue;
        }

        Console.WriteLine($"Importiert: {name}");

        existingNames.Add(name);
        imported++;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"FEHLER: {name} - {ex.Message}");
        failed++;
    }
}

Console.WriteLine();
Console.WriteLine("-----------------------------");
Console.WriteLine("Import abgeschlossen");
Console.WriteLine($"Importiert:    {imported}");
Console.WriteLine($"Übersprungen:  {skipped}");
Console.WriteLine($"Fehler:        {failed}");
Console.WriteLine("-----------------------------");

public sealed class MealDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
}