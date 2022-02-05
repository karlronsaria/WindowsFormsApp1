using System.Collections.Generic;

// TODO: Remove property instantiations

namespace Application
{
    public class MyEnumerable<T> : HashSet<T> { }

    public class Document
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class Date
    {
        public int Id { get; set; }
        public System.DateTime Value { get; set; }
    }

    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
