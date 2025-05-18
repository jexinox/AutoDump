using AutoDump.Server.Dumps;
using AutoDump.Server.DumpsMetadata;
using AutoDump.Server.Modules;
using AutoDump.Server.Reports;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
var modules = new IApiModule[] { new DumpsModule(), new DumpsMetadataModule(), new ReportsModule() };

if (builder.Environment.IsDevelopment())
{
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
}

builder.Services.AddOpenApi().AddSwaggerGen(options => options.CustomSchemaIds(type => type.ToString().Replace("+", ".")));
builder.Services
    .AddSingleton<IMongoClient>(_ => new MongoClient(
        new MongoUrl("mongodb://localhost:27017/?readPreference=primary&appname=autodump.server&directConnection=true&ssl=false")))
    .AddSingleton<IMongoDatabase>(sp => sp.GetRequiredService<IMongoClient>().GetDatabase("AutoDump"))
    .AddModulesServices(modules);

var app = builder.Build();

app
    .MapModulesEndpoints(modules)
    .MapOpenApi();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();