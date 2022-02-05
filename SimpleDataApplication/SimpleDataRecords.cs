using Application;

namespace SimpleData
{
    public class Document : Application.Document
    {
        public MyEnumerable<string> Tags { get; set; }
            = new MyEnumerable<string>();
        public MyEnumerable<System.DateTime> Dates { get; set; }
            = new MyEnumerable<System.DateTime>();
    }

    public class Tag : Application.Tag
    {
        public MyEnumerable<int> DocumentIds { get; set; }
            = new MyEnumerable<int>();
    }
}
