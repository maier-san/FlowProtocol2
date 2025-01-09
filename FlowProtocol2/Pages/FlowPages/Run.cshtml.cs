using FlowProtocol2.Commands;
using FlowProtocol2.Core;
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
        private const string FlowProtocol2Extension = ".fp2";

        public RunModel(IConfiguration configuration)
        {
            ScriptPath = configuration["ScriptPath"] ?? throw new InvalidOperationException();
            ScriptFilePath = string.Empty;
            ScriptName = string.Empty;
            ScriptBaseURL = string.Empty;
            BoundVars = new Dictionary<string, string>();
            RunContext = new RunContext();
            RunContext.LinkWhitelist = configuration.GetSection("LinkWhitelist").Get<List<string>>() ?? throw new InvalidOperationException();
        }
        public IActionResult OnGet(string relativepath)
        {
            relativepath = relativepath.Replace('|', Path.DirectorySeparatorChar);
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
            ScriptRunner sr = new ScriptRunner();
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

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            return RedirectToPage("./Run", BoundVars);
        }
    }
}
