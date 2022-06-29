using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Person.Data.Entities;
using System.IO;

public class DesignTimeDbContextFactory :
        IDbContextFactory<Context>
{
    public Context CreateDbContext()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        var builder = new DbContextOptionsBuilder<Context>();
        var connectionString = configuration.GetConnectionString("MainDatabase");
        builder.UseSqlServer(connectionString);
        return new Context(builder.Options);    
    }
}