using System;
using System.Collections.Generic;

namespace Application
{
    public class MyEnumerable<T> : HashSet<T> { }

    public class Document
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public MyEnumerable<string> Tags { get; set; }
        public MyEnumerable<DateTime> Dates { get; set; }
        public string Description { get; set; }
    }

    public class Tag
    {
        public string Name { get; set; }
        public MyEnumerable<int> DocumentIds { get; set; }
    }

    public class Root
    {
        public MyEnumerable<Document> Documents { get; set; }
        public MyEnumerable<Tag> Tags { get; set; }
    }

}
