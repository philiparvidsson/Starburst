namespace Fab5.Starburst.States.Playing.Entities
{
    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Fab5.Engine;

    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;

    public static class Player_Ship
    {
        static int lol = 1;
        public static Component[] create_components()
        {
            var inputhandler = new Inputhandler();
            if (lol == 2) {
                // this ass code sucks
                inputhandler = new Inputhandler() {
                    left  = Keys.A,
                    right = Keys.D,
                    up    = Keys.W,
                    down  = Keys.S
                };
            }
            string ship = "ships/ship1" + lol;
            lol++;
            return new Component[]
            {
                inputhandler,
                new Angle() { angle = 0 },
                new Position() {x = 300, y = 200 },
                new Velocity() {x = 0.0f, y = 0.0f },
                new Sprite()
                {
                    texture = Starburst.inst().get_content<Texture2D>(ship),
                    color = new Color(0.6f, 0.9f, 1.0f)
                }
,
                new Bounding_Circle() { radius = 20.0f },
                new Mass() { mass = 15.0f }
            };
        }
    }
}
