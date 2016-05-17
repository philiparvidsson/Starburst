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
        private static Random rand = new Random();
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
                    if (bmusic != null)
                    {
                        MediaPlayer.Play(bmusic.BackSong);
                        MediaPlayer.IsRepeating = bmusic.IsRepeat;
                        music.NowPlayingIndex = 0;
                        music.IsSongStarted = true;
                    }


                }
            }

            if (fading_in)
            {

                fade_cur += fade_val * dt;
                if (fade_cur > fade_vol)
                {
                    fade_cur = fade_vol;
                    fading_in = false;
                }
                MediaPlayer.Volume = (float)Math.Max(0.0f, Math.Min(fade_cur, 1.0f));
            }
            else if (fading_out)
            {
                fade_cur += fade_val * dt;
                if (fade_cur < fade_vol)
                {
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

        bool fading_out = false;
        bool fading_in = false;
        float fade_cur = 0.0f;
        float fade_val = 0.0f;
        float fade_vol = 0.0f;
        private void music_fade_out(float t, float vol = 0.0f)
        {
            if (fading_out)
            {
                return;
            }
            fading_in = false;
            fading_out = true;

            fade_vol = vol;
            fade_cur = MediaPlayer.Volume;

            fade_val = (fade_vol - fade_cur) / t; // lerp val
        }

        private void music_fade_in(float t, float vol = 0.7f)
        {
            if (fading_in)
            {
                return;
            }
            fading_in = true;
            fading_out = false;

            fade_vol = vol;
            fade_cur = MediaPlayer.Volume;
            fade_val = (fade_vol - fade_cur) / t; // lerp val
        }

        private Dictionary<string, string> soundlib = new Dictionary<string, string>() {
            { "begin_game", "sound/effects/air_horn" },
            { "menu_click", "sound/effects/click" },
            { "menu_positive", "sound/effects/menu_positive" },
            { "menu_negative", "sound/effects/menu_negative" }
        };

        public override void on_message(string msg, dynamic data)
        {
            //var volume = 1.0;
            if (msg == "play_sound_asset")
            {
                string asset = data.name;

                if (soundlib.ContainsKey(asset))
                {
                    // map name to asset file
                    asset = soundlib[asset];
                }

                //Console.WriteLine("playing " + asset);

                var varying_pitchprop = data.GetType().GetProperty("varying_pitch");
                float pitchval = 0.0f;
                bool varying_pitch = false;
                if (varying_pitchprop != null)
                    varying_pitch = varying_pitchprop;

                var sound_effect = Fab5_Game.inst().get_content<SoundEffect>(asset);
                if (varying_pitch)
                    pitchval = 0.2f * (float)Math.Sign((float)rand.NextDouble() - 0.5f) * (float)Math.Pow(rand.NextDouble(), 3.0f);
                var vol = 1.0f;

                if (data.GetType().GetProperty("pos") != null) {
                    vol = ShouldBePlayed((Position)data.pos);
                }

                sound_effect.Play(volume: vol, pitch: (float)(pitchval), pan: 0);
                return;
            }
            else if (msg == "play_song_asset")
            {
                var asset = data.name;
                if (soundlib.ContainsKey(asset))
                {
                    // map name to asset file
                    asset = soundlib[asset];
                }

                var song = Fab5_Game.inst().get_content<SoundEffect>(asset);
                music_fade_out(1.2f);
                song.Play();

                Fab5_Game.inst().create_entity(new Component[] {
                    new TTL {
                        max_time = data.fade_time,
                        destroy_cb = () => {
                            music_fade_in(1.2f);
                        }
                    }
                });
            }
            else if (msg == "weapon_fired" || msg == "turretsound" || msg=="spawn")
            {
                var p1 = data.entity1.get_component<Position>();
                var vol = ShouldBePlayed(p1);
                if (vol > 0)
                    Fab5_Game.inst().message("play_sound", new { name = data.name, volume = vol, varying_pitch = data.varying_pitch });
                else
                    return;
            }
            // ifsatserna ovanför är temporära tills tobbe implementerar något så vi kan spela upp ljud när vi vill :-)
            else if (msg == "collision")
            {
                var p1 = data.entity1.get_component<Position>();
                var vol = ShouldBePlayed(p1);
                if (vol != 0)
                {

                    if (data.entity2 == null)
                    {
                        var e1 = data.entity1;
                        Velocity velo = e1.get_component<Velocity>();
                        var speed = Math.Sqrt(Math.Pow(velo.x, 2) + Math.Pow(velo.y, 2));
                        var sprite1 = data.entity1.get_component<Sprite>();
                        if (sprite1 != null)
                        {
                            var texttureName = sprite1.texture.Name;
                            if (texttureName.Contains("ship") && speed > 27)
                                Fab5_Game.inst().message("play_sound", new { pos = p1, name = "bang2", volume = vol , entity1=data.entity1, timedependent= true});
                            else if (texttureName.Contains("beams1"))
                                Fab5_Game.inst().message("play_sound", new { name = "laser_impact", pos = p1, volume = vol });
                            else if (texttureName.Contains("beams2"))
                                Fab5_Game.inst().message("play_sound", new { name = "knock1m", pos = p1, volume = vol });
                        }
                    }
                    else
                    {

                        var e1 = data.entity1;
                        var e2 = data.entity2;

                        //Decide which soound to play based on speeed
                        Velocity velo = e1.get_component<Velocity>();
                        Velocity velo2 = e2.get_component<Velocity>();

                        var sprite1 = data.entity1.get_component<Sprite>();
                        var sprite2 = data.entity2.get_component<Sprite>();

                        if (sprite1 == null || sprite2 == null)
                            return;

                        var texttureName = sprite1.texture.Name;
                        var texttureName2 = sprite2.texture.Name;

                        var speed = Math.Sqrt(Math.Pow(velo.x, 2) + Math.Pow(velo.y, 2));
                        var speed2 = Math.Sqrt(Math.Pow(velo2.x, 2) + Math.Pow(velo2.y, 2));
                        var colspeed = (float)Math.Abs(speed - speed2 * ((velo.x * velo2.x + velo.y * velo2.y) / (speed * speed2)));

                        if (colspeed > 30)
                        {
                            if ((texttureName.Contains("ship") && texttureName2 == "soccerball") || (texttureName == "soccerball" && texttureName2.Contains("ship")))
                                if (texttureName.Contains("ship"))
                                {
                                    Fab5_Game.inst().message("play_sound", new { name = "BatmanPunch", pos = p1, volume = vol, entity1 = data.entity1, timedependent = true, });
                                }
                                else {
                                    Fab5_Game.inst().message("play_sound", new { name = "BatmanPunch", pos = p1, volume = vol, entity1 = data.entity2, timedependent = true, });
                                }
                            else if ((texttureName.Contains("asteroid") && texttureName2.Contains("ship")) || (texttureName2.Contains("asteroid") && texttureName.Contains("ship")))
                            {
                                if(texttureName.Contains("ship")){
                                    Fab5_Game.inst().message("play_sound", new { name = "rockslide_small", pos = p1, volume = vol, entity1 = data.entity1, timedependent = true, });
                                }
                                else { 
                                    Fab5_Game.inst().message("play_sound", new { name = "rockslide_small", pos = p1, volume = vol, entity1 = data.entity2, timedependent = true, });
                                }
                            }
                            else if (texttureName.Contains("ship") && texttureName2.Contains("ship"))
                                Fab5_Game.inst().message("play_sound", new { name = "bang", pos = p1, volume = vol,timedependent = true , entity1 = data.entity1 });
                        }

                        if (texttureName.Contains("beams1") && texttureName2.Contains("asteroid") || texttureName.Contains("asteroid") && texttureName2.Contains("beams1"))
                            Fab5_Game.inst().message("play_sound", new { name = "laser_impact", pos = p1, volume = vol });
                        else if (texttureName.Contains("beams2") && texttureName2.Contains("asteroid") || texttureName.Contains("asteroid") && texttureName2.Contains("beams2"))
                            Fab5_Game.inst().message("play_sound", new { name = "small_explosion", pos = p1, volume = vol });
                        else if (texttureName.Contains("beams1") && texttureName2.Contains("ship") || texttureName.Contains("ship") && texttureName2.Contains("beams1"))
                            Fab5_Game.inst().message("play_sound", new { name = "laser_impact", pos = p1, volume = vol });
                        else if (texttureName.Contains("beams2") && texttureName2.Contains("ship") || texttureName.Contains("ship") && texttureName2.Contains("beams2"))
                            Fab5_Game.inst().message("play_sound", new { name = "small_explosion", pos = p1, volume = vol });
                        else if (texttureName.Contains("powerup") && texttureName2.Contains("ship") || texttureName.Contains("ship") && texttureName2.Contains("powerup"))
                            Fab5_Game.inst().message("play_sound", new { name = "pickup", pos = p1, volume = vol });
                    }
                }
            }
            else if (msg == "play_sound")
            {
                var varying_pitchprop = data.GetType().GetProperty("varying_pitch");
                float pitchval = 0.0f;
                bool varying_pitch = false;
                if (varying_pitchprop != null)
                    varying_pitch = true;
                bool timedependent = false;
                var timedependentprop = data.GetType().GetProperty("timedependent");
                if (timedependentprop != null)
                    timedependent = true;

                var entities = Fab5_Game.inst().get_entities_fast(typeof(SoundLibrary));
                int num_components = entities.Count;
                for (int i = 0; i < num_components; i++)
                {
                    var entity = entities[i];
                    var lib = entity.get_component<SoundLibrary>();
                    var effect = lib.Library.FirstOrDefault().Value as Fab5SoundEffect;
                    var music = lib.Library.FirstOrDefault().Value as BackgroundMusic;
                    if (effect != null && lib.Library.ContainsKey(data.name))
                    {
                        var gp_index = data.GetType().GetProperty("gp_index");
                        effect = lib.Library[data.name] as Fab5SoundEffect;
                        //thrust uses this
                        if (gp_index != null)
                        {
                            if (!lib.ActiveSoundIns.ContainsKey(data.name + data.gp_index))
                            {
                                var ins = effect.SoundEffect.CreateInstance();
                                ins.Play();
                                ins.Volume = 0.4f;
                                lib.ActiveSoundIns.Add(data.name + data.gp_index, new ActiveSound() { SoundEffectIns = ins });
                            }
                            else
                            {
                                if ((DateTime.Now - effect.LastPlayed).Seconds > 0.5)
                                {
                                    var active = lib.ActiveSoundIns[data.name + data.gp_index] as ActiveSound;
                                    if (active.SoundEffectIns.State == SoundState.Stopped)
                                    {
                                        active.SoundEffectIns.Play();
                                        effect.LastPlayed = DateTime.Now;
                                    }
                                }
                            }
                        }
                        else
                        {
                            Entity e1 = null;
                            //if timedependent och nyckeln inte finns.. safe att spela.. och om timedependent och ljudet spelades för mer än 0.2 sec
                            if (timedependent){ 
                                e1 = data.entity1;
                                if (!effect.LastPLayedDic.ContainsKey(e1.id) || (DateTime.Now - effect.LastPLayedDic[e1.id]).Seconds > 0.2)
                                    effect.LastPLayedDic[e1.id] = DateTime.Now;
                                else
                                    return;
                            }
                            if (varying_pitch)
                                pitchval = 0.2f * (float)Math.Sign((float)rand.NextDouble() - 0.5f) * (float)Math.Pow(rand.NextDouble(), 3.0f);
                            effect.SoundEffect.Play(volume: (float)data.volume,pan:0,pitch:pitchval);
                            
                        }
                     
                    }
                    else if (music != null)
                    {
                        var timesince = DateTime.Now - lib.LastChanged;
                        if (data.name == "change_song" && timesince.Seconds > 0.1)
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
                        if (data.name == "mute" && timesince.Seconds > 0.1)
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
            else if (msg == "stop_sound")
            {
                var entities = Fab5_Game.inst().get_entities_fast(typeof(SoundLibrary));
                int num_components = entities.Count;
                for (int i = 0; i < num_components; i++)
                {
                    var entity = entities[i];
                    var lib = entity.get_component<SoundLibrary>();
                    var effect = lib.Library.FirstOrDefault().Value as Fab5SoundEffect;
                    var music = lib.Library.FirstOrDefault().Value as BackgroundMusic;
                    if (effect != null && lib.Library.ContainsKey(data.name))
                    {
                        effect = lib.Library[data.name] as Fab5SoundEffect;
                        if (data.name == "thrust" && lib.ActiveSoundIns.ContainsKey(data.name + data.gp_index))
                        {
                            var active = lib.ActiveSoundIns[data.name + data.gp_index] as ActiveSound;
                            active.SoundEffectIns.Stop();
                        }
                    }
                }
            }
        }
        public static float ShouldBePlayed(Position p1)
        {
            var min_dist = 99999999.0f;
            foreach (var e in Fab5_Game.inst().get_entities_fast(typeof (Input))) {
                var p = e.get_component<Position>();
                var dx = p.x-p1.x;
                var dy = p.y-p1.y;
                var dist = dx*dx+dy*dy;
                if (dist < min_dist) {
                    min_dist = dist;
                }
            }

            if (min_dist < 0.1f) {
                // probably player making the sound
                return 1.0f;
            }

            var r = (float)Math.Sqrt(min_dist);
            r *= 3.0f/(float)Fab5_Game.inst().GraphicsMgr.PreferredBackBufferWidth;
            return (float)Math.Min(1.0f, 1.0f/(r*r));
        }

    }
}
