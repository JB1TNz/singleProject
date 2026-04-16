using System;

namespace singleProject.Models.Db;

public partial class UserLibrary
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public int ProductId { get; set; }
    public DateTime PurchaseDate { get; set; }
}
