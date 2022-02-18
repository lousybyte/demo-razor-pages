using System;
using System.Linq;
using System.Threading.Tasks;

using DemoRazor.Contexts;

using Microsoft.EntityFrameworkCore;

using Quartz;

using static DemoRazor.Helpers.Accessor;

namespace DemoRazor.Jobs
{
    public class EmailJob : IJob
    {
        private readonly IEmailProcessor EmailSender;
        private readonly AppDbContext AppDb;

        public EmailJob(
            IEmailProcessor emailSender,
            AppDbContext appDb)
        {
            EmailSender = emailSender;
            AppDb       = appDb;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await ProcessEmailsAsync();
        }

        public async Task ProcessEmailsAsync()
        {
            var emails = await AppDb.EmailQueue
                    .Where(data => data.Status == EmailStatus.Pending)
                    .OrderBy(data => data.RequestedOn)
                    .Take(10)
                    .ToListAsync();

            foreach (var entry in emails)
            {
                entry.Status = await EmailSender.SendEmailAsync(entry);
                entry.UpdatedOn = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc); // PostgreSQL does not have a simple way to update timestamp field on UPDATE, requires triggers

                await AppDb.SaveChangesAsync();
            }
        }
    }
}
