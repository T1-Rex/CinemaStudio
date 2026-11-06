using System;
using System.Collections.Generic;

namespace CinemaStudio.Models;

public partial class Area
{
    public int Id { get; set; }

    public string Address { get; set; } = null!;

    public int TypeId { get; set; }

    public double CostRentalInDay { get; set; }

    public int CapacityArea { get; set; }

    public virtual ICollection<ShootingSchedule> ShootingSchedules { get; set; } = new List<ShootingSchedule>();

    public virtual TypeArea Type { get; set; } = null!;
}
