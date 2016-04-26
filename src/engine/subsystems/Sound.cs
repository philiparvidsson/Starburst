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
                typeof(BackgroundMusicLibrary),
                typeof(Fab5SoundEffect));

            for (int i = 0; i < num_components; i++)
            {
                var entity = entities[i];
                var music = entity.get_component<BackgroundMusicLibrary>();
                if (!music.IsSongStarted)
                {
                    MediaPlayer.Play(music.Library.FirstOrDefault().BackSong);
                    MediaPlayer.IsRepeating = music.Library.FirstOrDefault().IsRepeat;
                    music.NowPlayingIndex = 0;
                    music.IsSongStarted = true;
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
            typeof(BackgroundMusicLibrary),
            typeof(Fab5SoundEffect));

            if (msg == "Fire")
            {
                for (int i = 0; i < num_components; i++)
                {
                    var entity = entities[i];
                    var effect = entity.get_component<Fab5SoundEffect>();

                    if (effect.Desc == data.Weapon)
                        effect.SoundEffect.CreateInstance().Play();
                }
            }

            if (msg == "ChangeBackSong")
            {
                for (int i = 0; i < num_components; i++)
                {
                    var entity = entities[i];
                    var music = entity.get_component<BackgroundMusicLibrary>();
                    var timesince = DateTime.Now - music.LastChanged;
                    if (timesince.Seconds > 0.2)
                    {
                        MediaPlayer.Stop();
                        music.NowPlayingIndex++;
                        if (music.NowPlayingIndex == music.Library.Count)
                            music.NowPlayingIndex = 0;
                        var song = music.Library.ElementAt(music.NowPlayingIndex);
                        Console.WriteLine("song changed to" + song.File);

                        MediaPlayer.Play(song.BackSong);
                        MediaPlayer.IsRepeating = song.IsRepeat;
                        music.IsSongStarted = true;
                        music.LastChanged = DateTime.Now;
                    }
                }
            }
            if (msg == "Mute"){
                if (MediaPlayer.IsMuted)
                    MediaPlayer.IsMuted = false;
                else
                    MediaPlayer.IsMuted = true;
            }

        }

    }
}
