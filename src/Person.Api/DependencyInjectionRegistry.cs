using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Person.BusinessLogic.Domains;
using Person.BusinessLogic.Domains.Abstractions;
using Person.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Person.Api
{
    public static class DependencyInjectionRegistry
    {
        public static IServiceCollection AddMyServices(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Person.Api", Version = "v1" });
            });


            services.AddSingleton<IPersonRepository, PersonRepository>();
            services.AddSingleton<IPersonDomain, PersonDomain>();
            services.AddSingleton<DesignTimeDbContextFactory>();


            return services;
        }
    }
}
