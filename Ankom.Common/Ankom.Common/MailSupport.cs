using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace Ankom.Common
{
    public static class MailSupport
    {
        /// <summary>
        /// Отправка письма на почтовый ящик c использованием SMTP-клиента
        /// </summary>
        /// <param name="smtpServer">Имя SMTP-сервера</param>
        /// <param name="from">Адрес отправителя</param>
        /// <param name="password">Пароль к почтовому ящику отправителя</param>
        /// <param name="mailto">Адрес получателя</param>
        /// <param name="subject">Тема письма</param>
        /// <param name="message">Сообщение</param>
        /// <param name="attachFiles">Список присоединенных файлов</param>
        public static void SendMailViaSmtpClient(
            string smtpServer, string from, string password,
            string mailto, string subject, string message, string[] attachFiles = null)
        {
            using (MailMessage mail = new MailMessage())
                try
                {
                    mail.From = new MailAddress(from);
                    mail.To.Add(new MailAddress(mailto));
                    mail.Subject = subject;
                    mail.Body = message;
                    if (attachFiles != null)
                    {
                        foreach (string attachFile in attachFiles)
                            if (!string.IsNullOrEmpty(attachFile) && File.Exists(attachFile))
                                mail.Attachments.Add(new Attachment(attachFile));
                    }
                    SmtpClient client = new SmtpClient()
                    {
                        Host = smtpServer
                        ,
                        Port = 587
                        ,
                        EnableSsl = true
                        ,
                        Credentials = new NetworkCredential(from.Split('@')[0], password)
                        ,
                        DeliveryMethod = SmtpDeliveryMethod.Network
                    };
                    client.Send(mail);
                }
                catch (Exception e)
                {
                    throw new Exception("Mail.Send: " + e.Message);
                }
        }

        /// <summary>
        /// Отправка письма на почтовый ящик c использованием почтового клиента по умолчанию
        /// </summary>
        /// <param name="from">Адрес отправителя</param>
        /// <param name="mailto">Адрес получателя</param>
        /// <param name="subject">Тема письма</param>
        /// <param name="message">Сообщение</param>
        public static void SendMailViaDefaultMailClient(string mailto, string subject, string message)
        {
            /* •———————————————————————————————————————————————————————————————————————————————•
               | Синтаксис команды                                                             |
               | mailto:[ПочтовыйАдресс]                                                       |
               |         [?]                                                                   |
               |         [subject=Тема сообщения]                                              |
               |         [&cc=АдрессКопии]                                                     |
               |         [&bcc=СкрытаяКопия]                                                   |
               |         [&body=Само сообщение]                                                |
               |                                                                               |
               |         Обязательный параметр:                                                |
               | ПочтовыйАдресс – адрес(e-mail) получателя                                     |
               |                                                                               |
               | Необезательные параметры:                                                     |
               | ? - указывается, если за адресом следует хотя бы один необязательный параметр |
               | subject – тема сообщения может содержать пробелы                              |
               | cc – адрес, на который отсылать копию письма                                  |
               | bcc – адрес, на который отсылать скрытую копию                                |
               | body – сообщение, текст письма                                                |
               •———————————————————————————————————————————————————————————————————————————————• */
            try
            {
                Process.Start(string.Format("mailto:{0}?subject={1}&body={2}", mailto, subject, message));
            }
            catch (Exception e)
            {
                throw new Exception("Mail.Send: " + e.Message);
            }
        }

    }
}

