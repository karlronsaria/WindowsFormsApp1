using Microsoft.EntityFrameworkCore;

namespace Persistent
{
    public class MssqlContext : Context
    {
        public const string DEFAULT_CONNECTION_STRING =
            "Server=(localdb)\\mssqllocaldb;Database=WindowsFormsApp1;Trusted_Connection=True;MultipleActiveResultSets=true";

        public MssqlContext()
        {
            Database.EnsureCreated();
        }

        public MssqlContext(string connectionString = DEFAULT_CONNECTION_STRING, bool reset = false)
        {
            _connectionString = connectionString;

            if (reset)
                Database.EnsureDeleted();

            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionbuilder)
        {
            optionbuilder.UseSqlServer(_connectionString);
        }
    }
}
