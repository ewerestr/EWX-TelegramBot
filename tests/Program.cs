using GroupDocs.Metadata;
using GroupDocs.Metadata.Formats.Audio;
using GroupDocs.Metadata.Tagging;
using System;
using System.IO;
using System.Net;
using System.Threading;

namespace tests
{
    class Program
    {
        static void Main(string[] args)
        {
            string audio = @"C:\usr\audios\Avril Lavigne - My Happy Ending.mp3";
            string path = @"C:\usr\audios";
            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                var tag = TagLib.File.Create(file);
                if (tag.Tag.FirstAlbumArtist != null) Console.Write(tag.Tag.FirstAlbumArtist);
                else if (tag.Tag.FirstArtist != null) Console.Write(tag.Tag.FirstArtist);
                else if (tag.Tag.FirstPerformer != null) Console.Write(tag.Tag.FirstPerformer);
                Console.Write(" - ");
                if (tag.Tag.Title != null) Console.WriteLine(tag.Tag.Title);
            }


                    string token = "";
            string request = "https://api.telegram.org/bot" + token + "/getChat?chat_id=-1001736794065";
            string response = SendRequest(request);
            Console.WriteLine(response);
            Console.ReadKey();
        }

        public static void Print(string[] args)
        {
            DateTime startPoint;
            int[] dt = new int[] { DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second };
            if (args.Length > 1)
            {
                string starttime = args[1];
                if (starttime.Contains(":"))
                {
                    string[] q = starttime.Split(':');
                    if (int.TryParse(q[0], out dt[4]) && int.TryParse(q[1], out dt[5]))
                    {
                        int a = 0;
                    }
                }
                // else
            }
            int[] ir = ParseCooldown(args[0]);
            if (ir[0] < 0)
            {
                // error
            }
            else
            {
                startPoint = new DateTime(dt[0], dt[1], dt[2], dt[3], dt[4], dt[5]);
                DateTime endPoint = startPoint.AddDays(ir[0]).AddHours(ir[1]).AddMinutes(ir[2]).AddSeconds(ir[3]);
                TimeSpan w = endPoint.Subtract(startPoint);
                Console.ReadKey();
            }
        }

        public static int[] ParseCooldown(string value)
        {
            int[] lReturn = { -1, -1, -1, -1 };
            try
            {
                int s = 0;
                int m = 0;
                int h = 0;
                int d = 0;
                int t = 0;
                int p;
                string lValue = value.ToLower() + "0";
                string[] lArray = lValue.Replace("d", "#").Replace("h", "#").Replace("m", "#").Replace("s", "#").Split('#');
                for (int i = 0; i < lArray.Length - 1; i++)
                {
                    t += lArray[i].Length;
                    switch (value[t])
                    {
                        case 'd':
                            if (Int32.TryParse(lArray[i], out p)) d = p;
                            break;
                        case 'h':
                            if (Int32.TryParse(lArray[i], out p)) h = p;
                            break;
                        case 'm':
                            if (Int32.TryParse(lArray[i], out p)) m = p;
                            break;
                        case 's':
                            if (Int32.TryParse(lArray[i], out p)) s = p;
                            break;
                    }
                    t++;
                }
                lReturn = new int[] { d, h, m, s };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return lReturn;
        }

        public static string SendRequest(string link, string yandexToken = null, string method = null)
        {
            Thread.Sleep(100);
            string web_response;
            try
            {
                WebRequest request = WebRequest.Create(link);
                if (!string.IsNullOrEmpty(yandexToken)) request.Headers.Add("Authorization", "OAuth " + yandexToken);
                request.Credentials = CredentialCache.DefaultCredentials;
                if (!string.IsNullOrEmpty(method)) request.Method = method;
                WebResponse response = request.GetResponse();
                using (Stream dataStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);
                    web_response = reader.ReadToEnd();
                }
                response.Close();
            }
            catch (WebException e)
            {
                //# Print("Method :: sendRequest :: Exception: " + e.Message + "; ServerResponse: " + e.Response, true, true);
                string lError = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                return lError;
            }
            return web_response;
        }
    }
}
