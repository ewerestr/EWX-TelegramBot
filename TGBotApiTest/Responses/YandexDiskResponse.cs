namespace me.ewerestr.ewxtelegrambot.Responses
{
    public class YandexDiskResponse
    {
        public string message { get; set; }
        public string description { get; set; }
        public string error { get; set; }
        public long max_file_size { get; set; }
        public bool unlimited_autoupload_enabled { get; set; }
        public long total_space { get; set; }
        public long trash_size { get; set; }
        public bool is_paid { get; set; }
        public long used_space { get; set; }
        public YandexSystemFoldersResponsePart system_folders { get; set; }
        public YandexUserResponsePart user { get; set; }
        public long revision { get; set; }
    }
}
