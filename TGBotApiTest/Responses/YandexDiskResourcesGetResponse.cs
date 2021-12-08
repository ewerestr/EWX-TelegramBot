namespace me.ewerestr.ewxtelegrambot.Responses
{
    public class YandexDiskResourcesGetResponse
    {
        // error block
        public string message { get; set; }
        public string description { get; set; }
        public string error { get; set; }
        // response block
        public YandexDiskResourcesList _embedded { get; set; }
    }
}
