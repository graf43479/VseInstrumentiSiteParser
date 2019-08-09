using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using VseInstrumenti.Core;
using VseInstrumenti.Core.Target;
using Domain.DAL;
using Domain.Entity;
using System.Linq;
using System.Data.Entity;
using Domain.DAL.Concrete;
using Domain.DAL.Abstract;
using System.Diagnostics;
using System.Net;
//using System.Data.Entity;

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
           
           // EmailNotifier.CreateMessageAsync("test2");
        }

        public static void MakeParse()
        {
            var timer  = Stopwatch.StartNew();
            //ParserWorker<List<Product>> parser = new ParserWorker<List<Product>>(new VSParser(), new VSSettings(1, 20));
            ParserWorker<List<Product>> parser = new ParserWorker<List<Product>>(new VSParser(), new VSSettings(1, 20));
            //parser.Settings = new VSSettings(1, 20);
            parser.Strart();
            Console.WriteLine("Main over");

            timer.Stop();
            string result = "total time: " + timer.ElapsedMilliseconds / 1000.0 / 60.0 + " min";
            Console.WriteLine(result); //4 min //2 min async //2ю76 ,,
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
