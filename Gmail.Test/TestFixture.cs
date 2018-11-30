using System;
using System.IO;
using Gml;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gmail.Test
{
    public class TestFixture : IDisposable
    {
        public TestFixture()
        {
            var config = Deserialize<GmailConfiguration>("GmailConfiguration.json");
            Client = new GmailClient(config);

            FileProvider = new DocumentTemplatesProvider(new PhysicalFileProvider(AppContext.BaseDirectory));
        }

        public GmailClient Client { get; }
        public IDocumentTemplatesProvider FileProvider { get; }

        public void Dispose()
        {
            //            var messagesSent = Wait.For(() => Client.GetMessages(new FromFilter("ipreo.pcs@gmail.com")))
            //                .Become(m => m.Count == 12, 10, TimeSpan.FromSeconds(2));
            //            messagesSent.ForEach(m => Client.Delete(m));  
        }

        public static T Deserialize<T>(string path)
        {
            JObject obj;
            using (var reader = new StreamReader(path))
            {
                obj = (JObject) JToken.ReadFrom(new JsonTextReader(reader));
            }

            return obj.ToObject<T>(JsonSerializer.Create());
        }
    }
}