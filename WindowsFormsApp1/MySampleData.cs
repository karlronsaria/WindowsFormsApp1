using System.Collections.Generic;
using Application;

namespace Infrastructure
{
    public static class MySampleData
    {
        public static IEnumerable<string>
        MyTags => new MyEnumerable<string>
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

        public static IEnumerable<SimpleData.Document>
        MyDocuments => new MyEnumerable<SimpleData.Document>
        {
            new SimpleData.Document
            {
                Id = 2,
                Name = "Scan11172021143731.pdf",
                Tags = new MyEnumerable<string>()
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

            new SimpleData.Document
            {
                Id = 3,
                Name = "Scan11282021155143.pdf",
                Tags = new MyEnumerable<string>()
                {
                    "case",
                    "court",
                    "judgment",
                },
            },

            new SimpleData.Document
            {
                Id = 4,
                Name = "Scan12052021201615.pdf",
                Tags = new MyEnumerable<string>()
				{
                    "case",
                    "court",
                    "judgment",
				},
            },

            new SimpleData.Document
            {
                Id = 5,
                Name = "Scan12052021155822.pdf",
                Tags = new MyEnumerable<string>()
				{
                     "charity",
				},
            },

            new SimpleData.Document
            {
                Id = 6,
                Name = "Scan11172021140909.pdf",
                Tags = new MyEnumerable<string>()
				{
					"auto",
					"claim",
					"insurance",
				},
            },

            new SimpleData.Document
            {
                Id = 7,
                Name = "Scan11172021174637.pdf",
                Tags = new MyEnumerable<string>()
				{
					"auto",
					"claim",
					"insurance",
				},
            },

            new SimpleData.Document
            {
                Id = 8,
                Name = "Scan11172021174900.pdf",
                Tags = new MyEnumerable<string>()
				{
					"auto",
					"claim",
					"insurance",
				},
            },

            new SimpleData.Document
            {
                Id = 9,
                Name = "Scan11172021175722.pdf",
                Tags = new MyEnumerable<string>()
				{
					"auto",
					"claim",
					"insurance",
				},
            },

            new SimpleData.Document
            {
                Id = 10,
                Name = "Scan12012021203535.pdf",
                Tags = new MyEnumerable<string>()
				{
					"auto",
					"claim",
					"insurance",
				},
            },

            new SimpleData.Document
            {
                Id = 11,
                Name = "Scan12052021190925.pdf",
                Tags = new MyEnumerable<string>()
				{
					"auto",
					"claim",
					"insurance",
				},
            },

            new SimpleData.Document
            {
                Id = 12,
                Name = "Scan12012021205234.pdf",
                Tags = new MyEnumerable<string>()
				{
					"Code-a-thon",
					"Lightsys",
					"event",
				},
            },
        };
    }
}
