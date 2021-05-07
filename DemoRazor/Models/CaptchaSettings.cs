namespace DemoRazor.Models
{
    public class CaptchaSettings
    {
        public Captcha Google { get; set; }
        public class Captcha
        {
            public string ClientKey { get; set; }
            public string ServerKey { get; set; }
        }
    }
}
