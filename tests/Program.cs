using System;

namespace tests
{
    class Program
    {
        static void Main(string[] args)
        {
            int max = 100;
            Random r = new Random();
            while (true)
            {
                Console.WriteLine(r.Next(0, max));
                Console.WriteLine(r.Next(0, max));
                Console.ReadKey();
            }
        }
    }
}
