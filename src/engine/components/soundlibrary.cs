using System;
using System.Collections.Generic;
using Fab5.Engine.Core;
using Microsoft.Xna.Framework.Audio;

namespace Fab5.Engine.Components
{
    public class SoundLibrary : Component
    {
        public SoundLibrary()
        {
            ActiveSoundIns = new Dictionary<string, ActiveSound>();
            Library = new Dictionary<string, Component>();
            LastChanged = DateTime.Now;
        }
        public int song_index;
        public int NowPlayingIndex { get; set; }
        public bool IsSongStarted { get; set; }
        public Dictionary<string, Component> Library { get; set; }
        public DateTime LastChanged { get; set; }
        public Dictionary<string, ActiveSound> ActiveSoundIns { get; set; }
    }
    public class ActiveSound
    {
        public SoundEffectInstance SoundEffectIns { get; set; }
        public AudioEmitter Emitter { get; set; }
    }
}
