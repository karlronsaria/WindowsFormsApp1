namespace Application
{
    public class Root
    {
        public MyEnumerable<Document> Documents { get; set; }
            = new MyEnumerable<Document>();
        public MyEnumerable<Date> Dates { get; set; }
            = new MyEnumerable<Date>();
        public MyEnumerable<Tag> Tags { get; set; }
            = new MyEnumerable<Tag>();
        public MyEnumerable<DocumentDate> DocumentDates { get; set; }
            = new MyEnumerable<DocumentDate>();
        public MyEnumerable<DocumentTag> DocumentTags { get; set; }
            = new MyEnumerable<DocumentTag>();
    }
}
