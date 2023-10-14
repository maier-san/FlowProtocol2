using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FlowProtocol2.Helper;

namespace FlowProtocol2.Pages.FlowPages
{
    public class ScriptsModel : PageModel
    {
        public bool ScriptGroupPathFound { get; set; }
        public string ScriptPath { get; set; }
        public string GroupName { get; set; }
        public string ScriptGroupPath => ScriptPath + Path.DirectorySeparatorChar + GroupName;
        public Triplelist<LinkItem>? ScriptList { get; set; }

        private const string FlowProtocol2Extension = ".fp2";

        public ScriptsModel(IConfiguration configuration)
        {
            ScriptPath = configuration["ScriptPath"];
            GroupName = string.Empty;
        }

        public void OnGet(string groupname)
        {
            GroupName = groupname;
            ScriptGroupPathFound = Directory.Exists(ScriptGroupPath);
            if (ScriptGroupPathFound)
            {
                DirectoryInfo di = new DirectoryInfo(ScriptGroupPath);
                List<LinkItem> scriplist = di.GetFiles("*" + FlowProtocol2Extension)
                    .OrderBy(x => x.Name)
                    .Select(x => new LinkItem(x.Name.Replace(FlowProtocol2Extension, string.Empty), GroupName + '|' + x.Name.Replace(FlowProtocol2Extension, string.Empty)))
                    .ToList();
                ScriptList = new Triplelist<LinkItem>(scriplist);
            }
        }
    }
}
