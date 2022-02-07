namespace Application
{
    public class MyEnumerable<T> : System.Collections.Generic.HashSet<T> { }

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
