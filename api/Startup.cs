using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System;

using dotenv.net;

using api.Models;
using api.Services;

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

			DotEnv.Config();
			builder.AddUserSecrets<Startup>();
			_conf = builder.Build();
		}

		private IHostingEnvironment _env { get; set; }
		private IConfiguration _conf { get; set; }
		private ILogger<Startup> _logger;

		public void ConfigureServices(IServiceCollection services)
		{
			#region DB
			var hostName = Environment.GetEnvironmentVariable("DATABASE_HOST");
			var databaseName = Environment.GetEnvironmentVariable("DATABASE_NAME");
			var userName = Environment.GetEnvironmentVariable("DATABASE_USERNAME");
			var password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");
			var port = Environment.GetEnvironmentVariable("DATABASE_PORT");

			var connectionString = $"Server={hostName},{port};Database={databaseName};User Id={userName};Password={password}";
			_logger.LogInformation($"Startup.ConfigureServices(): connectionString is {connectionString}.");

			services.AddDbContext<StudentContext>(opt => opt.UseSqlServer(connectionString));
			#endregion

			#region LDAP
			services.AddTransient<ILdapService, LdapService>();
			#endregion

			#region JWT
			var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
			var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
			services.AddSingleton<JwtConfig>(new JwtConfig(jwtIssuer, jwtKey));
			services.AddTransient<IJwtService, JwtService>();
			#endregion

			services.AddMvc();
		}

		public void Configure(IApplicationBuilder app, StudentContext context)
		{
			app.UseMvc();
			context.Database.Migrate();
		}
	}
}
