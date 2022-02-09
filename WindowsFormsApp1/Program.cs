using Infrastructure;

namespace WindowsFormsApp1
{
    internal static class Program
    {
        const string
        SETTINGS_FILE = @".\settings.json";

        const string
        STARTING_DIRECTORY = @"C:\Users\karlr\OneDrive\__POOL";

        static readonly Settings
        DEFAULT_SETTINGS = new Settings()
        {
            StartingDirectory = STARTING_DIRECTORY,
            MostRecentJsonFile = $@"{STARTING_DIRECTORY}\__NEW_2021_12_11_153848\db.json",
            ConnectionString = @"Data Source=D:\Databases\Sample9.db",
        };

        static Settings mySettings;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [System.STAThread]
        static void Main()
        {
            mySettings = Settings.Read(SETTINGS_FILE);
            Settings.Complete(mySettings, DEFAULT_SETTINGS);

            var context = new Persistent.SqliteContext(
                connectionString: mySettings.ConnectionString
            );

            var dataConnector = new Infrastructure.PersistentConnector<Persistent.SqliteContext>(
                context: context
            );

            var jsonConnector = new Infrastructure.JsonFileConnector();

            MyForms.IDataConnector myData
                = new Infrastructure.FrameworkConnector<
                    PersistentConnector<Persistent.SqliteContext>,
                    JsonFileConnector,
                    Application.Root
                >(
                    dataConnector: dataConnector,
                    jsonConnector: jsonConnector
                );

            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            var mainForm = new MyForms.Form1(myData, STARTING_DIRECTORY)
            {
                MostRecentJsonFile = mySettings.MostRecentJsonFile,
            };

            System.Windows.Forms.Application.Run(
                mainForm: mainForm
            );

            mySettings.MostRecentJsonFile = mainForm.MostRecentJsonFile;
            Settings.Write(mySettings, SETTINGS_FILE);
        }
    }
}
