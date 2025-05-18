namespace AutoDump.Server.Modules;

public interface IApiModule
{
    IServiceCollection AddServices(IServiceCollection serviceCollection);

    IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints);
}