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
        public string ScriptPath { get; set; }
        public string ScriptFilePath { get; set; }
        public string ScriptName { get; set; }
        public RunContext RunContext { get; set; }
        public IMForm InputForm => RunContext.InputForm;
        public OMDocument Document => RunContext.DocumentBuilder.Document;
        public List<ErrorElement> Errors => RunContext.ErrorItems;
        private const string FlowProtocol2Extension = ".fp2";

        public RunModel(IConfiguration configuration)
        {
            ScriptPath = configuration["ScriptPath"];
            ScriptFilePath = string.Empty;
            ScriptName = string.Empty;
            ScriptBaseURL = string.Empty;
            BoundVars = new Dictionary<string, string>();
            RunContext = new RunContext();
        }
        public IActionResult OnGet(string scripttag)
        {
            ScriptBaseURL = this.HttpContext.Request.Scheme + "://" + this.HttpContext.Request.Host + this.HttpContext.Request.Path;
            ScriptFilePath = ScriptPath + Path.DirectorySeparatorChar
                + scripttag.Replace('|', Path.DirectorySeparatorChar)
                + FlowProtocol2Extension;
            System.IO.FileInfo fi = new System.IO.FileInfo(ScriptFilePath);
            if (fi != null && !fi.Exists)
            {
                return RedirectToPage("./NoScriptfile");
            }
            if (fi != null && fi.Directory != null)
            {
                ScriptName = fi.Name;
                RunContext.ScriptPath = fi.Directory.FullName;
            }
            ScriptParser sp = new ScriptParser();
            var sinfo = sp.ReadScript(RunContext, ScriptFilePath, 0);
            ScriptRunner sr = new ScriptRunner();
            RunContext.BoundVars = BoundVars;
            RunContext.MyBaseURL = this.HttpContext.Request.Scheme + "://" + this.HttpContext.Request.Host + this.HttpContext.Request.Path;
            RunContext.MyResultURL = RunContext.MyBaseURL + this.HttpContext.Request.QueryString;
            sr.RunScript(RunContext, sinfo.StartCommand);
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
