using System.Threading.Tasks;
using DemoRazor.Contexts;
using DemoRazor.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RazorHtmlEmails.RazorClassLib.Services;
using RazorHtmlEmails.RazorClassLib.Views.Emails.ConfirmAccount;
using static DemoRazor.Helpers.Accessor;

namespace DemoRazor.Services
{
    public class EmailQueueService
    {
        private readonly ILogger<EmailQueueService> Logger;
        private readonly IConfiguration             Config;
        private readonly AppDbContext               AppDb;
        private readonly AccessorService            AccessorSvc;
        private readonly IRazorViewToStringRenderer EmailRenderer;

        public EmailQueueService(
            ILogger<EmailQueueService> logger,
            IConfiguration             config,
            AppDbContext               appDbContext,
            AccessorService            accessorSvc,
            IRazorViewToStringRenderer emailRenderer)
        {
            Logger        = logger;
            Config        = config;
            AppDb         = appDbContext;
            AccessorSvc   = accessorSvc;
            EmailRenderer = emailRenderer;
        }

        public async Task QueueActivationEmailAsync(IdentityUser user, string callbackUrl)
        {
            var model = new ConfirmAccountViewModel($"{callbackUrl}");
            string body = await EmailRenderer.RenderViewToStringAsync("/Templates/Emails/ConfirmAccount/ConfirmAccount.cshtml", model);

            var record = new EmailQueueData
            {
                Account     = user.Id,
                Email       = user.Email,
                Ip          = AccessorSvc.GetIp(),
                Subject     = "Please confirm your account",
                ContentHtml = body,
                Status      = EmailStatus.Pending
            };
            AppDb.EmailQueue.Add(record);

            await AppDb.SaveChangesAsync();

            Logger.LogInformation("Registered account activation email | {@record}", record);
        }

        public async Task QueueConfirmationEmailChangeAsync(IdentityUser user, string callbackUrl)
        {
            var model = new ConfirmEmailChangeViewModel($"{callbackUrl}");
            string body = await EmailRenderer.RenderViewToStringAsync("/Templates/Emails/ConfirmEmailChange/ConfirmEmailChange.cshtml", model);

            var record = new EmailQueueData
            {
                Account     = user.Id,
                Email       = user.Email,
                Ip          = AccessorSvc.GetIp(),
                Subject     = "Please confirm your email change",
                ContentHtml = body,
                Status      = EmailStatus.Pending
            };
            AppDb.EmailQueue.Add(record);

            await AppDb.SaveChangesAsync();

            Logger.LogInformation("Registered email change confirmation email | {@record}", record);
        }
    }
}
