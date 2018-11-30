using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Gml.Helpers.GuardArgument;
using Gml.Interfaces;

namespace Gml.Components
{
    public class MailMessageModel : IMailMessageModel
    {
        public MailMessageModel(params MailAddress[] to)
        {
            To = to.ToList();
        }

        public MailMessageModel(params string[] to)
        {
            var regex = new Regex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");
            GuardArgument.ArgumentOutOfCondition(to, m => m.All(x => regex.IsMatch(x)),
                "Some of arguments was not recognized like email address");
            To = to.Select(address => new MailAddress(address)).ToList();
        }

        public IList<MailAddress> To { get; }
        public IList<MailAddress> Cc { get; set; } = new List<MailAddress>();
        public IList<MailAddress> Bcc { get; set; } = new List<MailAddress>();
        public string Subject { get; set; }
        public string Body { get; set; } = string.Empty;
        public ICollection<IMailAttachment> Attachments { get; set; } = new List<IMailAttachment>();
    }
}