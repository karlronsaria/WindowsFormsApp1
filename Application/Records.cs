using System.Collections.Generic;

// TODO: Remove property instantiations

namespace Application
{
    public class MyEnumerable<T> : HashSet<T> { }

    public class Document
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public MyEnumerable<string> Tags { get; set; }
            = new MyEnumerable<string>();
        public MyEnumerable<System.DateTime> Dates { get; set; }
            = new MyEnumerable<System.DateTime>();
        public string Description { get; set; }
    }

    public class Tag
    {
        public string Name { get; set; }
        public MyEnumerable<int> DocumentIds { get; set; }
            = new MyEnumerable<int>();
    }

    public class Root
    {
        public MyEnumerable<Document> Documents { get; set; }
            = new MyEnumerable<Document>();
        public MyEnumerable<Tag> Tags { get; set; }
            = new MyEnumerable<Tag>();
    }
}
