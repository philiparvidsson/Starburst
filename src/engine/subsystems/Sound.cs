using Fab5.Engine.Components;
using Fab5.Engine.Core;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Fab5.Engine.Subsystems
{
    public class Sound : Subsystem
    {
        public override void draw(float t, float dt)
        {

            var entities = Fab5_Game.inst().get_entities_fast(typeof(SoundLibrary));
            int num_components = entities.Count;

            for (int i = 0; i < num_components; i++)
            {
                var entity = entities[i];
                var music = entity.get_component<SoundLibrary>();
                if (!music.IsSongStarted)
                {
                    var bmusic = music.Library.ElementAt(music.song_index).Value as BackgroundMusic;
                    if (bmusic != null) {
                        MediaPlayer.Play(bmusic.BackSong);
                        MediaPlayer.IsRepeating = bmusic.IsRepeat;
                        music.NowPlayingIndex = 0;
                        music.IsSongStarted = true;
                    }


                }
            }

            if (fading_in) {

                fade_cur += fade_val*dt;
                if (fade_cur > fade_vol) {
                    fade_cur = fade_vol;
                    fading_in = false;
                }
                MediaPlayer.Volume = (float)Math.Max(0.0f, Math.Min(fade_cur, 1.0f));
            }
            else if (fading_out) {
                fade_cur += fade_val*dt;
                if (fade_cur < fade_vol) {
                    fade_cur = fade_vol;
                    fading_out = false;
                }
                //System.Console.WriteLine("faded down to " + fade_cur + ", " + fade_val);
                MediaPlayer.Volume = (float)Math.Max(0.0f, Math.Min(fade_cur, 1.0f));
            }
        }
        public override void cleanup()
        {
            //MediaPlayer.Stop();
        }

        private Dictionary<string, string> soundlib = new Dictionary<string, string>() {
            { "begin_game", "sound/effects/air_horn" },
            { "menu_click", "sound/effects/click" }
        };

        bool fading_out = false;
        bool fading_in = false;
        float fade_cur = 0.0f;
        float fade_val = 0.0f;
        float fade_vol = 0.0f;
        private void music_fade_out(float t, float vol=0.0f) {
            if (fading_out) {
                return;
            }
            fading_in = false;
            fading_out = true;

            fade_vol = vol;
            fade_cur = MediaPlayer.Volume;

            fade_val = (fade_vol-fade_cur)/t; // lerp val
        }

        private void music_fade_in(float t, float vol=0.7f) {
            if (fading_in) {
                return;
            }
            fading_in = true;
            fading_out = false;

            fade_vol = vol;
            fade_cur = MediaPlayer.Volume;
            fade_val = (fade_vol-fade_cur)/t; // lerp val
        }

        public override void on_message(string msg, dynamic data)
        {
            // @To-do: TOBIAS, KOLLA OM MEDDELANDET ÄR TILL DIG INNAN DU LOOPAR!! :-)

            if (msg == "play_sound") {
                var asset = data.name;
                if (soundlib.ContainsKey(asset)) {
                    // map name to asset file
                    asset = soundlib[asset];
                }

                var sound_effect = Fab5_Game.inst().get_content<SoundEffect>(asset);
                sound_effect.Play();
                return;
            }
            else if (msg == "play_song") {
                var asset = data.name;
                if (soundlib.ContainsKey(asset)) {
                    // map name to asset file
                    asset = soundlib[asset];
                }

                var song = Fab5_Game.inst().get_content<SoundEffect>(asset);
                music_fade_out(1.0f);
                song.Play();

                Fab5_Game.inst().create_entity(new Component[] {
                    new TTL {
                        max_time = data.fade_time,
                        destroy_cb = () => {
                            music_fade_in(1.0f);
                        }
                    }
                });
            }

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
                        var gp_index = (int)data.gp_index;
                        var thrust = lib.Library["thrust"] as Fab5SoundEffect;
                        if (thrust.SoundEffectIns.ContainsKey(gp_index))
                        {
                            if (msg == "throttle" && !thrust.IsStarted[gp_index])
                            {
                                thrust.SoundEffectIns[gp_index].IsLooped = true;
                                thrust.IsStarted[gp_index] = true;
                                thrust.SoundEffectIns[gp_index].Play();
                            }
                            else if (msg == "nothrottle" && thrust.IsStarted[gp_index])
                            {
                                thrust.IsStarted[gp_index] = false;
                                thrust.SoundEffectIns[gp_index].Stop();
                            }
                        }
                        else
                        {
                            var ins = thrust.SoundEffect.CreateInstance();
                            ins.Play();
                            thrust.IsStarted[gp_index] = true;
                            thrust.SoundEffectIns.Add(gp_index, ins);
                        }
                    }
                    if (msg == "fire")
                    {
                        effect = lib.Library[data.sound] as Fab5SoundEffect;
                        effect.SoundEffect.Play();
                    }

                    if (msg == "collision")
                    {
                        string texttureName1 = null;
                        string texttureName2 = null;
                        if (data.entity1 != null)
                        {
                            texttureName1 = data.entity1.get_component<Sprite>().texture.Name;
                            //System.Console.WriteLine(texttureName1);
                        }
                        if (data.entity2 != null)
                        {
                            texttureName2 = data.entity2.get_component<Sprite>().texture.Name;
                            //System.Console.WriteLine(texttureName2);
                        }
                        if (!string.IsNullOrEmpty(texttureName1) && !string.IsNullOrEmpty(texttureName2))
                        {
                            Velocity velo = data.entity1.get_component<Velocity>();
                            Velocity velo2 = data.entity2.get_component<Velocity>();
                            var speed = Math.Sqrt(Math.Pow(velo.x, 2) + Math.Pow(velo.y, 2));
                            var speed2 = Math.Sqrt(Math.Pow(velo2.x, 2) + Math.Pow(velo2.y, 2));
                            var coolspeed = speed - speed2 * ((velo.x * velo.x + velo.y * velo2.y) / (speed * speed2));
                            //Console.WriteLine("Coolspeed " + coolspeed);
                            if (coolspeed > 15.0f)
                            {
                                if (texttureName1.Contains("ship") && texttureName2.Contains("ship"))

                                    effect = lib.Library["bang"] as Fab5SoundEffect;
                                if ((DateTime.Now - effect.LastPlayed).Seconds > 0.1)
                                {
                                    effect.SoundEffect.Play();
                                    effect.LastPlayed = DateTime.Now;
                                }

                            }
                            if ((texttureName1.Contains("asteroid") && texttureName2.Contains("ship")) || (texttureName2.Contains("asteroid") && texttureName1.Contains("ship")))
                            {
                                effect = lib.Library["rockslide_small"] as Fab5SoundEffect;
                                if ((DateTime.Now - effect.LastPlayed).Seconds > 0.2)
                                {
                                    effect.SoundEffect.Play();
                                    effect.LastPlayed = DateTime.Now;
                                }
                            }
                            if ((texttureName1.Contains("ship") && texttureName2 == "soccerball") || (texttureName1 == "soccerball" && texttureName2.Contains("ship")))
                            {

                                effect = lib.Library["punch"] as Fab5SoundEffect;
                                if ((DateTime.Now - effect.LastPlayed).Seconds > 0.2)
                                {
                                    effect.SoundEffect.Play();
                                    effect.LastPlayed = DateTime.Now;
                                }
                            }
                            if ((texttureName1.Contains("goal") && texttureName2 == "soccerball") || (texttureName1 == "soccerball" && texttureName2.Contains("goal")))
                            {
                                effect = lib.Library["Cheering"] as Fab5SoundEffect;
                                effect.SoundEffect.Play();
                            }
                        }
                        else if (!string.IsNullOrEmpty(texttureName1))
                        {
                            Velocity velo = data.entity1.get_component<Velocity>();
                            var speed = Math.Sqrt(Math.Pow(velo.x, 2) + Math.Pow(velo.y, 2));
                            //Console.WriteLine(speed);
                            if(speed> 50) {
                                effect = lib.Library["bang2"] as Fab5SoundEffect;
                                if ((DateTime.Now - effect.LastPlayed).Seconds > 0.2)
                                {
                                    effect.SoundEffect.Play();
                                    effect.LastPlayed = DateTime.Now;
                                }
                            }
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
