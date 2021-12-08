using me.ewerestr.ewxtelegrambot.Components;
using me.ewerestr.ewxtelegrambot.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
		//private static EWXTelegramInstance _telegramInstance;
		//private static EWXTelegramLongPoll _telegramLongpoll;
		//private static EWXYandexInstance _yandexInstance;
		private static EWXController _controller;
		private static bool _allowLogger = true;
		private static bool _debug = true;
		private static bool _componentsReady = false;
		private static string _datafolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
											Path.DirectorySeparatorChar +
											"EWXTelegramBot";
		private static Dictionary<string, string> _translit = new Dictionary<string, string>();
		private const int _build = 0101;

		// entry block
		static void Main(string[] args)
		{
			_logger = new EWXLogger();
			FillTranslit();
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
			using (StreamReader sr = new StreamReader(EWXTelegramBot.GetDataFolder() + Path.DirectorySeparatorChar + "localdataholder.ewx", System.Text.Encoding.Default))
			{
				output = JsonSerializer.Deserialize<EWXLocalData>(sr.ReadToEnd());
			}
			return output == null ? new EWXLocalData() : output;
        }

		private static void FillTranslit()
        {
			_translit.Add("А", "A"); 
			_translit.Add("Б", "B"); 
			_translit.Add("В", "V"); 
			_translit.Add("Г", "G"); 
			_translit.Add("Д", "D"); 
			_translit.Add("Е", "E"); 
			_translit.Add("Ё", "JO"); 
			_translit.Add("Ж", "ZH"); 
			_translit.Add("З", "Z"); 
			_translit.Add("И", "I"); 
			_translit.Add("Й", "JJ"); 
			_translit.Add("К", "K"); 
			_translit.Add("Л", "L"); 
			_translit.Add("М", "M"); 
			_translit.Add("Н", "N"); 
			_translit.Add("О", "O"); 
			_translit.Add("П", "P"); 
			_translit.Add("Р", "R"); 
			_translit.Add("С", "S"); 
			_translit.Add("Т", "T"); 
			_translit.Add("У", "U"); 
			_translit.Add("Ф", "F"); 
			_translit.Add("Х", "KH"); 
			_translit.Add("Ц", "C"); 
			_translit.Add("Ч", "CH"); 
			_translit.Add("Ш", "SH"); 
			_translit.Add("Щ", "SHH"); 
			_translit.Add("Ъ", "'"); 
			_translit.Add("Ы", "Y"); 
			_translit.Add("Ь", ""); 
			_translit.Add("Э", "EH"); 
			_translit.Add("Ю", "YU"); 
			_translit.Add("Я", "YA"); 
			_translit.Add("а", "a"); 
			_translit.Add("б", "b"); 
			_translit.Add("в", "v"); 
			_translit.Add("г", "g"); 
			_translit.Add("д", "d"); 
			_translit.Add("е", "e"); 
			_translit.Add("ё", "jo"); 
			_translit.Add("ж", "zh"); 
			_translit.Add("з", "z"); 
			_translit.Add("и", "i"); 
			_translit.Add("й", "jj"); 
			_translit.Add("к", "k"); 
			_translit.Add("л", "l"); 
			_translit.Add("м", "m"); 
			_translit.Add("н", "n"); 
			_translit.Add("о", "o"); 
			_translit.Add("п", "p"); 
			_translit.Add("р", "r"); 
			_translit.Add("с", "s"); 
			_translit.Add("т", "t"); 
			_translit.Add("у", "u"); 
			_translit.Add("ф", "f"); 
			_translit.Add("х", "kh"); 
			_translit.Add("ц", "c"); 
			_translit.Add("ч", "ch"); 
			_translit.Add("ш", "sh"); 
			_translit.Add("щ", "shh"); 
			_translit.Add("ъ", ""); 
			_translit.Add("ы", "y"); 
			_translit.Add("ь", ""); 
			_translit.Add("э", "eh"); 
			_translit.Add("ю", "yu"); 
			_translit.Add("я", "ya");

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
				_controller.SendMessage("An error occured at EWXTelegramBot.StopAll()" + Environment.NewLine + "See last log to get more details");
				Console.WriteLine("An error occured at EWXTelegramBot.StopAll()" + Environment.NewLine + "See last log to get more details");
				PrintService("[WARN] Class:EWXTelegramBot Method:StopAll IncomingParams:none");
				PrintService("[WARN] Exception: " + e.GetType().ToString() + "; Message: " + e.Message);
				PrintService("[WARN] StackTrace: " + e.StackTrace);
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
				_controller.SendMessage("An error occured at EWXTelegramBot.SaveAll()" + Environment.NewLine + "See last log to get more details");
				Console.WriteLine("An error occured at EWXTelegramBot.SaveAll()" + Environment.NewLine + "See last log to get more details");
				PrintService("[WARN] Class:EWXTelegramBot Method:SaveAll IncomingParams:none");
				PrintService("[WARN] Exception: " + e.GetType().ToString() + "; Message: " + e.Message);
				PrintService("[WARN] StackTrace: " + e.StackTrace);
			}
		}

		public static bool SaveLocalData(EWXLocalData ld)
        {
            try
            {
				if (File.Exists(_datafolder + Path.DirectorySeparatorChar + "localdataholder.ewx")) File.Delete(_datafolder + Path.DirectorySeparatorChar + "localdataholder.ewx");
				using (FileStream fs = new FileStream(_datafolder + Path.DirectorySeparatorChar + "localdataholder.ewx", FileMode.OpenOrCreate))
				{
					byte[] array = System.Text.Encoding.Default.GetBytes(JsonSerializer.Serialize(ld));
					fs.Write(array, 0, array.Length);
					fs.Close();
				}
				return true;
			}
			catch (Exception e)
            {
				_controller.SendMessage("An error occured at EWXTelegramBot.SaveLocalData()" + Environment.NewLine + "See last log to get more details");
				Console.WriteLine("An error occured at EWXTelegramBot.SaveLocalData()" + Environment.NewLine + "See last log to get more details");
				PrintService("[WARN] Class:EWXTelegramBot Method:SaveLocalData IncomingParams:EWXLocalData");
				PrintService("[WARN] Exception: " + e.GetType().ToString() + "; Message: " + e.Message);
				PrintService("[WARN] StackTrace: " + e.StackTrace);
			}
			return false;
        }

		/* UNUSED CODE
		public static long ParseCooldown(string value)
		{
			long lReturn = 0;
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
			if (d > 0) lReturn += d * 86400;
			if (h > 0) lReturn += h * 3600;
			if (m > 0) lReturn += m * 60;
			if (s > 0) lReturn += s;
			return lReturn;
		} 
		*/

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
				_controller.SendMessage("An error occured at EWXTelegramBot.RestartLogger()" + Environment.NewLine + "See last log to get more details");
				Console.WriteLine("An error occured at EWXTelegramBot.RestartLogger()" + Environment.NewLine + "See last log to get more details");
				PrintService("[WARN] Class:EWXTelegramBot Method:RestartLogger IncomingParams:none");
				PrintService("[WARN] Exception: " + e.GetType().ToString() + "; Message: " + e.Message);
				PrintService("[WARN] StackTrace: " + e.StackTrace);
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
				_controller.SendMessage("An error occured at EWXTelegramBot.Erase()" + Environment.NewLine + "See last log to get more details");
				Console.WriteLine("An error occured at EWXTelegramBot.Erase()" + Environment.NewLine + "See last log to get more details");
				PrintService("[WARN] Class:EWXTelegramBot Method:Erase IncomingParams:byte>level=" + level);
				PrintService("[WARN] Exception: " + e.GetType().ToString() + "; Message: " + e.Message);
				PrintService("[WARN] StackTrace: " + e.StackTrace);
			}
        }

		public static string Transliterate(string source)
		{
			try
			{
				string output = source;
				foreach (string key in _translit.Keys) output = output.Replace(key, _translit[key]);
				return output;
			}
			catch (Exception e)
			{
				_controller.SendMessage("An error occured at EWXTelegramBot.Transliterate()" + Environment.NewLine + "See last log to get more details");
				Console.WriteLine("An error occured at EWXTelegramBot.Transliterate()" + Environment.NewLine + "See last log to get more details");
				PrintService("[WARN] Class:EWXTelegramBot Method:Transliterate IncomingParams:source=\"" + source + "\"");
				PrintService("[WARN] Exception: " + e.GetType().ToString() + "; Message: " + e.Message);
				PrintService("[WARN] StackTrace: " + e.StackTrace);
			}
			return source;
		}

		public static bool HasCyrillic(string source)
        {
			foreach (char c in source) if (_translit.ContainsKey(c.ToString())) return true;
			return false;
        }

		public static long GetCurrentTimeMillis()
		{
			TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
			return (long)ts.TotalMilliseconds;
		}

		// get/set block
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
