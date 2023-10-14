using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FlowProtocol2.Pages.FlowPages
{
    public class RunModel : PageModel
    {
        public string ScriptPath { get; set; }
        public string? ScriptFilePath { get; set; }
        private const string FlowProtocol2Extension = ".fp2";
        public RunModel(IConfiguration configuration)
        {
            ScriptPath = configuration["ScriptPath"];
        }
        public void OnGet(string scripttag)
        {
            ScriptFilePath = ScriptPath + Path.DirectorySeparatorChar
                + scripttag.Replace('|', Path.DirectorySeparatorChar)
                + FlowProtocol2Extension;
        }
    }
}
