using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;

using api.Models;

namespace api
{
    public class Startup
    {   
        public Startup(IHostingEnvironment environment, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _env = environment;
            _conf = configuration;
            _loggerFactory = loggerFactory;
        }

        private IHostingEnvironment _env { get; set; }
        private IConfiguration _conf { get; set; }
        public ILoggerFactory _loggerFactory { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var hostName = _conf["environmentVariables:Database:SQLSERVER_HOST"] ?? "localhost";
            var userName = _conf["environmentVariables:Database:USERNAME"] ?? "sa";
            var password = _conf["environmentVariables:Database:PASSWORD"] ?? "Br0ken horse carrot";
            var port = _conf["environmentVariables:Database:PORT"] ?? "1401";
            var connectionString = $"Data Source={hostName},{port};User ID={userName};Password={password}";

            var logger = _loggerFactory.CreateLogger<ConsoleLogger>();
            logger.LogDebug($"Startup.ConfigureServices(): connectionString is {connectionString}."); 

            services.AddDbContext<StudentContext>(opt => opt.UseSqlServer(connectionString));
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            // this establishes the foundation for a single, comprehensive logging mechanism within the entire app
            _loggerFactory.AddConsole();
            _loggerFactory.AddDebug();

            app.UseMvc();
        }
    }
}