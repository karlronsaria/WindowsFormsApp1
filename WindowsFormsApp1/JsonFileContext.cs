using Application;

namespace Infrastructure
{
    public class JsonFileContext : SimpleDataContext
    {
        public JsonFileContext(string filePath)
        {
            _data = new NewtonsoftJsonData.Db<Root>(filePath).Data;

            if (_data == null)
                _data = new Root();
        }
    }
}
