using System.Net.Mail;
using Gml.Interfaces;
using Google.Apis.Gmail.v1.Data;

namespace Gml.Converters
{
    public static class MailMessageModelConverter
    {
        public static Message ConvertToMessage(this IMailMessageModel messageModel, MailAddress sender)
        {
            return messageModel.ConvertToMailMessage(sender).ToMessage();
        }

        public static Message ConvertToPlainTextMessage(this IMailMessageModel messageModel, MailAddress sender)
        {
            return messageModel.ConvertToMailMessage(sender).ToPlainText(messageModel.Body).ToMessage();
        }

        private static MailMessage ConvertToMailMessage(this IMailMessageModel messageModel, MailAddress sender)
        {
            var message = new MailMessage
            {
                Subject = messageModel.Subject,
                From = sender
            };
            message.AddBody(messageModel.Body)
                .AddTo(messageModel.To).AddCc(messageModel.Cc).AddBcc(messageModel.Bcc)
                .AddAttachments(messageModel.Attachments);
            return message;
        }
    }
}