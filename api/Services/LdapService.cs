using System;

namespace api.Services
{
	public struct LdapUser
	{
		public string Username { get; }

		public LdapUser(string username)
		{
			Username = username;
		}
	}

	public interface ILdapService
	{
		LdapUser GetUser(string username, string password);
	}

	public class LdapConnectionException : Exception { }

	public class LdapUnauthorizedException : Exception { }

	public class LdapService : ILdapService
	{
		public LdapUser GetUser(string username, string password) => throw new NotImplementedException();
	}
}
