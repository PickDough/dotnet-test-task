namespace TestTask.Data.Repositories;

using Microsoft.EntityFrameworkCore;
using Models;

public class ReportRepositoryDbContext(IDbContextFactory<TestDbContext> dbContextFactory) : IReportRepository
{
    private static FormattableString TopNQuery(int n) =>
        $"""
         WITH DailyPurchase as (select "ItemId",
                                       count(*)                              daily_purchase,
                                       "PurchaseDateD",
                                       extract(year from "PurchaseDateD") as "year"
                                from "UserItems"
                                group by "ItemId", "PurchaseDateD"),
              MostPopularDays as (select max(daily_purchase) max_purchases,
                                         "ItemId",
                                         "year"
                                  from DailyPurchase
                                  group by "ItemId", "year"),
              RankedByYear as (select mpd.*,
                                      dp."PurchaseDateD",
                                      row_number() over (partition by mpd."year" order by max_purchases desc) rank
                               from MostPopularDays mpd
                                        left join DailyPurchase dp
                                                  on dp."ItemId" = mpd."ItemId" and daily_purchase = max_purchases and dp.year = mpd.year)
         select distinct on (rby."ItemId", rby.year) rby."PurchaseDateD" as "Date", i."Name" as "ItemName", rby.max_purchases as "MaxTimesBought"
         from RankedByYear rby
                  left join "Items" i on i."Id" = rby."ItemId"
         where rby.rank <= {n}
         order by rby.year, "ItemId";
         """;

    public async Task<IEnumerable<Report>> GetTopNEachYear(int n)
    {
        var db = await dbContextFactory.CreateDbContextAsync();
        return await db.Database
            .SqlQuery<Report>(TopNQuery(n))
            .ToListAsync();
    }
}
