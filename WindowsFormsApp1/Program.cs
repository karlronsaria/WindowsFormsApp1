namespace WindowsFormsApp1
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [System.STAThread]
        static void Main()
        {
            const string JSON_FILE_PATH =
                @"C:\Users\karlr\OneDrive\__POOL\__NEW_2021_12_11_153848\db.json";

            const string STARTING_DIRECTORY = @"C:\Users\karlr\OneDrive\__POOL";
            // const string STARTING_DIRECTORY = @"C:\";

            // MyForms.IDataContext myDatabase = new Infrastructure.ExampleDatabase();
            MyForms.IDataContext myDatabase = new Infrastructure.JsonFileContext(JSON_FILE_PATH);

            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.Run(new MyForms.Form1(myDatabase, STARTING_DIRECTORY));
        }
    }
}
