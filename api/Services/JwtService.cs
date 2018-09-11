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
		public SecurityKey Key { get; }

		public JwtConfig(string issuer, SecurityKey key)
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
		private readonly SecurityKey _key;

		public JwtService(JwtConfig cfg)
		{
			_issuer = cfg.Issuer;
			_key = cfg.Key;
		}

		public JwtSecurityToken BuildToken(Claim[] claims = null) => BuildToken(DateTime.Now, claims);

		public JwtSecurityToken BuildToken(DateTime time, Claim[] claims = null)
		{
			return new JwtSecurityToken(
				issuer: _issuer,
				audience: _issuer,
				claims: claims,
				notBefore: time,
				expires: time.AddHours(8),
				signingCredentials: new SigningCredentials(_key, SecurityAlgorithms.HmacSha256)
			);
		}
	}
}
