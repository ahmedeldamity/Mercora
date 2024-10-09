using BlazorEcommerce.Persistence.Store;
using BlazorEcommerce.Server;
using BlazorEcommerce.Server.Errors;
using BlazorEcommerce.Server.Middlewares;
using BlazorEcommerce.Server.ServicesExtension;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

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

// Server Error Middleware (we catch it in class ExceptionMiddleware)
app.UseMiddleware<ExceptionMiddleware>();

// Add Swagger Middlewares In Extension Method
app.UseSwaggerMiddleware();

// Add Rate Limiter Middleware
app.UseRateLimiter();

// Add Serilog Middleware
app.UseSerilogRequestLogging();

// To this application can resolve on any static file like (html, wwwroot, etc..)
app.UseStaticFiles();

// Add Health Check Middleware
app.UseHealthChecks("/_health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Add Cors Policy Middleware
app.UseCors("CorsPolicy");

// To Redirect Any Http Request To Https
app.UseHttpsRedirection();

// Error Not Found End Point: Here When This Error Thrown: It Redirects To This End Point in (Controller: Errors)
app.UseStatusCodePagesWithReExecute("/error/{0}");

// we use this middleware to talk program that: your routing depend on route written on the controller
app.MapControllers();
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