using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FlowProtocol2.Pages.FlowPages
{
    public class NoBaseDirectoryModel : PageModel
    {
        public string ScriptPath {get; set;}
        public NoBaseDirectoryModel(IConfiguration configuration)
        {
            ScriptPath = configuration["ScriptPath"];            
        }
        public void OnGet()
        {
        }
    }
}
