using System;
using System.Collections.Generic;

namespace CinemaStudio.Models;

public partial class Rental
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public DateOnly DateExit { get; set; }

    public int CountCinema { get; set; }

    public double Fees { get; set; }

    public int Rating { get; set; }

    public virtual Project Project { get; set; } = null!;
}
