namespace TestTask.Data.Models;

using Entities;

public class Report
{
    public DateOnly Date { get; set; }
    public string ItemName { get; set; }
    public int MaxTimesBought { get; set; }
}
