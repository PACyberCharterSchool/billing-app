using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace api.Services
{
	public class JwtConfig
	{
		public string Issuer { get; }
		public string Key { get; }

		public JwtConfig(string issuer, string key)
		{
			Issuer = issuer;
			Key = key;
		}
	}

	public interface IJwtService
	{
		JwtSecurityToken BuildToken(Claim[] claims = null);
		JwtSecurityToken BuildToken(DateTime time, Claim[] claims = null);
	}

	public class JwtService : IJwtService
	{
		private readonly string _issuer;
		private readonly string _key;

		public JwtService(JwtConfig cfg)
		{
			_issuer = cfg.Issuer;
			_key = cfg.Key;
		}

		public JwtSecurityToken BuildToken(Claim[] claims = null) => BuildToken(DateTime.Now, claims);

		public JwtSecurityToken BuildToken(DateTime time, Claim[] claims = null)
		{
			var key = new SymmetricSecurityKey(Encoding.Unicode.GetBytes(_key));
			return new JwtSecurityToken(
				issuer: _issuer,
				audience: _issuer,
				claims: claims,
				expires: time.AddHours(1), // TODO(Erik): configurable?
				signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
			);
		}
	}
}
