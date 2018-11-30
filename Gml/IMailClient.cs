using System.Collections.Generic;
using Gml.Interfaces;

namespace Gml
{
    public interface IMailClient
    {
        IList<IMailMessage> GetMessages(params ISearchMailFilter[] conditions);

        void Send(IMailMessageModel messageBuilder);

        void Delete(IMailMessage message);

        void Delete(params ISearchMailFilter[] conditions);
    }
}