using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Application;

namespace Infrastructure
{
    public class SimpleDataContext : MyForms.IDataContext
    {
        protected Root _data;

        protected Document GetDocumentMatchingName(string name)
        {
            return (
                from document in _data.Documents
                where document.Name == name
                select document
            )
            .FirstOrDefault()
            ;
        }

        protected IEnumerable<Document> GetDocumentsMatchingPattern(string pattern)
        {
            return (
                from document in _data.Documents
                where Regex.IsMatch(document.Name, pattern)
                select document
            )
            ;
        }

        protected IEnumerable<Document> GetDocumentsMatchingSubstring(string substring)
        {
            return (
                from document in _data.Documents
                where document.Name.Contains(substring)
                select document
            )
            ;
        }

        protected IEnumerable<Document> GetDocumentsMatchingTag(string tag)
        {
            return (
                from document in _data.Documents
                where document.Tags.Contains(tag)
                select document
            )
            ;
        }

        protected IEnumerable<string> GetTagsMatchingDocument(Document myDocument)
        {
            return myDocument.Tags;
        }

        public IEnumerable<string> GetNamesMatchingPattern(string pattern)
        {
            return (
                from document in _data.Documents
                where Regex.IsMatch(document.Name, pattern)
                select document.Name
            )
            ;
        }

        public IEnumerable<string> GetNamesMatchingSubstring(string substring)
        {
            return (
                from document in _data.Documents
                where document.Name.Contains(substring)
                select document.Name
            )
            ;
        }

        public IEnumerable<string> GetNamesMatchingTag(string tag)
        {
            return (
                from document in _data.Documents
                where document.Tags.Contains(tag)
                select document.Name
            )
            ;
        }

        public IEnumerable<string> GetTagsMatchingPattern(string pattern)
        {
            return (
                from tag in _data.Tags
                where Regex.IsMatch(tag.Name, pattern)
                select tag.Name
            )
            ;
        }

        public IEnumerable<string> GetTagsMatchingSubstring(string substring)
        {
            return (
                from tag in _data.Tags
                where tag.Name.Contains(substring)
                select tag.Name
            )
            ;
        }

        public IEnumerable<string> GetTagsMatchingName(string tag)
        {
            return GetTagsMatchingDocument(GetDocumentMatchingName(tag));
        }

        public void SetTags(IEnumerable<string> documentNames, IEnumerable<string> tagNames)
        {
            var savedDocuments =
                from name in documentNames
                select GetDocumentMatchingName(name);

            var savedDocumentIds =
                from document in savedDocuments
                select document.Id;

            var savedTagNames =
                from tag in _data.Tags
                select tag.Name;

            foreach (var document in savedDocuments)
                foreach (var tag in tagNames)
                    if (!document.Tags.Contains(tag))
                        document.Tags.Add(tag);

            foreach (var name in tagNames)
            {
                if (!savedTagNames.Contains(name))
                    _data.Tags.Add(
                        new Tag { Name = name, }
                    );

                foreach (Tag myTag in (
                    from tag in _data.Tags
                    where tag.Name == name
                    select tag
                ))
                    foreach (var documentId in savedDocumentIds)
                        if (!myTag.DocumentIds.Contains(documentId))
                            myTag.DocumentIds.Add(documentId);
            }
        }

        public void SetDates(IEnumerable<string> documentNames, IEnumerable<string> dateStrings, string format)
        {
            var savedDocuments =
                from name in documentNames
                select GetDocumentMatchingName(name);

            var dateTimeObjects = new List<System.DateTime>();

            foreach (var dateString in dateStrings)
                if (System.DateTime.TryParseExact(
                    s: dateString,
                    format: format,
                    provider: null,
                    style: System.Globalization.DateTimeStyles.None,
                    result: out var myDate
                ))
                    dateTimeObjects.Add(myDate);

            foreach (var document in savedDocuments)
                foreach (var date in dateTimeObjects)
                    if (!document.Dates.Contains(date))
                        document.Dates.Add(date);
        }
    }
}
