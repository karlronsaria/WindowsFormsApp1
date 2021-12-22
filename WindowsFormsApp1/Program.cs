using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Application;
using Newtonsoft.Json;

namespace WindowsFormsApp1
{
    internal static class Program
    {
        private class JsonFileDatabase : IDataContext
        {
            // private class Root
            // {
            //     public List<Document> Documents { get; set; }
            //     public List<Tag> Tags { get; set; }

            //     public Root()
            //     {
            //         Documents = new List<Document>();
            //         Tags = new List<Tag>();
            //     }
            // }

            private readonly Root _data;

            public JsonFileDatabase(string filePath)
            {
                // _data = new NewtonsoftJsonData.Db<Root>(filePath).Data; 

                string json = File.ReadAllText(filePath);
                _data = JsonConvert.DeserializeObject<Root>(json);

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

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            const string jsonFilePath = @"C:\Users\karlr\OneDrive\__POOL\__NEW_2021_12_11_153848\db.json";

            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            // System.Windows.Forms.Application.Run(new Form1(new ExampleDatabase()));
            System.Windows.Forms.Application.Run(new Form1(new JsonFileDatabase(jsonFilePath)));
        }
    }
}
