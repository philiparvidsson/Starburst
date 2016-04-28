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

            var entities = Fab5_Game.inst().get_entities_fast(typeof(SoundLibrary));
            int num_components = entities.Count;

            for (int i = 0; i < num_components; i++)
            {
                var entity = entities[i];
                var music = entity.get_component<SoundLibrary>();
                if (!music.IsSongStarted)
                {
                    var bmusic = music.Library.ElementAt(0).Value as BackgroundMusic;
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

            var entities = Fab5_Game.inst().get_entities_fast(typeof(SoundLibrary));
            int num_components = entities.Count;
            for (int i = 0; i < num_components; i++)
            {
                var entity = entities[i];
                var lib = entity.get_component<SoundLibrary>();
                var effect = lib.Library.FirstOrDefault().Value as Fab5SoundEffect;
                var music = lib.Library.FirstOrDefault().Value as BackgroundMusic;
                if (effect != null)
                {
                    if (msg == "throttle" || msg == "nothrottle")
                    {
                        var playerid = (int)data.player;
                        var thrust = lib.Library["thrust"] as Fab5SoundEffect;
                        if (thrust.SoundEffectIns.ContainsKey(playerid))
                        {
                            if (msg == "throttle" && !thrust.IsStarted[playerid])
                            {
                                thrust.SoundEffectIns[playerid].IsLooped = true;
                                thrust.IsStarted[playerid] = true;
                                thrust.SoundEffectIns[playerid].Play();
                            }
                            else if (msg == "nothrottle" && thrust.IsStarted[playerid])
                            {
                                thrust.IsStarted[playerid] = false;
                                thrust.SoundEffectIns[playerid].Stop();
                            }
                        }
                        else
                        {
                            var ins = thrust.SoundEffect.CreateInstance();
                            ins.Play();
                            thrust.IsStarted[playerid] = true;
                            thrust.SoundEffectIns.Add(playerid, ins);
                        }
                    }
                    if (msg == "fire")
                    {
                        effect = lib.Library[data.sound].SoundEffect.Play();
                    }

                    if (msg == "collision")
                    {
                        string texttureName1 = null;
                        string texttureName2 = null;
                        if (data.entity1 != null) {
                            texttureName1 = data.entity1.get_component<Sprite>().texture.Name;
                            //System.Console.WriteLine(texttureName1);
                        }
                        if (data.entity2 != null) {
                            texttureName2 = data.entity2.get_component<Sprite>().texture.Name;
                            //System.Console.WriteLine(texttureName2);
                        }
                        if (!string.IsNullOrEmpty(texttureName1) && !string.IsNullOrEmpty(texttureName2))
                        {
                            if (texttureName1.Contains("ship") && texttureName2.Contains("ship"))
                            {
                                effect = lib.Library["bang"] as Fab5SoundEffect;
                                effect.SoundEffect.Play();
                            }
                            if ((texttureName1.Contains("asteroid") && texttureName2.Contains("ship")) || (texttureName2.Contains("asteroid") && texttureName1.Contains("ship")))
                            {
                                effect = lib.Library["rockslide_small"] as Fab5SoundEffect;
                                effect.SoundEffect.Play();
                            }
                            if((texttureName1.Contains("ship") && texttureName2 == "soccerball") || (texttureName1 == "soccerball" && texttureName2.Contains("ship")))
                            {
                                effect = lib.Library["punch"] as Fab5SoundEffect;
                                effect.SoundEffect.Play();
                            }
                            if ((texttureName1.Contains("goal") && texttureName2 == "soccerball") || (texttureName1 == "soccerball" && texttureName2.Contains("goal")))
                            {
                                effect = lib.Library["Cheering"] as Fab5SoundEffect;
                                effect.SoundEffect.Play();
                            }
                        }
                        else if (!string.IsNullOrEmpty(texttureName1))
                        {
                            effect = lib.Library["bang2"] as Fab5SoundEffect;
                            effect.SoundEffect.Play();
                        }
                    }
                }
                if (music != null)
                {
                    var timesince = DateTime.Now - lib.LastChanged;
                    if (msg == "songchanged" && timesince.Seconds > 0.1)
                    {
                        MediaPlayer.Stop();
                        lib.NowPlayingIndex++;
                        if (lib.NowPlayingIndex == lib.Library.Count)
                            lib.NowPlayingIndex = 0;
                            music = lib.Library.ElementAt(lib.NowPlayingIndex).Value as BackgroundMusic;
                            Console.WriteLine("song changed to" + music.File);
                            MediaPlayer.Play(music.BackSong);
                            MediaPlayer.IsRepeating = music.IsRepeat;
                            lib.IsSongStarted = true;
                            lib.LastChanged = DateTime.Now;
                        }
                    if (msg == "mute" && timesince.Seconds > 0.1)
                    {
                        if (MediaPlayer.IsMuted)
                            MediaPlayer.IsMuted = false;
                        else
                            MediaPlayer.IsMuted = true;
                        lib.LastChanged = DateTime.Now;
                    }
                }
            }
        }
    }
}
