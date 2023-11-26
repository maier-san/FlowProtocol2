using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FlowProtocol2.Pages.FlowPages
{
    public class RunModel : PageModel
    {
        public string ScriptPath { get; set; }
        public string ScriptFilePath { get; set; }
        public string ScriptName {get; set;}
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
            if (fi == null || !fi.Exists)
            {
                return RedirectToPage("./NoScriptfile");
            }
            return Page();
        }
    }
}
