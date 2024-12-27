using System.ComponentModel.DataAnnotations.Schema;

namespace TestTask.Data.Entities;

using System.Runtime.Serialization;

public class UserItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    [ForeignKey(nameof(UserId))] public User? User { get; set; }

    public int ItemId { get; set; }
    [ForeignKey(nameof(ItemId))] public Item? Item { get; set; }
    
    [IgnoreDataMember]
    public DateOnly PurchaseDateD { get; private set; }
    [IgnoreDataMember]
    public TimeOnly PurchaseDateT { get; private set; }


    [NotMapped]
    public DateTime PurchaseDate
    {
        get => new(PurchaseDateD, PurchaseDateT);
        set
        {
            PurchaseDateD = DateOnly.FromDateTime(value);
            PurchaseDateT = TimeOnly.FromDateTime(value);
        }
    }

    public UserItem()
    {
        PurchaseDate = DateTime.Now;
    }
}