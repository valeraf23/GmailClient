using System;
using System.Collections.Generic;
using System.Linq;
using Gml.Components;
using Gml.Interfaces;

namespace Gml
{
    public class GmailClientWrapper
    {
        private readonly GmailClient _client;

        public GmailClientWrapper(GmailClient client)
        {
            _client = client;
        }

        public void SendInPlainTextMode(string[] to, Func<MailMessageBuilder, MailMessageBuilder> createMsg)
        {
            _client.SendInPlainTextMode(createMsg(MailMessageBuilder.To(to)).Build());
        }

        public void SendInPlainTextMode(string to, Func<MailMessageBuilder, MailMessageBuilder> createMsg)
        {
            SendInPlainTextMode(new[] {to}, createMsg);
        }

        public void Send(Func<MailMessageBuilder, MailMessageBuilder> createMsg, string[] to)
        {
            _client.Send(createMsg(MailMessageBuilder.To(to)).Build());
        }

        public void Send(Func<MailMessageBuilder, MailMessageBuilder> createMsg, string to)
        {
            Send(createMsg, new[] {to});
        }

        public void Send(Func<MailMessageBuilder, MailMessageBuilder> createMsg, string to, params string[] toOthers)
        {
            var recipients = new List<string>(toOthers) {to};
            Send(createMsg, recipients.ToArray());
        }

        public IList<IMailMessage> GetMessages(params By[] filters)
        {
            return _client.GetMessages(filters.Select(f => f.Filter).ToArray());
        }
    }
}