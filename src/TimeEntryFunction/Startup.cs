using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.PowerPlatform.Dataverse.Client;

using TimeEntryFunction;
using TimeEntryFunction.Application;
using TimeEntryFunction.Infrastructure;

[assembly: FunctionsStartup(typeof(Startup))]

namespace TimeEntryFunction;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddSingleton<IDynamicsFacade, DynamicsFacade>();
        builder.Services.AddSingleton<TimeEntriesMaker>();
        builder.Services.AddSingleton<IOrganizationServiceAsync2>(
            _ =>
                new ServiceClient(Environment.GetEnvironmentVariable("ConnectionStrings:CrmConnectionString")));
    }
}
