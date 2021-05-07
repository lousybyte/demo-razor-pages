using DemoRazor.Services;
using Microsoft.AspNetCore.Mvc;

namespace DemoRazor.Pages
{
    public class IpViewComponent : ViewComponent
    {
        private readonly AccessorService AccessorSvc;

        public IpViewComponent(AccessorService accessorSvc)
        {
            AccessorSvc = accessorSvc;
        }

        public IViewComponentResult Invoke() // Synchronuos on purpose
        {
            var details = new Details
            {
                Ip = AccessorSvc.GetIp()
            };

            return View(details);
        }
    }

    public class Details
    {
        public string Ip { get; set; }
    }
}
