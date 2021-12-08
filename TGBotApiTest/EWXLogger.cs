using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace me.ewerestr.ewxtelegrambot.Components
{
    class EWXLogger
    {
        private string _path;
        private List<string> _templog = new List<string>();
        private int _cooldown;
        private Thread t_logger;

        public EWXLogger(bool restart = false, int saveCooldown = 300)
        {
            string lString = DateTime.Now.ToString("G").Replace(".", "-").Replace(" ", "_").Replace(":", "-");
            _path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "EWXTelegramBot" + Path.DirectorySeparatorChar + "logs" + Path.DirectorySeparatorChar + "log_" + lString + ".ewx";
            CheckPath();
            _cooldown = saveCooldown;
            t_logger = new Thread(Start);
            Log(">> Модуль ведения журнала " + (restart ? "перезапущен!" : "запущен!"), true);
            t_logger.Start();
        }

        private void CheckPath()
        {
            /* 
             *      C:\Users\ewerestr\AppData\Roaming\EWX\log.ewx
             * 0    C:
             * 1    Users
             * 2    ewerestr
             * 3    AppData
             * 4    Roaming
             * 5    EWX
             * 6    log.ewx
             */

            string[] npath = _path.Split(Path.DirectorySeparatorChar);
            string wpath = npath[0];
            for (int i = 1; i < npath.Length - 1; i++)
            {
                wpath += Path.DirectorySeparatorChar + npath[i];
                if (!Directory.Exists(wpath)) Directory.CreateDirectory(wpath);
            }
            if (!File.Exists(_path)) File.Create(_path);
        }

        public void Log(string text, bool print = false)
        {
            _templog.Add("[" + DateTime.Now.ToString("G") + (text[0].Equals('[') ? "]" : "] ") + text);
            if (print) Console.WriteLine(text);
        }

        public void SetActivity(bool activity)
        {
            if (!activity)
            {
                t_logger.Abort();
            }
        }

        private void Start()
        {
            while (true)
            {
                Thread.Sleep(_cooldown * 1000);
                DumpLog();
            }
        }

        public void Stop()
        {
            t_logger.Abort();
        }

        public void ForceDump()
        {
            DumpLog();
        }

        private void DumpLog()
        {
            if (_templog.Count > 0)
            {
                using (StreamWriter sw = new StreamWriter(_path, true, System.Text.Encoding.UTF8))
                {
                    foreach (string l in _templog) sw.WriteLine(l);
                }
                _templog.Clear();
            }
        }
    }
}
