using Microsoft.AspNetCore.Mvc.RazorPages;
using FlowProtocol2.Helper;

namespace FlowProtocol2.Pages.FlowPages
{
    public class ScriptGroupsModel : PageModel
    {
        public bool ScriptPathFound { get; set; }
        public string ScriptPath { get; set; }
        public Triplelist<LinkItem>? ScriptGroupList { get; set; }

        public ScriptGroupsModel(IConfiguration configuration)
        {
            ScriptPath = configuration["ScriptPath"];
        }

        public void OnGet()
        {
            ScriptPathFound = Directory.Exists(ScriptPath);
            if (ScriptPathFound)
            {
                DirectoryInfo di = new DirectoryInfo(ScriptPath);
                List<LinkItem> groups = di.GetDirectories().Select(x => x.Name)
                    .Where(x => !x.StartsWith("_") && !x.StartsWith(".") && x != "SharedFunctions")
                    .OrderBy(x => x).Select(x => new LinkItem(x, x)).ToList();
                ScriptGroupList = new Triplelist<LinkItem>(groups);
            }
        }
    }
}
