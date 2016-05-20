namespace Fab5.Starburst.States
{

    using Fab5.Engine;
    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Fab5.Engine.Subsystems;

    using Fab5.Starburst.States.Playing.Entities;
    using Main_Menu.Entities;
    using Main_Menu.Subsystems;
    using Menus.Subsystems;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Media;
    using Microsoft.Xna.Framework.Input;

    using System;
    using System.Collections.Generic;
    using Playing;
    public class Pre_Results_State : Game_State
    {
        private List<Entity> players;
        private Game_Config gameConfig;


        public Pre_Results_State(List<Entity> players, Game_Config config)
        {
            this.players = players;
            this.gameConfig = config;
        }

        public override void init() {
            var sprite_batch = new SpriteBatch(Starburst.inst().GraphicsDevice);
            sprite_batch.Begin();
            GFX_Util.fill_rect(sprite_batch, new Rectangle(0, 0, Starburst.inst().GraphicsDevice.Viewport.Width, Starburst.inst().GraphicsDevice.Viewport.Height), Color.Black * 0.45f);
            sprite_batch.End();
        }


        public override void draw(float t, float dt)
        {
            if (t > 2.0f) {
                Starburst.inst().leave_state();
                Starburst.inst().enter_state(new Results_State(players, gameConfig));
            }
        }

    }
}
