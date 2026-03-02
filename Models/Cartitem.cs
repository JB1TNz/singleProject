using System;
using System.Collections.Generic;

namespace singleProject.Models;

public partial class Cartitem
{
    public string CartItemId { get; set; } = null!;

    public int? CartId { get; set; }

    public int? ProductId { get; set; }

    public int? Quantity { get; set; }
}
