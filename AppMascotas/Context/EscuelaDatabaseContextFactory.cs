using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace AppMascotas.Context
{
    public class EscuelaDatabaseContextFactory : IDesignTimeDbContextFactory<EscuelaDatabaseContext>
    {
        public EscuelaDatabaseContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<EscuelaDatabaseContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            return new EscuelaDatabaseContext(optionsBuilder.Options);
        }
    }
}
