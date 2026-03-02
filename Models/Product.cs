using System;
using System.Collections.Generic;

namespace singleProject.Models;

public partial class Product
{
    public string ProductId { get; set; } = null!;

    public string? ProductName { get; set; }

    public string? Descriptions { get; set; }

    public decimal? Price { get; set; }

    public int? Stock { get; set; }
}
