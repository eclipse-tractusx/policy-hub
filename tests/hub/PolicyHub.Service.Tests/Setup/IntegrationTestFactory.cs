/********************************************************************************
 * Copyright (c) 2021, 2023 BMW Group AG
 * Copyright (c) 2021, 2023 Contributors to the Eclipse Foundation
 *
 * See the NOTICE file(s) distributed with this work for additional
 * information regarding copyright ownership.
 *
 * This program and the accompanying materials are made available under the
 * terms of the Apache License, Version 2.0 which is available at
 * https://www.apache.org/licenses/LICENSE-2.0.
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 *
 * SPDX-License-Identifier: Apache-2.0
 ********************************************************************************/

using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Org.Eclipse.TractusX.PolicyHub.Entities;
using Org.Eclipse.TractusX.PolicyHub.Migrations.Seeder;
using Org.Eclipse.TractusX.PolicyHub.Service.BusinessLogic;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Logging;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Seeding;
using Testcontainers.PostgreSql;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace Org.Eclipse.TractusX.PolicyHub.Service.Tests.Setup;

public class IntegrationTestFactory : WebApplicationFactory<PolicyHubBusinessLogic>, IAsyncLifetime
{
    protected readonly PostgreSqlContainer Container = new PostgreSqlBuilder()
        .WithDatabase("test_db")
        .WithImage("postgres")
        .WithCleanUp(true)
        .WithName(Guid.NewGuid().ToString())
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var projectDir = Directory.GetCurrentDirectory();
        var configPath = Path.Combine(projectDir, "appsettings.IntegrationTests.json");

        builder.ConfigureAppConfiguration((_, conf) =>
        {
            conf.AddJsonFile(configPath, true);
        });
        builder.ConfigureTestServices(services =>
        {
            services.RemoveProdDbContext<PolicyHubContext>();
            services.AddDbContext<PolicyHubContext>(options =>
            {
                options.UseNpgsql(Container.GetConnectionString(),
                    x => x.MigrationsAssembly(typeof(BatchInsertSeeder).Assembly.GetName().Name)
                        .MigrationsHistoryTable("__efmigrations_history_portal"));
            });
            services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
        });
    }

    /// <inheritdoc />
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.AddLogging();
        var host = base.CreateHost(builder);

        var optionsBuilder = new DbContextOptionsBuilder<PolicyHubContext>();

        optionsBuilder.UseNpgsql(
            Container.GetConnectionString(),
            x => x.MigrationsAssembly(typeof(BatchInsertSeeder).Assembly.GetName().Name)
                .MigrationsHistoryTable("__efmigrations_history_hub")
        );
        var context = new PolicyHubContext(optionsBuilder.Options);
        context.Database.Migrate();

        var seederOptions = Options.Create(new SeederSettings
        {
            TestDataEnvironments = new[] { "test" },
            DataPaths = new[] { "Seeder/Data" }
        });
        var insertSeeder = new BatchInsertSeeder(context,
            LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<BatchInsertSeeder>(),
            seederOptions);
        insertSeeder.ExecuteAsync(CancellationToken.None).GetAwaiter().GetResult();
        return host;
    }

    public async Task InitializeAsync() => await Container.StartAsync();

    public new async Task DisposeAsync() => await Container.DisposeAsync();
}
