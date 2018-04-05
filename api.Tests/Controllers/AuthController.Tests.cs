using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Moq;
using NUnit.Framework;

using api.Controllers;
using api.Services;

namespace api.Tests.Controllers
{
	[TestFixture]
	public class AuthControllerTests
	{
		private Mock<ILdapService> _ldap;
		private Mock<IJwtService> _jwt;
		private Mock<ILogger<AuthController>> _logger;

		private AuthController _uut;

		[SetUp]
		public void SetUp()
		{
			_ldap = new Mock<ILdapService>();
			_jwt = new Mock<IJwtService>();
			_logger = new Mock<ILogger<AuthController>>();

			_uut = new AuthController(_ldap.Object, _jwt.Object, _logger.Object);
		}

		[Test]
		public async Task BadRequestWhenMissingHeader()
		{
			var result = await _uut.Login(null);
			Assert.That(result, Is.TypeOf(typeof(BadRequestResult)));
		}

		[Test]
		public async Task BadRequestWhenAuthorizationNotBasic()
		{
			var auth = Convert.ToBase64String(Encoding.Unicode.GetBytes("username:password"));
			var result = await _uut.Login(auth);
			Assert.That(result, Is.TypeOf(typeof(BadRequestResult)));
		}

		[Test]
		public async Task BadRequestWhenAuthorizationBadFormat()
		{
			var auth = Convert.ToBase64String(Encoding.Unicode.GetBytes("badauth"));
			var result = await _uut.Login($"Basic {auth}");
			Assert.That(result, Is.TypeOf(typeof(BadRequestResult)));
		}

		[Test]
		public async Task InternalServerErrorWhenLdapServiceThrowsConnectionException()
		{
			var username = "username";
			var password = "password";
			var auth = Convert.ToBase64String(Encoding.Unicode.GetBytes($"{username}:{password}"));

			_ldap.Setup(l => l.GetUser(username, password)).Throws(new LdapConnectionException());

			var result = await _uut.Login($"Basic {auth}");
			Assert.That(result, Is.TypeOf(typeof(ObjectResult)));
			Assert.That(((ObjectResult)result).StatusCode, Is.EqualTo(500));

			var value = ((ObjectResult)result).Value;
			Assert.That(value, Is.TypeOf(typeof(AuthController.ConnectionError)));
			Assert.That(((AuthController.ConnectionError)value).Error, Is.EqualTo("could not connect to LDAP server"));
		}

		[Test]
		public async Task UnauthorizedWhenLdapServiceThrowsUnauthorizedException()
		{
			var username = "username";
			var password = "password";
			var auth = Convert.ToBase64String(Encoding.Unicode.GetBytes($"{username}:{password}"));

			_ldap.Setup(l => l.GetUser(username, password)).Throws(new LdapUnauthorizedException());

			var result = await _uut.Login($"Basic {auth}");
			Assert.That(result, Is.TypeOf(typeof(UnauthorizedResult)));
		}

		[Test]
		public async Task ReturnsTokenWithUserData()
		{
			var username = "username";
			var password = "password";
			var auth = Convert.ToBase64String(Encoding.Unicode.GetBytes($"{username}:{password}"));

			var user = new LdapUser(username);
			_ldap.Setup(l => l.GetUser(username, password)).Returns(user);

			var token = new JwtSecurityToken();
			// verify necessary claims are passed
			_jwt.Setup(j => j.BuildToken(It.Is<Claim[]>(cs => cs[0].Value == user.Username))).Returns(token);

			var result = await _uut.Login($"Basic {auth}");

			Assert.That(result, Is.TypeOf(typeof(OkObjectResult)));
			var value = ((OkObjectResult)result).Value;
			Assert.That(value, Is.TypeOf(typeof(AuthController.TokenResponse)));

			var actual = ((AuthController.TokenResponse)value).Token;
			var expected = new JwtSecurityTokenHandler().WriteToken(token);
			Assert.That(actual, Is.EqualTo(expected));
		}
	}
}
