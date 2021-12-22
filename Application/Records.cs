using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    /*
    public class Document
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Tags { get; set; }
        public List<DateTime> Dates { get; set; }
        public string Description { get; set; }
    }

    public class Tag
    {
        public string Name { get; set; }
        public List<int> DocumentIds { get; set; }
    }

    public class Root
    {
        public List<Document> Documents { get; set; }
        public List<Tag> Tags { get; set; }
    }
    */


    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 

    public class Document
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Tags { get; set; }
    }

    public class Tag
    {
        public string Name { get; set; }
        public List<int> DocumentIds { get; set; }
    }

    public class Root
    {
        public List<Document> Documents { get; set; }
        public List<Tag> Tags { get; set; }
    }

}
