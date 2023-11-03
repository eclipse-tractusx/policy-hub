using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Org.Eclipse.TractusX.PolicyHub.Entities;
using Org.Eclipse.TractusX.PolicyHub.Service.BusinessLogic;

namespace Org.Eclipse.TractusX.PolicyHub.Service.Tests.Setup;

public class HubApplication : WebApplicationFactory<PolicyHubBusinessLogic>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var root = new InMemoryDatabaseRoot();

        builder.ConfigureServices(services =>
        {
            services.AddScoped(sp => new DbContextOptionsBuilder<PolicyHubContext>()
                .UseInMemoryDatabase("hub", root)
                .UseApplicationServiceProvider(sp)
                .Options);

            services.EnsureDbCreatedWithSeeding();
            services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
        });

        return base.CreateHost(builder);
    }
}
