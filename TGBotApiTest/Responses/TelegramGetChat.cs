namespace me.ewerestr.ewxtelegrambot.Responses
{
    public class TelegramGetChat
    {
        public bool ok { get; set; }
        public long id { get; set; }
        public int error_code { get; set; }
        public string description { get; set; }
    }
}
