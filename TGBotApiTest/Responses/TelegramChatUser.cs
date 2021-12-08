namespace me.ewerestr.ewxtelegrambot.Responses
{
    public class TelegramChatUser
	{
        public long id { get; set; }
		public bool is_bot { get; set; }
		public string first_name { get; set; }
		public string username { get; set; }
    }
}
