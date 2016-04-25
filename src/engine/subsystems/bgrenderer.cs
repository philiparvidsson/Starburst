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

            sprite_batch.Begin(SpriteSortMode.BackToFront,
                BlendState.Additive, null, null, null, null
                               /*transformMatrix: camera.getViewMatrix(camera.viewport)*/);

            for (int j = 0; j < num_entities; j++)
            {
                var entity = tities[j];
                var bgtexture = entity.get_component<Backdrop>();
                

                var fac1 = 0.05f;
                sprite_batch.Draw(bgtexture.backdrop,
                                  Vector2.Zero,
                                  null,
                                  Color.White,
                                  0.0f,
                                  new Vector2(bgtexture.backdrop.Width/2.0f  + playerPosition.x * fac1,
                                              bgtexture.backdrop.Height/2.0f + playerPosition.y * fac1),
                                  new Vector2(1.5f, 1.5f),
                                  SpriteEffects.None,
                                  1.0f);

                var fac2 = 0.25f;
                sprite_batch.Draw(bgtexture.stardrop,
                                  Vector2.Zero,
                                  null,
                                  Color.White,
                                  0.0f,
                                  new Vector2(bgtexture.backdrop.Width/2.0f  + playerPosition.x * fac2,
                                              bgtexture.backdrop.Height/2.0f + playerPosition.y * fac2),
                                  new Vector2(2.0f, 2.0f),
                                  SpriteEffects.None,
                                  0.9f);
            }
            sprite_batch.End();
        }
    }

}
