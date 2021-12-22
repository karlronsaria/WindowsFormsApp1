using System;
using System.Collections.Generic;
using System.Linq;
using Application;

namespace WindowsFormsApp1
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

            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            // System.Windows.Forms.Application.Run(new Form1(new ExampleDatabase()));
            System.Windows.Forms.Application.Run(new Form1(new JsonFileDatabase(JSON_FILE_PATH)));
        }
    }
}
