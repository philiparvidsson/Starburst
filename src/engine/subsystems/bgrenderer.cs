namespace Fab5.Engine.Subsystems
{

    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Fab5.Starburst.States.Playing;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class BG_Renderer
    {
        public void drawBackground(SpriteBatch sprite_batch, Position playerPosition, Camera camera)
        {
            int num_entities;

            var tities = Fab5_Game.inst().get_entities(out num_entities,
            typeof(Backdrop));

            sprite_batch.Begin(SpriteSortMode.FrontToBack,
                BlendState.AlphaBlend, null, null, null, null,
                transformMatrix: camera.getViewMatrix(camera.viewport));

            for (int j = 0; j < num_entities; j++)
            {
                var entity = tities[j];
                var bgtexture = entity.get_component<Backdrop>();
                
                if (bgtexture == null)
                    continue;

                sprite_batch.Draw(bgtexture.backdrop,
                    destinationRectangle: new Rectangle(0 - (int)(playerPosition.x / 100),
                        0 - (int)(playerPosition.y / 100),
                        bgtexture.backdrop.Width*2,
                        bgtexture.backdrop.Height*2),
                    origin: new Vector2(bgtexture.backdrop.Width / 2, bgtexture.backdrop.Height / 2));
                sprite_batch.Draw(bgtexture.stardrop,
                    destinationRectangle: new Rectangle(0 - (int)(playerPosition.x / 10),
                        0 - (int)(playerPosition.y / 10),
                        bgtexture.stardrop.Width*2,
                        bgtexture.stardrop.Height*2),
                    origin: new Vector2(bgtexture.stardrop.Width / 2, bgtexture.stardrop.Height / 2),
                    color: (Color.White));
            }
            sprite_batch.End();
        }
    }

}
