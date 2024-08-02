using API.Errors;
using API.Middlewares;
using API.ServicesExtension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.Identity;

var builder = WebApplication.CreateBuilder(args);

#region Add services to the container

// Register API Controller
builder.Services.AddControllers();

// Register Required Services For Swagger In Extension Method
builder.Services.AddSwaggerServices();

// Add Identity Context and Configurations
builder.Services.AddIdentityConfigurations(builder.Configuration);

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
		var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count() > 0)
		// --- then we use SelectMany to make one array of all error  
		.SelectMany(P => P.Value.Errors)
		// --- then we use Select to bring from errors just ErrorMessages
		.Select(E => E.ErrorMessage)
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

// --> Bring Object Of IdentityContext For Update His Migration
var _identiyContext = services.GetRequiredService<IdentityContext>();
// --> Bring Object Of ILoggerFactory For Good Show Error In Console    
var loggerFactory = services.GetRequiredService<ILoggerFactory>();

try
{
	// Migrate IdentityContext
	await _identiyContext.Database.MigrateAsync();
}
catch (Exception ex)
{
	var logger = loggerFactory.CreateLogger<Program>();
	logger.LogError(ex, "an error has been occured during apply the migration!");
}

#endregion

#region Configure the Kestrel pipeline

// -- Server Error Middleware (we catch it in class ExceptionMiddleware)
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
	// -- Add Swagger Middelwares In Extension Method
	app.UseSwaggerMiddleware();
}

// -- To Redirect Any Http Request To Https
app.UseHttpsRedirection();

// -- Error Not Found End Point: Here When This Error Thrown: It Redirect To This End Point in (Controller: Errors)
app.UseStatusCodePagesWithReExecute("/error/{0}");

/// -- In MVC We Used This Way For Routing
///app.UseRouting(); // -> we use this middleware to match request to an endpoint
///app.UseEndpoints  // -> we use this middleware to excute the matched endpoint
///(endpoints =>  
///{
///    endpoints.MapControllerRoute(
///        name: "default",
///        pattern: "{controller}/{action}"
///        );
///});
/// -- But We Use MapController Instead Of It Because We Create Routing On Controller Itself
app.MapControllers(); // -> we use this middleware to talk program that: your routing depend on route written on the controller

#endregion

app.Run();
