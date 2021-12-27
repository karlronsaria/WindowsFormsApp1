using System.Collections.Generic;
using System.Linq;
using Application;

namespace Infrastructure
{
    class ExampleDatabase : SimpleDataContext
    {
        public ExampleDatabase()
        {
            IEnumerable<Tag> tags = (
                from tag in MySampleData.MyTags
                select new Tag { Name = tag, DocumentIds = new MyEnumerable<int>(), }
            );

            foreach (Tag tag in tags)
                foreach (int documentId in (
                    from document in MySampleData.MyDocuments
                    where document.Tags.Contains(tag.Name)
                    select document.Id
                ))
                    tag.DocumentIds.Add(documentId);

            _data = new Root()
            {
                Documents = new MyEnumerable<Document>(),
                Tags = new MyEnumerable<Tag>(),
            };

            foreach (Document document in MySampleData.MyDocuments)
                _data.Documents.Add(document);

            foreach (Tag tag in tags)
                _data.Tags.Add(tag);
        }
    }
}
