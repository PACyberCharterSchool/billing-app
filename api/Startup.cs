using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;

using dotenv.net;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.ReDoc;

using static api.Common.UserRoles;
using api.Controllers;
using api.Services;
using models;
using models.Reporters;
using models.Reporters.Exporters;

using Aspose.Cells;

// TODO(Erik): send back stacktrace on 500?
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
			#region Models
			var hostName = Environment.GetEnvironmentVariable("DATABASE_HOST");
			var databaseName = Environment.GetEnvironmentVariable("DATABASE_NAME");
			var userName = Environment.GetEnvironmentVariable("DATABASE_USERNAME");
			var password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");
			var port = Environment.GetEnvironmentVariable("DATABASE_PORT");

			var connectionString = $"Server={hostName},{port};Database={databaseName};User Id={userName};Password={password}";
			_logger.LogInformation($"Startup.ConfigureServices(): connectionString is {connectionString}.");

			services.AddDbContextPool<PacBillContext>(opt =>
			{
				opt.UseSqlServer(connectionString);
				opt.UseLazyLoadingProxies();
				opt.ConfigureWarnings(b => b.Ignore(CoreEventId.DetachedLazyLoadingWarning));
			});
			services.AddTransient<IAuditRecordRepository, AuditRecordRepository>();
			services.AddTransient<ICalendarRepository, CalendarRepository>();
			services.AddTransient<IPaymentRepository, PaymentRepository>();
			services.AddTransient<IRefundRepository, RefundRepository>();
			services.AddTransient<IReportRepository, ReportRepository>();
			services.AddTransient<ITemplateRepository, TemplateRepository>();
			services.AddTransient<ISchoolDistrictRepository, SchoolDistrictRepository>();
			services.AddTransient<IDigitalSignatureRepository, DigitalSignatureRepository>();
			services.AddTransient<IStudentRecordRepository, StudentRecordRepository>();

			services.AddTransient<IFilterParser, FilterParser>();

			services.AddSingleton<IReporterFactory, ReporterFactory>();
			services.AddSingleton<IXlsxExporter, XlsxExporter>();
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

					var handler = o.SecurityTokenValidators.OfType<JwtSecurityTokenHandler>().SingleOrDefault();
					if (handler != null)
						handler.InboundClaimTypeMap = handler.InboundClaimTypeMap.
							Where(m => m.Key != "sub").
							ToDictionary(m => m.Key, m => m.Value);
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

			#region Swagger
			services.AddSwaggerGen(o =>
			{
				o.SwaggerDoc("v1", new Info { Title = "PACBill API", Version = "v1" });
				o.AddSecurityDefinition("Bearer", new ApiKeyScheme
				{
					Type = "apiKey",
					Name = "Authorization",
					Description = "JSON Web Token: http://jwt.io.",
					In = "header",
				});
				o.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>{
						{"Bearer", null},
				});
				o.SchemaFilter<EnumerationSchemaFilter>();
				o.OperationFilter<EnumerationOperationFilter>();
				o.OperationFilter<AuthorizeOperationFilter>();
			});
			#endregion

			#region Aspose.Cells
			Aspose.Cells.License license = new Aspose.Cells.License();
			license.SetLicense("Aspose.Cells.lic");
			#endregion

			services.AddMvc().
				AddJsonOptions(o => o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
		}

		public void Configure(IApplicationBuilder app, PacBillContext context)
		{
			app.UseAuthentication();

			// TODO(Erik): figure something out for production
			#region Development
			app.UseCors(builder => builder.
				AllowAnyOrigin().
				AllowAnyMethod().
				AllowAnyHeader().
				AllowCredentials());

			app.UseSwagger();
			app.UseReDoc(o =>
			{
				o.SpecUrl = "v1/swagger.json";
				o.RoutePrefix = "swagger";
			});
			// app.UseSwaggerUI(o => o.SwaggerEndpoint("/swagger/v1/swagger.json", "PACBill API"));
			#endregion

			app.UseMvc();

			context.Database.Migrate();
		}
	}
}
