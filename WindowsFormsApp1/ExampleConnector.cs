using System.Collections.Generic;
using System.Linq;
using Application;

namespace Infrastructure
{
    class ExampleConnector : SimpleDataConnector
    {
        public ExampleConnector()
        {
            IEnumerable<Tag> tags = (
                from tag in MySampleData.MyTags
                select new SimpleData.Tag { Name = tag, DocumentIds = new MyEnumerable<int>(), }
            );

            foreach (SimpleData.Tag tag in tags)
                foreach (int documentId in (
                    from document in MySampleData.MyDocuments
                    where document.Tags.Contains(tag.Name)
                    select document.Id
                ))
                    tag.DocumentIds.Add(documentId);

            _data = new SimpleDataConnector.Root()
            {
                Documents = new MyEnumerable<SimpleData.Document>(),
                Tags = new MyEnumerable<SimpleData.Tag>(),
            };

            foreach (SimpleData.Document document in MySampleData.MyDocuments)
                _data.Documents.Add(document);

            foreach (SimpleData.Tag tag in tags)
                _data.Tags.Add(tag);
        }
    }
}
