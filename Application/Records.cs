using System;
using System.Collections.Generic;

namespace Application
{
    public class Document
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public HashSet<string> Tags { get; set; }
        public HashSet<DateTime> Dates { get; set; }
        public string Description { get; set; }
    }

    public class Tag
    {
        public string Name { get; set; }
        public HashSet<int> DocumentIds { get; set; }
    }

    public class Root
    {
        public HashSet<Document> Documents { get; set; }
        public HashSet<Tag> Tags { get; set; }
    }

}
