using System;
using System.Collections.Generic;

namespace CinemaStudio.Models;

public partial class GenresInProject
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public int GenreId { get; set; }

    public virtual Genre Genre { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;
}
