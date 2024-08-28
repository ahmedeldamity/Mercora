using API.Errors;
using System.Net;
using System.Text.Json;

#region Explanation
// -- When We Need To Create Middleware With Inconvention Way We Must:
// ---- 1. Class Name End With (Middleware)
// ---- 2. Ask Clr To Bring (RequestDelegate next) To Move The Next Middleware
// ---- 3. Ask Clr To Bring (ILogger<ExceptionMiddleware> logger) To Log Exception In Console
// ---- 4. Ask Clr To Bring (IHostEnvironment environment) To Check We In Development | Production Environment
// ---- 5. Create Function With Name InvokeAsync And Take (HttpContext httpContext) To Use It When Sending Response
// -- In This Case We Create This Class For Server Error
// ---- So We Need To Send In Respone Body:
// ------ If We In Development Environment Three Parameters (Status Code, Exception Message, Exception Details) To FrontEnd
// ------ If We In Production Environment One Parameter (Status Code) To Client And We Send (Exception Message, Exception Details) To Database | File -> To Backend Developer
#endregion
namespace API.Middlewares;
public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
{
    private readonly RequestDelegate next = next;
    private readonly ILogger<ExceptionMiddleware> logger = logger;
    private readonly IHostEnvironment env = env;

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next.Invoke(httpContext);
        }
        catch (Exception ex)
        {
            // -- If We In Production Environment: We Log This Error In Database Or Files

            // -- If We In Development Environment: We Log This Error In Console Like This
            logger.LogError(ex, ex.Message);

            // -- After Logged Exception: The Frontend Developer Waiting From Us Response
            // -- And Response Consists Of Header And Body
            // .... This is What We Need To Send In Header
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // .... This is What We Need To Send In Body
            // .... If We In Development Environment: We Send (Status Code, Exception Message, Exception Details) To FrontEnd
            // .... If We In Production Environment: We Send (Status Code) To Client And We Send (Exception Message, Exception Details) To Database | File
            var response = env.IsDevelopment() ?
                new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace!.ToString()) :
                new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);

            // Here We Create Options To Make Json Be Camel Case And We Send This Options To JsonSerializer 
            var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            var json = JsonSerializer.Serialize(response, options);

            // .... Here We Send Body To Response
            await httpContext.Response.WriteAsync(json);
        }
    }
}