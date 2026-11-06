using System;
using System.Collections.Generic;

namespace CinemaStudio.Models;

public partial class Expense
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public int TypeId { get; set; }

    public double Sum { get; set; }

    public DateOnly Date { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual TypeExpense Type { get; set; } = null!;
}
