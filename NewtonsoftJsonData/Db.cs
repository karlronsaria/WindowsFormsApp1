using Newtonsoft.Json;
using System.IO;

namespace NewtonsoftJsonData
{
    public class Db<T>
    {
        private readonly T _data;

        public Db(string fileName)
        {
            var serializer = new JsonSerializer();

            using (FileStream fs = File.Open(fileName, FileMode.Open))
            using (var reader = new StreamReader(fs))
            using (var jsonReader = new JsonTextReader(reader))
            {
                jsonReader.SupportMultipleContent = true;
                _data = serializer.Deserialize<T>(jsonReader);
            }
        }

        public T Data { get { return _data; } }

        public static void OutFile(T data, string filePath)
        {
            var serializer = new JsonSerializer();

            serializer.Converters.Add(new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (var sw = new StreamWriter(filePath))
            using (var writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, data);
            }
        }

        public void OutFile(string filePath)
        {
            Db<T>.OutFile(_data, filePath);
        }
    }
}
