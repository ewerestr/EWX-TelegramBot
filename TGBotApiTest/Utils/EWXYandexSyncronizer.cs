using me.ewerestr.ewxtelegrambot.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;

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
            _localData = EWXTelegramBot.GetLocalData();
            ScanHierarchy();
            GetSubfolders("disk:/EWXTelegramBot");
            ScanSubfolders();
            DownloadAll();
            Console.WriteLine();
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
            if (fullscanSubfolders.Count > 0) foreach (List<string> list in fullscanSubfolders) _subfolders = _subfolders.Concat(list).ToList();
            _images = _images.Concat(rootImages).ToList();
            if (fullscanImages.Count > 0) foreach (List<string> list in fullscanImages) _images = _images.Concat(list).ToList();
            _audios = _audios.Concat(rootAudios).ToList();
            if (fullscanAudios.Count > 0) foreach (List<string> list in fullscanAudios) _audios = _audios.Concat(list).ToList();
        }

        private void ScanSubfolders()
        {
            List<List<string>> images = new List<List<string>>();
            List<List<string>> audios = new List<List<string>>();
            if (_subfolders.Count > 0)
            {
                foreach (string folder in _subfolders)
                {
                    images.Add(GetImages(folder));
                    audios.Add(GetAudios(folder));
                }
            }
            if (images.Count > 0) foreach (List<string> list in images) _images = _images.Concat(list).ToList();
            if (audios.Count > 0) foreach (List<string> list in audios) _audios = _audios.Concat(list).ToList();
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
            int audioCounter = 0;
            int imageCounter = 0;
            foreach (string image in _images)
            {
                string lString = EWXTelegramBot.ParseRight(Path.GetFileName(image));
                if (_localData.ContainsImage(lString)) continue;
                else
                {
                    string request = new EWXRequestBuilder(EWXTelegramBot.GetYandexAPI()).SetMethod("disk/resources/download").AddParameter("path", image).BuildRequest(); ;
                    YandexDiskResourceDownloadResponse downloadResponse = JsonSerializer.Deserialize<YandexDiskResourceDownloadResponse>(EWXTelegramBot.SendRequest(request, _yandexToken, "GET"));
                    WebClient client = new WebClient();
                    string filePath = EWXTelegramBot.GetDataFolder() + Path.DirectorySeparatorChar + "localdataholder" + Path.DirectorySeparatorChar + "images" + Path.DirectorySeparatorChar + lString;
                    client.DownloadFile(downloadResponse.href, filePath);
                    EWXTelegramBot.PrintLine("Файл " + lString + " загружен в локальное хранилище");
                    _localData.AddImage(lString);
                    imageCounter++;
                }
            }
            foreach (string audio in _audios)
            {
                string lString = EWXTelegramBot.ParseRight(Path.GetFileName(audio));
                if (_localData.ContainsAudio(lString)) continue;
                else
                {
                    string request = new EWXRequestBuilder(EWXTelegramBot.GetYandexAPI()).SetMethod("disk/resources/download").AddParameter("path", audio).BuildRequest(); ;
                    YandexDiskResourceDownloadResponse downloadResponse = JsonSerializer.Deserialize<YandexDiskResourceDownloadResponse>(EWXTelegramBot.SendRequest(request, _yandexToken, "GET"));
                    WebClient client = new WebClient();
                    string filePath = EWXTelegramBot.GetDataFolder() + Path.DirectorySeparatorChar + "localdataholder" + Path.DirectorySeparatorChar + "audios" + Path.DirectorySeparatorChar + lString;
                    client.DownloadFile(downloadResponse.href, filePath);
                    EWXTelegramBot.PrintLine("Файл " + lString + " загружен в локальное хранилище");
                    _localData.AddAudio(lString);
                    audioCounter++;
                }
            }
            Console.WriteLine("Загружено " + imageCounter + " изображений и " + audioCounter + " аудиозаписей");
            _localData.Save();
        }
    }
}
