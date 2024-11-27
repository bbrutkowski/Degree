using Degree.Services.Interfaces;
using Degree.Services;
using Serilog;
using Degree.Models;
using Polly;
using System.Net.Http.Headers;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();  
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true; 
    options.Cookie.IsEssential = true;
});

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) 
    .Enrich.FromLogContext() 
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.Configure<FakeStoreApiOptions>(builder.Configuration.GetSection("FakeStoreApi"));


builder.Services.AddHttpClient("FakeStoreApiClient", client =>
{
    var configuration = builder.Configuration.GetSection("FakeStoreApi");
    var baseUrl = configuration.GetValue<string>("BaseUrl") ?? string.Empty;
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
})
.AddPolicyHandler(Policy
    .Handle<HttpRequestException>()
    .OrResult<HttpResponseMessage>(x => !x.IsSuccessStatusCode)
    .RetryAsync(3));

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ISessionService, SessionService>();
builder.Services.AddScoped<IHttpService, HttpService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddTransient<IShoppingCartService, ShoppingCartService>();
builder.Services.AddTransient<IPaginationService, PaginationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}");

app.Run();
