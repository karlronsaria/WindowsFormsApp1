namespace Infrastructure
{
    public class JsonFileSimpleDataConnector : SimpleDataConnector
    {
        public JsonFileSimpleDataConnector(string filePath)
        {
            SetFromJson(filePath);
        }
    }
}
