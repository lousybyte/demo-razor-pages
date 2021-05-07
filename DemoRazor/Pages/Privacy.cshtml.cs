using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace DemoRazor.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> Logger;

        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            Logger = logger;
        }


        public void OnGet()
        {
        }
    }
}
