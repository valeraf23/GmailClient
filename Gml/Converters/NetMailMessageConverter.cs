using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using Gml.Helpers;
using Gml.Helpers.Extensions;
using Gml.Interfaces;
using Google.Apis.Gmail.v1.Data;
using MimeKit;

namespace Gml.Converters
{
    public static class NetMailMessageConverter
    {
        public static MailMessage AddBody(this MailMessage message, string body)
        {
            return message.AddAlternateView(body);
        }

        public static MailMessage ToPlainText(this MailMessage message, string body)
        {
            message.AlternateViews.Clear();
            message.BodyEncoding = Encoding.UTF8;
            message.Body = body;
            return message;
        }

        private static MailMessage AddAlternateView(this MailMessage message, string mailBody)
        {
            var plainView =
                AlternateView.CreateAlternateViewFromString(mailBody, Encoding.UTF8,
                    MediaTypeNames.Text.Plain);
            message.AlternateViews.Add(plainView);

            var htmlView =
                AlternateView.CreateAlternateViewFromString(mailBody, Encoding.UTF8,
                    MediaTypeNames.Text.Html);
            message.AlternateViews.Add(htmlView);
            return message;
        }

        public static MailMessage AddAttachments(this MailMessage message, IEnumerable<IMailAttachment> attachments)
        {
            foreach (var attachment in attachments)
            {
                Stream stream = new MemoryStream(attachment.Data);
                message.Attachments.Add(new Attachment(stream, attachment.FileName, attachment.ContentType));
            }

            return message;
        }

        public static MailMessage AddBcc(this MailMessage message, IEnumerable<MailAddress> bcc)
        {
            bcc.ForEach(address => message.Bcc.Add(address));
            return message;
        }

        public static MailMessage AddCc(this MailMessage message, IEnumerable<MailAddress> cc)
        {
            cc.ForEach(address => message.CC.Add(address));
            return message;
        }

        public static MailMessage AddTo(this MailMessage message, IEnumerable<MailAddress> to)
        {
            to.ForEach(address => message.To.Add(address));
            return message;
        }

        public static Message ToMessage(this MailMessage message)
        {
            var mimiMsg = MimeMessage.CreateFromMailMessage(message);

            var emailMessage = new Message
            {
                Raw = Base64StringHelper.Encode(mimiMsg.ToString())
            };
            return emailMessage;
        }
    }
}