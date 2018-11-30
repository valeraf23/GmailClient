namespace Gml.Interfaces
{
    public interface IMailAttachment
    {
        string FileName { get; set; }
        string ContentType { get; set; }
        byte[] Data { get; set; }
        void SaveTo(string filePath);
    }
}