using Microsoft.EntityFrameworkCore;
using TestTask.Data;
using TestTask.Data.Entities;

namespace TestTask.Services;

using System.Data;

public class MarketService
{
    private readonly IDbContextFactory<TestDbContext> _testDbContextFactory;

    public MarketService(IDbContextFactory<TestDbContext> testDbContextFactory)
    {
        _testDbContextFactory = testDbContextFactory;
    }

    public async Task BuyAsync(int userId, int itemId)
    {
        var strategy = (await _testDbContextFactory.CreateDbContextAsync()).Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(
            async () =>
            {
                await using var testDbContext = await _testDbContextFactory.CreateDbContextAsync();
                await using var transaction = await testDbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);

                var user = await testDbContext.Users.FirstOrDefaultAsync(n => n.Id == userId);
                if (user == null)
                    throw new Exception("User not found");
                var item = await testDbContext.Items.FirstOrDefaultAsync(n => n.Id == itemId);
                if (item == null)
                    throw new Exception("Item not found");

                if (user.Balance < item.Cost)
                {
                    throw new Exception("Not enough balance");
                }

                user.Balance -= item.Cost;
                testDbContext.Update(user);
                await testDbContext.SaveChangesAsync();

                await testDbContext.UserItems.AddAsync(new UserItem
                    {
                        UserId = userId,
                        ItemId = itemId
                    }
                );
                await testDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
        );
    }
}
