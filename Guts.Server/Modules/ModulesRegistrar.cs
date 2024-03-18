namespace Guts.Server.Modules;

public static class ModulesRegistrar
{
    public static IServiceCollection AddModulesServices(
        this IServiceCollection serviceCollection, IEnumerable<IApiModule> modules)
    {
        foreach (var module in modules)
        {
            module.AddServices(serviceCollection);
        }

        return serviceCollection;
    }

    public static IEndpointRouteBuilder MapModulesEndpoints(
        this IEndpointRouteBuilder endpoints, IEnumerable<IApiModule> modules)
    {
        foreach (var module in modules)
        {
            module.MapEndpoints(endpoints);
        }

        return endpoints;
    }
}