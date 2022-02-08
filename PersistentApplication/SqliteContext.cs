using Microsoft.EntityFrameworkCore;

namespace Persistent
{
    public class SqliteContext : Context
    {
        public const string DEFAULT_CONNECTION_STRING = @"Data Source=D:\Databases\Sample2.db";

        public SqliteContext()
        {
            Database.EnsureCreated();
        }

        public SqliteContext(string connectionString = DEFAULT_CONNECTION_STRING, bool reset = false)
        {
            _connectionString = connectionString;

            if (reset)
                Database.EnsureDeleted();

            /*
             * System.IO.FileNotFoundException
             *   HResult=0x80070002
             *   Message=Could not load file or assembly 'Microsoft.Extensions.Primitives, Version=3.1.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60' or one of its dependencies. The system cannot find the file specified.
             *   Source=Microsoft.Extensions.Caching.Memory
             */
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionbuilder)
        {
            /*
             * System.Exception
             *   HResult=0x80131500
             *   Message=You need to call SQLitePCL.raw.SetProvider().  If you are using a bundle package, this is done by calling SQLitePCL.Batteries.Init().
             *   Source=SQLitePCLRaw.core
             */
            SQLitePCL.Batteries.Init();
            optionbuilder.UseSqlite(_connectionString);
        }
    }
}

