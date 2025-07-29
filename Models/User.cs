using System;
using System.Collections.Generic;

namespace SimpleMP3.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<PlayHistory> PlayHistories { get; set; } = new List<PlayHistory>();

    public virtual ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();

    public virtual ICollection<Track> Tracks { get; set; } = new List<Track>();
}
