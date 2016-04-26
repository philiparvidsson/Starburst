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
            };
            return (Component)effectlib;
        }
    }
}
