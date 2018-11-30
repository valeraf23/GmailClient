using System;
using System.Collections.Generic;
using Microsoft.Extensions.FileProviders;

namespace Gmail.Test
{
    public interface IDocumentTemplatesProvider
    {
        IEnumerable<IFileInfo> GetFiles();
        IFileInfo GetFileByName(string fileName);
        IEnumerable<IFileInfo> GiveByCriteria(Func<IFileInfo, bool> criteria);
    }
}