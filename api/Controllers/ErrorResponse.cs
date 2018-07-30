namespace api.Controllers
{
	public struct ErrorResponse
	{
		public string Error { get; }

		public ErrorResponse(string error)
		{
			Error = error;
		}
	}
}
