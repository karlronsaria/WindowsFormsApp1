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
        public static string[] MyTags => new string[]
        {
            "case",
            "court",
            "judgment",
            "resource",
            "tangential",
            "resource",
            "tangential",
            "charity",
            "auto",
            "claim",
            "insurance",
            "auto",
            "Code-a-thon",
            "Lightsys",
            "event",
        };

        public static Record[] MyRecords => new Record[]
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

            public Record GetRecordMatchingName(string name)
            {
                return (
                    from record in MySampleData.MyRecords
                    where record.RecordName == name
                    select record
                )
                .FirstOrDefault();
            }

            public Record[] GetRecordsMatchingTag(string tag)
            {
                return (Record[])(
                    from record in MySampleData.MyRecords
                    where record.Tags.Contains(tag)
                    select record
                );
            }

            public string[] GetTagsMatchingPattern(string pattern)
            {
                return (string[])(
                    from tag in MySampleData.MyTags
                    where Regex.IsMatch(tag, pattern)
                    select tag
                );
            }

            public string[] GetTagsMatchingRecord(Record myRecord)
            {
                return myRecord.Tags;
            }

            public string[] GetTagsMatchingSubstring(string substring)
            {
                return (string[])(
                    from tag in MySampleData.MyTags
                    where tag.Contains(substring)
                    select tag
                );
            }

            public void SetTags(Record[] records, string[] tags)
            {
                foreach (var record in records)
                {
                    record.Tags = tags;
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
