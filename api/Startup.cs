using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using api.Models;

using dotenv;

using System;

namespace api
{
    public class Startup
    {   
        public Startup(IHostingEnvironment environment, IConfiguration configuration, ILogger<Startup> logger)
        {
            _env = environment;
            _conf = configuration;
            _logger = logger;

            var builder = new ConfigurationBuilder();

            if (_env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }
            _conf = builder.Build();
        }

        private IHostingEnvironment _env { get; set; }
        private IConfiguration _conf { get; set; }
        private ILogger<Startup> _logger;

        public void ConfigureServices(IServiceCollection services)
        {
            var hostName = Environment.GetEnvironmentVariable("DATABASE_HOST");
            var databaseName = Environment.GetEnvironmentVariable("DATABASE_NAME");
            var userName = Environment.GetEnvironmentVariable("DATABASE_USERNAME");
            var password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");
            var port = Environment.GetEnvironmentVariable("DATABASE_PORT");

            var connectionString = $"Server={hostName},{port};Database={databaseName};User ID={userName};Password={password};Trusted_Connection=True";

            _logger.LogDebug("Startup.ConfigureServices(): connectionString is {connectionString}.");
            _logger.LogInformation("Startup.ConfigureServices():  WTF?");

            services.AddDbContext<StudentContext>(opt => opt.UseSqlServer(connectionString));
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            dotenv.net.DotEnv.Config();

            app.UseMvc();
        }
    }
}