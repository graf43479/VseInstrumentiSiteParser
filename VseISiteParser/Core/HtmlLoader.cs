﻿using System;
using OpenQA.Selenium.Chrome;



namespace VseInstrumenti.Core
{
    /// <summary>
    /// Класс для управленияем конфига браузера и навигации
    /// </summary>
    class HtmlLoader
    {
        readonly string  url;
        public ChromeDriver driver;

        public HtmlLoader(IParserSettings settings)
        {
            url = $"{settings.BaseURL}/{settings.Vendor}/{settings.Prefix}/";

            //инициализируем ChromeDriver
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--incognito");
            //options.AddArgument("--no-referrers");
            options.AddArguments("--disable-javascript");
            //options.AddArguments("--headless");
            //options.Proxy = null;
            //driver.set_window_size(1200, 600)
            options.AddArguments("--disable-gpu");
            
            options.AddArguments("--disable-extensions");
            //options.SetExperimentalOption("useAutomationExtension", false);
            options.AddArguments("--proxy-server='direct://'");
            options.AddArguments("--proxy-bypass-list=*");
            //options.AddArguments("--start-maximized");
            //options.AddArguments("--headless");

            options.AddUserProfilePreference("profile.default_content_setting_values.images", 2);               //без подгрузки изображений
            options.AddUserProfilePreference("profile.default_content_setting_values.javascript", 2);           //без подгрузки JS
            options.AddUserProfilePreference("profile.default_content_setting_values.plugins", 2);              //без подгрузки плагинов
            options.AddUserProfilePreference("profile.default_content_setting_values.popups", 2);               //без подгрузки всплывающих сообщений
            options.AddUserProfilePreference("profile.default_content_setting_values.notifications", 2);        //без подгрузки сообщений
            options.AddUserProfilePreference("profile.default_content_setting_values.automatic_downloads", 2);  //без автозагрузки
            options.AddUserProfilePreference("profile.default_content_setting_values.push_messaging", 2);       //без push-уведомлений
            options.AddUserProfilePreference("profile.default_content_setting_values.app_banner", 2);           //без подгрузки баннеров
            //options.AddArgument("--window-size=0,0"); //размер окна
             options.AddArgument("--window-position=-2000,0"); //прячет окно куда надо.
            //options.AddArguments("--window-size=1920,1080");
            
            //options.LeaveBrowserRunning = false;
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            
            bool isFirst = true;
            if (driver == null)
            { 
                driver = new ChromeDriver(service, options);                     
            }

            if (isFirst)
             {
                 var currentUrl = url.Replace("{currentId}", "");
                 currentUrl = currentUrl.Replace("{vendor}", "someshit");
                 driver.Navigate().GoToUrl(currentUrl);
                 driver.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie("goods_per_page", "80"));
                 driver.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie("notice-user-email", "unknown"));
                 driver.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie("wucf", "14"));
            }
                //driver.Manage().Window.Position = new System.Drawing.Point(-2000, 0);
                //driver.Manage().Window.Position = new System.Drawing.Point(300, 500);             
        }
               
        public string GetSource(string vendor, int id, string sortOption, bool isPlanA)
        {   
            var currentUrl = "";
            if (id == 1) //на первой странице нельзя указывать номер в url
            {
                currentUrl = url.Replace("page{currentId}/", "");
            }
            else
            {
                currentUrl = url.Replace("{currentId}", id.ToString());
            }

            if (isPlanA)
            {
                currentUrl = currentUrl.Replace("/{vendor}", "");
                currentUrl = currentUrl.Replace("www.vseinstrumenti.ru", vendor + ".vseinstrumenti.ru");
            }
            else
            {
                currentUrl = currentUrl.Replace("{vendor}", vendor);
            }

            currentUrl += sortOption;

            try
            {                
                var cookies = driver.Manage().Cookies.AllCookies; 
                foreach (OpenQA.Selenium.Cookie item in cookies)
                {
                    if (item.Name == "goods_per_page")
                    {
                        if (item.Value == "20")
                        {
                            driver.Manage().Cookies.DeleteCookie(item);
                            driver.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie("goods_per_page", "80"));
                        }
                    }   
                }
            }
            catch (Exception ex)
            {
               // Console.WriteLine(ex.Message);
            }
            
            driver.Navigate().GoToUrl(currentUrl);
           // Console.WriteLine(currentUrl);

            return driver.PageSource.ToString();         
        }
    }
}