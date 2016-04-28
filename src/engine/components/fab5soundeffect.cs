using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fab5.Engine.Core;
using Microsoft.Xna.Framework.Audio;

namespace Fab5.Engine.Components
{
    public class Fab5SoundEffect : Component
    {
        public Fab5SoundEffect(string file, string desc)
        {
            File = file;
            Desc = desc;
            SoundEffectIns = new Dictionary<int, SoundEffectInstance>();
            SoundEffect = Fab5_Game.inst().Content.Load<SoundEffect>(file);
            IsStarted = new bool[10];
        }
        public string Desc { get; set; }
        public string File { get; set; }
        public SoundEffect SoundEffect { get; set; }
        public Dictionary<int, SoundEffectInstance> SoundEffectIns { get; set; }
        public bool[] IsStarted { get; set; }
}
}

