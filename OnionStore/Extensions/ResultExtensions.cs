using Core.ErrorHandling;
using Microsoft.AspNetCore.Mvc;

namespace API.Extensions;
public static class ResultExtensions
{
    public static ObjectResult ToProblem(this Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("cannot convert success result to a problem");

        var problem = Results.Problem(statusCode: result.Error.StatusCode);

        var problemDetails = problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(problem) as ProblemDetails;

        problemDetails!.Type = null;

        problemDetails!.Extensions = new Dictionary<string, object?>
        {
            {
                "errors", new List<string> { result.Error.Description }
            }
        };

        return new ObjectResult(problemDetails);
    }
}