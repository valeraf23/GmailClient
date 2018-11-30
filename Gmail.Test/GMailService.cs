using Gml;

namespace Gmail.Test
{
    public static class GMailService
    {
        public static GmailClientWrapper ForClient(GmailClient client)
        {
            return new GmailClientWrapper(client);
        }
    }
}