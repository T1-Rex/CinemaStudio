using System;
using System.Collections.Generic;

namespace CinemaStudio.Models;

public partial class ShootingSchedule
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly Time { get; set; }

    public int ProjectId { get; set; }

    public int AreaId { get; set; }

    public string? Scene { get; set; }

    public string? Description { get; set; }

    public virtual Area Area { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;
}
