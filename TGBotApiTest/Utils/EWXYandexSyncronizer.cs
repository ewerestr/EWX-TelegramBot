using me.ewerestr.ewxtelegrambot.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace me.ewerestr.ewxtelegrambot.Utils
{
    public class EWXYandexSyncronizer
    {
        // vars
        private List<string> _formats = new List<string>(new string[] { "jpg", "jpeg", "png", "bmp" });
        private List<string> _subfolders = new List<string>();
        private List<string> _images = new List<string>();
        private List<string> _audios = new List<string>();
        private EWXLocalData _localData;
        private string _yandexToken = null;

        // constructor
        public EWXYandexSyncronizer(string yandexToken)
        {
            _yandexToken = yandexToken;
            ScanHierarchy();
            GetSubfolders("disk:/EWXTelegramBot");
            // Checking for existance in localfolder
        }

        private void ScanHierarchy()
        {
            List<List<string>> fullscanSubfolders = new List<List<string>>();
            List<List<string>> fullscanImages = new List<List<string>>();
            List<List<string>> fullscanAudios = new List<List<string>>();

            List<List<string>> subfolders_l1 = new List<List<string>>();
            List<List<string>> subfolders_l2 = new List<List<string>>();

            List<string> rootFolders = GetSubfolders("disk:/EWXTelegramBot");
            List<string> rootImages = GetImages("disk:/EWXTelegramBot");
            List<string> rootAudios = GetAudios("disk:/EWXTelegramBot");
            if (rootFolders.Count > 0)
            {
                foreach (string path in rootFolders)
                {
                    fullscanSubfolders.Add(GetSubfolders(path));
                    fullscanImages.Add(GetImages(path));
                    fullscanAudios.Add(GetAudios(path));
                }
            }
            _images = _images.Concat(rootImages).ToList();
            if (fullscanImages.Count > 0) foreach (List<string> list in fullscanImages) _images = _images.Concat(list).ToList();
            _audios = _audios.Concat(rootAudios).ToList();
            if (fullscanAudios.Count > 0) foreach (List<string> list in fullscanAudios) _audios = _audios.Concat(list).ToList();
        }

        private List<string> GetSubfolders(string path)
        {
            List<string> subfolders = new List<string>();
            string requestUrl = new EWXRequestBuilder(EWXTelegramBot.GetYandexAPI())
                .SetMethod("disk/resources")
                .AddParameter("path", path)
                .AddParameter("limit", "1000")
                .BuildRequest();
            YandexDiskResourcesGetResponse response = JsonSerializer.Deserialize<YandexDiskResourcesGetResponse>(EWXTelegramBot.SendRequest(requestUrl, _yandexToken, "GET"));
            if (response.error == null)
            {
                foreach (YandexDiskResourcesItem item in response._embedded.items) if (item.type.Equals("dir")) subfolders.Add(item.path);
            }
            return subfolders;
        }

        private List<string> GetImages(string path)
        {
            List<string> images = new List<string>();
            string requestUrl = new EWXRequestBuilder(EWXTelegramBot.GetYandexAPI())
                .SetMethod("disk/resources")
                .AddParameter("path", path)
                .AddParameter("limit", "1000")
                .BuildRequest();
            YandexDiskResourcesGetResponse response = JsonSerializer.Deserialize<YandexDiskResourcesGetResponse>(EWXTelegramBot.SendRequest(requestUrl, _yandexToken, "GET"));
            if (response.error == null)
            {
                foreach (YandexDiskResourcesItem item in response._embedded.items)
                {
                    if (item.name.Contains('.') && !item.type.Equals("dir"))
                    {
                        string format = item.name.Split('.')[item.name.Split('.').Length - 1].ToLower();
                        if (_formats.Contains(format)) images.Add(item.path);
                    }
                }
            }
            return images;
        }

        private List<string> GetAudios(string path)
        {
            List<string> audios = new List<string>();
            string requestUrl = new EWXRequestBuilder(EWXTelegramBot.GetYandexAPI())
                .SetMethod("disk/resources")
                .AddParameter("path", path)
                .AddParameter("limit", "1000")
                .BuildRequest();
            YandexDiskResourcesGetResponse response = JsonSerializer.Deserialize<YandexDiskResourcesGetResponse>(EWXTelegramBot.SendRequest(requestUrl, _yandexToken, "GET"));
            if (response.error == null)
            {
                foreach (YandexDiskResourcesItem item in response._embedded.items)
                {
                    if (item.name.Contains('.') && !item.type.Equals("dir"))
                    {
                        string format = item.name.Split('.')[item.name.Split('.').Length - 1].ToLower();
                        if (format.Equals("mp3")) audios.Add(item.path);
                    }
                }
            }
            return audios;
        }

        private void DownloadAll()
        {

        }
    }
}
