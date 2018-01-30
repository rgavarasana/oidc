using Marvin.IDP.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marvin.IDP.Services
{
    public class MarvinUserContextFactory : IDesignTimeDbContextFactory<MarvinUserContext>
    {
        
    
        public MarvinUserContext CreateDbContext(string[] args)
        {
            var connectionString = Startup.Configuration["connectionStrings:marvinUserDBConnectionString"];
            var optionsBuilder = new DbContextOptionsBuilder<MarvinUserContext>();
            optionsBuilder.UseSqlServer(connectionString);
            return new MarvinUserContext(optionsBuilder.Options);
        }
    }
}
