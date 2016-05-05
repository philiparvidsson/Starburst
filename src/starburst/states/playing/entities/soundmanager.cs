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
            backlib.Library.Add("Battle", new BackgroundMusic("sound/BoxCat_Games_-_03_-_Battle_Special", true));
            backlib.Library.Add("Menu", new BackgroundMusic("sound/BoxCat_Games_-_10_-_Epic_Song", true));
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
            effectlib.Library.Add("minigun", new Fab5SoundEffect("sound/effects/Minigun", "minigun"));
            effectlib.Library.Add("SharpPunch", new Fab5SoundEffect("sound/effects/SharpPunch", "SharpPunch"));
            effectlib.Library.Add("punch", new Fab5SoundEffect("sound/effects/BatmanPunch", "punch"));
            effectlib.Library.Add("throw_knife", new Fab5SoundEffect("sound/effects/throw_knife", "throw_knife"));
            effectlib.Library.Add("LaserBlaster", new Fab5SoundEffect("sound/effects/LaserBlaster", "LaserBlaster"));
            effectlib.Library.Add("LaserBlaster2", new Fab5SoundEffect("sound/effects/LaserBlaster2", "LaserBlaster2"));
            effectlib.Library.Add("menu_click", new Fab5SoundEffect("sound/effects/click", "menu_click"));
            effectlib.Library.Add("begin_game", new Fab5SoundEffect("sound/effects/air_horn", "begin_game"));
            effectlib.Library.Add("menu_positive", new Fab5SoundEffect("sound/effects/menu_positive", "menu_positive"));
            return (Component)effectlib;
        }
    }
}
