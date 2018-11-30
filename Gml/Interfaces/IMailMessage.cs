using System;
using System.Collections.Generic;

namespace Gml.Interfaces
{
    public interface IMailMessage
    {
        string From { get; }
        IList<string> To { get; }
        IList<string> Cc { get; }
        IList<string> Bcc { get; }
        string Subject { get; }
        string Body { get; }
        ICollection<IMailAttachment> Attachments { get; }
        DateTime ReceivedTime { get; }
        void AddAttachment(string filePath);
    }
}