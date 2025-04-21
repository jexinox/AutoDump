using Guts.Server.Dumps;
using Guts.Server.DumpsMetadata;
using Guts.Server.Modules;
using Guts.Server.Reports;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
var modules = new IApiModule[] { new DumpsModule(), new DumpsMetadataModule(), new ReportsModule() };

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