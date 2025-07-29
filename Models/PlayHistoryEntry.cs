using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class PlayHistoryEntry
    {
        public uint UserId { get; set; }
        public uint TrackId { get; set; }
        public float Label { get; set; } 
    }
}
