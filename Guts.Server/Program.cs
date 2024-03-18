using Guts.Server.Dumps;
using Guts.Server.Modules;
using Mapster;

var builder = WebApplication.CreateBuilder(args);
var modules = new IApiModule[] { new DumpsModule() };

if (builder.Environment.IsDevelopment())
{
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
}

builder.Services
    .AddModulesServices(modules)
    .AddMapster();

var app = builder.Build();

app.MapModulesEndpoints(modules);

app.Run();