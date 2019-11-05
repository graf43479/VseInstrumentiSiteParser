using System;
using System.Collections.Generic;
using VseInstrumenti.Core;
using VseInstrumenti.Core.Target;
using Domain.Entity;
using System.Diagnostics;
using System.Net;

namespace VseISiteParser
{

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            if (CheckForInternetConnection())
            {
                MakeParse();
            }
        }

        public static void MakeParse()
        {
            var timer  = Stopwatch.StartNew();
            ParserWorker<List<Product>> parser = new ParserWorker<List<Product>>(new VSParser(), new VSSettings(1, 20));
            parser.Strart();
            Console.WriteLine("Main over");

            timer.Stop();
            string result = "total time: " + timer.ElapsedMilliseconds / 1000.0 / 60.0 + " min";
            Console.WriteLine(result); 
            EmailNotifier.CreateMessage(result, "Success");
        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
