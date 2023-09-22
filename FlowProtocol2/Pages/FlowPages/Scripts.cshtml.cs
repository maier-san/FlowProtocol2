using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FlowProtocol2.Helper;

namespace FlowProtocol2.Pages.FlowPages
{
    public class ScriptsModel : PageModel
    {
        public bool ScriptGroupPathFound {get; set;}
        public string ScriptPath {get; set;}        
        public string GroupName {get; set;}
        public string ScriptGroupPath => ScriptPath + Path.DirectorySeparatorChar + GroupName;
        public Triplelist<string>? ScriptList { get; set; }
        
        public ScriptsModel(IConfiguration configuration)
        {
            ScriptPath = configuration["ScriptPath"];
        }

        public void OnGet(string groupname)
        {
            GroupName = groupname;      
            ScriptGroupPathFound = Directory.Exists(ScriptGroupPath);
            if (ScriptGroupPathFound)
            {
                DirectoryInfo di = new DirectoryInfo(ScriptGroupPath);
                List<string> scriplist = di.GetFiles("*.fp2").Select(x => x.Name.Replace(".fp2", string.Empty)).OrderBy(x=>x).ToList();
                ScriptList = new Triplelist<string>(scriplist);
            }
        }
    }
}
