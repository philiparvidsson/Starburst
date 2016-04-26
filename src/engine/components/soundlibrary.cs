using System;
using System.Collections.Generic;
using Fab5.Engine.Core;


namespace Fab5.Engine.Components
{
    public class SoundLibrary : Component
    {
        public SoundLibrary()
        {
            Library = new List<Component>();
            LastChanged = DateTime.Now;
        }
        public int NowPlayingIndex { get; set; }
        public bool IsSongStarted { get; set; }
        public List<Component> Library { get; set; }
        public DateTime LastChanged { get; set; }
    }
}
