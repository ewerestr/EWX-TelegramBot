namespace me.ewerestr.ewxtelegrambot.Responses
{
    public class TelegramGetUpdatesItem
    {
        public long update_id { get; set; }
        public TelegramGetUpdatesMessage message { get; set; }
        public TelegramChatMember my_chat_member { get; set; }
        public TelegramChannelPost channel_post { get; set; }
    }
}
