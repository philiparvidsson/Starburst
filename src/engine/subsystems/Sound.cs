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
                typeof(SoundLibrary));

            for (int i = 0; i < num_components; i++)
            {
                var entity = entities[i];
                var music = entity.get_component<SoundLibrary>();
                if (!music.IsSongStarted)
                {
                    var bmusic = music.Library.FirstOrDefault() as BackgroundMusic;
                    if (bmusic != null) { 
                        MediaPlayer.Play(bmusic.BackSong);
                        MediaPlayer.IsRepeating = bmusic.IsRepeat;
                        music.NowPlayingIndex = 0;
                        music.IsSongStarted = true;
                    }
                }
            }
        }
        public override void cleanup()
        {
            MediaPlayer.Stop();
        }
        public override void on_message(string msg, dynamic data)
        {
            int num_components;
            var entities = Fab5_Game.inst().get_entities(out num_components,
            typeof(SoundLibrary));
            for (int i = 0; i < num_components; i++)
            {
                var entity = entities[i];
                var lib = entity.get_component<SoundLibrary>();
                var effect = lib.Library.FirstOrDefault() as Fab5SoundEffect;
                var music = lib.Library.FirstOrDefault() as BackgroundMusic;
                if (effect != null)
                {
                    if (msg == "Fire") { 
                        var timesince = DateTime.Now - lib.LastChanged;
                        if (timesince.Seconds > 0.001)
                        {
                            if (effect.Desc == data.Weapon)
                                effect.SoundEffect.Play();
                            lib.LastChanged = DateTime.Now;
                        }
                    }
                }
                if (music != null)
                {
                    if (msg == "SongChanged")
                    {
                        var timesince = DateTime.Now - lib.LastChanged;
                        if (timesince.Seconds > 0.2)
                        {
                            MediaPlayer.Stop();
                            lib.NowPlayingIndex++;
                            if (lib.NowPlayingIndex == lib.Library.Count)
                                lib.NowPlayingIndex = 0;
                            Console.WriteLine("song changed to" + music.File);
                            MediaPlayer.Play(music.BackSong);
                            MediaPlayer.IsRepeating = music.IsRepeat;
                            lib.IsSongStarted = true;
                            lib.LastChanged = DateTime.Now;
                        }
                    }
                    if (msg == "Mute")
                    {
                        if (MediaPlayer.IsMuted)
                            MediaPlayer.IsMuted = false;
                        else
                            MediaPlayer.IsMuted = true;
                    }
                }   
            }
        }
    }
}
