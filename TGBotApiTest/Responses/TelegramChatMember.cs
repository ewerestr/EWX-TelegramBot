namespace me.ewerestr.ewxtelegrambot.Responses
{
    public class TelegramChatMember
    {
        public TelegramGetUpdatesMessageChat chat { get; set; }
        public TelegramgetUpdatesMessageFrom from { get; set; }
        public TelegramNewChatMember new_chat_member { get; set; }
    }
}
