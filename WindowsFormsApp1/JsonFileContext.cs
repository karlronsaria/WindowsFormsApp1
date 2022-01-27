using Application;

namespace Infrastructure
{
    public class JsonFileContext : SimpleDataContext
    {
        public JsonFileContext(string filePath)
        {
            FromJson(filePath);
        }
    }
}
