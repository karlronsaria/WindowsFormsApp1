namespace Infrastructure
{
    public enum DataSource
    {
        Sqlite,
        Mssql,
    }

    public class Settings
    {
        public string StartingDirectory { get; set; }
        public string MostRecentJsonFile { get; set; }
        public string SqliteConnectionString { get; set; }
        public string MssqlConnectionString { get; set; }
        public DataSource DataSource { get; set; }

        public static Settings Read(string filePath)
        {
            try
            {
                return NewtonsoftJsonData.Db<Settings>.FromFile(filePath);
            }
            catch (System.Exception)
            {
                return new Settings();
            }
        }

        public static void Write(Settings mySettings, string filePath)
        {
            NewtonsoftJsonData.Db<Settings>.OutFile(mySettings, filePath);
        }

        public static void Complete(Settings to, Settings from)
        {
            foreach (var prop in typeof(Settings).GetProperties())
                if (prop.GetValue(to) == null)
                    prop.SetValue(to, prop.GetValue(from));
        }
    }
}
