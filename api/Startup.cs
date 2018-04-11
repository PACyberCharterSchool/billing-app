using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

using dotenv.net;

using static api.Common.UserRoles;
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

			DotEnv.Config(false);
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
			services.AddSingleton<LdapConfig>(new LdapConfig
			{
				Url = Environment.GetEnvironmentVariable("LDAP_URL"),
				AuthDistinguishedName = Environment.GetEnvironmentVariable("LDAP_AUTHDN"),
				AuthPassword = Environment.GetEnvironmentVariable("LDAP_AUTHPWD"),
				SearchBase = Environment.GetEnvironmentVariable("LDAP_SEARCHBASE"),
				AdminDistinguishedName = Environment.GetEnvironmentVariable("LDAP_ADMINDN"),
				PayDistinguishedName = Environment.GetEnvironmentVariable("LDAP_PAYDN"),
				StandardDistinguishedName = Environment.GetEnvironmentVariable("LDAP_STDDN"),
			});
			services.AddTransient<ILdapService, LdapService>();
			#endregion

			#region JWT
			var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
			var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")));
			services.AddSingleton<JwtConfig>(new JwtConfig(jwtIssuer, jwtKey));
			services.AddTransient<IJwtService, JwtService>();
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
				AddJwtBearer(o =>
				{
					o.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidIssuer = jwtIssuer,

						ValidateAudience = true,
						ValidAudience = jwtIssuer,

						ValidateIssuerSigningKey = true,
						IssuerSigningKey = jwtKey,

						RequireExpirationTime = true,
						ValidateLifetime = true,
					};
				});
			services.AddAuthorization(o =>
			{
				o.AddPolicy("STD+", p => p.RequireRole(new[] {
					STANDARD_USER,
					PAYMENT_MANAGER,
					ADMIN,
				}));
				o.AddPolicy("PAY+", p => p.RequireRole(new[] {
					PAYMENT_MANAGER,
					ADMIN,
				}));
				o.AddPolicy("ADM=", p => p.RequireRole(new[] { ADMIN }));
			});
			#endregion

			services.AddMvc();
		}

		public void Configure(IApplicationBuilder app, StudentContext context)
		{
			app.UseAuthentication();
			app.UseMvc();
			context.Database.Migrate();
		}
	}
}
