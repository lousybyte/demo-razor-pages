using System.Threading.Tasks;

using DemoRazor.Data;

using static DemoRazor.Helpers.Accessor;

namespace DemoRazor
{
    public interface IEmailProcessor
    {
        public abstract Task<EmailStatus> SendEmailAsync(EmailQueueData data);
    }
}