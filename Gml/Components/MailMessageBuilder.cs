using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Gml.Helpers.Extensions;
using Gml.Interfaces;

namespace Gml.Components
{
    public class MailMessageBuilder
    {
        private readonly MailMessageModel _mailMessageModel;

        private MailMessageBuilder(MailAddress[] to)
        {
            _mailMessageModel = new MailMessageModel(to);
        }

        private MailMessageBuilder(string[] to)
        {
            _mailMessageModel = new MailMessageModel(to);
        }

        public static MailMessageBuilder To(MailAddress[] to)
        {
            return new MailMessageBuilder(to);
        }

        public static MailMessageBuilder To(string[] to)
        {
            return new MailMessageBuilder(to);
        }

        public static MailMessageBuilder To(MailAddress to)
        {
            return new MailMessageBuilder(new[] {to});
        }

        public static MailMessageBuilder To(string to)
        {
            return new MailMessageBuilder(new[] {to});
        }

        public MailMessageBuilder WithCc(params MailAddress[] cc)
        {
            _mailMessageModel.Cc.AddRange(cc);
            return this;
        }

        public MailMessageBuilder WithBcc(params MailAddress[] bcc)
        {
            _mailMessageModel.Bcc.AddRange(bcc);
            return this;
        }

        public MailMessageBuilder WithBody(string body)
        {
            _mailMessageModel.Body = body;
            return this;
        }

        public MailMessageBuilder WithSubject(string subject)
        {
            _mailMessageModel.Subject = subject;
            return this;
        }

        public MailMessageBuilder WithAttachments(IMailAttachment[] attachments)
        {
            attachments.ForEach(a => _mailMessageModel.Attachments.Add(a));
            return this;
        }

        public MailMessageBuilder WithAttachments(string[] physicalPath)
        {
            var atts = physicalPath.Select(a => new MailAttachment(a)).Cast<IMailAttachment>().ToArray();
            WithAttachments(atts);
            return this;
        }

        public MailMessageBuilder WithAttachments(string physicalPath)
        {
            WithAttachments(new[] {physicalPath});
            return this;
        }

        public MailMessageBuilder WithAttachments(string physicalPath, params string[] physicalPaths)
        {
            var atts = new List<string>(physicalPaths) {physicalPath}.ToArray();
            WithAttachments(atts);
            return this;
        }

        public MailMessageModel Build()
        {
            return _mailMessageModel;
        }
    }
}