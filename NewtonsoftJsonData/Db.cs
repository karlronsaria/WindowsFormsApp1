using Newtonsoft.Json;
using System.IO;

namespace NewtonsoftJsonData
{
    public class Db<T>
        where T : new()
    {
        private readonly T _data;

        public static JsonSerializer NewSerializer
        {
            get
            {
                var mySerializer = new JsonSerializer();
                mySerializer.Converters.Add(new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter());
                mySerializer.NullValueHandling = NullValueHandling.Ignore;
                return mySerializer;
            }
        }

        public Db(string fileName)
        {
            var serializer = NewSerializer;

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
            var serializer = NewSerializer;

            using (var sw = new StreamWriter(filePath))
            using (var writer = new JsonTextWriter(sw))
                serializer.Serialize(writer, data);
        }

        public void OutFile(string filePath)
        {
            Db<T>.OutFile(_data, filePath);
        }

        public static T FromFile(string filePath)
        {
            T data = new Db<T>(filePath).Data;

            if (data == null)
                data = new T();

            return data;
        }
    }
}
