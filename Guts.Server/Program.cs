using Guts.Server.Dumps.Module;
using Mapster;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
}

var dumpsModule = new DumpsModule();

dumpsModule.AddServices(builder.Services);
builder.Services.AddMapster();

var app = builder.Build();

dumpsModule.MapEndpoints(app);
app.MapGet("/", () => "Hello World!");

app.Run();