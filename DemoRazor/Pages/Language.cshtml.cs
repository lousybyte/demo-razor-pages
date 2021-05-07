using System;
using DemoRazor.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace DemoRazor.Pages
{
    public class LanguageModel : PageModel
    {
        public CookieSettings CookieConfig { get; }
        public LanguageModel(IOptionsSnapshot<CookieSettings> config)
        {
            CookieConfig = config.Value;
        }

        public IActionResult OnPost(string culture, string returnUrl)
        {
            Response.Cookies.Append(
               CookieConfig.Localization.Name,
               CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
               new CookieOptions {
                   Expires     = DateTimeOffset.UtcNow.AddYears(1),
                   IsEssential = true,
                   Secure      = true
               }
            );

            return LocalRedirect(returnUrl);
        }
    }
}
