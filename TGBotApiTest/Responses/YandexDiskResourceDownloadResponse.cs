namespace me.ewerestr.ewxtelegrambot.Responses
{
    public class YandexDiskResourceDownloadResponse
    {
        // error block
        public string message { get; set; }
        public string description { get; set; }
        public string error { get; set; }
        // response block
        public string href { get; set; }
    }
}
