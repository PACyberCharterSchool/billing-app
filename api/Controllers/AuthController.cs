using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using api.Services;

namespace api.Controllers
{
	[Route("api/[controller]")]
	public class AuthController : Controller
	{
		private readonly ILdapService _ldap;
		private readonly IJwtService _jwt;
		private readonly ILogger _logger;

		public AuthController(ILdapService ldap, IJwtService jwt, ILogger<AuthController> logger)
		{
			_ldap = ldap;
			_jwt = jwt;
			_logger = logger;
		}

		public struct ConnectionError
		{
			public string Error { get; }

			public ConnectionError(string error)
			{
				Error = error;
			}
		}

		public struct TokenResponse
		{
			public string Token { get; }

			public TokenResponse(string token)
			{
				Token = token;
			}
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromHeader]string authorization)
		{
			if (string.IsNullOrEmpty(authorization))
				return BadRequest();

			if (!authorization.StartsWith("Basic "))
				return BadRequest();

			authorization = authorization.Substring("Basic ".Length);
			authorization = Encoding.Unicode.GetString(Convert.FromBase64String(authorization));
			var parts = authorization.Split(":");
			if (parts.Length != 2)
				return BadRequest();

			var username = parts[0];
			var password = parts[1];
			LdapUser user;
			try
			{
				user = _ldap.GetUser(username, password);
			}
			catch (LdapConnectionException)
			{
				return StatusCode(500, new ConnectionError("could not connect to LDAP server"));
			}
			catch (LdapUnauthorizedException)
			{
				return Unauthorized();
			}

			var token = _jwt.BuildToken(new[]{
				new Claim(JwtRegisteredClaimNames.Sub, user.Username)
			});
			return Ok(new TokenResponse(new JwtSecurityTokenHandler().WriteToken(token)));
		}
	}
}
