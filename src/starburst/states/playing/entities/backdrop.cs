namespace Fab5.Starburst.States.Playing.Entities
{
    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Fab5.Engine;
    using System;

    using Microsoft.Xna.Framework.Graphics;

    public static class Back_drop
    {

        public static Component[] create_components()
        {
            int i;
            Random random = new Random();
            i = random.Next(1, 7);
            string back = "backdrops/backdrop" +i;

            return new Component[]
            {
                new Backdrop() {
                    backdrop = Starburst.inst().get_content<Texture2D>(back),
                    stardrop = Starburst.inst().get_content<Texture2D>("backdrops/stardrop")
                }
            };
        }
    }
}
