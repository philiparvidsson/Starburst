namespace Fab5.Starburst.States.Playing.Entities
{
    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Fab5.Engine;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SoundManager
    {
        public static Component create_backmusic_component() {
            var backlib = new SoundLibrary();
            backlib.Library = new Dictionary<string, Component>();
            backlib.Library.Add("Battle", new BackgroundMusic("sound/music/BoxCat_Games_-_25_-_Victory", true));

            backlib.Library.Add("Menu", new BackgroundMusic("sound/music/BoxCat_Games_-_10_-_Epic_Song", true));
            //backlib.Library.Add("SpaceLoungeLoop", new BackgroundMusic("sound/SpaceLoungeLoop", true));
            //backlib.Library.Add("SpaceCube", new BackgroundMusic("sound/SpaceCube", true));
            return (Component)backlib;

        }
         public static Component create_soundeffects_component() {
            var effectlib = new SoundLibrary();
            effectlib.Library = new Dictionary<string, Component>();

            effectlib.Library.Add("rockslide_small", new Fab5SoundEffect("sound/effects/rockslide_smallcombo", "rockslide_small"));
            effectlib.Library.Add("thrust", new Fab5SoundEffect("sound/effects/thrust", "thrust"));
            effectlib.Library.Add("bang", new Fab5SoundEffect("sound/effects/bang1", "bang"));
            effectlib.Library.Add("bang2", new Fab5SoundEffect("sound/effects/bang2", "bang2"));
            effectlib.Library.Add("BatmanPunch", new Fab5SoundEffect("sound/effects/BatmanPunch", "BatmanPunch"));
            effectlib.Library.Add("LaserBlaster", new Fab5SoundEffect("sound/effects/LaserBlaster", "LaserBlaster"));
            effectlib.Library.Add("LaserBlaster2", new Fab5SoundEffect("sound/effects/LaserBlaster2", "LaserBlaster2"));
            effectlib.Library.Add("menu_click", new Fab5SoundEffect("sound/effects/click", "menu_click"));
            effectlib.Library.Add("begin_game", new Fab5SoundEffect("sound/effects/air_horn", "begin_game"));
            effectlib.Library.Add("menu_positive", new Fab5SoundEffect("sound/effects/menu_positive", "menu_positive"));
            effectlib.Library.Add("knock1m", new Fab5SoundEffect("sound/effects/knock1m", "knock1m"));
            effectlib.Library.Add("laser_impact", new Fab5SoundEffect("sound/effects/laser_impact", "laser_impact"));
            effectlib.Library.Add("small_explosion", new Fab5SoundEffect("sound/effects/small_explosion", "small_explosion"));
            effectlib.Library.Add("pickup", new Fab5SoundEffect("sound/effects/pickup", "pickup"));
            effectlib.Library.Add("turret", new Fab5SoundEffect("sound/effects/turret", "turret"));
            effectlib.Library.Add("spawn", new Fab5SoundEffect("sound/effects/spawn", "spawn"));
            effectlib.Library.Add("use_powerup", new Fab5SoundEffect("sound/effects/userpowerup", "use_powerup"));
            effectlib.Library.Add("nextpowerup", new Fab5SoundEffect("sound/effects/nextpowerup", "nextpowerup"));
            effectlib.Library.Add("explosion", new Fab5SoundEffect("sound/effects/explosion", "explosion"));


            return (Component)effectlib;
        }
    }
}
