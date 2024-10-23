using BlazorEcommerce.Shared.Models;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.Configure<GithubData>(builder.Configuration.GetSection("Github"));
builder.Services.Configure<GoogleData>(builder.Configuration.GetSection("Google"));
builder.Services.Configure<Urls>(builder.Configuration.GetSection("Urls"));

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();

builder.Services.AddTransient<AuthenticationHandler>();

builder.Services.AddBlazoredToast();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped(typeof(IAccountService), typeof(AccountService));
builder.Services.AddScoped(typeof(IProductService), typeof(ProductService));
builder.Services.AddScoped(typeof(ICategoryService), typeof(CategoryService));
builder.Services.AddScoped(typeof(ICartService), typeof(CartService));
builder.Services.AddScoped(typeof(ICheckoutService), typeof(CheckoutService));

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

builder.Services.AddHttpClient("Auth", options =>
{
	options.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
}).AddHttpMessageHandler<AuthenticationHandler>();

await builder.Build().RunAsync();