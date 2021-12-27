using System;
using WindowsFormsApp1;

namespace Infrastructure
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            const string JSON_FILE_PATH =
                @"C:\Users\karlr\OneDrive\__POOL\__NEW_2021_12_11_153848\db.json";

            // IDataContext myDatabase = new ExampleDatabase();
            IDataContext myDatabase = new JsonFileDatabase(JSON_FILE_PATH);
            new WindowsFormsApp1.Program(myDatabase);
            WindowsFormsApp1.Program.Main();
        }
    }
}
