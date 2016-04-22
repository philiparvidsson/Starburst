using Fab5.Engine.Core;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;

namespace Fab5.Engine.Components
{
    public class BackgroundMusic : Component
    {
        public BackgroundMusic(String file, bool isRepeat)
        {
            File = file;
            IsSongStarted = false;
            IsRepeat = isRepeat;
            BackSong = Fab5_Game.inst().Content.Load<Song>(file);

        }
        public Song BackSong { get; set; }
        public bool IsSongStarted { get; set; }
        public String File { get; set; }
        public bool IsRepeat { get; set; }

    }
}
