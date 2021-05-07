using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoRazor.Models
{
    public class CookieSettings
    {
        public Details Localization { get; set; }

        public class Details
        {
            public string Name { get; set; }
        }
    }
}
