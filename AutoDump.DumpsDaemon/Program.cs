using AutoDump.DumpsDaemon;
using MassTransit;
using Refit;

var builder = Host.CreateApplicationBuilder(args);
builder.Services
    .AddOptions<AutoDumpServerClientOptions>()
    .BindConfiguration(AutoDumpServerClientOptions.Section);

builder.Services
    .AddMassTransit(massTransit =>
    {
        massTransit.AddConsumer<DumpEventsConsumer>();
        massTransit.SetKebabCaseEndpointNameFormatter();
        massTransit.UsingRabbitMq((context, configurator) =>
        {
            configurator.ConfigureEndpoints(context);
        });
    })
    .AddLogging(c => c.AddConsole())
    .AddSingleton<IAutoDumpServerClient>(
        services => RestService.For<IAutoDumpServerClient>(services.GetRequiredService<AutoDumpServerClientOptions>().Url));

var host = builder.Build();
host.Run();
