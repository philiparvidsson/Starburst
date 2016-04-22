using Fab5.Engine.Components;
using Fab5.Engine.Core;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;

namespace Fab5.Engine.Subsystems
{
    public class Sound : Subsystem
    {
        public override void update(float t, float dt)
        {
            int num_components;
            var entities = Fab5_Game.inst().get_entities(out num_components,
                typeof(BackgroundMusic));
                //typeof(Fab5SoundEffect));

            for (int i = 0; i < num_components; i++)
            {
                var entity = entities[i];
                var music = entity.get_component<BackgroundMusic>();
                if (!music.IsSongStarted)
                {
                    music.BackSong = Fab5_Game.inst().Content.Load<Song>(music.File);
                    MediaPlayer.Play(music.BackSong);
                    MediaPlayer.IsRepeating = music.IsRepeat;
                    music.IsSongStarted = true;
                }
         

            }
        }
        public override void cleanup()
        {
            MediaPlayer.Stop();
        }
    }
}
