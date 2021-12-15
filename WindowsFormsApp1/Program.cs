using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PdfiumViewer;
using System.IO;
using Application;

namespace WindowsFormsApp1
{
    internal static class Program
    {
        class ExampleDatabase : IRecordContext
        {
            public ExampleDatabase() { }

            public ICollection<Record> Records { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.Run(new Form1(new ExampleDatabase()));
        }
    }
}
