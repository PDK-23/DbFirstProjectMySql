﻿using System;
using System.Collections.Generic;

namespace DbFirstProjectMySql.Infrastructure.Entities;

public partial class Role
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
