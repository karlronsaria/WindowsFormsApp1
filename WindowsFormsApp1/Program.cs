using Infrastructure;

namespace WindowsFormsApp1
{
    internal static class Program
    {
        const string
        STARTING_DIRECTORY = @"C:\Users\karlr\OneDrive\__POOL";

        static readonly string
        JSON_FILE_PATH = $@"{STARTING_DIRECTORY}\__NEW_2021_12_11_153848\db.json";

        const string
        DEFAULT_CONNECTION_STRING = @"Data Source=D:\Databases\Sample9.db";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [System.STAThread]
        static void Main()
        {
            // MyForms.IDataContext myDatabase = new Infrastructure.ExampleDatabase();
            // MyForms.IDataConnector myDatabase = new Infrastructure.JsonFileSimpleDataConnector(JSON_FILE_PATH);

            MyForms.IDataConnector myDatabase2
                = new Infrastructure.FrameworkConnector
                    <PersistentConnector, JsonFileConnector, Application.Root>
                    (
                        new Infrastructure.PersistentConnector(
                            connectionString: DEFAULT_CONNECTION_STRING
                        ),
                        new Infrastructure.JsonFileConnector()
                    );

            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            System.Windows.Forms.Application.Run(
                new MyForms.Form1(myDatabase2, STARTING_DIRECTORY)
                {
                    MostRecentJsonFile = JSON_FILE_PATH,
                }
            );
        }
    }
}
