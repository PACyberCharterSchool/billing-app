using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace api.Tests.Util
{
	public static class ControllerExtensions
	{
		public static void SetUsername(this Controller controller, string username)
		{
			controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]{
							new Claim(JwtRegisteredClaimNames.Sub, username),
					})),
				},
			};
		}
	}
}
