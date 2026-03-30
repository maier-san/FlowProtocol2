using FlowProtocol2.Commands;
using FlowProtocol2.Core;
using FlowProtocol2.Helper;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FlowProtocol2.Pages.FlowPages
{
    public class RunModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public Dictionary<string, string> BoundVars { get; set; }
        public string ScriptBaseURL { get; set; }
        public required string ScriptPath { get; set; }
        public string ScriptFilePath { get; set; }
        public string ScriptName { get; set; }
        public RunContext RunContext { get; set; }
        public IMForm InputForm => RunContext.InputForm;
        public OMDocument Document => RunContext.DocumentBuilder.Document;
        public List<ErrorElement> Errors => RunContext.ErrorItems;
        public List<BreadcrumbItem> Breadcrumbs { get; set; }
        
        // Properties zur Steuerung der clientseitigen Skripte
        public bool NeedsUploadFunction { get; set; }
        public bool NeedsCopyFunction { get; set; }
        public bool NeedsSaveFunction { get; set; }
        
        private const string FlowProtocol2Extension = ".fp2";

        public RunModel(IConfiguration configuration)
        {
            ScriptPath = configuration["ScriptPath"] ?? throw new InvalidOperationException();
            ScriptFilePath = string.Empty;
            ScriptName = string.Empty;
            ScriptBaseURL = string.Empty;
            BoundVars = new Dictionary<string, string>();
            RunContext = new RunContext();
            Breadcrumbs = new List<BreadcrumbItem>();
            RunContext.LinkWhitelist = configuration.GetSection("LinkWhitelist").Get<List<string>>() ?? throw new InvalidOperationException();
        }
        public IActionResult OnGet(string relativepath)
        {
            relativepath = relativepath.Replace('|', Path.DirectorySeparatorChar);
            
            // Build breadcrumbs using local variable
            Breadcrumbs.Add(new BreadcrumbItem("Start", "x"));
            if (!string.IsNullOrEmpty(relativepath))
            {
                string[] parts = relativepath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    string pathUpToHere = string.Join(Path.DirectorySeparatorChar.ToString(), parts, 0, i + 1);
                    Breadcrumbs.Add(new BreadcrumbItem(parts[i], pathUpToHere.Replace(Path.DirectorySeparatorChar.ToString(), "|")));
                }
                // Add the script name as the last item (not a link)
                Breadcrumbs.Add(new BreadcrumbItem(parts[parts.Length - 1], string.Empty));
            }
            ScriptBaseURL = this.HttpContext.Request.Scheme + "://" + this.HttpContext.Request.Host + this.HttpContext.Request.Path;
            ScriptFilePath = ScriptPath + Path.DirectorySeparatorChar + relativepath + FlowProtocol2Extension;
            System.IO.FileInfo fi = new System.IO.FileInfo(ScriptFilePath);
            if (!fi.Exists)
            {
                return RedirectToPage("./NoScriptfile");
            }
            if (fi.Directory != null)
            {
                ScriptName = fi.Name.Replace(".fp2", string.Empty);
                RunContext.CurrentScriptPath = fi.Directory.FullName;
                RunContext.ScriptPath = ScriptPath;
            }
            ScriptParser sp = new ScriptParser();
            var sinfo = sp.ReadScript(RunContext, ScriptFilePath, 0);
            RunContext.ScriptRepository[ScriptFilePath] = sinfo;
            ScriptRunner sr = new ScriptRunner();
            // Decompress any compressed values in the query string
            foreach (var k in BoundVars.Keys.ToList())
            {
                var v = BoundVars[k] ?? string.Empty;
                if (v.StartsWith(Core.UrlCompressor.Marker))
                {
                    try
                    {
                        BoundVars[k] = Core.UrlCompressor.DecompressFromUrl(v);
                    }
                    catch
                    {
                        // on error leave as-is
                    }
                }
            }
            RunContext.BoundVars = BoundVars;
            RunContext.MyDomain = this.HttpContext.Request.Scheme + "://" + this.HttpContext.Request.Host;
            RunContext.MyBaseURL = RunContext.MyDomain + this.HttpContext.Request.Path;
            RunContext.MyResultURL = RunContext.MyBaseURL + this.HttpContext.Request.QueryString;

            // Alle gebundenen Variablen ohne BaseKey-Anteil als interne Variablen verfügbar machen:
            foreach (var p in RunContext.BoundVars)
            {
                if (!string.IsNullOrEmpty(p.Value) && !p.Value.Contains('_')) RunContext.InternalVars[p.Key] = p.Value;
            }

            sr.RunScript(RunContext, sinfo.StartCommand);

            // Füge die restlichen gesetzten Parameter als gegeben dazu:
            RunContext.GivenKeys.AddRange(
                RunContext.BoundVars
                .Where(x => !string.IsNullOrEmpty(x.Value) && !RunContext.GivenKeys.Contains(x.Key))
                .Select(x => x.Key)
                .ToList());

            // Bestimme, welche clientseitigen Skripte benötigt werden
            DetermineRequiredScripts();

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            // Compress long or multiline values to keep the URL compact
            foreach (var k in BoundVars.Keys.ToList())
            {
                var v = BoundVars[k] ?? string.Empty;
                if (string.IsNullOrEmpty(v)) continue;
                if (v.StartsWith(Core.UrlCompressor.Marker)) continue; // already compressed
                if (v.Contains('\n') || v.Contains('\r') || v.Length > 200)
                {
                    try
                    {
                        BoundVars[k] = Core.UrlCompressor.CompressToUrl(v);
                    }
                    catch
                    {
                        // on error leave original
                    }
                }
            }
            return RedirectToPage("./Run", BoundVars);
        }

        /// <summary>
        /// Bestimmt, welche clientseitigen Skripte benötigt werden.
        /// Die Flags werden automatisch von InputForm und Document gesetzt.
        /// </summary>
        private void DetermineRequiredScripts()
        {
            NeedsUploadFunction = InputForm.NeedsUploadFunction;
            NeedsCopyFunction = Document.NeedsCopyFunction;
            NeedsSaveFunction = Document.NeedsSaveFunction;
        }
    }
}
