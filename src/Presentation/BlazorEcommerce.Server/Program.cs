var builder = WebApplication.CreateBuilder(args);

#region Add services to the container

builder.AddDependencies();

#endregion

#region Validation Error - Bad Request
// -- Validation Error (Bad Request) 
// --- First: We need to bring options which have InvalidModelState
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
	// --- then we need all data (actionContext) of action has validation error
	options.InvalidModelStateResponseFactory = (actionContext) =>
	{
		// --- then we bring ModelState: Dictionary key/value pair for each parameter, and value has property Errors Array have all errors
		// --- and we use where to bring dictionary key/value pair which is value has errors 
		var errors = actionContext.ModelState.Where(p => p.Value!.Errors.Count > 0)
		// --- then we use SelectMany to make one array of all error  
		.SelectMany(p => p.Value!.Errors)
		// --- then we use Select to bring from errors just ErrorMessages
		.Select(e => e.ErrorMessage)
		.ToArray();

		// --- then we insert this errors to the class we made
		var validationErrorResponse = new ApiValidationErrorResponse()
		{
			Errors = errors
		};
		// then return it :)
		return new BadRequestObjectResult(validationErrorResponse);
	};
});
#endregion

var app = builder.Build();

#region Update Database With Using Way And Seeding Data

// We Said To Update Database You Should Do Two Things (1. Create Instance From DbContext 2. Migrate It)

// To Ask Clr To Create Instance Explicitly From Any Class
//    1 ->  Create Scope (Life Time Per Request)
using var scope = app.Services.CreateScope();
//    2 ->  Bring Service Provider Of This Scope
var services = scope.ServiceProvider;

// --> Bring Object Of StoreContext For Update His Migration
var storeContext = services.GetRequiredService<StoreContext>();
// --> Bring Object Of ILoggerFactory For Good Show Error In Console
var loggerFactory = services.GetRequiredService<ILoggerFactory>();

try
{
    // Migrate StoreContext
    await storeContext.Database.MigrateAsync();
    // Seeding Data For StoreContext
    await StoreContextSeed.SeedProductDataAsync(storeContext);
}
catch (Exception ex)
{
	var logger = loggerFactory.CreateLogger<Program>();
	logger.LogError(ex, "an error has been occured during apply the migration!");
}

#endregion

#region Configure the Kestrel pipeline

app.UseMiddleware<ExceptionMiddleware>();

app.UseSwaggerMiddleware();

if (app.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
}

app.UseRateLimiter();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();

app.UseStaticFiles();

app.UseHealthChecks("/_health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseCors("MyPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.UseStatusCodePagesWithReExecute("/error/{0}");

app.MapControllers();

app.MapRazorPages();

app.MapFallbackToFile("index.html");
#region Explaination
//	-- In MVC We Used This Way For Routing
//	app.UseRouting(); -> we use this middleware to match request to an endpoint
//	app.UseEndpoints  -> we use this middleware to excute the matched endpoint
//	(endpoints =>  
//	{	
//		endpoints.MapControllerRoute(
//        name: "default",
//        pattern: "{controller}/{action}"
//        );
//	});
//	-- But We Use MapController Instead Of It Because We Create Routing On Controller Itself
#endregion

app.UseHangfireDashboardAndRecurringJob(builder.Services);

#endregion

app.Run();