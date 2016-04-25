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

                var gameinst = Fab5_Game.inst();
                var keypressed = gameinst.MessagesQueue.Where(x => x.EventType == "KeyPressed").ToList();
                foreach(var item in keypressed)
                {
                    if(item.EventName == "Fire"){
                        var effect = entity.get_component<Fab5SoundEffect>();
                        var ins = effect.SoundEffect.CreateInstance();
                        ins.Play();
                    }
                    var timesince =  DateTime.Now- music.LastChanged;
                    if(item.EventName == "ChangeBackSong" && timesince.Seconds > 0.2)
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
        }
        public override void cleanup()
        {
            MediaPlayer.Stop();
        }
    }
}
