using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FlowProtocol2.Helper;

namespace FlowProtocol2.Pages.FlowPages
{
    public class TemplateGroupsModel : PageModel
    {
        public bool TemplatePathFound { get; set; }
        public string TemplatePath { get; set; }
        public Triplelist<string>? TList { get; set; }

        public TemplateGroupsModel(IConfiguration configuration)
        {
            TemplatePath = configuration.GetValue<string>("TemplatePath");
        }

        public void OnGet()
        {            
            TemplatePathFound = Directory.Exists(TemplatePath);
            if (TemplatePathFound)
            {
                DirectoryInfo di = new DirectoryInfo(TemplatePath);
                List<string> templateGroups = di.GetDirectories().Select(x => x.Name)
                    .Where(x => !x.StartsWith("_") && !x.StartsWith(".") && x != "SharedFunctions")
                    .OrderBy(x => x).ToList();
                TList = new Triplelist<string>(templateGroups);
            }
        }
    }
}
