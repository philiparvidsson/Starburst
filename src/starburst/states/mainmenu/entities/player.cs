namespace Fab5.Starburst.States.Main_Menu.Entities
{
    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Fab5.Engine;

    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;

    using System;

    public static class Player
    {
        static int lol = 0;
        public static Component[] create_components(Inputhandler inputhandler) {
            if (lol == 0) {
                // this ass code sucks
                inputhandler = new Inputhandler() {
                    left = Keys.A,
                    right = Keys.D,
                    up = Keys.W,
                    down = Keys.S,
                    gp_index = PlayerIndex.Two,
                    primary_fire = Keys.F,
                    secondary_fire = Keys.G
                };
            }
            string sprite = "keyboard";
            if (inputhandler.device == Inputhandler.InputType.Controller)
                sprite = "controller";
            lol++;

            var playerpos = new Position() { x = 0, y = 0 };
            return new Component[] {
                inputhandler,
                playerpos,
                new Sprite()
                {
                    texture = Starburst.inst().get_content<Texture2D>(sprite),
                }
            };
        }
        public static Component[] create_components()
        {
            return create_components(new Inputhandler());
        }
    }
}
