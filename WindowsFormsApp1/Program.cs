namespace WindowsFormsApp1
{
    internal static class Program
    {
        // const string STARTING_DIRECTORY = @"C:\";

        const string
        STARTING_DIRECTORY = @"C:\Users\karlr\OneDrive\__POOL";

        static readonly string
        JSON_FILE_PATH = $@"{STARTING_DIRECTORY}\__NEW_2021_12_11_153848\db.json";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [System.STAThread]
        static void Main()
        {
            // MyForms.IDataContext myDatabase = new Infrastructure.ExampleDatabase();
            MyForms.IDataContext myDatabase = new Infrastructure.JsonFileContext(JSON_FILE_PATH);

            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.Run(
                new MyForms.Form1(myDatabase, STARTING_DIRECTORY)
                {
                    MostRecentJsonFile = JSON_FILE_PATH,
                }
            );
        }
    }
}
