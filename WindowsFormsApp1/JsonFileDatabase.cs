using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Application;

namespace WindowsFormsApp1
{
    public class JsonFileDatabase : IDataContext
    {
        private readonly Root _data;

        public JsonFileDatabase(string filePath)
        {
            _data = new NewtonsoftJsonData.Db<Root>(filePath).Data; 

            if (_data == null)
            {
                _data = new Root();
            }
        }

        private Document GetRecordMatchingName(string name)
        {
            return (
                from record in _data.Documents
                where record.Name == name
                select record
            )
            .FirstOrDefault()
            ;
        }

        private IEnumerable<Document> GetRecordsMatchingPattern(string pattern)
        {
            return (
                from record in _data.Documents
                where Regex.IsMatch(record.Name, pattern)
                select record
            )
            ;
        }

        private IEnumerable<Document> GetRecordsMatchingSubstring(string substring)
        {
            return (
                from record in _data.Documents
                where record.Name.Contains(substring)
                select record
            )
            ;
        }

        private IEnumerable<Document> GetRecordsMatchingTag(string tag)
        {
            return (
                from record in _data.Documents
                where record.Tags.Contains(tag)
                select record
            )
            ;
        }

        private IEnumerable<string> GetTagsMatchingRecord(Document myRecord)
        {
            return myRecord.Tags;
        }

        public IEnumerable<string> GetNamesMatchingPattern(string pattern)
        {
            return (
                from record in _data.Documents
                where Regex.IsMatch(record.Name, pattern)
                select record.Name
            )
            ;
        }

        public IEnumerable<string> GetNamesMatchingSubstring(string substring)
        {
            return (
                from record in _data.Documents
                where record.Name.Contains(substring)
                select record.Name
            )
            ;
        }

        public IEnumerable<string> GetNamesMatchingTag(string tag)
        {
            return (
                from record in _data.Documents
                where record.Tags.Contains(tag)
                select record.Name
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
            return GetTagsMatchingRecord(GetRecordMatchingName(tag));
        }

        public void SetTags(IEnumerable<string> names, IEnumerable<string> tags)
        {
            throw new NotImplementedException();
        }
    }
}
