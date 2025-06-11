using System;
using System.Collections.Generic;

namespace DbFirstProjectMySql.Infrastructure.Entities;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
