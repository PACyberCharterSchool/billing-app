using System;

namespace api.Controllers
{
	public struct ErrorResponse
	{
		public string Error { get; }

		public ErrorResponse(string error) => Error = error;
		public ErrorResponse(Exception e) => Error = e.Message;
	}
}
