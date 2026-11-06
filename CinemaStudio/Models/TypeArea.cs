using System;
using System.Collections.Generic;

namespace CinemaStudio.Models;

public partial class TypeArea
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Area> Areas { get; set; } = new List<Area>();
}
