using System;
using System.Collections.Generic;

namespace SimpleMP3.Models;

public partial class Artist
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Bio { get; set; }

    public string? AvatarUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Track> Tracks { get; set; } = new List<Track>();
}
