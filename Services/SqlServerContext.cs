using Microsoft.EntityFrameworkCore;
using CSharpWpfChatGPT.Models;

namespace CSharpWpfChatGPT.Services
{
    // Reference ('Code First'): 
    // PM> Add-Migration InitialCreate (set to 'Any CPU', not x86)
    // PM> Update-Database 
    public class SqlServerContext : DbContext
    {
        private readonly string _connectionString;

        // This ctor is needed for PM> Update-Database
        public SqlServerContext()
        {
            _connectionString = SqlHistoryRepo.SqlConnectionString;
        }

        // DbSet maps a table in DB
        public DbSet<HistoryChat> HistoryChat { get; set; }
        public DbSet<HistoryMessage> HistoryMessage { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
