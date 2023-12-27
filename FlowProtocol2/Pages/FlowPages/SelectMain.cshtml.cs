using FlowProtocol2.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FlowProtocol2.Pages.FlowPages
{
    public class SelectMainModel : PageModel
    {
        public string ScriptPath { get; set; }
        public ObjectArray<NavLink> ScriptGroups { get; set; }
        public SelectMainModel(IConfiguration configuration)
        {
            ScriptPath = configuration["ScriptPath"];
            ScriptGroups = new ObjectArray<NavLink>();
        }

        public IActionResult OnGet()
        {
            string resultPath = ScriptPath;
            if (!Directory.Exists(resultPath))
            {
                return RedirectToPage("./NoBaseDirectory");
            }
            DirectoryInfo di = new DirectoryInfo(resultPath);
            List<NavLink> scriptgrouplist = di.GetDirectories()
                .Select(x => x.Name)
                .Where(x => !x.StartsWith("_") && !x.StartsWith(".") && x != "SharedFunctions")
                .OrderBy(x => x)
                .Select(x => new NavLink(x, x)).ToList();
            ScriptGroups.ReadList(scriptgrouplist);
            return Page();
        }
    }
}
