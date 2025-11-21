using webapi.Migrations;

namespace ExampleWebApp.Backend.WebApi.Services.Fake;

public class FakeService : IFakeService
{

    readonly ILogger logger;
    readonly AppDbContext dbContext;
    readonly IConfiguration configuration;

    static SemaphoreSlim semFake = new SemaphoreSlim(1, 1);
    static bool fakeInitialized = false;

    public FakeService(
        ILogger<FakeService> logger,
        AppDbContext dbContext,
        IConfiguration configuration
    )
    {
        this.logger = logger;
        this.dbContext = dbContext;
        this.configuration = configuration;
    }

    async Task FakerInit(CancellationToken cancellationToken)
    {
        await semFake.WaitAsync(cancellationToken);

        try
        {
            if (!fakeInitialized)
            {
                if (!await dbContext.FakeDatas.AnyAsync(cancellationToken))
                {
                    var BULK_SLICE_SIZE = 1_000;
                    var BULK_SLICE_CNT = 1_000;
                    var appConfig = configuration.GetAppConfig();

                    var CNT = BULK_SLICE_SIZE * BULK_SLICE_CNT;

                    logger.LogInformation($"initializing {CNT} fake data db ( bulk slice {BULK_SLICE_SIZE} )");

                    var userFaker = new Faker<FakeData>()
                        .RuleFor(u => u.Id, f => Guid.NewGuid())
                        .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                        .RuleFor(u => u.LastName, f => f.Name.LastName())
                        .RuleFor(u => u.Email, f => f.Internet.Email())
                        .RuleFor(u => u.GroupNumber, f => f.Internet.Port())
                        .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber())
                        .RuleFor(u => u.DateOfBirth, f =>
                        {
                            // Age 18-48
                            var q = f.Date.Past(30, DateTime.Now.AddYears(-18));
                            var dto = new DateTimeOffset(q);
                            return dto.ToOffset(TimeSpan.Zero);
                        });

                    {
                        var migration = new fakerdataindexes();

                        var sqlGenerator = dbContext.GetService<IMigrationsSqlGenerator>();

                        var appliedMigrations = (await dbContext.Database.GetAppliedMigrationsAsync(cancellationToken)).ToList();

                        if (appliedMigrations.Any(w => w.EndsWith("_faker-data-indexes")))
                        {
                            // remove indexes
                            var commands = sqlGenerator.Generate(migration.DownOperations);

                            foreach (var cmd in commands)
                                dbContext.Database.ExecuteSqlRaw(cmd.CommandText);

                        }

                        CsvWriter? csvWrite = null;
                        CsvReader? csvRead = null;

                        if (appConfig.FakeDataSet.SaveToCsvPathfilename is not null)
                        {
                            var writer = new StreamWriter(appConfig.FakeDataSet.SaveToCsvPathfilename);
                            csvWrite = new CsvWriter(writer, CultureInfo.InvariantCulture);
                        }
                        else if (appConfig.FakeDataSet.LoadFromCsvPathfilename is not null)
                        {
                            var reader = new StreamReader(appConfig.FakeDataSet.LoadFromCsvPathfilename);
                            csvRead = new CsvReader(reader, CultureInfo.InvariantCulture);
                        }

                        Stopwatch sw = new();

                        sw.Start();

                        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

                        try
                        {

                            for (var s = 0; s < BULK_SLICE_CNT; ++s)
                            {
                                List<FakeData> slice;

                                if (csvRead is not null)
                                {
                                    slice = new();
                                    for (int i = 0; i < BULK_SLICE_SIZE; ++i)
                                    {
                                        csvRead.Read();
                                        slice.Add(csvRead.GetRecord<FakeData>());
                                    }
                                }

                                else
                                    slice = userFaker.Generate(BULK_SLICE_SIZE);

                                if (csvWrite is not null)
                                {
                                    csvWrite.WriteRecords(slice);
                                }

                                await dbContext.BulkInsertAsync(slice, cancellationToken: cancellationToken);
                            }

                            await dbContext.SaveChangesAsync(cancellationToken);

                            await transaction.CommitAsync(cancellationToken);

                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync(cancellationToken);
                        }

                        sw.Stop();
                        logger.LogInformation($"bulk insert of {BULK_SLICE_CNT * BULK_SLICE_SIZE} rows taken {sw.Elapsed}");

                        if (csvWrite is not null)
                            csvWrite.Dispose();

                        {
                            // get indexes back
                            var commands = sqlGenerator.Generate(migration.UpOperations);

                            foreach (var cmd in commands)
                                dbContext.Database.ExecuteSqlRaw(cmd.CommandText);
                        }
                    }
                }

                fakeInitialized = true;
            }
        }
        finally
        {
            semFake.Release();
        }
    }

    public async Task<int> CountAsync(string? dynFilter, CancellationToken cancellationToken)
    {
        await FakerInit(cancellationToken);

        var q = dbContext.FakeDatas.AsQueryable();

        q = q.ApplySortAndFilter(sort: null, dynFilter);

        return await q.CountAsync(cancellationToken);
    }

    public async Task<List<FakeData>> GetViewAsync(
        int off, int cnt, string? dynFilter, GenericSort? sort, CancellationToken cancellationToken)
    {
        await FakerInit(cancellationToken);

        var q = dbContext.FakeDatas.AsQueryable();

        q = q.ApplySortAndFilter(sort, dynFilter);

        var q2 = q.Skip(off);
        if (cnt >= 0) q2 = q2.Take(cnt);
        var tmp = await q2.ToListAsync(cancellationToken);

        return tmp;
    }

    public async Task<FakeData?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var qq = await dbContext.FakeDatas
            // .Include(w => w.RecordAuditLog)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

        return qq;
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var q = await dbContext.FakeDatas.FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

        if (q is not null)
        {
            dbContext.Remove(q);
            await dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        return false;
    }

    public async Task<FakeData> UpdateAsync(GenericItemWithOrig<FakeData> req, CancellationToken cancellationToken)
    {
        if (req.updatedItem.Id != Guid.Empty)
        {
            var cur = dbContext.FakeDatas.FirstOrDefault(w => w.Id == req.updatedItem.Id);
            if (cur is null) throw new Exception($"can't find ${nameof(FakeData)} with id {req.updatedItem.Id}");

            CopyRecord(cur, req.origItem, req.updatedItem);
        }

        else
        {
            dbContext.Attach(req.updatedItem);
            dbContext.Add(req.updatedItem);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return req.updatedItem;
    }


}