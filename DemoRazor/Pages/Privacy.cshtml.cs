using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using DemoRazor.Services;
using System.Security.Cryptography.X509Certificates;
using DemoRazor.Data;

namespace DemoRazor.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> Logger;

        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            Logger     = logger;
        }


        public void OnGet()
        {
        }
    }
}
