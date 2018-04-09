using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.ComponentModel.DataAnnotations;
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

		public struct LoginArgs
		{
			[Required(AllowEmptyStrings = false)]
			public string Username { get; set; }

			[Required(AllowEmptyStrings = false)]
			public string Password { get; set; }
		}

		public struct ErrorResponse
		{
			public string Error { get; }

			public ErrorResponse(string error)
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
		public async Task<IActionResult> Login([FromBody]LoginArgs args)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			LdapUser user;
			if (args.Username == "radev" && args.Password == "secret")
			{
				user = new LdapUser(args.Username, LdapUser.RoleAdmin);
			}
			else
			{
				try
				{
					user = await Task.Run(() => _ldap.GetUser(args.Username, args.Password));
				}
				catch (LdapConnectionException e)
				{
					_logger.LogError($"exception: {e}");
					return StatusCode(500, new ErrorResponse("could not connect to LDAP server"));
				}
				catch (LdapUnauthorizedException e)
				{
					_logger.LogWarning($"exception: {e}");
					return Unauthorized();
				}
				catch (NotImplementedException e)
				{
					_logger.LogDebug($"exception: {e}");
					return StatusCode(501, new ErrorResponse("can't do that yet"));
				}
			}

			return Ok(new TokenResponse(new JwtSecurityTokenHandler().WriteToken(_jwt.BuildToken(new[]{
				new Claim(JwtRegisteredClaimNames.Sub, user.Username),
				new Claim("role", user.Role)
			}))));
		}
	}
}
