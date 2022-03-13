using System;
using System.Threading;

namespace tests
{
    class Program
    {
        static void Main(string[] args)
        {
            Print(new string[] { "1d1h1m10s"});
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
    }
}
