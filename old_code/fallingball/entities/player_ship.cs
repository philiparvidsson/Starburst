namespace Fab5.Starburst.States.Falling_Ball.Entities
{
    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Fab5.Engine;

    using Microsoft.Xna.Framework.Graphics;

    public static class Player_Ship
    {

        public static Component[] create_components()
        {
            return new Component[]
            {
                new Inputhandler(),
                new Angle() { angle = 0 },
                new Position() {x = 300, y = 200 },
                new Velocity() {x = 0.0f, y = 0.0f },
                new Sprite()
                {
                    texture = Starburst.inst().get_content<Texture2D>("ball")
                }
            };
        }
    }
}
