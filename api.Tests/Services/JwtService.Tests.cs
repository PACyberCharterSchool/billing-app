using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

using NUnit.Framework;

using api.Services;

namespace api.Tests.Services
{
	[TestFixture]
	public class JwtServiceTests
	{
		private TimeSpan GetSecondsFromEpoch(DateTime time)
		{
			var t = time.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));

			// avoid rounding issues
			return t.Subtract(new TimeSpan(0, 0, 0, 0, t.Milliseconds));
		}

		[Test]
		public void BuildTokenWithClaims()
		{
			var issuer = "issuer";
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("thisisasupersecretkey"));
			var uut = new JwtService(new JwtConfig(issuer, key));

			var username = "username";
			var custom = "custom";
			var claims = new[] {
				new Claim(JwtRegisteredClaimNames.Sub, username),
				new Claim("cus", custom),
			};
			var now = DateTime.Now;
			var result = uut.BuildToken(now, claims);

			// ensure token is writable; if key is too short will fail
			new JwtSecurityTokenHandler().WriteToken(result);

			// assert always-present claims
			var iss = result.Claims.First(c => c.Type == "iss").Value;
			Assert.That(iss, Is.EqualTo(issuer));

			var aud = result.Claims.First(c => c.Type == "aud").Value;
			Assert.That(aud, Is.EqualTo(issuer));

			var time = GetSecondsFromEpoch(now);
			var nbf = result.Claims.First(c => c.Type == "nbf").Value;
			Assert.That(nbf, Is.EqualTo(time.TotalSeconds.ToString("0")));

			time = GetSecondsFromEpoch(now.AddHours(1));
			var exp = result.Claims.First(c => c.Type == "exp").Value;
			Assert.That(exp, Is.EqualTo(time.TotalSeconds.ToString("0")));

			// assert additional claims
			var sub = result.Claims.First(c => c.Type == "sub").Value;
			Assert.That(sub, Is.EqualTo(username));

			var cus = result.Claims.First(c => c.Type == "cus").Value;
			Assert.That(cus, Is.EqualTo(custom));
		}
	}
}
