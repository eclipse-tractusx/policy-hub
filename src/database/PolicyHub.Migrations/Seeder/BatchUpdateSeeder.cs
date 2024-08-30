/********************************************************************************
 * Copyright (c) 2023 Contributors to the Eclipse Foundation
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
/// Seeder to modify the is_active flag of the configured entities
/// </summary>
public class BatchUpdateSeeder : ICustomSeeder
{
    private readonly PolicyHubContext _context;
    private readonly ILogger<BatchUpdateSeeder> _logger;
    private readonly SeederSettings _settings;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="logger">The logger</param>
    /// <param name="options">The options</param>
    public BatchUpdateSeeder(PolicyHubContext context, ILogger<BatchUpdateSeeder> logger, IOptions<SeederSettings> options)
    {
        _context = context;
        _logger = logger;
        _settings = options.Value;
    }

    /// <inheritdoc />
    public int Order => 2;

    /// <inheritdoc />
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (!_settings.DataPaths.Any())
        {
            _logger.LogInformation("There a no data paths configured, therefore the {SeederName} will be skipped", nameof(BatchUpdateSeeder));
            return;
        }

        _logger.LogInformation("Start BaseEntityBatch Seeder");
        await SeedTable<Policy>(
            "policies",
            x => new { x.Id },
            x => x.dbEntity.IsActive != x.dataEntity.IsActive || x.dbEntity.TechnicalKey != x.dataEntity.TechnicalKey || x.dbEntity.LeftOperandValue != x.dataEntity.LeftOperandValue || x.dbEntity.AttributeKeyId != x.dataEntity.AttributeKeyId,
            (dbEntity, entity) =>
            {
                dbEntity.IsActive = entity.IsActive;
                dbEntity.TechnicalKey = entity.TechnicalKey;
                dbEntity.LeftOperandValue = entity.LeftOperandValue;
                dbEntity.AttributeKeyId = entity.AttributeKeyId;
            }, cancellationToken).ConfigureAwait(false);

        await SeedTable<PolicyAttribute>(
            "policy_attributes",
            x => new { x.Id },
            x => x.dbEntity.IsActive != x.dataEntity.IsActive || x.dbEntity.AttributeValue != x.dataEntity.AttributeValue || x.dbEntity.Key != x.dataEntity.Key || x.dbEntity.PolicyId != x.dataEntity.PolicyId,
            (dbEntry, entry) =>
            {
                dbEntry.IsActive = entry.IsActive;
                dbEntry.AttributeValue = entry.AttributeValue;
                dbEntry.Key = entry.Key;
                dbEntry.PolicyId = entry.PolicyId;
            }, cancellationToken).ConfigureAwait(false);

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Finished BaseEntityBatch Seeder");
    }

    private async Task SeedTable<T>(string fileName, Func<T, object> keySelector, Func<(T dataEntity, T dbEntity), bool> whereClause, Action<T, T> updateEntries, CancellationToken cancellationToken) where T : class
    {
        _logger.LogDebug("Start seeding {Filename}", fileName);
        var additionalEnvironments = _settings.TestDataEnvironments ?? Enumerable.Empty<string>();
        var data = await SeederHelper.GetSeedData<T>(_logger, fileName, _settings.DataPaths, cancellationToken, additionalEnvironments.ToArray()).ConfigureAwait(false);
        _logger.LogDebug("Found {ElementCount} data", data.Count);
        if (data.Any())
        {
            var typeName = typeof(T).Name;
            var entriesForUpdate = data
                .Join(_context.Set<T>(), keySelector, keySelector, (dataEntry, dbEntry) => (DataEntry: dataEntry, DbEntry: dbEntry))
                .Where(whereClause.Invoke)
                .ToList();
            if (entriesForUpdate.Any())
            {
                _logger.LogDebug("Started to Update {EntryCount} entries of {TableName}", entriesForUpdate.Count, typeName);
                foreach (var entry in entriesForUpdate)
                {
                    updateEntries.Invoke(entry.DbEntry, entry.DataEntry);
                }

                _logger.LogDebug("Updated {TableName}", typeName);
            }
        }
    }
}
