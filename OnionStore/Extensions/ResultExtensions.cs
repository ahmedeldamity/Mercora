using Core.ErrorHandling;
using Microsoft.AspNetCore.Mvc;

namespace API.Extensions;
public static class ResultExtensions
{
    public static ActionResult ToProblem(this Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Cannot convert success result to a problem");

        var problem = Results.Problem(statusCode: result.Error?.StatusCode, title: result.Error?.Title.Split(", ")[0]);

        var problemDetails = problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(problem) as ProblemDetails;

        problemDetails!.Type = null;

        if (result.Error?.StatusCode != 200)
        {
            problemDetails!.Extensions = new Dictionary<string, object?>
            {
                {
                    "errors", result.Error?.Title.Split(", ")
                }
            };
        }

        return new ObjectResult(problemDetails);
    }

    public static ActionResult ToSuccess(this Result result)
    {
        if (!result.IsSuccess)
            throw new InvalidOperationException("Cannot convert success result to a problem");

        var problem = Results.Problem(statusCode: result.Error?.StatusCode, title: result.SuccessMessage);

        var problemDetails = problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(problem) as ProblemDetails;

        problemDetails!.Type = null;

        return new ObjectResult(problemDetails);
    }
}