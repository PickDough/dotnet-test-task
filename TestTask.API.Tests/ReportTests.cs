namespace TestTask.API.Tests;

using System.Net;
using System.Net.Http.Json;
using Controllers;
using Data.Entities;
using Data.Models;
using Microsoft.AspNetCore.Http;

public class ReportTests : BaseTest
{
    private readonly User user = new()
    {
        Email = "Email@gmail.com",
        Balance = 0
    };

    private readonly List<Item> items = Enumerable.Range(1, 3).Select(i => new Item
        {
            Id = i,
            Name = $"Item {i}",
            Cost = 0
        }
    ).ToList();

    protected override async Task SetupBase()
    {
        await Context.DbContext.Users.AddAsync(user);
        items.ForEach(async i => await Context.DbContext.Items.AddAsync(i));
        await Context.DbContext.SaveChangesAsync();
    }

    [Test]
    public async Task GenerateReport_ShouldReturnItemsCountEqualToN()
    {
        // Arrange
        var boughtItems = new Dictionary<int, List<(Item item, int count)>>
        {
            { 2024, [(items[0], 3), (items[1], 2)] },
            { 2025, [(items[0], 3), (items[1], 2)] },
            { 2026, [(items[0], 3), (items[1], 2)] },
            { 2027, [(items[0], 3), (items[1], 2)] }
        };
        await InsertUserItemsBasedOnPurchaseCount(boughtItems);
        
        //Act
        var responseN1 = await Rait<ReportController>().CallH(controller => controller.GetTopNEachYear(1));
        var responseN2 = await Rait<ReportController>().CallH(controller => controller.GetTopNEachYear(2));
        
        //Assert
        Assert.That(responseN1.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var reportsN1 = await responseN1.Content.ReadFromJsonAsync<List<Report>>();
        Assert.That(reportsN1, Has.Count.EqualTo(4*1));
        
        Assert.That(responseN2.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var reportsN2 = await responseN2.Content.ReadFromJsonAsync<List<Report>>();
        Assert.That(reportsN2, Has.Count.EqualTo(4*2));
    }

    [Test]
    public async Task GenerateReport_ShouldChoseDifferentItemsEachYear()
    {
        // Arrange
        var boughtItems = new Dictionary<int, List<(Item item, int count)>>
        {
            { 2024, [(items[0], 3), (items[1], 2), (items[2], 1)] },
            { 2025, [(items[1], 3), (items[2], 2), (items[0], 1)] },
            { 2026, [(items[2], 3), (items[0], 2), (items[1], 1)] },
        };
        await InsertUserItemsBasedOnPurchaseCount(boughtItems);

        //Act
        var response = await Rait<ReportController>().CallH(controller => controller.GetTopNEachYear(1));
        
        //Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var reports = await response.Content.ReadFromJsonAsync<List<Report>>();
        Assert.That(reports, Has.Count.EqualTo(3));
        
        Assert.That(reports[0].Date.Year, Is.EqualTo(2024));
        Assert.That(reports[0].ItemName, Is.EqualTo("Item 1"));
        
        Assert.That(reports[1].Date.Year, Is.EqualTo(2025));
        Assert.That(reports[1].ItemName, Is.EqualTo("Item 2"));
        
        Assert.That(reports[2].Date.Year, Is.EqualTo(2026));
        Assert.That(reports[2].ItemName, Is.EqualTo("Item 3"));
    }

    private async Task InsertUserItemsBasedOnPurchaseCount(Dictionary<int, List<(Item item, int count)>> boughtItems)
    {
        foreach (var yearlyPurchases in boughtItems)
        {
            foreach (var itemCountPair in yearlyPurchases.Value)
            {
                for (var i = 0; i < itemCountPair.count; i++)
                {
                    await Context.DbContext.UserItems.AddAsync(new UserItem
                        {
                            Item = itemCountPair.item,
                            User = user,
                            PurchaseDate = new DateTime(yearlyPurchases.Key, 1, 1),
                        }
                    );
                }
            }
        }
        await Context.DbContext.SaveChangesAsync();
    }
}
