using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using VseISiteParser.Core;
using Domain.DAL;
using Domain.Entity;
using VseISiteParser;

namespace VseInstrumenti.Core
{
    /// <summary>
    /// Основной класс по работе с парсером
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ParserWorker<T> where T : class
    {
        private IParser<T> parser;
        IParserSettings parserSettings;
        
        private HtmlLoader loader;

        DBLoader dbLoader;

        bool isActive;        

        #region Properties

        public IParser<T> Parser
        {
            get { return parser; }
            set { parser = value; }
        }
        
        public IParserSettings Settings
        {
            get { return parserSettings; }
            set { 
                parserSettings = value;
                loader = new HtmlLoader(value);
            }
        }

        public bool IsActive { get { return isActive; } }

        #endregion Properties

       
        public ParserWorker(IParser<T> parser)
        {
            this.parser = parser;
            this.dbLoader = new DBLoader(new ViDBContext());
        }

        public ParserWorker(IParser<T> parser, IParserSettings settings) : this(parser)
        {
            Settings = settings;
            //this.parserSettings = settings;
        }

        public void Strart()
        {
            try
            {

                isActive = true;
                //WorkerAsync();
                Task t = WorkerAsync();
                t.Wait();
            }
            catch (Exception ex)
            {
                EmailNotifier.CreateMessage("Ошибка при парсинге! " + ex.InnerException.Message, "Error"); 
                throw;
            }
        }

        public void Abort()
        {
            isActive = false;
        }

        //private void Worker()
        private async Task WorkerAsync()
        {
            Statistic statistic = dbLoader.GetStatistic().FirstOrDefault(x => x.CreationDate.Date == DateTime.Now.Date);
            if (statistic == null)
            {
                List<Product> oldResult = null;
                List<Vendor> vendors = (await dbLoader.GetVendorsAsync());// as string[]; 
                string oldSortSetting="";                                                         //string[] vendors = { "crown", "ryobi" };

                    foreach (Vendor vendor in vendors)
                    {
                    // Vendor vendor = vendors.FirstOrDefault(x => x.Name == "Ryobi");
                    bool IsPlanA = true;
                    bool first = true;
                    retry:

                    for (int j = 0; j < parserSettings.SortList.Length; j++)
                    {
                        for (int i = parserSettings.StartPoint; i < parserSettings.EndPoint; i++)
                        {                            
                            if (!isActive)
                            {
                                //Console.WriteLine("Not Active");
                                return;
                            }
                            //Console.WriteLine(i);
                            string htmlResult = "";
                            try
                            {
                                htmlResult = loader.GetSource(vendor.SubUrl, i, parserSettings.SortList[j], IsPlanA);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Ошибка загрузки страницы: {ex.Message}");
                                goto retry2;
                            }
                            
                        var domParser = new HtmlParser();
                        var document = await domParser.ParseDocumentAsync(htmlResult.ToString());
                        
                        T result = Parser.Parse(document);
                            
                            //если лист товаров не получен, то значит страница не существует
                            if (result == null || (result as List<Product>).Count() == 0)
                            {
                                if (IsPlanA)
                                {
                                    IsPlanA = false;
                                    //Console.WriteLine("BreakA0");
                                    goto retry;
                                }
                               // Console.WriteLine("BreakA");
                                break;
                            }

                            List<Product> newResult = result as List<Product>;

                            string newSortSetting = parserSettings.SortList[j];
                            try
                            {
                                if (oldResult != null && oldResult.Select(x => x.Code).Intersect(newResult.Select(x => x.Code)).Count() > 0)
                                {
                                    if (oldSortSetting == newSortSetting)
                                    {
                                       // Console.WriteLine("BreakB");
                                    oldSortSetting = "";
                                        break;
                                    }
                                    else
                                    {
                                        //Console.WriteLine("BreakC");
                                        oldSortSetting = newSortSetting;
                                       // continue;
                                    }                                    
                                }
                            }
                            catch (Exception ex)
                            {
                                //Console.WriteLine("Exception: " + ex.Message);
                            }

                            oldSortSetting = newSortSetting;
                            oldResult = newResult;

                            foreach (Product item in newResult)
                            {
                                //item.Vendor = vendor;
                                item.VendorID = vendor.VendorID;
                            }

                            // Statistic statistic = dbLoader.CreateStatistic();
                           dbLoader.LoadProductList(newResult); 

                            //statistic.Status = snapshotState;
                            //dbLoader.SaveStatistic(statistic);

                            if (first)
                            {
                                first = false;
                                if (oldResult != null && oldResult.Count!= 20 && oldResult.Count < 79)
                                {
                                    goto retry2;
                                }
                            }
                        }
                    }
                retry2:;
                }

               await dbLoader.MakePriceSnapshotAsync();
               // Parallel.Invoke( async () => { await dbLoader.MakePriceSnapshotAsync(); });                
            }

            //Parallel.Invoke(()=> { loader.driver.Quit(); });
            loader.driver.Quit();
            //Console.WriteLine("WorkerAsync over");
        }
    }
}
