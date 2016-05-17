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
        public static Component[] create_components(Input inputhandler) {
            string sprite ="";
            if (inputhandler != null)
            {
                if (inputhandler.up == Keys.W)
                    sprite = "keys1";
                else if (inputhandler.up == Keys.Up)
                    sprite = "keys2";
                else if (inputhandler.device == Input.InputType.Controller)
                    sprite = "controller" + (int)(inputhandler.gp_index + 1);
            }

            var playerpos = new Position() { x = 0, y = 0 };
            return new Component[] {
                inputhandler,
                playerpos,
                new Sprite() { texture = (inputhandler != null) ? Starburst.inst().get_content<Texture2D>("menu/"+sprite) : null }
            };
        }
        public static Component[] create_components()
        {
            return create_components(new Input());
        }
    }
}
