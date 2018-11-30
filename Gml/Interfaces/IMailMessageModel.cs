using System.Collections.Generic;
using System.Net.Mail;

namespace Gml.Interfaces
{
    public interface IMailMessageModel
    {
        IList<MailAddress> To { get; }
        IList<MailAddress> Cc { get; }
        IList<MailAddress> Bcc { get; }
        string Subject { get; set; }
        string Body { get; set; }
        ICollection<IMailAttachment> Attachments { get; }
    }
}