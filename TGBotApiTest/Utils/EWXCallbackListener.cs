using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace me.ewerestr.ewxtelegrambot.Utils
{
    class EWXCallbackListener
    {
        private Thread _listenerThread;
        private bool _waitingResponse = true;

        public EWXCallbackListener()
        {
            EWXTelegramBot.PrintLine("Модуль EWXCallbackListener запущен! Подготовка к запуску компонента HTTPListener...");
            _listenerThread = new Thread(listen);
            _listenerThread.Start();
            string uid = Guid.NewGuid().ToString();
            EWXRequestBuilder builder = new EWXRequestBuilder("https://oauth.yandex.ru/authorize")
                .AddParameter("response_type", "token")
                .AddParameter("client_id", "6407e38e878a4ff399a6edae77d796e5")
                .AddParameter("device_id", Guid.NewGuid().ToString())
                .AddParameter("device_name", Environment.MachineName)
                .AddParameter("display", "popup");
            Process p = Process.Start("iexplore.exe", builder.BuildRequest());
            while (_waitingResponse) Thread.Sleep(5000);
            p.Kill();
            EWXTelegramBot.PrintLine("Поток компонента HTTPListener остановлен.");
            EWXTelegramBot.PrintLine("Модуль EWXCallbackListener получил данные от HTTPListener и обработал их. Подготовка к остановке модуля...");
            EWXTelegramBot.PrintLine("Остановка потока модуля EWXCallbackListener...");
        }

        private void listen()
        {
            EWXTelegramBot.PrintLine("Компонент HTTPListener запущен! Подготовка к прослушиванию...");
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://127.0.0.1:3000/");
            listener.Start();
            EWXTelegramBot.PrintLine("Компонент HTTPListener прослушивает порт 3000");
            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                if (request.Url.ToString().Contains("token"))
                {
                    EWXTelegramBot.PrintLine("Компонент HTTPListener получил входящее подключение. Обработка...");
                    String token = request.Url.ToString().Replace("http://127.0.0.1:3000/callback?access_token=", "").Replace("&token_type=bearer&expires_in=31536000", "");
                    EWXTelegramBot.GetController().SetYandexToken(token);
                    _waitingResponse = false;
                    listener.Stop();
                    listener.Close();
                    break;
                }
                string responseString = "<html><head><meta charset=\"UTF-8\"><title>Redirecting</title></head><body><script>var url = window.location.href;var link = window.location.toString();var array = link.split(\"#\");window.setTimeout(function(){window.location.href = \"http://127.0.0.1:3000/callback?\" + array[1];}, 1000);</script></body></html>";
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
            }
            EWXTelegramBot.PrintLine("Компонент HTTPListener получил и обработал входящие данные. Остановка потока компонента...");
            _listenerThread.Abort();
        }
    }
}
