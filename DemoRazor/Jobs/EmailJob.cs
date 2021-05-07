using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DemoRazor.Contexts;
using DemoRazor.Extensions;
using DemoRazor.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using SendGrid;
using SendGrid.Helpers.Mail;
using static DemoRazor.Helpers.Accessor;

namespace DemoRazor.Jobs
{
    public class EmailJob : IJob
    {
        private readonly ILogger<EmailJob> Logger;
        private readonly EmailSettings     Config;
        private readonly AppDbContext      AppDb;

        public EmailJob(
            ILogger<EmailJob>               logger,
            IOptionsSnapshot<EmailSettings> config,
            AppDbContext                    appDb)
        {
            Logger = logger;
            Config = config.Value;
            AppDb  = appDb;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await SendEmailAsync();
        }

        public async Task SendEmailAsync()
        {
            var emails = await AppDb.EmailQueue
                    .Where(data => data.Status == EmailStatus.Pending)
                    .OrderBy(data => data.RequestedOn)
                    .Take(10)
                    .ToListAsync();

            foreach (var entry in emails)
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
                    entry.Status = EmailStatus.Sent;
                    Logger.LogInformation("Email successfully sent | {@entry} | {@response}", entry, response);
                }
                else
                {
                    entry.Status = EmailStatus.Error;
                    Logger.LogError("Could not send email | {@entry} | {@response}", entry, response);
                }

                entry.UpdatedOn = DateTime.Now; // PostgreSQL does not have a simple way to update timestamp field on UPDATE, requires triggers
                await AppDb.SaveChangesAsync();
            }
        }
    }
}
