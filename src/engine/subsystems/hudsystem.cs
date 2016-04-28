namespace Fab5.Engine.Subsystems
{
    using Microsoft.Xna.Framework;

    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Microsoft.Xna.Framework.Graphics;
    using Engine;

    public class Hudsystem
    {
        SpriteBatch sprite_batch;
        Texture2D hpbar_texture;
        Texture2D energybar_texture;
        Ship_Info ship_info;

        public Hudsystem(SpriteBatch sprite_batch)
        {
            this.sprite_batch = sprite_batch;
            this.hpbar_texture = Fab5_Game.inst().get_content<Texture2D>("HPBar");
            this.energybar_texture = Fab5_Game.inst().get_content<Texture2D>("EnergyBar");
        }

        public void drawHUD(Entity player)
        {

            sprite_batch.Begin(SpriteSortMode.Deferred);

            this.ship_info = player.get_component<Ship_Info>();
            drawHP();
            drawEnergy();
            drawScore(player.get_component<Score>());

            sprite_batch.End();
        }

        private void drawHP()
        {
            Position hpposition;
            hpposition = new Position() { x = 20, y = Fab5_Game.inst().GraphicsDevice.Viewport.Height - 15 - hpbar_texture.Height };
            sprite_batch.Draw(hpbar_texture, new Vector2(hpposition.x, hpposition.y), color: Color.White);

            // X = 9, Y = 7, W = 68, H = 16 Coordinates for the filling in hpbar-sprite

            sprite_batch.Draw(hpbar_texture,
                new Vector2(hpposition.x + 9, hpposition.y + 7),
                sourceRectangle: new Rectangle(9, 7, 68, 16),
                color: Color.Black);

            sprite_batch.Draw(hpbar_texture,
                destinationRectangle: new Rectangle((int)hpposition.x + 9, (int)hpposition.y + 7, (int)(68 * (ship_info.hp_value / 100)), 16),
                sourceRectangle: new Rectangle(9, 7, 68, 16),
                color: Color.Red);

        }

        private void drawEnergy()
        {
            Position energyposition;
            energyposition = new Position()
            {
                x = (Fab5_Game.inst().GraphicsDevice.Viewport.Width - energybar_texture.Width) / 2,
                y = Fab5_Game.inst().GraphicsDevice.Viewport.Height - 20 - energybar_texture.Height
            };
            sprite_batch.Draw(energybar_texture, new Vector2(energyposition.x, energyposition.y), color: Color.White);
            
            // X = 10, Y = 6, W = 226, H = 8 Coordinates for the filling in energybar-sprite

            sprite_batch.Draw(energybar_texture,
                new Vector2(energyposition.x + 10, energyposition.y + 6),
                sourceRectangle: new Rectangle(10, 6, 226, 8),
                color: Color.Black);

            sprite_batch.Draw(energybar_texture,
                destinationRectangle: new Rectangle((int)energyposition.x + 10, (int)energyposition.y + 6,
                (int)(226 * (ship_info.energy_value / ship_info.top_energy)), 8),
                sourceRectangle: new Rectangle(10, 6, 226, 8),
                color: Color.AliceBlue);
        }

        private void drawScore(Score score)
        {
            Vector2 scoreposition;
            scoreposition = new Vector2(10, 10);

            SpriteFont spriteFont = Fab5_Game.inst().get_content<SpriteFont>("sector034");

            score.score++;

            sprite_batch.DrawString(spriteFont,
                "Score: " + score.score.ToString(), 
                position: scoreposition, 
                color: Color.Black * 0.75f
            );

            sprite_batch.DrawString(spriteFont,
                "Score: " + score.score.ToString(), 
                                    position: scoreposition + new Vector2(4.0f, 4.0f), 
                color: Color.White
            );
        }
    }
}
