namespace RazorHtmlEmails.RazorClassLib.Views.Emails.ConfirmAccount
{
    public class ConfirmEmailChangeViewModel
    {
        public ConfirmEmailChangeViewModel(string confirmEmailUrl)
        {
            ConfirmEmailUrl = confirmEmailUrl;
        }

        public string ConfirmEmailUrl { get; set; }
    }
}