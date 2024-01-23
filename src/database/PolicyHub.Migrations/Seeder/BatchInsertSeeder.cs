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

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Org.Eclipse.TractusX.PolicyHub.Entities;
using Org.Eclipse.TractusX.PolicyHub.Entities.Entities;
using Org.Eclipse.TractusX.Portal.Backend.Framework.Seeding;

namespace Org.Eclipse.TractusX.PolicyHub.Migrations.Seeder;

/// <summary>
/// Seeder to seed the all configured entities
/// </summary>
public class BatchInsertSeeder : ICustomSeeder
{
    private readonly PolicyHubContext _context;
    private readonly ILogger<BatchInsertSeeder> _logger;
    private readonly SeederSettings _settings;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="logger">The logger</param>
    /// <param name="options">The options</param>
    public BatchInsertSeeder(PolicyHubContext context, ILogger<BatchInsertSeeder> logger, IOptions<SeederSettings> options)
    {
        _context = context;
        _logger = logger;
        _settings = options.Value;
    }

    /// <inheritdoc />
    public int Order => 1;

    /// <inheritdoc />
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (!_settings.DataPaths.Any())
        {
            _logger.LogInformation("There a no data paths configured, therefore the {SeederName} will be skipped", nameof(BatchInsertSeeder));
            return;
        }

        await SeedTable<Policy>("policies", x => x.Id, cancellationToken).ConfigureAwait(false);
        await SeedTable<PolicyAttribute>("policy_attributes", x => new { x.PolicyId, x.Key, x.AttributeValue }, cancellationToken).ConfigureAwait(false);
        await SeedTable<PolicyKindConfiguration>("policy_kind_configurations", x => x.PolicyKindId, cancellationToken).ConfigureAwait(false);
        await SeedTable<PolicyAssignedTypes>("policy_assigned_types", x => new { x.PolicyId, x.PolicyTypeId }, cancellationToken).ConfigureAwait(false);
        await SeedTable<PolicyAssignedUseCases>("policy_assigned_use_cases", x => new { x.PolicyId, x.UseCaseId }, cancellationToken).ConfigureAwait(false);

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task SeedTable<T>(string fileName, Func<T, object> keySelector, CancellationToken cancellationToken) where T : class
    {
        _logger.LogDebug("Start seeding {Filename}", fileName);
        var additionalEnvironments = _settings.TestDataEnvironments ?? Enumerable.Empty<string>();
        var data = await SeederHelper.GetSeedData<T>(_logger, fileName, _settings.DataPaths, cancellationToken, additionalEnvironments.ToArray()).ConfigureAwait(false);
        _logger.LogDebug("Found {ElementCount} data", data.Count);
        if (data.Any())
        {
            var typeName = typeof(T).Name;
            _logger.LogDebug("Started to Seed {TableName}", typeName);
            data = data.GroupJoin(_context.Set<T>(), keySelector, keySelector, (d, dbEntry) => new { d, dbEntry })
                .SelectMany(t => t.dbEntry.DefaultIfEmpty(), (t, x) => new { t, x })
                .Where(t => t.x == null)
                .Select(t => t.t.d).ToList();
            _logger.LogDebug("Seeding {DataCount} {TableName}", data.Count, typeName);
            await _context.Set<T>().AddRangeAsync(data, cancellationToken).ConfigureAwait(false);
            _logger.LogDebug("Seeded {TableName}", typeName);
        }
    }
}
