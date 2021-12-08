namespace me.ewerestr.ewxtelegrambot.Responses
{
    public class TelegramgetUpdatesMessageFrom
    {
        public long id { get; set; }
        public bool is_bot { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string username { get; set; }
        public string language_code { get; set; }
    }
}
