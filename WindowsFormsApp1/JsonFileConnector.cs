using Application;
using NewtonsoftJsonData;

namespace Infrastructure
{
    public class JsonFileConnector :
        IJson<Application.Root>
    {
        public Application.Root Get(string filePath)
        {
            return NewtonsoftJsonData.Db<Application.Root>.FromFile(filePath);
        }

        public void Set(Application.Root root, string filePath)
        {
            NewtonsoftJsonData.Db<Application.Root>.OutFile(root, filePath);
        }
    }
}
