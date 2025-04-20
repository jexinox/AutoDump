using Guts.Server.Dumps;
using Guts.Server.Modules;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
var modules = new IApiModule[] { new DumpsModule() };

if (builder.Environment.IsDevelopment())
{
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
}

builder.Services
    .AddSingleton<IMongoClient>(_ => new MongoClient(
        new MongoUrl("mongodb://localhost:27017/?readPreference=primary&appname=guts.server&directConnection=true&ssl=false")))
    .AddSingleton<IMongoDatabase>(sp => sp.GetRequiredService<IMongoClient>().GetDatabase("Guts"))
    .AddModulesServices(modules);

var app = builder.Build();

app.MapModulesEndpoints(modules);

app.Run();