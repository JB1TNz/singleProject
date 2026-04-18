using System;

namespace singleProject.Models.Db;

public partial class ProductData
{
    public int ProductId { get; set; }

    public string? ProductName { get; set; }

    public string? ProductDescription { get; set; }

    public decimal? Price { get; set; }

    public string? FilePath { get; set; }

    public string? CoverPicture { get; set; }

    public string? SellerId { get; set; }

    public int? CategoryId { get; set; }

    public int? Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public decimal? PromotionPrice { get; set; }

    public DateTime? PromotionEndDate { get; set; }

    public DateTime? LastPromotionEdit { get; set; }
}
