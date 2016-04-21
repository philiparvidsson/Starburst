using Fab5.Engine.Core;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;

namespace Fab5.Engine.Components
{
    public class BackgroundMusic : Component
    {
        public BackgroundMusic(String file)
        {
            File = file;
            IsSongStarted = false;
        }
        public Song BackSong { get; set; }
        public bool IsSongStarted { get; set; }
        public String File { get; set; }
        public bool IsRepeat { get; set; }

    }
}
