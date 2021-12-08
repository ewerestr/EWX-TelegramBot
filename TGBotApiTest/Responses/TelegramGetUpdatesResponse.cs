namespace me.ewerestr.ewxtelegrambot.Responses
{
    public class TelegramGetUpdatesResponse
    {
        public string message { get; set; }
        public string description { get; set; }
        public string error { get; set; }
        public bool ok { get; set; }
        public TelegramGetUpdatesItem[] result { get; set; }
    }
}
