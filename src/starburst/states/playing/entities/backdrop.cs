namespace Fab5.Starburst.States.Playing.Entities
{
    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Fab5.Engine;

    using Microsoft.Xna.Framework.Graphics;

    public static class Back_drop
    {

        public static Component[] create_components()
        {
            return new Component[]
            {
                new Backdrop() {
                    backdrop = Starburst.inst().get_content<Texture2D>("backdrops/backdrop1"),
                    stardrop = Starburst.inst().get_content<Texture2D>("backdrops/stardrop")
                }
            };
        }
    }
}
