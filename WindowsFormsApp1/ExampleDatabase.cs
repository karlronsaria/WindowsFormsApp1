using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Application;

namespace WindowsFormsApp1
{
    class ExampleDatabase : IDataContext
    {
        public ExampleDatabase() { }

        private Document GetDocumentMatchingName(string name)
        {
            return (
                from document in MySampleData.MyDocuments
                where document.Name == name
                select document
            )
            .FirstOrDefault()
            ;
        }

        private IEnumerable<Document> GetDocumentsMatchingPattern(string pattern)
        {
            return (
                from document in MySampleData.MyDocuments
                where Regex.IsMatch(document.Name, pattern)
                select document
            )
            ;
        }

        private IEnumerable<Document> GetDocumentsMatchingSubstring(string substring)
        {
            return (
                from document in MySampleData.MyDocuments
                where document.Name.Contains(substring)
                select document
            )
            ;
        }

        private IEnumerable<Document> GetDocumentsMatchingTag(string tag)
        {
            return (
                from document in MySampleData.MyDocuments
                where document.Tags.Contains(tag)
                select document
            )
            ;
        }

        private IEnumerable<string> GetTagsMatchingDocument(Document myDocument)
        {
            return myDocument.Tags;
        }

        public IEnumerable<string> GetNamesMatchingPattern(string pattern)
        {
            return (
                from document in MySampleData.MyDocuments
                where Regex.IsMatch(document.Name, pattern)
                select document.Name
            )
            ;
        }

        public IEnumerable<string> GetNamesMatchingSubstring(string substring)
        {
            return (
                from document in MySampleData.MyDocuments
                where document.Name.Contains(substring)
                select document.Name
            )
            ;
        }

        public IEnumerable<string> GetNamesMatchingTag(string tag)
        {
            return (
                from document in MySampleData.MyDocuments
                where document.Tags.Contains(tag)
                select document.Name
            )
            ;
        }

        public IEnumerable<string> GetTagsMatchingPattern(string pattern)
        {
            return (
                from tag in MySampleData.MyTags
                where Regex.IsMatch(tag, pattern)
                select tag
            )
            ;
        }

        public IEnumerable<string> GetTagsMatchingSubstring(string substring)
        {
            return (
                from tag in MySampleData.MyTags
                where tag.Contains(substring)
                select tag
            )
            ;
        }

        public IEnumerable<string> GetTagsMatchingName(string tag)
        {
            return GetTagsMatchingDocument(GetDocumentMatchingName(tag));
        }

        public void SetTags(IEnumerable<string> names, IEnumerable<string> tags)
        {
            throw new NotImplementedException();
        }
    }
}
