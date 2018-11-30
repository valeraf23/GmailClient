using System.Collections.Generic;

namespace Gml
{
    public class GmailConfiguration
    {
        public string User { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public List<string> Scope { get; set; }
    }
}