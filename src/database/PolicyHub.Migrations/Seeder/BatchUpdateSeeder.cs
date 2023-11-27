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
            x => x.dbEntity.IsActive != x.dataEntity.IsActive,
            (dbEntity, entity) =>
            {
                dbEntity.IsActive = entity.IsActive;
            }, cancellationToken).ConfigureAwait(false);

        await SeedTable<PolicyAttribute>(
            "policy_attributes",
            x => new { x.PolicyId, x.Key, x.AttributeValue },
            x => x.dbEntity.IsActive != x.dataEntity.IsActive,
            (dbEntry, entry) =>
            {
                dbEntry.IsActive = entry.IsActive;
            }, cancellationToken).ConfigureAwait(false);

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Finished BaseEntityBatch Seeder");
    }

    private async Task SeedTable<T>(string fileName, Func<T, object> keySelector, Func<(T dataEntity, T dbEntity), bool> whereClause, Action<T, T> updateEntries, CancellationToken cancellationToken) where T : class
    {
        _logger.LogInformation("Start seeding {Filename}", fileName);
        var additionalEnvironments = _settings.TestDataEnvironments ?? Enumerable.Empty<string>();
        var data = await SeederHelper.GetSeedData<T>(_logger, fileName, _settings.DataPaths, cancellationToken, additionalEnvironments.ToArray()).ConfigureAwait(false);
        _logger.LogInformation("Found {ElementCount} data", data.Count);
        if (data.Any())
        {
            var typeName = typeof(T).Name;
            var entriesForUpdate = data
                .Join(_context.Set<T>(), keySelector, keySelector, (dataEntry, dbEntry) => (DataEntry: dataEntry, DbEntry: dbEntry))
                .Where(whereClause.Invoke)
                .ToList();
            if (entriesForUpdate.Any())
            {
                _logger.LogInformation("Started to Update {EntryCount} entries of {TableName}", entriesForUpdate.Count, typeName);
                foreach (var entry in entriesForUpdate)
                {
                    updateEntries.Invoke(entry.DbEntry, entry.DataEntry);
                }
                _logger.LogInformation("Updated {TableName}", typeName);
            }
        }
    }
}
