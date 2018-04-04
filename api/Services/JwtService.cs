using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

		public JwtSecurityToken BuildToken(Claim[] claims = null) => throw new NotImplementedException();
	}
}
