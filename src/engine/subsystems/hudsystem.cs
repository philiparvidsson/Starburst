namespace Fab5.Engine.Subsystems
{
    using Microsoft.Xna.Framework;

    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Microsoft.Xna.Framework.Graphics;
    using Engine;

    public static class Hudsystem
    {

        public static void updateHUD()
        {
            int num_ent;
            var entities = Fab5_Game.inst().get_entities(out num_ent, typeof(Hud_Component));

            for (int i = 0; i < num_ent; i++)
            {
                var entity = entities[i];
                var hud_comp = entity.get_component<Hud_Component>();

                hud_comp.value -= 1;
                if (hud_comp.value == 0)
                    hud_comp.value = 100;

            }
        }

        public static void drawHUD(SpriteBatch sprite_batch, Entity incomingEntity, int currentPlayer)
        {
            int num_components;
            var entities = Fab5_Game.inst().get_entities(out num_components,
                typeof(Hud_Component),
                typeof(Sprite)
            );

            sprite_batch.Begin();


            for (int i = 0; i < num_components; i++)
            {
                var entity = entities[i];
                var hudValue = entity.get_component<Hud_Component>();
                var sprite = entity.get_component<Sprite>();
                Position position = null;

                if (hudValue.type == 1)
                    position = new Position() { x = 20, y = Fab5_Game.inst().GraphicsDevice.Viewport.Height - 15 - sprite.texture.Height };
                else if (hudValue.type == 2)
                    position = new Position() { x = (Fab5_Game.inst().GraphicsDevice.Viewport.Width - sprite.texture.Width) / 2, y = Fab5_Game.inst().GraphicsDevice.Viewport.Height - 20 - sprite.texture.Height };

                sprite_batch.Draw(sprite.texture, new Vector2(position.x, position.y), color: Color.White, layerDepth: 0);

                if (hudValue.type == 1)
                {
                    // X = 9, Y = 7, W = 68, H = 16 Coordinates for the filling in hpbar-sprite

                    sprite_batch.Draw(sprite.texture, 
                        new Vector2(position.x + 9, position.y + 7), 
                        sourceRectangle: new Rectangle(9, 7, 68, 16), 
                        color: Color.Black, 
                        layerDepth: 0);
                    
                    sprite_batch.Draw(sprite.texture, 
                        destinationRectangle: new Rectangle((int)position.x + 9, (int)position.y + 7, (int)(68 * (hudValue.value / 100)), 16), 
                        sourceRectangle: new Rectangle(9, 7, 68, 16), 
                        color: Color.Red, 
                        layerDepth: 0);
                }
                else if (hudValue.type == 2)
                {
                    // X = 10, Y = 6, W = 226, H = 8 Coordinates for the filling in energybar-sprite

                    sprite_batch.Draw(sprite.texture, 
                        new Vector2(position.x + 10, position.y + 6), 
                        sourceRectangle: new Rectangle(10, 6, 226, 8), 
                        color: Color.Black,
                        layerDepth: 0);

                    sprite_batch.Draw(sprite.texture,
                       destinationRectangle: new Rectangle((int)position.x + 10, (int)position.y + 6, (int)(226 * (hudValue.value / 100)), 8),
                       sourceRectangle: new Rectangle(10, 6, 226, 8), 
                       color: Color.Blue,
                       layerDepth: 0);
                }
            }
            sprite_batch.End();
        }
    }
}
