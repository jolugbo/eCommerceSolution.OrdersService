using BusinessLogicLayer.OrdersService.BusinessLogicLayer.HTTPClients;
using eCommerce.OrdersService.API.Middleware;
//using eCommerce.OrdersService.API.APIEndPoints;
using eCommerce.OrdersService.API.Middleware;
using eCommerce.OrdersService.BusinessLogicLayer;
using eCommerce.OrdersService.BusinessLogicLayer.Policies;
using eCommerce.OrdersService.DataAccessLayer;
using FluentValidation.AspNetCore;
using Polly;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddBusinessLogicLayer(builder.Configuration);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.ConfigureHttpJsonOptions(options =>
options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});
builder.Services.AddTransient<IUsersMicroservicePolicies, UsersMicroservicePolicies>();
builder.Services.AddTransient<IProductsMicroservicePolicies, ProductsMicroservicePolicies>();
builder.Services.AddTransient<IPollyPolicies, PollyPolicies>();
var usersMicroserviceName = builder.Configuration["UsersMicroserviceName"];
var usersMicroservicePort = builder.Configuration["UsersMicroservicePort"];
builder.Services.AddHttpClient<UsersMicroserviceClient>(client =>
{
    client.BaseAddress = new Uri($"http://{usersMicroserviceName}:{usersMicroservicePort}");
}).AddPolicyHandler(
   builder.Services.BuildServiceProvider()
   .GetRequiredService<IUsersMicroservicePolicies>()
   .GetCombinedPolicy());
var productsMicroserviceName = builder.Configuration["ProductsMicroserviceName"];
var productsMicroservicePort = builder.Configuration["ProductsMicroservicePort"];
builder.Services.AddHttpClient<ProductsMicroserviceClient>(client => {
    client.BaseAddress = new Uri($"http://{productsMicroserviceName}:{productsMicroservicePort}");
}).AddPolicyHandler(
   builder.Services.BuildServiceProvider()
   .GetRequiredService<IProductsMicroservicePolicies>()
   .GetFallbackPolicy())
.AddPolicyHandler(
   builder.Services.BuildServiceProvider()
   .GetRequiredService<IProductsMicroservicePolicies>()
   .GetBulkHeadIsolationPolicy());
var app = builder.Build();

app.UseExceptionHandlingMiddleware();
app.UseRouting();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseHttpsRedirection();
}
app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
//app.MapProductAPIEndpoints();
app.Run();