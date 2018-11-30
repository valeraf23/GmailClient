using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.FileProviders;

namespace Gmail.Test
{
    public class DocumentTemplatesProvider : IDocumentTemplatesProvider
    {
        private readonly IFileProvider _filesProvider;

        public DocumentTemplatesProvider(IFileProvider filesProvider)
        {
            _filesProvider = filesProvider;
        }

        public IFileInfo GetFileByName(string fileName)
        {
            return GiveByCriteria(f => f.Name == fileName).FirstOrDefault();
        }

        public IEnumerable<IFileInfo> GiveByCriteria(Func<IFileInfo, bool> criteria)
        {
            return GetFiles().Where(criteria);
        }

        public IEnumerable<IFileInfo> GetFiles()
        {
            var directoryContents = _filesProvider.GetDirectoryContents("DocumentTemplates");
            return directoryContents.Where(x => !x.IsDirectory);
        }
    }
}