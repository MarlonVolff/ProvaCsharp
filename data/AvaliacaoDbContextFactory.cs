using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;


namespace avaliacao.data
{
    public class AvaliacaoDbContextFactory : IDesignTimeDbContextFactory<AvaliacaoDbContext>
    {
        public AvaliacaoDbContext CreateDbContext(String[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AvaliacaoDbContext>();

            //Build
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 26)));

            return new AvaliacaoDbContext(optionsBuilder.Options);

        }
    }
}