using EFCore.BulkExtensions;
using webapi.Migrations;

namespace ExampleWebApp.Backend.WebApi.Services.Fake;

public class FakeService : IFakeService
{

    readonly ILogger logger;
    readonly AppDbContext dbContext;

    static SemaphoreSlim semFake = new SemaphoreSlim(1, 1);
    static bool fakeInitialized = false;

    public FakeService(
        ILogger<FakeService> logger,
        AppDbContext dbContext
    )
    {
        this.logger = logger;
        this.dbContext = dbContext;
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
                    var BULK_SLICE = 10_000;
                    var BULK_SLICE_CNT = 1_000;

                    var CNT = BULK_SLICE * BULK_SLICE_CNT;

                    logger.LogInformation($"initializing {CNT} fake data db ( bulk slice {BULK_SLICE} )");

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


                    var migration = new fakerdataindexes();

                    var sqlGenerator = dbContext.GetService<IMigrationsSqlGenerator>();

                    // remove indexes
                    var commands = sqlGenerator.Generate(migration.DownOperations);

                    foreach (var cmd in commands)                    
                        dbContext.Database.ExecuteSqlRaw(cmd.CommandText);                    

                    for (var s = 0; s < BULK_SLICE_CNT; ++s)
                    {
                        var slice = userFaker.Generate(BULK_SLICE);

                        await dbContext.BulkInsertAsync(slice, cancellationToken: cancellationToken);
                    }

                    // get indexes back
                    commands = sqlGenerator.Generate(migration.UpOperations);

                    foreach (var cmd in commands)                    
                        dbContext.Database.ExecuteSqlRaw(cmd.CommandText);                    
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