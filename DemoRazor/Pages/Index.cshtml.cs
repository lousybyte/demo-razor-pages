using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace DemoRazor.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> Logger;
        private readonly IStringLocalizer<SharedResources> Localizer;

        public IndexModel(ILogger<IndexModel> logger, IStringLocalizer<SharedResources> localizer)
        {
            Logger    = logger;
            Localizer = localizer;
        }


        public void OnGet()
        {
        }
    }
}
