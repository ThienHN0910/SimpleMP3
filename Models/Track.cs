using System;
using System.Collections.Generic;

namespace SimpleMP3.Models;

public partial class Track
{
    public int Id { get; set; }

    public string YouTubeId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Album { get; set; }

    public int? DurationSeconds { get; set; }

    public string? FilePath { get; set; }

    public int? ArtistId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Artist? Artist { get; set; }

    public virtual ICollection<PlayHistory> PlayHistories { get; set; } = new List<PlayHistory>();

    public virtual ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
}
