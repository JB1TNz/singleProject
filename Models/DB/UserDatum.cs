using System;
using System.Collections.Generic;

namespace singleProject.Models.Db;

public partial class UserDatum
{
    public string UserId { get; set; } = null!;

    public string? UserPassword { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? UserRole { get; set; }
}
