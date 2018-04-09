using Microsoft.Extensions.Logging;
using System;

using dotenv.net;
using NUnit.Framework;

using api.Services;
using api.Tests.Util;

namespace api.Tests.Services
{
	[TestFixture]
	public class LdapServiceTests
	{
		[Test]
		[Category("Integration")]
		public void GetUserShouldWork()
		{
			Assert.Ignore("comment out for development");

			DotEnv.Config(throwOnError: true, filePath: "../../../.env");
			var ldap = new LdapService(new LdapConfig
			{
				Url = Environment.GetEnvironmentVariable("LDAP_URL"),
				AuthDistinguishedName = Environment.GetEnvironmentVariable("LDAP_AUTHDN"),
				AuthPassword = Environment.GetEnvironmentVariable("LDAP_AUTHPWD"),
				SearchBase = Environment.GetEnvironmentVariable("LDAP_SEARCHBASE"),
				AdminDistinguishedName = Environment.GetEnvironmentVariable("LDAP_ADMINDN"),
				PayDistinguishedName = Environment.GetEnvironmentVariable("LDAP_PAYDN"),
				StandardDistinguishedName = Environment.GetEnvironmentVariable("LDAP_STDDN"),
			}, new TestLogger<LdapService>());

			var user = ldap.GetUser(
				Environment.GetEnvironmentVariable("LDAPTEST_USERNAME"),
				Environment.GetEnvironmentVariable("LDAPTEST_PASSWORD")
			);
			Console.WriteLine($"user: {user}");
			Assert.Fail("show me output");
		}
	}
}
