namespace API.Errors
{
	public class ApiResponse
	{
		public int Status { get; set; }
		public string? Title { get; set; }

		public ApiResponse(int statusCode, string? message = null)
		{
            Status = statusCode;
            Title = message ?? GetDefaultMessageForStatusCode(statusCode);
		}

		private string? GetDefaultMessageForStatusCode(int statusCode)
		{
			return statusCode switch
			{
				400 => "A bad request, you have made!",
				401 => "Authorized, you are not!",
				404 => "Resourse was not found!",
				500 => "Server Error",
				_ => null
			};
		}
	}
}