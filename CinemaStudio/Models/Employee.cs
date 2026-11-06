using System;
using System.Collections.Generic;

namespace CinemaStudio.Models;

public partial class Employee
{
    public int Id { get; set; }

    public string Surname { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Patronymic { get; set; }

    public int SpecializationId { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public double Fees { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public byte[]? Photo { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<FilmGroup> FilmGroups { get; set; } = new List<FilmGroup>();

    public virtual Specialization Specialization { get; set; } = null!;

    public virtual Status? StatusNavigation { get; set; }

    public string FullName
    {
        get
        {
            if (Patronymic == "" || Patronymic == null)
                return Surname + " " + Name[0] + ".";
            else
                return Surname + " " + Name[0] + ". " + Patronymic[0] + ".";
        }
    }
}
