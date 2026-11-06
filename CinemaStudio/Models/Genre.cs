using System;
using System.Collections.Generic;

namespace CinemaStudio.Models;

public partial class Genre
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<GenresInProject> GenresInProjects { get; set; } = new List<GenresInProject>();
}
