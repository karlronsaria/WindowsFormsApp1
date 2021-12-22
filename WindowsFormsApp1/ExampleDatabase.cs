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

        private Document GetRecordMatchingName(string name)
        {
            return (
                from record in MySampleData.MyRecords
                where record.Name == name
                select record
            )
            .FirstOrDefault()
            ;
        }

        private IEnumerable<Document> GetRecordsMatchingPattern(string pattern)
        {
            return (
                from record in MySampleData.MyRecords
                where Regex.IsMatch(record.Name, pattern)
                select record
            )
            ;
        }

        private IEnumerable<Document> GetRecordsMatchingSubstring(string substring)
        {
            return (
                from record in MySampleData.MyRecords
                where record.Name.Contains(substring)
                select record
            )
            ;
        }

        private IEnumerable<Document> GetRecordsMatchingTag(string tag)
        {
            return (
                from record in MySampleData.MyRecords
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
                from record in MySampleData.MyRecords
                where Regex.IsMatch(record.Name, pattern)
                select record.Name
            )
            ;
        }

        public IEnumerable<string> GetNamesMatchingSubstring(string substring)
        {
            return (
                from record in MySampleData.MyRecords
                where record.Name.Contains(substring)
                select record.Name
            )
            ;
        }

        public IEnumerable<string> GetNamesMatchingTag(string tag)
        {
            return (
                from record in MySampleData.MyRecords
                where record.Tags.Contains(tag)
                select record.Name
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
            return GetTagsMatchingRecord(GetRecordMatchingName(tag));
        }

        public void SetTags(IEnumerable<string> names, IEnumerable<string> tags)
        {
            throw new NotImplementedException();

            // foreach (var name in names)
            // {
            //     var records = from record in MySampleData.MyRecords
            //                   where record.Name == name
            //                   select record;

            //     if (!records.Any())
            //     {
            //         MySampleData.MyRecords.Add(
            //             new Document
            //             {
            //                 Name = name,
            //                 Tags = tags,
            //             }
            //         );

            //         return;
            //     }

            //     foreach (var record in records)
            //     {
            //         record.Tags = tags.ToArray<string>();
            //     }
            // }
        }
    }
}
