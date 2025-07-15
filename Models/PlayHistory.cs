using System;
using System.Collections.Generic;

namespace SimpleMP3.Models;

public partial class PlayHistory
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int TrackId { get; set; }

    public DateTime PlayedAt { get; set; }

    public virtual Track Track { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
