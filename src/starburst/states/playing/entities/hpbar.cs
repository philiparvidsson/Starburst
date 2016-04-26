namespace Fab5.Starburst.States.Playing.Entities
{

    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Fab5.Engine;
    using Microsoft.Xna.Framework.Graphics;


    public static class HPBar
    {
        public static Component[] create_components()
        {
            Texture2D temp = Starburst.inst().get_content<Texture2D>("HPBar");

            return new Component[]
            {
                new Hud_Component(1, 1),
                new Sprite()
                {
                    texture = temp
                }
            };
        }
    }
}
