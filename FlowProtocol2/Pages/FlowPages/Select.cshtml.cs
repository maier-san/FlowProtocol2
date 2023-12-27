using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FlowProtocol2.Helper;

namespace FlowProtocol2.Pages.FlowPages
{
    public class SelectModel : PageModel
    {
        public string ScriptPath { get; set; }
        public string RelativePath { get; set; }
        public string RelativeBackPath { get; set; }
        public string BreadcrumbPath => RelativePath.Replace(Path.DirectorySeparatorChar.ToString(), " - ");
        public ObjectArray<NavLink> ScriptGroups { get; set; }
        public ObjectArray<NavLink> Scripts { get; set; }
        private const string FlowProtocol2Extension = ".fp2";

        public SelectModel(IConfiguration configuration)
        {
            ScriptPath = configuration["ScriptPath"];
            RelativePath = string.Empty;
            RelativeBackPath = string.Empty;
            ScriptGroups = new ObjectArray<NavLink>();
            Scripts = new ObjectArray<NavLink>();
        }
        public IActionResult OnGet(string relativepath)
        {
            if (relativepath == "x") relativepath = string.Empty;
            RelativePath = relativepath;
            string resultPath = ScriptPath + Path.DirectorySeparatorChar + RelativePath;
            if (!Directory.Exists(resultPath))
            {
                return RedirectToPage("./NoGroupDirectory");
            }
            DirectoryInfo di = new DirectoryInfo(resultPath);
            if (di.Parent != null)
            {
                RelativeBackPath = di.Parent.FullName.Replace(ScriptPath, string.Empty).Trim();
            }
            List<NavLink> scriptgrouplist = di.GetDirectories()
                .Select(x => x.Name)
                .Where(x => !x.StartsWith("_") && !x.StartsWith("."))
                .OrderBy(x => x)
                .Select(x => new NavLink(x, RelativePath + Path.DirectorySeparatorChar + x)).ToList();
            ScriptGroups.ReadList(scriptgrouplist);
            List<NavLink> scriplist = di.GetFiles("*" + FlowProtocol2Extension)
                .Select(x => x.Name.Replace(FlowProtocol2Extension, string.Empty))
                .Where(x => !x.StartsWith("_") && !x.StartsWith("."))
                .OrderBy(x => x)
                .Select(x => new NavLink(x, RelativePath + Path.DirectorySeparatorChar + x))
                .ToList();
            Scripts.ReadList(scriplist);
            return Page();
        }
    }
}
