using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.ComponentModel;

namespace VseISiteParser
{
    public static class EmailNotifier
    {
        //private EmailSettings emailSettings;

        //public EmailNotifier(EmailSettings settings)
        //{
        //    emailSettings = settings;
        //}

   
        private static void Mailer(MailMessage mailMessage)
        {
            mailMessage.IsBodyHtml = true;

            //mailMessage.Body = "<p>Добрый день! Клиент сайта <b>" + Constants.SITE_NAME + "</b>, представившийся " +
            //                        "как <b>" + message.Name + "</b>, направил вам сообщение с обратным адресом: " +
            //                        message.Email + "</p>" +
            //                        "<p>Текст сообщения ниже</p>" +
            //                        "<p style='font-weight: bold;color: indigo;background-color: lavender'>" +
            //                        message.Text + "</p>" +
            //                        "<p>Отвечайте на письмо на адрес: " + message.Email + "</p>";

            if (EmailSettings.WriteAsFile)
            {
                mailMessage.BodyEncoding = Encoding.UTF8;
            }

            using (var smtpClient = new SmtpClient())
            {
                // emailSettings.MailToAddress = user.Email;
                smtpClient.EnableSsl = EmailSettings.UseSsl;
                smtpClient.Host = EmailSettings.ServerName;
                smtpClient.Port = EmailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(EmailSettings.UserName, EmailSettings.Password);
                try
                {
                   
                   smtpClient.Send(mailMessage);                    
                    //smtpClient.Send(mailMessage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                mailMessage.Dispose();
            }
        }
        

        public static void CreateMessage(string messageText, string subject)
        {
            MailMessage mailMessage = new MailMessage(
                        EmailSettings.MailFromAddress,
                EmailSettings.MailToAddress,                        
                        subject,
                        messageText
                        );

            Mailer(mailMessage);            

        }

    }

    internal static class EmailSettings
    {
        public static string MailToAddress { get; set; } = "graf43479@ya.ru";
        public static string MailFromAddress { get; set; } = "grafgrafgraf43479@ya.ru";
        public static bool UseSsl { get; set; } = true;
        public static string UserName { get; set; } = "grafgrafgraf43479";
        public static string Password { get; set; } = "graf43479";
        public static string ServerName { get; set; } = "smtp.yandex.ru";
        public static int ServerPort { get; set; } = 587;
        public static bool WriteAsFile { get; set; } = false;
        public static string FileLocation { get; set; } = @"c:/sportstore/emails";
    }
}


/*
 //emailSettings.MailFromAddress = ((sets.FirstOrDefault(x=>x.MailSettingsID=="MAIL_FROM_ADDRESS"))!=null) ? sets.FirstOrDefault(x=>x.MailSettingsID=="MAIL_FROM_ADDRESS").SettingsValue : Constants.MAIL_FROM_ADDRESS;
           //emailSettings.UseSsl = ((sets.FirstOrDefault(x => x.MailSettingsID == "MAIL_USE_SSL")) != null) ? Boolean.Parse(sets.FirstOrDefault(x => x.MailSettingsID == "MAIL_USE_SSL").SettingsValue) : Constants.USE_SSL;
           //emailSettings.UserName = ((sets.FirstOrDefault(x=>x.MailSettingsID=="MAIL_SERVER_USER_NAME"))!=null) ? sets.FirstOrDefault(x=>x.MailSettingsID=="MAIL_FROM_ADDRESS").SettingsValue : Constants.USERNAME;
           //emailSettings.Password = ((sets.FirstOrDefault(x=>x.MailSettingsID=="MAIL_SERVER_PASSWORD"))!=null) ? sets.FirstOrDefault(x=>x.MailSettingsID=="MAIL_FROM_ADDRESS").SettingsValue : Constants.PASSWORD;
           //emailSettings.ServerName = ((sets.FirstOrDefault(x=>x.MailSettingsID=="MAIL_SERVER_NAME"))!=null) ? sets.FirstOrDefault(x=>x.MailSettingsID=="MAIL_FROM_ADDRESS").SettingsValue : Constants.SERVERNAME;
           //emailSettings.ServerPort = ((sets.FirstOrDefault(x=>x.MailSettingsID=="MAIL_SERVER_PORT"))!=null) ? Int32.Parse(sets.FirstOrDefault(x=>x.MailSettingsID=="MAIL_FROM_ADDRESS").SettingsValue) : Constants.SERVER_PORT;
           //emailSettings.WriteAsFile = ((sets.FirstOrDefault(x=>x.MailSettingsID=="MAIL_WRITE_AS_FILE"))!=null) ? Boolean.Parse(sets.FirstOrDefault(x=>x.MailSettingsID=="MAIL_FROM_ADDRESS").SettingsValue) : Constants.WRITE_AS_FILE;
           //emailSettings.FileLocation = ((sets.FirstOrDefault(x=>x.MailSettingsID=="MAIL_FILE_LOCATION"))!=null) ? sets.FirstOrDefault(x=>x.MailSettingsID=="MAIL_FROM_ADDRESS").SettingsValue : @"c:/sportstore";//Constants.FILE_LOCATION;
           
       //настройки сервера электронной почты 
        public const string MAIL_TO_ADDRESS = "graf43479@ya.ru";
        public const string MAIL_FROM_ADDRESS = "graf43479@ya.ru";
        public const bool USE_SSL = true;
        public const string USERNAME = "graf43479";
        public const string PASSWORD = "votsnorov1987";
        public const string SERVERNAME = "smtp.yandex.ru";
        public const int SERVER_PORT = 587;
        public const bool WRITE_AS_FILE = false;
        public const string FILE_LOCATION = @"c:/sportstore/emails";
   
     */
