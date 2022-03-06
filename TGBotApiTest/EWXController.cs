﻿using me.ewerestr.ewxtelegrambot.Responses;
using me.ewerestr.ewxtelegrambot.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Web;
using System.Web.Security;

namespace me.ewerestr.ewxtelegrambot
{
    public class EWXController
    {
        // vars block
        [JsonIgnore]
        private EWXComponentStatus _status = EWXComponentStatus.WarmingUp;                             // common

        [JsonIgnore]
        private Thread _controllerThread;
        [JsonIgnore]// controller
        private Thread _longpollThread;                                 // longpoll

        public bool _postAgain { get; set; } = false;                              // controller
        public bool _allowController { get; set; } = false;                          // controller
        public bool _allowLongpoll { get; set; } = false;                            // longpoll
        //private bool _saveAfterPost = true;                             // controller
        //private bool _serviceBool = false;

        public int _refreshCooldown { get; set; } = 15;                              // controller
        public int _deviation { get; set; } = 1;                                         // controller   / need to make def value
        public int _timeout { get; set; } = 5;                                       // refresh delay (seconds)  ::  longpolltimeout
        [JsonIgnore]
        private int _invites = 0;                                       // longpoll
        public int _secretLength { get; set; } = 32;
        public int _syncCooldown { get; set; } = 60;

        public long _offset { get; set; } = 0;                                       // longpoll
        public long _lastSync { get; set; } = 0;                                     // yandex

        public string _lastPostDate { get; set; } = "neverbefore";                   // controller
        public string _telegramToken { get; set; }                                  // tg ins
        public string _myself { get; set; }                                         // tg longpoll
        public string _secret { get; set; } = null;                                  // longpoll
        public string _channelSecret { get; set; } = null;                           // longpoll
        [JsonIgnore]
        private string _error;                                          // common
        public string _yandexToken { get; set; }                                    //yandex

        public byte[] _postTime { get; set; } = new byte[2] { 10, 0 };               // 0 - hours, 1 - minutes / controller

        public List<string> _telegramPeers { get; set; } = new List<string>();       // tg ins
        public List<TLAdmin> _telegramAdmins { get; set; } = new List<TLAdmin>();      // tg longpoll
        //private List<string> _postedAudios = new List<string>();        // controller
        [JsonIgnore]
        private EWXLocalData localdata;

        [JsonIgnore]
        private const string _yapi = "https://cloud-api.yandex.net/v1/";
        [JsonIgnore]
        private const string _tapi = "https://api.telegram.org/bot";

        // entry block
        public EWXController()
        {
            _status = EWXComponentStatus.WarmingUp;
            //init();
            //ScanLocalFolders();
            EWXTelegramBot.PrintLine("Главный вичислительный модуль инициализирован. Подготовка к запуску...");
        }

        // method block
        // init/start/stop
        public void Start()
        {
            try
            {
                if (_status.Equals(EWXComponentStatus.Working))
                {
                    EWXTelegramBot.PrintLine("Компонент уже запущен");
                    return;
                }
                if (string.IsNullOrEmpty(_telegramToken) || string.IsNullOrEmpty(_yandexToken))
                {
                    EWXTelegramBot.PrintLine("Необходимо указать Telegram токен бота и/или авторизоваться в сервисах Yandex перед тем, как запускать главный вычислительный модуль");
                    return;
                }
                if (File.Exists(EWXTelegramBot.GetDataFolder() + Path.DirectorySeparatorChar + "localdataholder.ewx")) localdata = EWXTelegramBot.GetLocalData();
                else localdata = new EWXLocalData();
                if (CheckApplicationFolder()) SyncData();
                else { /* Notify user */ }
                EWXTelegramBot.PrintLine("Программа получила команду на запуск. Запуст главного вычислительного модуля...");
                _allowController = true;
                _allowLongpoll = true;
                _status = EWXComponentStatus.Working;
                _longpollThread = new Thread(Longpoll);
                _controllerThread = new Thread(Controller);
                _longpollThread.Start();
                _controllerThread.Start();
                if (_telegramPeers.Count == 0) EWXTelegramBot.PrintLine("Внимание! Список получателей пуст. Добавьте получателей при помощи команд \"generatechannelsecret\" или \"setinvites <number>\"");
            }
            catch (Exception e)
            {
                SendMessage("An error occured at EWXController.Start()" + Environment.NewLine + "See last log to get more details");
                Console.WriteLine("An error occured at EWXController.Start()" + Environment.NewLine + "See last log to get more details");
                EWXTelegramBot.PrintService("[WARN] Class:EWXController Method:Start IncomingParams:none");
                EWXTelegramBot.PrintService("[WARN] Exception: " + e.GetType().ToString() + "; Message: " + e.Message);
                EWXTelegramBot.PrintService("[WARN] StackTrace: " + e.StackTrace);
            }
        }

        public void Stop()
        {
            try
            {
                EWXTelegramBot.PrintLine("Выполнение программы остановлено. Прерывание потоков модулей...");
                _allowController = false;
                _allowLongpoll = false;
                Thread.Sleep(1000);
                if (_controllerThread != null) _controllerThread.Abort();
                if (_longpollThread != null) _longpollThread.Abort();
                EWXTelegramBot.PrintLine("Главный вычислительный модуль остановлен. Уничтожение компонента...");
                EWXTelegramBot.PrintLine("Компонент уничтожен");
            }
            catch (Exception e)
            {
                SendMessage("An error occured at EWXController.Stop()" + Environment.NewLine + "See last log to get more details");
                Console.WriteLine("An error occured at EWXController.Stop()" + Environment.NewLine + "See last log to get more details");
                EWXTelegramBot.PrintService("[WARN] Class:EWXController Method:Stop IncomingParams:none");
                EWXTelegramBot.PrintService("[WARN] Exception: " + e.GetType().ToString() + "; Message: " + e.Message);
                EWXTelegramBot.PrintService("[WARN] StackTrace: " + e.StackTrace);
            }
        }

        // controller
        private void Controller()
        {
            try
            {
                EWXTelegramBot.PrintLine("Главный вычислительный модуль запущен");
                while (_allowController)
                {
                    if (CheckTime())
                    {
                        Post();
                        if (CheckUpdates())
                        {
                            EWXTelegramBot.PrintLine("Новая версия EWX TelegramBot уже доступна! Используйте команду \"installUpdate\", чтобы начать обновление.");
                            SendMessage("Новая версия EWX TelegramBot уже доступна! Используйте команду \"installUpdate\", чтобы начать обновление.");
                        }
                        SyncData();
                        localdata.Save();
                        EWXTelegramBot.SaveAll();
                    }
                    //if (EWXTelegramBot.GetCurrentTimeMillis() >= (_lastSync + (_syncCooldown * 1000 * 60))) SyncData();
                    Thread.Sleep(_refreshCooldown * 1000);
                }
            }
            catch (Exception e)
            {
                SendMessage("An error occured at EWXController.Controller()" + Environment.NewLine + "See last log to get more details");
                Console.WriteLine("An error occured at EWXController.Controller()" + Environment.NewLine + "See last log to get more details");
                EWXTelegramBot.PrintService("[WARN] Class:EWXController Method:Controller IncomingParams:none");
                EWXTelegramBot.PrintService("[WARN] Exception: " + e.GetType().ToString() + "; Message: " + e.Message);
                EWXTelegramBot.PrintService("[WARN] StackTrace: " + e.StackTrace);
            }
        }

        // longpoll
        private void Longpoll()
        {
            try
            {
                while (_allowLongpoll)
                {
                    string url = new EWXRequestBuilder(_tapi + _telegramToken)
                        .SetMethod("getUpdates")
                        .AddParameter("timeout", _timeout.ToString())
                        .AddParameter("offset", _offset.ToString())
                        .BuildRequest();
                    TelegramGetUpdatesResponse response = JsonSerializer.Deserialize<TelegramGetUpdatesResponse>(EWXTelegramBot.SendRequest(url));
                    if (response.ok && response.result.Length > 0)
                    {
                        if (response.result.Length > 0)
                        {
                            foreach (TelegramGetUpdatesItem item in response.result)
                            {
                                _offset = item.update_id + 1;
                                if (!string.IsNullOrEmpty(_secret))
                                {
                                    if (item.message != null)
                                    {
                                        if (_secret.Equals(item.message.text))
                                        {
                                            _telegramAdmins.Add(new TLAdmin() { username = item.message.from.username, id = item.message.from.id });
                                            SendMessage(item.message.from.id.ToString(), "Принято! Теперь Вы администратор!");
                                            EWXTelegramBot.PrintLine(item.message.from.first_name + " " + item.message.from.last_name + " (@" + item.message.from.username + ") теперь администратор программы");
                                            _secret = null;
                                            continue;
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(_channelSecret))
                                {
                                    if (item.channel_post != null)
                                    {
                                        if (!string.IsNullOrEmpty(item.channel_post.text) && item.channel_post.text.Equals(_channelSecret))
                                        {
                                            _telegramPeers.Add(item.channel_post.chat.id.ToString());
                                            EWXTelegramBot.PrintLine("Канал " + item.channel_post.chat.title + " добавлен в список получателей при помощи секретного кода");
                                            _channelSecret = null;
                                            continue;
                                        }
                                    }
                                }
                                if (_invites > 0)
                                {
                                    if (item.my_chat_member != null)
                                    {
                                        if (item.my_chat_member.new_chat_member != null)
                                        {
                                            if (item.my_chat_member.new_chat_member.user.username.Equals(_myself))
                                            {
                                                _telegramPeers.Add(item.my_chat_member.chat.id.ToString());
                                                EWXTelegramBot.PrintLine("Канал " + item.my_chat_member.chat.title + " добавлен в список получателей по приглашению");
                                                _invites--;
                                            }
                                        }
                                    }
                                }
                                if (_telegramAdmins.Count == 0) continue;
                                if (item.message != null)
                                {
                                    if (item.message.text != null)
                                    {
                                        if (item.message.chat.type.Equals("private"))
                                        {
                                            foreach (TLAdmin a in _telegramAdmins)
                                            {
                                                if (a.username.Equals(item.message.chat.username)) break;
                                                return;
                                            }
                                            EWXTelegramBot.GetCommandHandler().HandleCommand(new EWXCommand(item.message.text[0].Equals('/') ? item.message.text.Remove(0, 1) : item.message.text), item.message.from.id.ToString());
                                        }
                                    }
                                }
                            }
                        }
                    }
                    Thread.Sleep(_timeout * 1000);
                }
            }
            catch (Exception e)
            {
                SendMessage("An error occured at EWXController.Longpoll()" + Environment.NewLine + "See last log to get more details");
                Console.WriteLine("An error occured at EWXController.Longpoll()" + Environment.NewLine + "See last log to get more details");
                EWXTelegramBot.PrintService("[WARN] Class:EWXController Method:Longpoll IncomingParams:none");
                EWXTelegramBot.PrintService("[WARN] Exception: " + e.GetType().ToString() + "; Message: " + e.Message);
                EWXTelegramBot.PrintService("[WARN] StackTrace: " + e.StackTrace);
            }
        }

        // telegram interract
        public void SendMessage(string peer, string text)
        {
            try
            {
                long q;
                string lPeer = long.TryParse(peer, out q) ? peer : "@" + peer;
                EWXRequestBuilder builder = new EWXRequestBuilder(_tapi + _telegramToken)
                    .SetMethod("sendMessage")
                    .AddParameter("chat_id", lPeer)
                    .AddParameter("text", HttpUtility.UrlEncode(text));
                string rawResponse = EWXTelegramBot.SendRequest(builder.BuildRequest());
            }
            catch (Exception e)
            {
                SendMessage("An error occured at EWXController.SendMessage()" + Environment.NewLine + "See last log to get more details");
                Console.WriteLine("An error occured at EWXController.SendMessage()" + Environment.NewLine + "See last log to get more details");
                EWXTelegramBot.PrintService("[WARN] Class:EWXController Method:SendMessage IncomingParams:peer=" + peer + " text=\""+ text + "\"");
                EWXTelegramBot.PrintService("[WARN] Exception: " + e.GetType().ToString() + "; Message: " + e.Message);
                EWXTelegramBot.PrintService("[WARN] StackTrace: " + e.StackTrace);
            }
        }

        public void SendMessage(string text)
        {
            try
            {
                foreach (TLAdmin a in _telegramAdmins)
                {
                    EWXRequestBuilder builder = new EWXRequestBuilder(_tapi + _telegramToken)
                    .SetMethod("sendMessage")
                    .AddParameter("chat_id", a.id.ToString())
                    .AddParameter("text", HttpUtility.UrlEncode(text));
                    string rawResponse = EWXTelegramBot.SendRequest(builder.BuildRequest());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occured at EWXController.SendMessage()" + Environment.NewLine + "See last log to get more details");
                EWXTelegramBot.PrintService("[WARN] Class:EWXController Method:SendMessage IncomingParams:text=\"" + text + "\"");
                EWXTelegramBot.PrintService("[WARN] Exception: " + e.GetType().ToString() + "; Message: " + e.Message);
                EWXTelegramBot.PrintService("[WARN] StackTrace: " + e.StackTrace);
            }
        }

        public void SendPhoto(string filePath)
        {
            try
            {
                string rawResponse;
                if (_telegramPeers.Count == 0)
                {
                    EWXTelegramBot.PrintLine("Не удалось отправить изображение, потому что список получателей пуст");
                    return;
                }
                foreach (string peer in _telegramPeers)
                {
                    long q;
                    string lPeer = long.TryParse(peer, out q) ? peer : "@" + peer;
                    EWXRequestBuilder builder = new EWXRequestBuilder(_tapi + _telegramToken)
                        .SetMethod("sendPhoto")
                        .AddParameter("chat_id", lPeer)
                        .AddParameter("photo", string.Empty);
                    using (var form = new MultipartFormDataContent())
                    {
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            form.Add(new StreamContent(fileStream), "photo", Path.GetFileName(filePath));
                            using (var client = new HttpClient())
                            {
                                rawResponse = client.PostAsync(builder.BuildRequest(), form).Result.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                SendMessage("An error occured at EWXController.SendImage()" + Environment.NewLine + "See last log to get more details");
                Console.WriteLine("An error occured at EWXController.SendImage()" + Environment.NewLine + "See last log to get more details");
                EWXTelegramBot.PrintService("[WARN] Class:EWXController Method:SendImage IncomingParams:filePath=\"" + filePath + "\"");
                EWXTelegramBot.PrintService("[WARN] Exception: " + e.GetType().ToString() + "; Message: " + e.Message);
                EWXTelegramBot.PrintService("[WARN] StackTrace: " + e.StackTrace);
            }
        }

        public void SendAudio(string filePath)
        {
            try
            {
                if (_telegramPeers.Count == 0)
                {
                    EWXTelegramBot.PrintLine("Не удалось отправить аудиозапись, потому что список получателей пуст");
                    return;
                }
                string rawResponse;
                foreach (string peer in _telegramPeers)
                {
                    long q;
                    string lPeer = long.TryParse(peer, out q) ? peer : "@" + peer;
                    EWXRequestBuilder builder = new EWXRequestBuilder(_tapi + _telegramToken)
                        .SetMethod("sendAudio")
                        .AddParameter("chat_id", lPeer)
                        .AddParameter("audio", string.Empty);
                    string url = builder.BuildRequest();
                    using (var form = new MultipartFormDataContent())
                    {
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            form.Add(new StreamContent(fileStream), "audio", Path.GetFileName(filePath));
                            using (var client = new HttpClient())
                            {
                                rawResponse = client.PostAsync(builder.BuildRequest(), form).Result.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                SendMessage("An error occured at EWXController.SendAudio()" + Environment.NewLine + "See last log to get more details");
                Console.WriteLine("An error occured at EWXController.SendAudio()" + Environment.NewLine + "See last log to get more details");
                EWXTelegramBot.PrintService("[WARN] Class:EWXController Method:SendAudio IncomingParams:filePath=\"" + filePath + "\"");
                EWXTelegramBot.PrintService("[WARN] Exception: " + e.GetType().ToString() + "; Message: " + e.Message);
                EWXTelegramBot.PrintService("[WARN] StackTrace: " + e.StackTrace);
            }
        }

        public string TestTelegramToken(string token)
        {
            try
            {
                string url = new EWXRequestBuilder(_tapi + token)
                .SetMethod("getMe")
                .BuildRequest();
                TelegramGetMeResponse response = JsonSerializer.Deserialize<TelegramGetMeResponse>(EWXTelegramBot.SendRequest(url));
                if (response.error == null)
                {
                    if (!string.IsNullOrEmpty(response.result.username))
                    {
                        if (string.IsNullOrEmpty(token))
                        {
                            EWXTelegramBot.PrintLine("Сохраненный ранее Telegram токен прошел валидацию");
                            _status = EWXComponentStatus.ReadyToWork;
                            _telegramToken = token;
                        }
                        return response.result.username;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(token))
                        {
                            EWXTelegramBot.PrintLine("Сохраненный ранее Telegram токен не прошел валидацию. Укажите актуальный токен при помощи команды \"setTelegramToken\"");
                            _status = EWXComponentStatus.Error;
                        }
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occured at EWXController.TestTelegramToken()" + Environment.NewLine + "See last log to get more details");
                EWXTelegramBot.PrintService("[WARN] Class:EWXController Method:TestTelegramToken IncomingParams:token=\"" + token + "\"");
                EWXTelegramBot.PrintService("[WARN] Exception: " + e.GetType().ToString() + "; Message: " + e.Message);
                EWXTelegramBot.PrintService("[WARN] StackTrace: " + e.StackTrace);
            }
            return null;
        }

        // yandex interract
        private bool TestYandexToken(string token)
        {
            try
            {
                YandexDiskResponse response = JsonSerializer.Deserialize<YandexDiskResponse>(EWXTelegramBot.SendRequest(new EWXRequestBuilder(_yapi).SetMethod("disk").BuildRequest(), token));
                if (response.error == null && response.user.display_name != null)
                {
                    EWXTelegramBot.PrintLine("Сохраненный ранее Yandex токен прошел валидацию");
                    return true;
                }
                else
                {
                    EWXTelegramBot.PrintLine("Сохраненный ранее Yandex токен не прошел валидацию. Авторизуйтесь в сервисах Yandex при помощи команды \"authYandex\"");
                    return false;
                }
            }
            catch (Exception e)
            {
                SendMessage("An error occured at EWXController.TestYandexToken()" + Environment.NewLine + "See last log to get more details");
                Console.WriteLine("An error occured at EWXController.TestYandexToken()" + Environment.NewLine + "See last log to get more details");
                EWXTelegramBot.PrintService("[WARN] Class:EWXController Method:TestYandexToken IncomingParams:token=\"" + token + "\"");
                EWXTelegramBot.PrintService("[WARN] Exception: " + e.GetType().ToString() + "; Message: " + e.Message);
                EWXTelegramBot.PrintService("[WARN] StackTrace: " + e.StackTrace);
            }
            return false;
        }

        public void AuthorizeYandex()
        {
            try
            {
                EWXTelegramBot.PrintLine("Запуск модуля EWXCallbackListener для авторизации в сервисах Yandex...");
                new EWXCallbackListener();
                EWXTelegramBot.PrintLine("Выполнение модуля EWXCallbackListener завершено");
            }
            catch (Exception e)
            {
                SendMessage("An error occured at EWXController.AuthorizeYandex()" + Environment.NewLine + "See last log to get more details");
                Console.WriteLine("An error occured at EWXController.AuthorizeYandex()" + Environment.NewLine + "See last log to get more details");
                EWXTelegramBot.PrintService("[WARN] Class:EWXController Method:AuthorizeYandex IncomingParams:none");
                EWXTelegramBot.PrintService("[WARN] Exception: " + e.GetType().ToString() + "; Message: " + e.Message);
                EWXTelegramBot.PrintService("[WARN] StackTrace: " + e.StackTrace);
            }
        }

        private bool CheckApplicationFolder()
        {
            try
            {
                string url = new EWXRequestBuilder(_yapi)
                .SetMethod("disk/resources")
                .AddParameter("path", "EWXTelegramBot")
                .BuildRequest();
                ErrorResponse response = JsonSerializer.Deserialize<ErrorResponse>(EWXTelegramBot.SendRequest(url, _yandexToken, "GET"));
                if (response.error != null)
                {
                    url = new EWXRequestBuilder(_yapi)
                        .SetMethod("disk/resources")
                        .AddParameter("path", "EWXTelegramBot")
                        .BuildRequest();
                    response = JsonSerializer.Deserialize<ErrorResponse>(EWXTelegramBot.SendRequest(url, _yandexToken, "PUT"));
                    if (response.error != null) EWXTelegramBot.PrintLine("Не удалось создать папку \"EWXTelegramBot\" в облаке Yandex. Описание ошибки >> " + response.description);
                    url = new EWXRequestBuilder(_yapi)
                        .SetMethod("disk/resources")
                        .AddParameter("path", "EWXTelegramBot/images")
                        .BuildRequest();
                    response = JsonSerializer.Deserialize<ErrorResponse>(EWXTelegramBot.SendRequest(url, _yandexToken, "PUT"));
                    if (response.error != null) EWXTelegramBot.PrintLine("Не удалось создать папку  \"EWXTelegramBot\\images\" в облаке Yandex. Описание ошибки >> " + response.description);
                    url = new EWXRequestBuilder(_yapi)
                        .SetMethod("disk/resources")
                        .AddParameter("path", "EWXTelegramBot/audios")
                        .BuildRequest();
                    response = JsonSerializer.Deserialize<ErrorResponse>(EWXTelegramBot.SendRequest(url, _yandexToken, "PUT"));
                    if (response.error != null) EWXTelegramBot.PrintLine("Не удалось создать папку  \"EWXTelegramBot\\audios\" в облаке Yandex. Описание ошибки >> " + response.description);
                    return false;
                }
                else EWXTelegramBot.PrintLine("Папка приложения найдена на облаке Yandex");
                return true;
            }
            catch (Exception e)
            {
                SendMessage("An error occured at EWXController.ChechApplicationFolder()" + Environment.NewLine + "See last log to get more details");
                Console.WriteLine("An error occured at EWXController.CheckApplicationFolder()" + Environment.NewLine + "See last log to get more details");
                EWXTelegramBot.PrintService("[WARN] Class:EWXController Method:ChechApplicationFolder IncomingParams:none");
                EWXTelegramBot.PrintService("[WARN] Exception: " + e.GetType().ToString() + "; Message: " + e.Message);
                EWXTelegramBot.PrintService("[WARN] StackTrace: " + e.StackTrace);
            }
            return false;
        }

        public void SyncData()
        {
            try
            {
                List<string> newPhotos = new List<string>();
                List<string> newAudios = new List<string>();

                string url = new EWXRequestBuilder(_yapi).SetMethod("disk/resources").AddParameter("path", "EWXTelegramBot/images").AddParameter("limit", "1000").BuildRequest();
                YandexDiskResourcesGetResponse response = JsonSerializer.Deserialize<YandexDiskResourcesGetResponse>(EWXTelegramBot.SendRequest(url, _yandexToken, "GET"));
                if (response.error == null)
                {
                    foreach (YandexDiskResourcesItem file in response._embedded.items)
                    {
                        string lString = file.name;
                        if (EWXTelegramBot.HasCyrillic(lString)) lString = EWXTelegramBot.Transliterate(lString);
                        if (localdata.HasImage(lString)) continue;
                        else
                        {
                            newPhotos.Add(lString);
                            url = new EWXRequestBuilder(_yapi).SetMethod("disk/resources/download").AddParameter("path", file.path).BuildRequest();
                            YandexDiskResourceDownloadResponse downloadResponse = JsonSerializer.Deserialize<YandexDiskResourceDownloadResponse>(EWXTelegramBot.SendRequest(url, _yandexToken, "GET"));
                            WebClient client = new WebClient();
                            string filePath = EWXTelegramBot.GetDataFolder() + Path.DirectorySeparatorChar + "localdataholder" + Path.DirectorySeparatorChar + "images" + Path.DirectorySeparatorChar + lString;
                            client.DownloadFile(downloadResponse.href, filePath);
                            EWXTelegramBot.PrintLine("Файл " + lString + " загружен в локальное хранилище");
                        }
                    }
                    if (newPhotos.Count > 0)
                    {
                        localdata.AddImages(newPhotos);
                        EWXTelegramBot.PrintLine("Загружено " + newPhotos.Count + " новых изображений");
                    }
                    _lastSync = EWXTelegramBot.GetCurrentTimeMillis();
                }
                else EWXTelegramBot.PrintLine("Не удалось получить содержимое облака Yandex. Описание ошибки >> " + response.description);
                url = new EWXRequestBuilder(_yapi).SetMethod("disk/resources").AddParameter("path", "EWXTelegramBot/audios").AddParameter("limit", "1000").BuildRequest();
                response = JsonSerializer.Deserialize<YandexDiskResourcesGetResponse>(EWXTelegramBot.SendRequest(url, _yandexToken, "GET"));
                if (response.error == null)
                {
                    foreach (YandexDiskResourcesItem file in response._embedded.items)
                    {
                        string lString = file.name;
                        if (EWXTelegramBot.HasCyrillic(lString)) lString = EWXTelegramBot.Transliterate(lString);
                        if (localdata.HasAudio(lString)) continue;
                        else
                        {
                            newAudios.Add(lString);
                            url = new EWXRequestBuilder(_yapi).SetMethod("disk/resources/download").AddParameter("path", file.path).BuildRequest();
                            YandexDiskResourceDownloadResponse downloadResponse = JsonSerializer.Deserialize<YandexDiskResourceDownloadResponse>(EWXTelegramBot.SendRequest(url, _yandexToken, "GET"));
                            WebClient client = new WebClient();
                            string filePath = EWXTelegramBot.GetDataFolder() + Path.DirectorySeparatorChar + "localdataholder" + Path.DirectorySeparatorChar + "audios" + Path.DirectorySeparatorChar + lString;
                            client.DownloadFile(downloadResponse.href, filePath);
                            EWXTelegramBot.PrintLine("Файл " + lString + " загружен в локальное хранилище");
                        }
                    }
                    if (newAudios.Count > 0)
                    {
                        localdata.AddAudios(newAudios);
                        EWXTelegramBot.PrintLine("Загружено " + newAudios.Count + " новых аудиозаписей");
                    }
                    _lastSync = EWXTelegramBot.GetCurrentTimeMillis();
                    localdata.Save();
                }
                else EWXTelegramBot.PrintLine("Не удалось получить содержимое облака Yandex. Описание ошибки >> " + response.description);
            }
            catch (Exception e)
            {
                SendMessage("An error occured at EWXController.SyncData()" + Environment.NewLine + "See last log to get more details");
                Console.WriteLine("An error occured at EWXController.SyncData()" + Environment.NewLine + "See last log to get more details");
                EWXTelegramBot.PrintService("[WARN] Class:EWXController Method:SyncData IncomingParams:none");
                EWXTelegramBot.PrintService("[WARN] Exception: " + e.GetType().ToString() + "; Message: " + e.Message);
                EWXTelegramBot.PrintService("[WARN] StackTrace: " + e.StackTrace);
            }
        }

        // configuration / datafolder interract

        private void ScanLocalFolders()
        {
            try
            {
                localdata = new EWXLocalData();
                string path = EWXTelegramBot.GetDataFolder() + Path.DirectorySeparatorChar + "localdataholder";
                if (Directory.Exists(path))
                {
                    path = path + Path.DirectorySeparatorChar;
                    if (Directory.Exists(path + "images") && Directory.Exists(path + "audios"))
                    {
                        foreach (string file in Directory.GetFiles(path + "images")) localdata.AddImage(Path.GetFileName(file));
                        EWXTelegramBot.PrintLine("Обнаружено " + localdata.GetImagesCount() + " изображений в локальном хранилище");
                        foreach (string file in Directory.GetFiles(path + "audios")) localdata.AddAudio(Path.GetFileName(file));
                        EWXTelegramBot.PrintLine("Обнаружено " + localdata.GetAudiosCount() + " аудиозаписей в локальном хранилище");
                    }
                    else EWXTelegramBot.PrintLine("Локальное хранилище сознано ранее, но не имеет подпапок. Ожидание заполнения подпапок файлами с облака Yandex");
                }
                else EWXTelegramBot.PrintLine("Локальное хранилище не было создано ранее. Ожидание заполнения подпапок файлами с облака Yandex");
                localdata.Save();
            }
            catch (Exception e)
            {
                SendMessage("An error occured at EWXController.ScanLocalFolders()" + Environment.NewLine + "See last log to get more details");
                Console.WriteLine("An error occured at EWXController.ScanLocalFolders()" + Environment.NewLine + "See last log to get more details");
                EWXTelegramBot.PrintService("[WARN] Class:EWXController Method:ScanLocalFolders IncomingParams:none");
                EWXTelegramBot.PrintService("[WARN] Exception: " + e.GetType().ToString() + "; Message: " + e.Message);
                EWXTelegramBot.PrintService("[WARN] StackTrace: " + e.StackTrace);
            }
        }

        // local utils
        public bool Post()
        {
            try
            {
                //localdata = EWXTelegramBot.GetLocalData();
                string photo;
                string audio;
                Random random = new Random();
                if (localdata.HasUnpostedImages()) photo = localdata.GetRandomImage();
                else if (_postAgain)
                {
                    localdata.RenewImages();
                    photo = localdata.GetRandomImage();
                }
                else photo = null;
                if (localdata.HasUnpostedAudios()) audio = localdata.GetRandomAudio();
                else if (_postAgain)
                {
                    localdata.RenewAudios();
                    audio = localdata.GetRandomAudio();
                }
                else audio = null;
                if (string.IsNullOrEmpty(photo) || string.IsNullOrEmpty(audio))
                {
                    EWXTelegramBot.PrintLine("Не удалось опубликовать запись. Список изображений или список файлов пуст");
                    _error = "Failed to post. Can't get photo or audio because the list is clear";
                    //_lastPostDate = DateTime.Now.ToString("G").Split(' ')[0];
                    return false;
                }
                photo = EWXTelegramBot.GetDataFolder() + Path.DirectorySeparatorChar + "localdataholder" + Path.DirectorySeparatorChar + "images" + Path.DirectorySeparatorChar + photo;
                audio = EWXTelegramBot.GetDataFolder() + Path.DirectorySeparatorChar + "localdataholder" + Path.DirectorySeparatorChar + "audios" + Path.DirectorySeparatorChar + audio;
                SendPhoto(photo);
                SendAudio(audio);
                _lastPostDate = DateTime.Now.ToString("G").Split(' ')[0];
                //EWXTelegramBot.PrintLine("[EWXController] The post has been successfuly delegated to TelegramInstance);
                return true;
            }
            catch (Exception e)
            {
                SendMessage("An error occured at EWXController.Post()" + Environment.NewLine + "See last log to get more details");
                Console.WriteLine("An error occured at EWXController.Post()" + Environment.NewLine + "See last log to get more details");
                EWXTelegramBot.PrintService("[WARN] Class:EWXController Method:Post IncomingParams:none");
                EWXTelegramBot.PrintService("[WARN] Exception: " + e.GetType().ToString() + "; Message: " + e.Message);
                EWXTelegramBot.PrintService("[WARN] StackTrace: " + e.StackTrace);
            }
            return false;
        }

        private bool CheckTime()
        {
            bool output = false;
            try
            {
                string currentDay;
                byte[] time = new byte[2];
                string[] q = DateTime.Now.ToString("G").Split(' ');
                currentDay = q[0];
                q = q[1].Split(':');
                time[0] = byte.Parse(q[0]);
                time[1] = byte.Parse(q[1]);
                if (_lastPostDate != currentDay) if (_postTime[0] == time[0] && (time[1] >= _postTime[1] && time[1] <= _postTime[1] + _deviation))
                    {
                        EWXTelegramBot.PrintLine("Время публикации новой записи пришло. Подготовка материалов...");
                        output = true;
                    }
                else output = false;
            }
            catch (Exception e)
            {
                SendMessage("An error occured at EWXController.CheckTime()" + Environment.NewLine + "See last log to get more details");
                Console.WriteLine("An error occured at EWXController.CheckTime()" + Environment.NewLine + "See last log to get more details");
                EWXTelegramBot.PrintService("[WARN] Class:EWXController Method:CheckTime IncomingParams:none");
                EWXTelegramBot.PrintService("[WARN] Exception: " + e.GetType().ToString() + "; Message: " + e.Message);
                EWXTelegramBot.PrintService("[WARN] StackTrace: " + e.StackTrace);
            }
            return output;
        }

        public string GetLiteStatus()
        {
            string status = "# # # # PRINTSTATUS START # # # #" + Environment.NewLine;
            try
            {
                status += "Component status: " + _status.ToString() + Environment.NewLine;
                //status += "Posted materials: Images :> " + _postedPhotos.Count + " :: Audios :> " + _postedAudios.Count + Environment.NewLine;
                //status += "Unposted materials: Images :> " + _unpostedPhotoList.Count + " :: Audios :> " + _unpostedAudioList.Count + Environment.NewLine;
                status += "Last post date: " + _lastPostDate + Environment.NewLine;
                status += "# # # # PRINTSTATUS END # # # # #";
            }
            catch (Exception e)
            {
                SendMessage("An error occured at EWXController.GetLiteStatus()" + Environment.NewLine + "See last log to get more details");
                Console.WriteLine("An error occured at EWXController.GetLiteStatus()" + Environment.NewLine + "See last log to get more details");
                EWXTelegramBot.PrintService("[WARN] Class:EWXController Method:GetLiteStatus IncomingParams:none");
                EWXTelegramBot.PrintService("[WARN] Exception: " + e.GetType().ToString() + "; Message: " + e.Message);
                EWXTelegramBot.PrintService("[WARN] StackTrace: " + e.StackTrace);
            }
            return status;
        }

        public string GetDetailedStatus()
        {
            string lString = "";
            string status = "# # # # PRINTDETSTATUS START # # # #" + Environment.NewLine;
            try
            {
                status += "Component status: " + _status.ToString() + Environment.NewLine;
                status += "Can the bot post again: " + (_postAgain ? "Yes" : "No") + Environment.NewLine;
                status += "*Refresh cooldown: " + _refreshCooldown + Environment.NewLine;
                status += "*Deviation: " + _deviation + Environment.NewLine;
                status += "*Timeout: " + _timeout + Environment.NewLine;
                status += "Current invites: " + _invites + Environment.NewLine;
                status += "Current secretcode length: " + _secretLength + Environment.NewLine;
                status += "*Sync cooldown: " + _syncCooldown + Environment.NewLine;
                status += "Current longpoll offset: " + _offset + Environment.NewLine;
                status += "Last sync date in milliseconds: " + _lastSync + Environment.NewLine;
                status += "Last post date: " + _lastPostDate + Environment.NewLine;
                status += "*Current telegram token: " + _telegramToken + Environment.NewLine;
                status += "Bot's telegram name: " + _myself + Environment.NewLine;
                status += "Current secret: " + (string.IsNullOrEmpty(_secret) ? "no secret code" : _secret) + Environment.NewLine;
                status += "Current channel secret: " + (string.IsNullOrEmpty(_channelSecret) ? "no channel secret code" : _channelSecret) + Environment.NewLine;
                status += "Last error message: " + _error + Environment.NewLine;
                status += "*Current Yandex token: " + _yandexToken + Environment.NewLine;
                status += "*Post time: " + (_postTime[0] < 10 ? ("0" + _postTime[0]) : _postTime[0].ToString()) + ":" + (_postTime[1] < 10 ? ("0" + _postTime[1]) : _postTime[1].ToString()) + Environment.NewLine;
                if (_telegramAdmins.Count > 0)
                {

                    foreach (TLAdmin a in _telegramAdmins) lString += a.username + ",";
                    lString = lString.Remove(lString.Length - 1, 1);
                    status += "Telegram admins: " + lString + Environment.NewLine;
                    lString = string.Empty;
                }
                if (_telegramPeers.Count > 0)
                {

                    foreach (string i in _telegramPeers) lString += i + ",";
                    lString = lString.Remove(lString.Length - 1, 1);
                    status += "Telegram peers: " + lString + Environment.NewLine;
                    lString = string.Empty;
                }
                status += "To get information about current posted materials and materials to post use \"getDataStatus\"" + Environment.NewLine;
                status += "# # # # PRINTDETSTATUS END # # # # #";
            }
            catch (Exception e)
            {
                SendMessage("An error occured at EWXController.GetDetailedStatus()" + Environment.NewLine + "See last log to get more details");
                Console.WriteLine("An error occured at EWXController.GetDetailedStatus()" + Environment.NewLine + "See last log to get more details");
                EWXTelegramBot.PrintService("[WARN] Class:EWXController Method:GetDetailedStatus IncomingParams:none");
                EWXTelegramBot.PrintService("[WARN] Exception: " + e.GetType().ToString() + "; Message: " + e.Message);
                EWXTelegramBot.PrintService("[WARN] StackTrace: " + e.StackTrace);
            }
            return status;
        }

        public string GenerateSecretCode()
        {
            return _secret = Membership.GeneratePassword(_secretLength, 5);
        }

        public string GenerateChannelSecretCode()
        {
            return _channelSecret = Membership.GeneratePassword(_secretLength, 5);
        }

        public bool CheckUpdates()
        {
            try
            {
                bool lBool = false;
                WebClient client = new WebClient();
                Stream stream = client.OpenRead("https://ewerestr.ru/ewxscsversions.txt");
                StreamReader reader = new StreamReader(stream);
                String content = reader.ReadToEnd();
                content = content.Replace(Environment.NewLine, "*");
                string[] builds = content.Split('*');
                foreach (string build in builds) if (build.Contains("EWXTelegramBot")) lBool = build.Contains(" ") ? (int.Parse(build.Split(' ')[1]) > EWXTelegramBot.GetBuild() ? lBool = true : false) : false;
                return lBool;
            }
            catch (Exception e)
            {
                SendMessage("An error occured at EWXController.CheckUpdates()" + Environment.NewLine + "See last log to get more details");
                Console.WriteLine("An error occured at EWXController.CheckUpdates()" + Environment.NewLine + "See last log to get more details");
                EWXTelegramBot.PrintService("[WARN] Class:EWXController Method:CheckUpdates IncomingParams:none");
                EWXTelegramBot.PrintService("[WARN] Exception: " + e.GetType().ToString() + "; Message: " + e.Message);
                EWXTelegramBot.PrintService("[WARN] StackTrace: " + e.StackTrace);
            }
            return false;
        }

        // get/set block
        public EWXComponentStatus GetStatus()
        {
            return _status;
        }
        public void SetPostAgain(bool postagain)
        {
            _postAgain = postagain;
        }

        public void SetRefreshCooldown(int refreshCooldown)
        {
            _refreshCooldown = refreshCooldown;
        }

        public void SetDeviation(int deviation)
        {
            _deviation = deviation;
        }

        public void SetLongpollTimeout(int timeout)
        {
            _timeout = timeout;
        }

        public void SetInvitesCount(int invites)
        {
            _invites = invites;
        }

        public void ResetInvitesCount()
        {
            _invites = 0;
        }

        public void SetTelegramToken(string telegramToken)
        {
            _telegramToken = telegramToken;
        }

        public void SetYandexToken(string yandexToken)
        {
            _yandexToken = yandexToken;
            //InitYandex();
        }

        public void SetSecretLength(int secretLength)
        {
            _secretLength = secretLength;
        }

        public void ResetSecret()
        {
            _secret = null;
        }

        public void ResetChannelSecret()
        {
            _channelSecret = null;
        }

        public string GetLastError()
        {
            return _error;
        }

        //yandextoken

        public void SetLastPostDate(string lastPostDate)
        {
            _lastPostDate = lastPostDate;
        }

        public void SetTimeToPost(byte[] postTime)
        {
            _postTime = postTime;
        }

        public void SetBotUsername(string username)
        {
            _myself = username;
        }

        public bool HasPeers()
        {
            return _telegramPeers.Count > 0 ? true : false;
        }
    }
}