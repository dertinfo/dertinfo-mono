using AutoMapper;
using DertInfo.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace DertInfo.Api
{
    public class Startup
    {
        private MapperConfiguration _mapperConfiguration { get; set; }
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            // Application configuration
            Configuration = configuration;
            Environment = env;


            var imagesStorageAccount = $"https://{Configuration["StorageAccount:Images:Name"]}.blob.core.windows.net";
            if (env.IsDevelopment())
            {
                imagesStorageAccount = $"http://127.0.0.1:10000/{Configuration["StorageAccount:Images:Name"]}";
            }

            //AutoMapper Config
            _mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new Core.AutoMapperProfileConfiguration(imagesStorageAccount));
            });

            /*
             * WAS: 
             * This was the code that originally built the configuration as IConfigurationRoot
             * But this didn't include the user secrets. Adding the user secrets could be applied 
             * using the builder but the below solves the problem and allows the application to load via convention.
             * 
             * note - at this point I'm not sure why we needed to use .SetBasePath() we need to be aware of this when we test these changes. 
             */
            //var builder = new ConfigurationBuilder()
            //    .SetBasePath(env.ContentRootPath)
            //    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            //    .AddEnvironmentVariables();
            //Configuration = builder.Build();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            // - database context
            services.AddEntityFrameworkSqlServer()
            .AddDbContext<DertInfoContext>(options =>
            {
                var sqlServerName = Configuration["SqlConnection:ServerName"];
                var sqlServerAdminUsername = Configuration["SqlConnection:ServerAdminName"];
                var sqlServerAdminPassword = Configuration["SqlConnection:ServerAdminPassword"];
                var sqlServerDatabaseName = Configuration["SqlConnection:DatabaseName"];

                string connectionString =
                    "Server=" + sqlServerName + ";" +
                    "Database=" + sqlServerDatabaseName + ";" +
                    "User Id=" + sqlServerAdminUsername + ";" +
                    "Password=" + sqlServerAdminPassword + ";" +
                    "Persist Security Info=False;";

                if (Environment.IsDevelopment())
                { 
                    connectionString += "Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";
                }

                options.UseSqlServer(connectionString);
            });

            // Add IIS Options for IIS Hostng 
            services.Configure<IISOptions>(options => { });

            // Services
            services.AddCors(options =>
            {
                var allowedCorsOriginsString = Configuration["Cors:AllowedOrigins"];
                var allowedCorsOriginsArray = allowedCorsOriginsString.Split(",");

                if (allowedCorsOriginsArray.Length == 0 || string.IsNullOrEmpty(allowedCorsOriginsArray.First()))
                {
                    throw new Exception("AllowedOrigins is not set in the configuration file.");
                }

                options.AddPolicy("AllowSpecificOrigins",
                    policy =>
                    {
                        policy.WithOrigins(allowedCorsOriginsArray)
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });

            });

            // We required the nuget package of Microsoft.AspNetCore.Mvc.NewtonsoftJson in order to support the ReferenceLoopHandling.Ignore
            // this is because System.Text.Json doesn't support this functionality at this time and it's needed in the emails.
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                });

            Start.Authorisations.Apply(services, Configuration);
            Start.DependencyInjections.Apply(services, Configuration);
            Start.ConfigureSwagger.Apply(services, Configuration);

            // AutoMapper
            services.AddSingleton<IMapper>(sp => _mapperConfiguration.CreateMapper());
            services.AddApplicationInsightsTelemetry();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, DertInfoContext context)
        {
            if (this.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // todo - we should do this. This is what happens in the default template and makes the http requests https
            //      - 2021/03/02 - we need to test this and then implement
            // app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            //if (env.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DertInfo API v1");
            });
            //}


            // Runs matching. An endpoint is selected and set on the HttpContext if a match is found.
            app.UseRouting();
            app.UseCors("AllowSpecificOrigins");

            // Middleware that run after routing occurs. Usually the following appear here:
            app.UseAuthentication();
            app.UseAuthorization();

            // These middleware can take different actions based on the endpoint.

            // Executes the endpoint that was selected by routing.
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                // Forces all controllers to have the default policy. Wehere the default policy is authenticated.
                // Where annonymous permitted these must be decorated with the [AllowAnonymous] attribute. 
                // todo - endpoints.MapDefaultControllerRoute().RequireAuthorization();
                // todo - turn this on and refactor the decorations
                // note - 20210302 this will require a refactor of the whole API and is the reason that it has not been completed
            });

            // On startup always migrate to the latest version.
            // note - we did this so that it updates on launch. Whenever we add a migration it will run it so that we no longer need to use "Update-Database"
            //      - this also means that we no longer need the TemporaryDBConext Factory in repository as it'll apply on launch.
            context.Database.Migrate();
            context.EnsureSeedData();

            // WAS
            // if (!this.Environment.IsDevelopment())
            // {
            //   context.Database.Migrate();
            //   context.EnsureSeedData();
            // }
            // else
            // {
            //    context.EnsureSeedData();
            // }

            // These are special methods that are used to migrate the database when making changes that need additional information.
            // context.ApplyUuidValues();
        }
    }
}
