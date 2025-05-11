using Guts.DumpsDaemon;
using MassTransit;
using Refit;

var builder = Host.CreateApplicationBuilder(args);
builder.Services
    .AddOptions<GutsServerClientOptions>()
    .BindConfiguration(GutsServerClientOptions.Section);

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
    .AddSingleton<IGutsServerClient>(
        services => RestService.For<IGutsServerClient>(services.GetRequiredService<GutsServerClientOptions>().Url));

var host = builder.Build();
host.Run();