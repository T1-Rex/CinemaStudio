using System;
using System.Collections.Generic;

namespace CinemaStudio.Models;

public partial class Project
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public double Budget { get; set; }

    public DateOnly DateStart { get; set; }

    public DateOnly DateEnd { get; set; }

    public int StatusId { get; set; }

    public string? TargetAudience { get; set; }

    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    public virtual ICollection<FilmGroup> FilmGroups { get; set; } = new List<FilmGroup>();

    public virtual ICollection<GenresInProject> GenresInProjects { get; set; } = new List<GenresInProject>();

    public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();

    public virtual ICollection<ShootingSchedule> ShootingSchedules { get; set; } = new List<ShootingSchedule>();

    public virtual StatusProject Status { get; set; } = null!;

    public string GenreStr
    {
        get
        {
            return string.Join(", ", GenresInProjects.Select(x => x.Genre?.Name));
        }
    }
}
