using Application;

namespace Json
{
    public interface IDataSource
    {
        MyEnumerable<Document> GetDocuments();
        MyEnumerable<Tag> GetTags();
        MyEnumerable<Date> GetDates();
        MyEnumerable<DocumentDate> GetDocumentDates();
        MyEnumerable<DocumentTag> GetDocumentTags();
    }

    public class Root<DocT, DateT, TagT>
    {
        public MyEnumerable<DocT> Documents { get; set; }
            = new MyEnumerable<DocT>();
        public MyEnumerable<DateT> Dates { get; set; }
            = new MyEnumerable<DateT>();
        public MyEnumerable<TagT> Tags { get; set; }
            = new MyEnumerable<TagT>();
        public MyEnumerable<DocumentDate> DocumentDates { get; set; }
            = new MyEnumerable<DocumentDate>();
        public MyEnumerable<DocumentTag> DocumentTags { get; set; }
            = new MyEnumerable<DocumentTag>();
    }

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

        public static Root
        NewRootFromSource<DataSourceT>(DataSourceT source)
            where DataSourceT : IDataSource, new()
        {
            return new Root()
            {
                Documents = source.GetDocuments(),
                Dates = source.GetDates(),
                Tags = source.GetTags(),
                DocumentDates = source.GetDocumentDates(),
                DocumentTags = source.GetDocumentTags(),
            };
        }
    }
}
