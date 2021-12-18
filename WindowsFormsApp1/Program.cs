using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PdfiumViewer;
using System.IO;
using Application;
using System.Text.RegularExpressions;

namespace WindowsFormsApp1
{
    public static class MySampleData
    {
        public static IList<string> MyTags => new List<string>
        {
            "case",
            "court",
            "judgment",
            "resource",
            "tangential",
            "resource",
            "charity",
            "auto",
            "claim",
            "insurance",
            "Code-a-thon",
            "Lightsys",
            "event",
        };

        public static IList<Record> MyRecords => new List<Record>
        {
            new Record
            {
                RecordId = 2,
                RecordName = "Scan11172021143731.pdf",
                Tags = new string[]
                {
                    "case",
                    "court",
                    "judgment",
                    "resource",
                    "tangential",
                    "resource",
                    "tangential",
                },
            },

            new Record
            {
                RecordId = 3,
                RecordName = "Scan11282021155143.pdf",
                Tags = new string[]
                {
                    "case",
                    "court",
                    "judgment",
                },
            },

            new Record
            {
                RecordId = 4,
                RecordName = "Scan12052021201615.pdf",
                Tags = new string[]
				{
                    "case",
                    "court",
                    "judgment",
				},
            },

            new Record
            {
                RecordId = 5,
                RecordName = "Scan12052021155822.pdf",
                Tags = new string[]
				{
                     "charity",
				},
            },

            new Record
            {
                RecordId = 6,
                RecordName = "Scan11172021140909.pdf",
                Tags = new string[]
				{
					"auto",
					"claim",
					"insurance",
				},
            },

            new Record
            {
                RecordId = 7,
                RecordName = "Scan11172021174637.pdf",
                Tags = new string[]
				{
					"auto",
					"claim",
					"insurance",
				},
            },

            new Record
            {
                RecordId = 8,
                RecordName = "Scan11172021174900.pdf",
                Tags = new string[]
				{
					"auto",
					"claim",
					"insurance",
				},
            },

            new Record
            {
                RecordId = 9,
                RecordName = "Scan11172021175722.pdf",
                Tags = new string[]
				{
					"auto",
					"claim",
					"insurance",
				},
            },

            new Record
            {
                RecordId = 10,
                RecordName = "Scan12012021203535.pdf",
                Tags = new string[]
				{
					"auto",
					"claim",
					"insurance",
				},
            },

            new Record
            {
                RecordId = 11,
                RecordName = "Scan12052021190925.pdf",
                Tags = new string[]
				{
					"auto",
					"claim",
					"insurance",
				},
            },

            new Record
            {
                RecordId = 12,
                RecordName = "Scan12012021205234.pdf",
                Tags = new string[]
				{
					"Code-a-thon",
					"Lightsys",
					"event",
				},
            },
        };
    }

    internal static class Program
    {
        class ExampleDatabase : IRecordContext
        {
            public ExampleDatabase() { }

            private Record GetRecordMatchingName(string name)
            {
                return (
                    from record in MySampleData.MyRecords
                    where record.RecordName == name
                    select record
                )
                .FirstOrDefault()
                ;
            }

            private IEnumerable<Record> GetRecordsMatchingPattern(string pattern)
            {
                return (
                    from record in MySampleData.MyRecords
                    where Regex.IsMatch(record.RecordName, pattern)
                    select record
                )
                ;
            }

            private IEnumerable<Record> GetRecordsMatchingSubstring(string substring)
            {
                return (
                    from record in MySampleData.MyRecords
                    where record.RecordName.Contains(substring)
                    select record
                )
                ;
            }

            private IEnumerable<Record> GetRecordsMatchingTag(string tag)
            {
                return (
                    from record in MySampleData.MyRecords
                    where record.Tags.Contains(tag)
                    select record
                )
                ;
            }

            private IEnumerable<string> GetTagsMatchingRecord(Record myRecord)
            {
                return myRecord.Tags;
            }

            public IEnumerable<string> GetNamesMatchingPattern(string pattern)
            {
                return (
                    from record in MySampleData.MyRecords
                    where Regex.IsMatch(record.RecordName, pattern)
                    select record.RecordName
                )
                ;
            }

            public IEnumerable<string> GetNamesMatchingSubstring(string substring)
            {
                return (
                    from record in MySampleData.MyRecords
                    where record.RecordName.Contains(substring)
                    select record.RecordName
                )
                ;
            }

            public IEnumerable<string> GetNamesMatchingTag(string tag)
            {
                return (
                    from record in MySampleData.MyRecords
                    where record.Tags.Contains(tag)
                    select record.RecordName
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
                foreach (var name in names)
                {
                    var records = from record in MySampleData.MyRecords
                                  where record.RecordName == name
                                  select record;

                    if (!records.Any())
                    {
                        MySampleData.MyRecords.Add(
                            new Record
                            {
                                RecordName = name,
                                Tags = tags,
                            }
                        );

                        return;
                    }

                    foreach (var record in records)
                    {
                        record.Tags = tags.ToArray<string>();
                    }
                }
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.Run(new Form1(new ExampleDatabase()));
        }
    }
}
