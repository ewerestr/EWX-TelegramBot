namespace me.ewerestr.ewxtelegrambot.Responses
{
    public class TelegramGetUpdatesMessage
    {
        public int message_id { get; set; }
        public TelegramgetUpdatesMessageFrom from { get; set; }
        public TelegramGetUpdatesMessageChat chat { get; set; }
        public long date { get; set; }
        public string text { get; set; }
    }
}
