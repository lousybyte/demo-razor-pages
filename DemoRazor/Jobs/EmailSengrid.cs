using System;
using System.Net;
using System.Threading.Tasks;

using DemoRazor.Data;
using DemoRazor.Extensions;
using DemoRazor.Models;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SendGrid;
using SendGrid.Helpers.Mail;

using static DemoRazor.Helpers.Accessor;

namespace DemoRazor.Jobs
{
    public class EmailSengrid : IEmailProcessor
    {
        private readonly ILogger<EmailJob> Logger;
        private readonly EmailSettings     Config;

        public EmailSengrid(ILogger<EmailJob> logger, IOptionsSnapshot<EmailSettings> config)
        {
            Logger = logger;
            Config = config.Value;
        }

        public async Task<EmailStatus> SendEmailAsync(EmailQueueData entry)
        {
            var apiKey = Environment.GetEnvironmentVariable("EMAIL_SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress(
                    Config.FromAddress.NullIfEmpty() ?? Environment.GetEnvironmentVariable("EMAIL_SENDGRID_FROM_ADDRESS"),
                    Config.FromName.NullIfEmpty() ?? Environment.GetEnvironmentVariable("EMAIL_SENDGRID_FROM_NAME")
                );
            var to = new EmailAddress(Config.ToAddress.NullIfEmpty() ?? entry.Email, Config.ToName.NullIfEmpty() ?? entry.Email);

            var msg = MailHelper.CreateSingleEmail(from, to, entry.Subject, "", entry.ContentHtml);
            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode == HttpStatusCode.Accepted)
            {
                Logger.LogInformation("Email successfully sent | {@entry} | {@response}", entry, response);
                return EmailStatus.Sent;
            }
            else
            {
                Logger.LogError("Could not send email | {@entry} | {@response}", entry, response);
                return EmailStatus.Error;
            }
        }
    }
}
