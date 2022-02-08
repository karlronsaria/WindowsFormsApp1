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
            var context = new Persistent.SqliteContext(
                connectionString: DEFAULT_CONNECTION_STRING
            );

            var dataConnector = new Infrastructure.PersistentConnector<Persistent.SqliteContext>(
                context: context
            );

            var jsonConnector = new Infrastructure.JsonFileConnector();

            MyForms.IDataConnector myData
                = new Infrastructure.FrameworkConnector
                    <PersistentConnector<Persistent.SqliteContext>,
                    JsonFileConnector,
                    Application.Root>
                    (
                        dataConnector: dataConnector,
                        jsonConnector: jsonConnector
                    );

            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            System.Windows.Forms.Application.Run(
                new MyForms.Form1(myData, STARTING_DIRECTORY)
                {
                    MostRecentJsonFile = JSON_FILE_PATH,
                }
            );
        }
    }
}
