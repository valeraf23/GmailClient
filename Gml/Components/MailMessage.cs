using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gml.Helpers;
using Gml.Helpers.Extensions;
using Gml.Interfaces;
using Google.Apis.Gmail.v1.Data;
using MimeMapping;

namespace Gml.Components
{
    public class MailMessage : IMailMessage
    {
        private readonly Lazy<ICollection<IMailAttachment>> _attachments;
        private readonly Lazy<string> _body;
        private readonly GmailClient _client;

        private Message _message;

        public MailMessage(GmailClient client, string id)
        {
            _client = client;
            Id = id;
            _attachments = new Lazy<ICollection<IMailAttachment>>(GetAttachmentsFromMessage);
            _body = new Lazy<string>(GetBody);
        }

        public string Id { get; }
        public Message Message => _message ?? (_message = _client.GetMessage(Id));
        public string From => GetHeader("From");
        public IList<string> To => GetHeader("To").ReplaceWhitespaces().Split(',');
        public IList<string> Cc => GetHeader("Cc").ReplaceWhitespaces().Split(',');
        public IList<string> Bcc => GetHeader("Bcc").ReplaceWhitespaces().Split(',');
        public string Body => _body.Value;
        public string Subject => GetHeader("Subject");
        public ICollection<IMailAttachment> Attachments => _attachments.Value;

        public void AddAttachment(string filePath)
        {
            var attachment = new MailAttachment
            {
                FileName = Path.GetFileName(filePath),
                ContentType = MimeUtility.GetMimeMapping(filePath),
                Data = File.ReadAllBytes(filePath)
            };
            Attachments.Add(attachment);
        }

        public DateTime ReceivedTime => DateTimeOffset
            .FromUnixTimeMilliseconds(Message.InternalDate.GetValueOrDefault(DateTime.Now.Ticks)).DateTime;

        private string GetHeader(string key)
        {
            var data = Message.Payload.Headers.FirstOrDefault(x =>
                x.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
            return data == null ? string.Empty : data.Value;
        }

        private string GetBody()
        {
            if (!Message.Payload.MimeType.StartsWith("multipart/"))
                return Base64StringHelper.Decode(Message.Payload.Body.Data);
            var part = Message.Payload.Parts.FirstOrDefault(x => x.MimeType.StartsWith("text"));
            return part != null ? Base64StringHelper.Decode(part.Body.Data) : string.Empty;
        }

        private IList<IMailAttachment> GetAttachmentsFromMessage()
        {
            var attachments = new List<IMailAttachment>();
            if (Message?.Payload.Parts == null) return attachments;
            var attachmentParts = Message.Payload.Parts.Where(x => !string.IsNullOrEmpty(x.Filename));
            attachments.AddRange(attachmentParts.Select(part => new MailAttachment
            {
                Data = _client.GetAttachmentBytes(Message.Id, part.Body.AttachmentId),
                ContentType = part.MimeType,
                FileName = part.Filename
            }));

            return attachments;
        }
    }
}