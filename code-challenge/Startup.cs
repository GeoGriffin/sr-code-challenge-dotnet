using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using challenge.Data;
using Microsoft.EntityFrameworkCore;
using challenge.Repositories;
using challenge.Services;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace code_challenge
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<EmployeeContext>(options =>
            {
                options.UseInMemoryDatabase("EmployeeDB");
            });
            services.AddDbContext<CompensationContext>(options =>
            {
                options.UseInMemoryDatabase("CompensationDB");
            });
            services.AddScoped<IEmployeeRepository,EmployeeRespository>();
            services.AddScoped<ICompensationRepository, CompensationRespository>();
            services.AddTransient<EmployeeDataSeeder>();
            services.AddTransient<CompensationDataSeeder>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<ICompensationService, CompensationService>();
            services.AddMvc();

            // KFD
            // Configure Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Mindex Code Challenge",
                    Version = "v1",
                    Description = "API challenge prepared by [Kurt Devlin](https://www.linkedin.com/in/kurtdevlin/)"
                });

                // Configure Swagger to use the xml documentation file
                var xmlFile = System.IO.Path.ChangeExtension(typeof(Startup).Assembly.Location, ".xml");
                c.IncludeXmlComments(xmlFile);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, EmployeeDataSeeder employeeSeeder, CompensationDataSeeder compensationSeeder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                employeeSeeder.Seed().Wait();
                compensationSeeder.Seed().Wait();
            }

            app.UseMvc();

            // KFD
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Code Challenge API Documentation");
            });
        }
    }
}
