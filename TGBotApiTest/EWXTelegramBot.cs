using me.ewerestr.ewxtelegrambot.Components;
using me.ewerestr.ewxtelegrambot.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace me.ewerestr.ewxtelegrambot
{
	class EWXTelegramBot
	{
		/* NOTE
		 *  # - marks legacy code
		 */

		// vars block
		private static Thread _commandhandlerThread;
		private static EWXLogger _logger;
		private static EWXCommandHandler _commandHandler;
		private static TimerCallback _timerCallback;
		private static Timer _timer;
		private static EWXController _controller;
		private static bool _allowLogger = true;
		private static bool _debug = true;
		private static bool _componentsReady = false;
		private static string _datafolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
											Path.DirectorySeparatorChar +
											"EWXTelegramBot";
		private const int _build = 0101;
		private const string _telegramApi = "https://api.telegram.org/bot";
		private const string _yandexApi = "https://cloud-api.yandex.net/v1/";

		// entry block
		static void Main(string[] args)
		{
			_logger = new EWXLogger();
			_commandHandler = new EWXCommandHandler();
			_commandhandlerThread = new Thread(_commandHandler.Start);
			InitController();
			_controller.Start();
			_commandhandlerThread.Start(); 
		}

		// methods block
		/*
		 * "https://api.telegram.org/bot" + tg_token + "/sendMessage?chat_id=1157004942&text=" + text
		 * 
		 */
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

		public static void PrintLine(string text, bool b = false)
		{
			Console.WriteLine(b ? text : ">> " + text);
			if (_allowLogger) _logger.Log(text);
		}

		public static void PrintService(string text)
        {
			if (_allowLogger) _logger.Log(text);
		}

		public static void LogInput(string text)
        {
			if (_allowLogger) _logger.Log("<< " + text);
        }

		private static void InitController()
        {
			if (Directory.Exists(_datafolder))
            {
				if (File.Exists(_datafolder + Path.DirectorySeparatorChar + "configuration.ewx"))
				{
					using (StreamReader sr = new StreamReader(EWXTelegramBot.GetDataFolder() + Path.DirectorySeparatorChar + "configuration.ewx", System.Text.Encoding.Default))
					{
						_controller = JsonSerializer.Deserialize<EWXController>(sr.ReadToEnd());
					}
				}
				else _controller = new EWXController();
				if (!Directory.Exists(_datafolder + Path.DirectorySeparatorChar + "localdataholder"))
                {
					Directory.CreateDirectory(_datafolder + Path.DirectorySeparatorChar + "localdataholder");
					Directory.CreateDirectory(_datafolder + Path.DirectorySeparatorChar + "localdataholder" + Path.DirectorySeparatorChar + "audios");
					Directory.CreateDirectory(_datafolder + Path.DirectorySeparatorChar + "localdataholder" + Path.DirectorySeparatorChar + "images");
				}
            }
		}

		public static EWXLocalData GetLocalData()
        {
			EWXLocalData output;
			string fpath = EWXTelegramBot.GetDataFolder() + Path.DirectorySeparatorChar + "localdataholder.ewx";
			if (File.Exists(fpath))
			{
				using (StreamReader sr = new StreamReader(EWXTelegramBot.GetDataFolder() + Path.DirectorySeparatorChar + "localdataholder.ewx", System.Text.Encoding.Default))
				{
					output = JsonSerializer.Deserialize<EWXLocalData>(sr.ReadToEnd());
				}
			}
			else output = null;
			return output == null ? new EWXLocalData() : output;
        }

		public static string ParseRight(string source)
		{
			StringBuilder sb = new StringBuilder();
			foreach (char c in source)
			{
				if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= 'а' && c <= 'я') || (c >= 'А' && c <= 'Я') || c == '.' || c == '_' || c == '-' || c == ' ' || c == 'ё' || c == 'Ё')
				{
					sb.Append(c);
				}
			}
			return sb.ToString();
		}

		public static void StopAll()
        {
			try
			{
				SaveAll();
				_logger.ForceDump();
				_logger.Stop();
				System.Diagnostics.Process.GetCurrentProcess().Kill();
			}
			catch (Exception e)
			{
				StackTrace("EWXTelegramBot", "StopAll", e.GetType().ToString(), e.Message, e.StackTrace);
				Console.WriteLine("\n\nPress any key to close");
				Console.ReadKey();
				Console.WriteLine("\nSure? Press any key again to confirm");
				Console.ReadKey();
			}
        }

		public static void SaveAll()
        {
			try
			{
				string configurationPath = _datafolder + Path.DirectorySeparatorChar + "configuration.ewx";
				/* LEGACY CODE
				EWXComponentConfiguration config = _controller.GetCurrentConfiguration();
				List<string> rawConfiguration = new List<string>();
				Dictionary<string, string> conf = config.GetConfiguration();
				foreach (string key in conf.Keys) rawConfiguration.Add(key + ": " + conf[key]);
				string rowConfig = rawConfiguration[0];
				for (int i = 1; i < rawConfiguration.Count; i++) rowConfig += (Environment.NewLine + rawConfiguration[i]);
				*/
				if (File.Exists(configurationPath)) File.Delete(configurationPath);
				using (FileStream fs = new FileStream(_datafolder + Path.DirectorySeparatorChar + "configuration.ewx", FileMode.OpenOrCreate))
				{
					byte[] array = System.Text.Encoding.Default.GetBytes(JsonSerializer.Serialize(_controller));
					fs.Write(array, 0, array.Length);
					fs.Close();
				}
			}
			catch (Exception e)
			{
				StackTrace("EWXTelegramBot", "SaveAll", e.GetType().ToString(), e.Message, e.StackTrace);
			}
		}

		public static bool SaveLocalData(EWXLocalData ld)
        {
            try
            {
				if (File.Exists(_datafolder + Path.DirectorySeparatorChar + "localdataholder.ewx")) File.Delete(_datafolder + Path.DirectorySeparatorChar + "localdataholder.ewx");
				using (FileStream fs = new FileStream(_datafolder + Path.DirectorySeparatorChar + "localdataholder.ewx", FileMode.OpenOrCreate))
				{
					byte[] array = Encoding.Default.GetBytes(JsonSerializer.Serialize(ld));
					fs.Write(array, 0, array.Length);
					fs.Close();
				}
				return true;
			}
			catch (Exception e)
            {
				StackTrace("EWXTelegramBot", "SaveLocalData", e.GetType().ToString(), e.Message, e.StackTrace, "EWXLocalData");
			}
			return false;
        }

		public static void StartTimer(DateTime nextPostDate, bool overrideTimer = false)
        {
			if (overrideTimer) if (_timer != null) _timer.Dispose();
			TimeSpan difference = nextPostDate.Subtract(DateTime.Now);
			if(difference.TotalSeconds <= 0)
            {
				nextPostDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, nextPostDate.Hour, nextPostDate.Minute, nextPostDate.Second).AddDays(1);
				difference = nextPostDate.Subtract(DateTime.Now);
			}
			_timerCallback = new TimerCallback(RestartTimer);
			_timer = new Timer(_timerCallback, 0, (long)difference.TotalMilliseconds, 1000);
		}

		private static void RestartTimer(object obj)
        {
			_timer.Dispose();
			DateTime nextPostDate = _controller.GetNextPostDate();
			_controller.Post();
			int[] interval = _controller._postInterval;
			nextPostDate = nextPostDate.AddDays(interval[0]).AddHours(interval[1]).AddMinutes(interval[2]).AddSeconds(interval[3]);
			_controller._nextPostDate = new int[] {nextPostDate.Year, nextPostDate.Month, nextPostDate.Day, nextPostDate.Hour, nextPostDate.Minute, nextPostDate.Second};
			StartTimer(nextPostDate);
        }

		public static int[] ParseCooldown(string value)
		{
			int[] lReturn = {-1,-1,-1,-1};
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
				StackTrace("EWXTelegramBot", "ParseCooldown", e.GetType().ToString(), e.Message, e.StackTrace, ("value=" + value));
			}
			return lReturn;
		}

		public static bool RestartLogger()
        {
			try
			{
				if (_logger != null)
				{
					_logger.ForceDump();
					_logger.Stop();
					_logger = new EWXLogger(true);
					return true;
				}
				else if (_allowLogger)
				{
					_logger = new EWXLogger();
					return true;
				}
				else if (!_allowLogger)
				{
					_logger = new EWXLogger();
					_allowLogger = true;
					return true;
				}
				return false;
			}
			catch (Exception e)
			{
				StackTrace("EWXTelegramBot", "RestartLogger", e.GetType().ToString(), e.Message, e.StackTrace);
			}
			return false;
        }

		public static void Erase(byte level)
        {
            try
            {
				switch (level)
                {
					case 0: // config
                        {
							File.Delete(_datafolder + Path.DirectorySeparatorChar + "configuration.ewx");
							break;
                        }
					case 1: // logs
                        {
							Directory.Delete(_datafolder + Path.DirectorySeparatorChar + "logs", true);
							break;
                        }
					case 2: // audios
						{
							Directory.Delete(_datafolder + Path.DirectorySeparatorChar + "localdataholder" + Path.DirectorySeparatorChar + "audios", true);
							break;
						}
					case 3: // images
						{
							Directory.Delete(_datafolder + Path.DirectorySeparatorChar + "localdataholder" + Path.DirectorySeparatorChar + "images", true);
							break;
						}
					case 4: // localfolder
						{
							Directory.Delete(_datafolder + Path.DirectorySeparatorChar + "localdataholder", true);
							break;
						}
					case 5: // datafolder
						{
							Directory.Delete(_datafolder, true);
							break;
						}
					default:
                        {
							Console.WriteLine("WTFISGOINGON?! Class:EWXTelegramBot Method:Erase IncomingParams:byte>level=" + level);
							break;
                        }
				}
			}
			catch (Exception e)
            {
				StackTrace("EWXTelegramBot", "Erase", e.GetType().ToString(), e.Message, e.StackTrace, ("byte>level=" + level));
			}
        }

		public static void StackTrace(string errorClass, string errorMethod, string errorExceptionType, string errorMessage, string errorStacktrace, string methodParams = "none")
        {
			_controller.SendMessage("An error occured at " + errorClass + "." + errorMethod + "()" + Environment.NewLine + "See last log to get more details");
			Console.WriteLine("An error occured at " + errorClass + "." + errorMethod + "()" + Environment.NewLine + "See last log to get more details");
			PrintService("[WARN] Class:" + errorClass + " Method:" + errorMethod + " IncomingParams:" + methodParams);
			PrintService("[WARN] Exception: " + errorExceptionType + "; Message: " + errorMessage);
			PrintService("[WARN] StackTrace: " + errorStacktrace);
		}

		public static long GetCurrentTimeMillis()
		{
			TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
			return (long)ts.TotalMilliseconds;
		}

		// get/set block
		public static string GetYandexAPI()
		{
			return _yandexApi;
		}

		public static string GetTelegramAPI()
		{
			return _telegramApi;
		}

		public static void SetDebugStatus(bool debug)
        {
			_debug = debug;
        }

		public static bool GetDebugStatus()
        {
			return _debug;
        }

		public static void SetLogger(bool allowLogger)
        {
			_allowLogger = allowLogger;
			_logger.SetActivity(allowLogger);
        }

		public static bool IsAllowLogger()
        {
			return _allowLogger;
        }

		public static EWXController GetController()
        {
			return _controller;
        }

		public static EWXCommandHandler GetCommandHandler()
        {
			return _commandHandler;
        }

		public static string GetDataFolder()
        {
			return _datafolder;
        }

		public static void SetDataFolder(string datafolder)
        {
			_datafolder = datafolder;
        }

		public static bool IsAllWorking()
        {
			return _componentsReady;
        }

		public static int GetBuild()
        {
			return _build;
        }
	}
}
