using Newtonsoft.Json;
using System.IO;

namespace NewtonsoftJsonData
{
    public class Db<T>
    {
        private readonly T _data;

        public Db(string fileName)
        {
            // string json = File.ReadAllText(fileName);
            // _data = JsonConvert.DeserializeObject<T>(json);

            // string jsonProxy = JsonConvert.DeserializeObject<string>(json);
            // _data = JsonConvert.DeserializeObject<T>(jsonProxy);

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
    }
}
