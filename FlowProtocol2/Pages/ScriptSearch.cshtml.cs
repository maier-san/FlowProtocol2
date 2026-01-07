using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace FlowProtocol2.Pages;

public class ScriptSearchModel : PageModel
{
    public required string ScriptPath { get; set; }

    public ScriptSearchModel(IConfiguration configuration)
    {
        ScriptPath = configuration["ScriptPath"] ?? throw new InvalidOperationException();
    }

    public string? Query { get; set; }
    public List<SearchResult> Results { get; set; } = new();
    public string? Title { get; set; }

    public class SearchResult
    {
        public string? ScriptName { get; set; }
        public string? RelativePath { get; set; }
        public string? RelativePathForLink => RelativePath?.Replace(".fp2", "").Replace(Path.DirectorySeparatorChar, '|');

        public int OccurrenceCount { get; set; }
        public List<string> SampleLines { get; set; } = new();
    }

    public void OnGet(string query)
    {
        Query = query;
        if (string.IsNullOrWhiteSpace(Query))
        {
            Title = "Skriptsuche - Kein Suchbegriff eingegeben";
            return;
        }
        
        if (!Directory.Exists(ScriptPath))
        {
            Title = $"Skriptsuche nach \"{Query}\" - Skriptpfad nicht gefunden";
            return;
        }

        var fp2Files = Directory.GetFiles(ScriptPath, "*.fp2", SearchOption.AllDirectories);
        var pathMatches = new List<SearchResult>();
        var contentMatches = new List<SearchResult>();

        foreach (var file in fp2Files)
        {
            var relativePathFull = Path.GetRelativePath(ScriptPath, file);
            var scriptName = Path.GetFileNameWithoutExtension(file);
            var lines = System.IO.File.ReadAllLines(file);
            var matchingLines = lines.Where(l => l.Contains(Query, StringComparison.OrdinalIgnoreCase)).ToList();
            var pathContainsQuery = relativePathFull.Contains(Query, StringComparison.OrdinalIgnoreCase);

            if (pathContainsQuery || matchingLines.Any())
            {
                var result = new SearchResult
                {
                    ScriptName = scriptName,
                    RelativePath = relativePathFull,
                    OccurrenceCount = matchingLines.Count,
                    SampleLines = matchingLines.Take(3).ToList()
                };

                if (pathContainsQuery)
                {
                    pathMatches.Add(result);
                }
                else
                {
                    contentMatches.Add(result);
                }
            }
        }

        Results.AddRange(pathMatches);
        Results.AddRange(contentMatches);

        int count = Results.Count;
        string countText = count == 0 ? "keine" : count.ToString();
        Title = $"Skriptsuche nach \"{Query}\" - {countText} Skripte gefunden";
    }
}