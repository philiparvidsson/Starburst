namespace Fab5.Starburst.States.Playing.Entities
{

    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Fab5.Engine;
    using Microsoft.Xna.Framework.Graphics;


    public static class EnergyBar
    {
        public static Component[] create_components()
        {
            Texture2D temp = Starburst.inst().get_content<Texture2D>("EnergyBar");

            return new Component[]
            {
                new Hud_Component(2, 1),
                new Sprite()
                {
                    texture = temp
                }
            };
        }
    }
}
