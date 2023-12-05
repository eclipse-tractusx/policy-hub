/********************************************************************************
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
using System.Reflection;
using System.Text.Json.Serialization;
using Testcontainers.PostgreSql;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace Org.Eclipse.TractusX.PolicyHub.Service.Tests.Setup;

public class IntegrationTestFactory : WebApplicationFactory<PolicyHubBusinessLogic>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
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
            conf.AddJsonFile(configPath, true)
                .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
                .AddEnvironmentVariables();
        });
        builder.ConfigureTestServices(services =>
        {
            services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<PolicyHubContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<PolicyHubContext>(options =>
            {
                options.UseNpgsql(_container.GetConnectionString(),
                    x => x.MigrationsAssembly(typeof(BatchInsertSeeder).Assembly.GetName().Name)
                        .MigrationsHistoryTable("__efmigrations_history_hub", "public"));
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
            _container.GetConnectionString(),
            x => x.MigrationsAssembly(typeof(BatchInsertSeeder).Assembly.GetName().Name)
                .MigrationsHistoryTable("__efmigrations_history_hub", "public")
        );
        var context = new PolicyHubContext(optionsBuilder.Options);
        context.Database.Migrate();

        var seederOptions = Options.Create(new SeederSettings
        {
            TestDataEnvironments = new[] { "test" },
            DataPaths = new[] { "Seeder/Data" }
        });
        var insertSeeder = new BatchInsertSeeder(context,
            LoggerFactory.Create(c => c.AddConsole()).CreateLogger<BatchInsertSeeder>(),
            seederOptions);
        insertSeeder.ExecuteAsync(CancellationToken.None).GetAwaiter().GetResult();
        var updateSeeder = new BatchUpdateSeeder(context,
            LoggerFactory.Create(c => c.AddConsole()).CreateLogger<BatchUpdateSeeder>(),
            seederOptions);
        updateSeeder.ExecuteAsync(CancellationToken.None).GetAwaiter().GetResult();
        return host;
    }

    public async Task InitializeAsync() => await _container.StartAsync();

    public new async Task DisposeAsync() => await _container.DisposeAsync();
}
