using Microsoft.Extensions.Logging;
using System;
using System.Linq;

using Novell.Directory.Ldap;

namespace api.Services
{
	public struct LdapUser
	{
		public static readonly string RoleAdmin = "ADM";
		public static readonly string RolePayManager = "PAY";
		public static readonly string RoleStandardUser = "STD";

		public string Username { get; }
		public string Role { get; }

		public LdapUser(string username, string role)
		{
			Username = username;
			Role = role;
		}

		public override string ToString()
		{
			return $"[LdapUser: Username: {Username}; Role: {Role}]";
		}
	}

	public interface ILdapService
	{
		LdapUser GetUser(string username, string password);
	}

	public class LdapConnectionException : Exception
	{
		private static string _message = "could not connect to Active Directory";

		public LdapConnectionException(Exception e) : base(_message, e) { }
	}

	public class LdapUnauthorizedException : Exception
	{
		private static string _message = "could not authorize user";

		public LdapUnauthorizedException(Exception e) : base(_message, e) { }
		public LdapUnauthorizedException(string e) : base(_message, new Exception(e)) { }
	}

	public class LdapConfig
	{
		public string Url { get; set; }
		public string AuthDistinguishedName { get; set; }
		public string AuthPassword { get; set; }
		public string SearchBase { get; set; }
		public string AdminDistinguishedName { get; set; }
		public string PayDistinguishedName { get; set; }
		public string StandardDistinguishedName { get; set; }

		public override string ToString()
		{
			return $"[\n\tUrl: {Url}" +
			$"\n\tAuthDN: {AuthDistinguishedName}\n\tAuthPwd: {AuthPassword}" +
			$"\n\tSearchBase: {SearchBase}" +
			$"\n\tAdmin: {AdminDistinguishedName}\n\tPay: {PayDistinguishedName}\n\tStandard: {StandardDistinguishedName}" +
			$"\n]";
		}
	}

	public class LdapService : ILdapService
	{
		private readonly LdapConfig _cfg;
		private readonly ILogger<LdapService> _logger;

		public LdapService(LdapConfig cfg, ILogger<LdapService> logger)
		{
			_cfg = cfg;
			_logger = logger;
		}

		public LdapUser GetUser(string username, string password)
		{
			_logger.LogDebug($"config: {_cfg}");

			using (var conn = new LdapConnection())
			{
				try
				{
					conn.Connect(_cfg.Url, 389);
					conn.Bind(_cfg.AuthDistinguishedName, _cfg.AuthPassword);
				}
				catch (LdapException e)
				{
					throw new LdapConnectionException(e);
				}

				try
				{
					var result = conn.Search(
						@base: _cfg.SearchBase,
						scope: LdapConnection.SCOPE_SUB,
						filter: $"cn={username}",
						attrs: new[] { "memberOf" },
						typesOnly: false
					);

					var entry = result.next();
					_logger.LogDebug($"entry: {entry}");
					if (entry == null)
						throw new LdapUnauthorizedException("did not find user");

					conn.Bind(entry.DN, password);
					if (!conn.Bound)
						throw new LdapUnauthorizedException("invalid password");

					var role = GetRole(entry.getAttribute("memberOf").StringValueArray);
					if (string.IsNullOrEmpty(role))
						throw new LdapUnauthorizedException("not a member of configured group");

					return new LdapUser(username, role);
				}
				catch (Exception e) when (e is LdapException || e is LdapReferralException)
				{
					throw new LdapUnauthorizedException(e);
				}
			}

			string GetRole(string[] roles)
			{
				if (roles.Contains(_cfg.AdminDistinguishedName))
					return LdapUser.RoleAdmin;
				else if (roles.Contains(_cfg.PayDistinguishedName))
					return LdapUser.RolePayManager;
				else if (roles.Contains(_cfg.StandardDistinguishedName))
					return LdapUser.RoleStandardUser;

				return null;
			}
		}
	}
}
