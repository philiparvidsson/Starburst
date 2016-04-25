using System;
using System.Collections.Generic;
using Fab5.Engine.Core;


namespace Fab5.Engine.Components
{
    public class BackgroundMusicLibrary : Component
    {
        public BackgroundMusicLibrary()
        {
            Library = new List<BackgroundMusic>();
            LastChanged = DateTime.Now;
        }
        public int NowPlayingIndex { get; set; }
        public bool IsSongStarted { get; set; }
        public List<BackgroundMusic> Library { get; set; }
        public DateTime LastChanged { get; set; }
    }
}
