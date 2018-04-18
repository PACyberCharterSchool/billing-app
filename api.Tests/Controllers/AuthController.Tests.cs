using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Moq;
using NUnit.Framework;

using static api.Common.UserRoles;
using api.Controllers;
using api.Services;
using api.Tests.Util;

namespace api.Tests.Controllers
{
	[TestFixture]
	public class AuthControllerTests
	{
		private Mock<ILdapService> _ldap;
		private Mock<IJwtService> _jwt;
		private ILogger<AuthController> _logger;

		private AuthController _uut;

		[SetUp]
		public void SetUp()
		{
			_ldap = new Mock<ILdapService>();
			_jwt = new Mock<IJwtService>();
			_logger = new TestLogger<AuthController>();

			_uut = new AuthController(_ldap.Object, _jwt.Object, _logger);
		}

		[Test]
		public async Task LoginBadRequestWhenModelStateInvalid()
		{
			var key = "error key";
			var msg = "error message";
			_uut.ModelState.AddModelError(key, msg);

			var result = await _uut.Login(new AuthController.LoginArgs());
			Assert.That(result, Is.TypeOf<BadRequestObjectResult>());

			var value = (Dictionary<string, object>)((BadRequestObjectResult)result).Value;
			Assert.That(((string[])value.GetValueOrDefault(key))[0], Is.EqualTo(msg));
		}

		[Test]
		public async Task LoginInternalServerErrorWhenLdapServiceThrowsConnectionException()
		{
			var username = "username";
			var password = "password";

			_ldap.Setup(l => l.GetUser(username, password)).Throws(new LdapConnectionException(new Exception("borked")));

			var result = await _uut.Login(new AuthController.LoginArgs
			{
				Username = username,
				Password = password,
			});
			Assert.That(result, Is.TypeOf<ObjectResult>());
			Assert.That(((ObjectResult)result).StatusCode, Is.EqualTo(500));

			var value = ((ObjectResult)result).Value;
			Assert.That(value, Is.TypeOf<ErrorResponse>());
			Assert.That(((ErrorResponse)value).Error, Is.EqualTo("could not connect to LDAP server"));
		}

		[Test]
		public async Task LoginUnauthorizedWhenLdapServiceThrowsUnauthorizedException()
		{
			var username = "username";
			var password = "password";
			_ldap.Setup(l => l.GetUser(username, password)).Throws(new LdapUnauthorizedException("borked"));

			var result = await _uut.Login(new AuthController.LoginArgs
			{
				Username = username,
				Password = password,
			});
			Assert.That(result, Is.TypeOf<UnauthorizedResult>());
		}

		[Test]
		public async Task LoginReturnsTokenWithUserData()
		{
			var username = "username";
			var password = "password";
			var user = new LdapUser(username, ADMIN);
			_ldap.Setup(l => l.GetUser(username, password)).Returns(user);

			var token = new JwtSecurityToken();
			// verify necessary claims are passed
			_jwt.Setup(j => j.BuildToken(It.Is<Claim[]>(cs =>
				cs[0].Type == "sub" && cs[0].Value == user.Username &&
				cs[1].Type == "role" && cs[1].Value == user.Role
			))).Returns(token);

			var result = await _uut.Login(new AuthController.LoginArgs
			{
				Username = username,
				Password = password,
			});

			Assert.That(result, Is.TypeOf<OkObjectResult>());
			var value = ((OkObjectResult)result).Value;
			Assert.That(value, Is.TypeOf<AuthController.TokenResponse>());

			var actual = ((AuthController.TokenResponse)value).Token;
			var expected = new JwtSecurityTokenHandler().WriteToken(token);
			Assert.That(actual, Is.EqualTo(expected));
		}
	}
}
