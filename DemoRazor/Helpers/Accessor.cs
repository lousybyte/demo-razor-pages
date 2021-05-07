namespace DemoRazor.Helpers
{
    public static class Accessor
    {
        public enum EmailStatus : byte
        {
            Pending = 0,
            Sent    = 1,
            Error   = 2
        }
    }
}
