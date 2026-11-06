using System;
using System.Collections.Generic;

namespace CinemaStudio.Models;

public partial class StatusProject
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
