namespace Fab5.Engine.Subsystems
{

    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Fab5.Starburst.States.Playing;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class BG_Renderer : Subsystem
    {
        private readonly SpriteBatch sprite_batch;

        public BG_Renderer(SpriteBatch sprite_batch)
        {
            this.sprite_batch = sprite_batch;
        }

        public override void draw(float t, float dt)
        {
            int num_entities;
            Position player_pos = null;

            /* Get position of player */
            var players = Fab5_Game.inst().get_entities(out num_entities,
                typeof(Inputhandler),
                typeof(Position));
            for (int i = 0; i < num_entities; i++)
            {
                var player = players[i];
                player_pos = player.get_component<Position>();
            }
            /**/

            sprite_batch.GraphicsDevice.Clear(Color.Black);

            sprite_batch.Begin();

            var tities = Fab5_Game.inst().get_entities(out num_entities,
            typeof(Backdrop));


            for (int j = 0; j < num_entities; j++)
            {
                var entity = tities[j];
                var bgtexture = entity.get_component<Backdrop>();
                
                if (bgtexture == null)
                    continue;

                sprite_batch.Draw(bgtexture.backdrop,
                    destinationRectangle: new Rectangle(0 - (int)(player_pos.x / 10),
                        0 - (int)(player_pos.y / 10),
                        bgtexture.backdrop.Width*2,
                        bgtexture.backdrop.Height*2),
                    origin: new Vector2(bgtexture.backdrop.Width / 2, bgtexture.backdrop.Height / 2));
                sprite_batch.Draw(bgtexture.stardrop,
                    destinationRectangle: new Rectangle(0 - (int)(player_pos.x / 5),
                        0 - (int)(player_pos.y / 5),
                        bgtexture.stardrop.Width*2,
                        bgtexture.stardrop.Height*2),
                    origin: new Vector2(bgtexture.stardrop.Width / 2, bgtexture.stardrop.Height / 2),
                    color: (Color.White * 0.5f));


            }
            sprite_batch.End();
        }
    }

}
