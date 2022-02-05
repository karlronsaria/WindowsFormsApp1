using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Application;

namespace Infrastructure
{
    public class SimpleDataConnector : MyForms.IDataConnector
    {
        public class Root : Json.Root<SimpleData.Document, Application.Date, SimpleData.Tag> { }

        protected Root _data;

        protected SimpleData.Document
        GetDocumentMatchingName(string name)
        {
            return (
                from document in _data.Documents
                where document.Name == name
                select document
            )
            .FirstOrDefault()
            ;
        }

        protected IEnumerable<SimpleData.Document>
        GetDocumentsMatchingPattern(string pattern)
        {
            return (
                from document in _data.Documents
                where Regex.IsMatch(document.Name, pattern)
                select document
            )
            ;
        }

        protected IEnumerable<SimpleData.Document>
        GetDocumentsMatchingSubstring(string substring, bool exact = true)
        {
            return (
                from document in _data.Documents
                where exact
                    ? document.Name.Contains(substring)
                    : document.Name.ToLowerInvariant()
                        .Contains(substring.ToLowerInvariant())
                select document
            )
            ;
        }

        protected IEnumerable<SimpleData.Document>
        GetDocumentsMatchingTag(string tag)
        {
            return (
                from document in _data.Documents
                where document.Tags.Contains(tag)
                select document
            )
            ;
        }

        protected IEnumerable<string>
        GetTagsMatchingDocument(SimpleData.Document myDocument)
        {
            return myDocument.Tags;
        }

        public IEnumerable<string>
        GetNamesMatchingPattern(string pattern)
        {
            return (
                from document in _data.Documents
                where Regex.IsMatch(document.Name, pattern)
                select document.Name
            )
            ;
        }

        public IEnumerable<string>
        GetNamesMatchingSubstring(string substring, bool exact = true)
        {
            return (
                from document in _data.Documents
                where exact
                    ? document.Name.Contains(substring)
                    : document.Name.ToLowerInvariant()
                        .Contains(substring.ToLowerInvariant())
                select document.Name
            )
            ;
        }

        public IEnumerable<string>
        GetNamesMatchingDate(
                string date,
                string format,  // = MyForms.Formats.DATE_FORMAT
                string pattern  // = MyForms.Formats.DATE_PATTERN_NONCAPTURE
            )
        {
            if (!Regex.IsMatch(date, pattern))
                return new List<string>();

            var outList = new List<string>();
            bool nextDocument;

            foreach (var document in _data.Documents)
            {
                nextDocument = false;

                foreach (var docDate in document.Dates)
                {
                    if (docDate.ToString(format).Contains(date))
                    {
                        outList.Add(document.Name);
                        nextDocument = true;
                    }

                    if (nextDocument)
                        continue;
                }

                if (nextDocument)
                    continue;
            }

            return outList;
        }

        public IEnumerable<string>
        GetDatesMatchingName(
                string name,
                string format  // = MyForms.Formats.DATE_FORMAT
            )
        {
            return (
                from date in (GetDocumentMatchingName(name)?.Dates
                    ?? new MyEnumerable<System.DateTime>())
                select date.ToString(format)
            )
            ;
        }

        public IEnumerable<string>
        GetNamesMatchingTag(string tag)
        {
            return (
                from document in _data.Documents
                where document.Tags.Contains(tag)
                select document.Name
            )
            ;
        }

        public IEnumerable<string>
        GetTagsMatchingPattern(string pattern)
        {
            return (
                from tag in _data.Tags
                where Regex.IsMatch(tag.Name, pattern)
                select tag.Name
            )
            ;
        }

        public IEnumerable<string>
        GetTagsMatchingSubstring(string substring, bool exact = true)
        {
            return (
                from tag in _data.Tags
                where exact
                    ? tag.Name.Contains(substring)
                    : tag.Name.ToLowerInvariant()
                        .Contains(substring.ToLowerInvariant())
                select tag.Name
            )
            ;
        }

        public IEnumerable<string>
        GetTagsMatchingName(string name)
        {
            return GetTagsMatchingDocument(GetDocumentMatchingName(name));
        }

        public void
        AddTags(IEnumerable<string> documentNames, IEnumerable<string> tagNames)
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
                {
                    if (document.Tags == null)
                        document.Tags = new MyEnumerable<string>();

                    if (!document.Tags.Contains(tag))
                        document.Tags.Add(tag);
                }

            foreach (var name in tagNames)
            {
                if (!savedTagNames.Contains(name))
                    _data.Tags.Add(
                        new SimpleData.Tag
                        {
                            Name = name,
                            DocumentIds = new MyEnumerable<int>(),
                        }
                    );

                foreach (SimpleData.Tag myTag in (
                    from tag in _data.Tags
                    where tag.Name == name
                    select tag
                ))
                    foreach (var documentId in savedDocumentIds)
                    {
                        if (myTag.DocumentIds == null)
                            myTag.DocumentIds = new MyEnumerable<int>();

                        if (!myTag.DocumentIds.Contains(documentId))
                            myTag.DocumentIds.Add(documentId);
                    }
            }
        }

        public void
        AddDates(
                IEnumerable<string> documentNames,
                IEnumerable<string> dateStrings,
                string format
            )
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
                {
                    if (document.Dates == null)
                        document.Dates = new MyEnumerable<System.DateTime>();

                    if (!document.Dates.Contains(date))
                        document.Dates.Add(date);
                }
        }

        public static void
        ToJson(Root data, string filePath)
        {
            NewtonsoftJsonData.Db<Root>.OutFile(data, filePath);
        }

        public void
        ToJson(string filePath)
        {
            NewtonsoftJsonData.Db<Root>.OutFile(_data, filePath);
        }

        public void
        SetFromJson(string filePath)
        {
            _data = NewtonsoftJsonData.Db<Root>.FromFile(filePath);
        }

        public void RemoveTags(IEnumerable<string> names, IEnumerable<string> tags)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveDates(IEnumerable<string> names, IEnumerable<string> dates, string format)
        {
            throw new System.NotImplementedException();
        }

        public void AddFromJson(string filePath)
        {
            throw new System.NotImplementedException();
        }
    }
}
