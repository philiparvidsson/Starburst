using Fab5.Engine.Components;
using Fab5.Engine.Core;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Linq;

namespace Fab5.Engine.Subsystems
{
    public class Sound : Subsystem
    {
        public override void update(float t, float dt)
        {
            int num_components;
            var entities = Fab5_Game.inst().get_entities(out num_components,
                typeof(BackgroundMusic),
                typeof(Fab5SoundEffect));

            for (int i = 0; i < num_components; i++)
            {
                var entity = entities[i];
                var music = entity.get_component<BackgroundMusic>();
                if (!music.IsSongStarted)
                {
                    MediaPlayer.Play(music.BackSong);
                    MediaPlayer.IsRepeating = music.IsRepeat;
                    music.IsSongStarted = true;
                }
                var gameinst = Fab5_Game.inst();
                var keypressed = gameinst.MessagesQueue.Where(x => x.EventType == "KeyPressed").ToList();
                foreach(var item in keypressed)
                {
                    if(item.EventName == "Fire") {
                        
                        var effect = entity.get_component<Fab5SoundEffect>();
                        var ins = effect.SoundEffect.CreateInstance();
                        ins.Play();
                    }
                }
            }
        }
        public override void cleanup()
        {
            MediaPlayer.Stop();
        }
    }
}
