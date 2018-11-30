using System.IO;
using Gml.Interfaces;
using MimeMapping;

namespace Gml.Components
{
    public class MailAttachment : IMailAttachment
    {
        public MailAttachment()
        {
        }

        public MailAttachment(string filePath)
        {
            FileName = Path.GetFileName(filePath);
            ContentType = MimeUtility.GetMimeMapping(filePath);
            Data = File.ReadAllBytes(filePath);
        }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public byte[] Data { get; set; }

        public void SaveTo(string filePath)
        {
            File.WriteAllBytes(filePath, Data);
        }
    }
}