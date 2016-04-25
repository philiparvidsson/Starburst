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
        public static Component[] create_components() {
            var backlib = new BackgroundMusicLibrary();
            backlib.Library = new List<BackgroundMusic>() {
                        new BackgroundMusic("sound/ContactLoop", true),
                        new BackgroundMusic("sound/SpaceLoungeLoop", true),
                        new BackgroundMusic("sound/SpaceCube", true)
            };
            return new Component[]{
                new Fab5SoundEffect("sound/effects/Minigun"),
                (Component)backlib
            };
        }
    }
}
