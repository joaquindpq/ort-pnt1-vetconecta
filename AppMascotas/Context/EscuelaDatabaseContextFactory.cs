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
            // Construir la configuración desde appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Crear las opciones del DbContext
            var optionsBuilder = new DbContextOptionsBuilder<EscuelaDatabaseContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            return new EscuelaDatabaseContext(optionsBuilder.Options);
        }
    }
}
