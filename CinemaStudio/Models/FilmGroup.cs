using System;
using System.Collections.Generic;

namespace CinemaStudio.Models;

public partial class FilmGroup
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public int EmployeeId { get; set; }

    public string RoleInProject { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;
}
