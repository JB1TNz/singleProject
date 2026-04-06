using System;
using System.Collections.Generic;

namespace singleProject.Models;

public partial class Cart
{
    public string CartId { get; set; } = null!;

    public string? UserId { get; set; }
}
