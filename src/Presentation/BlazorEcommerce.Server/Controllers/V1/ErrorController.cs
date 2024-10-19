namespace BlazorEcommerce.Server.Controllers.V1;

[Route("error/{code:int}")]
[ApiController]
// -- Here Swagger Doesn't Work Because End Point (Error) Not Have Any Method (Get|Post..) And We Can't Put Method 
// -- Because We Don't Call This End Point But Application Will Do It
// -- So To Solving This Problem We Write
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : ControllerBase
{
    public ActionResult Error(int code)
    {
        var result = new Result(false, new Error(code));

        return result.ToProblem();
    }
}