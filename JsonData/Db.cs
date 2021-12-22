using System.Text.Json;
using System.IO;

namespace JsonData
{
    public class Db<T>
    {
        private readonly T _data;

        public Db(string fileName)
        {
            string json = File.ReadAllText(fileName);
            _data = JsonSerializer.Deserialize<T>(json);
        }

        public T Data { get { return _data; } }
    }
}
