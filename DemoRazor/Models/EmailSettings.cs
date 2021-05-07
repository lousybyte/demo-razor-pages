namespace DemoRazor.Models
{
    public class EmailSettings
    {
        public string FromAddress { get; set; }
        public string FromName    { get; set; }
        public string ToAddress   { get; set; }
        public string ToName      { get; set; }

        public SendgridSettings Sendgrid { get; set; }

        public class SendgridSettings
        {
            public string ApiKey { get; set; }
        }
    }
}
