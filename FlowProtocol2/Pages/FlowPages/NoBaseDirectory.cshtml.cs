using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FlowProtocol2.Pages.FlowPages
{
    public class NoBaseDirectoryModel(IConfiguration configuration) : PageModel
    {
        public string ScriptPath { get;} = configuration["ScriptPath"] ?? string.Empty;

        public void OnGet()
        {
        }
    }
}
