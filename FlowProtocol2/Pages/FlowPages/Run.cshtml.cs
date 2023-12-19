using System.Security.Cryptography;
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
        public string ScriptPath { get; set; }
        public string ScriptFilePath { get; set; }
        public string ScriptName { get; set; }
        private const string FlowProtocol2Extension = ".fp2";
        
        public RunModel(IConfiguration configuration)
        {
            ScriptPath = configuration["ScriptPath"];
            ScriptFilePath = string.Empty;
            ScriptName = string.Empty;
        }
        public IActionResult OnGet(string scripttag)
        {
            ScriptName = scripttag.Split('|').ToList().Last();
            ScriptFilePath = ScriptPath + Path.DirectorySeparatorChar
                + scripttag.Replace('|', Path.DirectorySeparatorChar)
                + FlowProtocol2Extension;
            System.IO.FileInfo fi = new System.IO.FileInfo(ScriptFilePath);
            if (fi != null && !fi.Exists)
            {
                return RedirectToPage("./NoScriptfile");
            }
            ScriptParser sp = new ScriptParser();
            sp.ReadScript(ScriptFilePath);
            ScriptRunner sr = new ScriptRunner();
            sr.RunScript(sp.StartCommand, BoundVars);
            return Page();
        }
    }
}
