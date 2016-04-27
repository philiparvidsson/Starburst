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
            backlib.Library = new List<Component>() {
                        new BackgroundMusic("sound/ContactLoop", true),
                        new BackgroundMusic("sound/SpaceLoungeLoop", true),
                        new BackgroundMusic("sound/SpaceCube", true)
            };
            return (Component)backlib;

        }
         public static Component create_soundeffects_component() {
            var effectlib = new SoundLibrary();
            effectlib.Library = new List<Component>() {
                new Fab5SoundEffect("sound/effects/Minigun","minigun"),
                new Fab5SoundEffect("sound/effects/bang1","bang"),
                new Fab5SoundEffect("sound/effects/bang2","bang2"),
                new Fab5SoundEffect("sound/effects/rockslide_smallcombo","rockslide_small"),
                new Fab5SoundEffect("sound/effects/Smashing","Smashing"),
                new Fab5SoundEffect("sound/effects/missile_impact","missile_impact"),
                new Fab5SoundEffect("sound/effects/Cheering","Cheering"),
                new Fab5SoundEffect("sound/effects/BatmanPunch","punch"),
                new Fab5SoundEffect("sound/effects/throw_knife","throw_knife"),
                new Fab5SoundEffect("sound/effects/glassbreak","glassbreak"),
                new Fab5SoundEffect("sound/effects/SharpPunch","SharpPunch"),
                new Fab5SoundEffect("sound/effects/UI_Misc10","UI_Misc10"),
                new Fab5SoundEffect("sound/effects/UI_Misc11","UI_Misc11"),
                new Fab5SoundEffect("sound/effects/UI_Misc12","UI_Misc12"),
                new Fab5SoundEffect("sound/effects/UI_Misc13","UI_Misc13"),
                new Fab5SoundEffect("sound/effects/UI_Misc14","UI_Misc14"),
                new Fab5SoundEffect("sound/effects/UI_Misc15","UI_Misc15"),
                new Fab5SoundEffect("sound/effects/UI_Misc16","UI_Misc16"),
                new Fab5SoundEffect("sound/effects/UI_Misc17","UI_Misc17"),
                new Fab5SoundEffect("sound/effects/UI_Misc18","UI_Misc18"),
                new Fab5SoundEffect("sound/effects/UI_Misc19","UI_Misc19"),
                new Fab5SoundEffect("sound/effects/UI_Misc20","UI_Misc20"),
                new Fab5SoundEffect("sound/effects/UI_Misc21","UI_Misc21"),
                new Fab5SoundEffect("sound/effects/UI_Misc22","UI_Misc22"),
                new Fab5SoundEffect("sound/effects/UI_Misc23","UI_Misc23")
            };
            return (Component)effectlib;
        }
    }
}
