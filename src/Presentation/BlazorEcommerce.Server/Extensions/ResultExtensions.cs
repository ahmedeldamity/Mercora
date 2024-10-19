namespace BlazorEcommerce.Server.Extensions;
public static class ResultExtensions
{
    public static ActionResult ToProblem(this Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Cannot convert success result to a problem");

		if (result.IsSuccess)
			throw new InvalidOperationException("Cannot convert success result to a problem");

		var error = new ErrorResponse
		{
			StatusCode = result.Error?.StatusCode ?? 500,
			Message = result.Error?.Title ?? "An error occurred"
		};

		return new ObjectResult(error)
		{
			StatusCode = error.StatusCode
		};
	}
}