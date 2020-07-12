namespace TurnipTallyApi.Helpers.Settings
{
    public class EmailSettings
    {
        public string From { get; set; }

        public SmtpSettings Smtp { get; set; }
    }

    public class SmtpSettings
    {
        public string Server { get; set; }

        public int Port { get; set; }

        public bool Ssl { get; set; }
    }
}