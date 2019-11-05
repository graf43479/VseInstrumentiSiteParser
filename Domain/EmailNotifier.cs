using System;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;


using System.Net.Mime;
using System.Threading;
using System.ComponentModel;

namespace VseISiteParser
{
    public static class EmailNotifier
    {
        static bool mailSent = false;

        private static async void SendMailAsync(MailMessage mailMessage)
        {
            mailMessage.IsBodyHtml = true;

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

                smtpClient.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
                string state = "";
                try
                {

                    smtpClient.SendAsync(mailMessage, state);
                    //Task
                    //if (!mailSent)
                    //{
                    //    smtpClient.SendAsyncCancel();
                    //}
                    await Task.Run(() => smtpClient.SendAsync(mailMessage, state));
                    //smtpClient.Send(mailMessage);                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                mailMessage.Dispose();
            }
        }

        private static void SendMail(MailMessage mailMessage)
        {
            mailMessage.IsBodyHtml = true;

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
            // SendMail(mailMessage));
            //await Task.Run(() => SendMailAsync(mailMessage));
            //await Task.Run(() => 
            SendMail(mailMessage);
            //Task.WaitAll();
            //Task t =  //Task.Run(()=> SendMailAsync(mailMessage));            
        }

        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            String token = (string)e.UserState;

            if (e.Cancelled)
            {
                Console.WriteLine("[{0}] Send canceled.", token);
            }
            if (e.Error != null)
            {
                Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
            }
            else
            {
                Console.WriteLine("Message sent.");
            }
            mailSent = true;
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