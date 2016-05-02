namespace Fab5.Engine.Subsystems
{
    using Microsoft.Xna.Framework;

    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Microsoft.Xna.Framework.Graphics;
    using Engine;
    using System;
    public class Hudsystem
    {
        SpriteBatch sprite_batch;
        Ship_Info ship_info;
        Sprite enball;

        private Texture2D minimap_tex;
        private Texture2D white_pixel_tex;
        private int number_of_players;

        public Hudsystem(SpriteBatch sprite_batch, Tile_Map tile_map)
        {
            this.sprite_batch = sprite_batch;

            Random random = new Random();

            this.number_of_players = Fab5_Game.inst().get_entities_fast(typeof(Inputhandler)).Count;

            enball = new Sprite()
            {
                texture = Fab5_Game.inst().get_content<Texture2D>("EnergiAtlas"),
                frame_width = 150,
                frame_height = 150,
                num_frames = 4,
                frame_timer = 0.2f * this.number_of_players,
                color = new Color(0.70f, 0.70f, 0.70f)
            };

            minimap_tex = new Texture2D(Fab5_Game.inst().GraphicsDevice, 256, 256);

            for (int i = 0; i < 256; i++) {
                for (int j = 0; j < 256; j++) {
                    var x = i>>1;
                    var y = j>>1;
                    minimap_tex.SetData(0, new Rectangle(x, y, 1, 1), new [] { new Color(0.0f, 0.0f, 0.0f, 0.0f) }, 0, 1);

                    if ((x%16)==0 || (y%16)==0) {
                        minimap_tex.SetData(0, new Rectangle(x, y, 1, 1), new [] { Color.White * 0.3f }, 0, 1);
                    }

                    var k = i+256*j;
                    if (tile_map.tiles[k] != 0 && tile_map.tiles[k] < 7) {
                        minimap_tex.SetData(0, new Rectangle(x, y, 1, 1), new [] { Color.White }, 0, 1);
                    }
                }
            }

            white_pixel_tex = new Texture2D(Fab5_Game.inst().GraphicsDevice, 1, 1);
            white_pixel_tex.SetData(new [] { Color.White });

        }

        private void draw_minimap(Position position) {
            var vp_size = Fab5_Game.inst().GraphicsDevice.Viewport.Width;
            var scale = 1.0f*(float)System.Math.Sqrt(1.0f+vp_size/1920.0f);

            var minimap_top  = Fab5_Game.inst().GraphicsDevice.Viewport.Height - 128.0f * scale - 15.0f;
            var minimap_left = Fab5_Game.inst().GraphicsDevice.Viewport.Width  - 128.0f * scale - 15.0f;

            var border = 8.0f;
            sprite_batch.Draw(white_pixel_tex,
                              new Vector2(minimap_left-border, minimap_top-border),
                              null,
                              new Color(0.0f, 0.0f, 0.0f, 0.4f),
                              0.0f,
                              Vector2.Zero,
                              new Vector2(scale*256.0f*0.5f+border*2.0f, scale*256.0f*0.5f+border*2.0f),
                              SpriteEffects.None,
                              0.0f);

            sprite_batch.Draw(minimap_tex,
                              new Vector2(minimap_left, minimap_top),
                              null,
                              Color.White * 0.7f,
                              0.0f,
                              Vector2.Zero,
                              new Vector2(scale, scale),
                              SpriteEffects.None,
                              0.0f);

            var tw = 16.0f;
            var th = 16.0f;
            var map_pos_x = minimap_left + scale*0.5f*(position.x+2048.0f) / tw;
            var map_pos_y = minimap_top + scale*0.5f*(position.y+2048.0f)  / th;

            sprite_batch.Draw(white_pixel_tex,
                              new Vector2(map_pos_x, map_pos_y),
                              null,
                              Color.White,
                              0.0f,
                              new Vector2(0.5f, 0.5f),
                              new Vector2(4.0f, 4.0f),
                              SpriteEffects.None,
                              1.0f);
        }

        public void drawHUD(Entity player, float dt, Camera camera)
        {

            //sprite_batch.Begin(SpriteSortMode.Deferred);

            Position playerPos = player.get_component<Position>();

            this.ship_info = player.get_component<Ship_Info>();
            //drawHP();
            drawEnergy(playerPos, camera, dt);
            drawScore(player.get_component<Score>(), dt);
            draw_minimap(playerPos);

            //sprite_batch.End();
        }

        private void drawHP()
        {
            /*Position hpposition;
            hpposition = new Position() { x = 20, y = Fab5_Game.inst().GraphicsDevice.Viewport.Height - 15 - hpbar_texture.Height };
            sprite_batch.Draw(hpbar_texture, new Vector2(hpposition.x, hpposition.y), color: Color.White);

            // X = 9, Y = 7, W = 68, H = 16 Coordinates for the filling in hpbar-sprite

            sprite_batch.Draw(hpball,
                new Vector2(50, 50),
                scale: new Vector2(5*(ship_info.hp_value / ship_info.top_hp), 5*(ship_info.hp_value / ship_info.top_hp))
                );

            sprite_batch.Draw(hpbar_texture,
                new Vector2(hpposition.x + 9, hpposition.y + 7),
                sourceRectangle: new Rectangle(9, 7, 68, 16),
                color: Color.Black);

            sprite_batch.Draw(hpbar_texture,
                destinationRectangle: new Rectangle((int)hpposition.x + 9, (int)hpposition.y + 7, (int)(68 * (ship_info.hp_value / 100)), 16),
                sourceRectangle: new Rectangle(9, 7, 68, 16),
                color: Color.Red);*/



        }

        private void drawEnergy(Position shipPos, Camera camera, float dt)
        {
            float energyScale = ship_info.energy_value / ship_info.top_energy;
            
            var source_rect = new Rectangle(enball.frame_x, enball.frame_y, enball.frame_width, enball.frame_height);
            
            sprite_batch.Draw(enball.texture,
                new Vector2((shipPos.x - camera.position.x) * camera.zoom + camera.viewport.Width * 0.5f, (shipPos.y - camera.position.y) * camera.zoom + camera.viewport.Height * 0.5f),
                scale: new Vector2(energyScale, energyScale),
                sourceRectangle: source_rect,
                origin: new Vector2(enball.frame_width * 0.5f, enball.frame_height * 0.5f),
                color: Color.White * energyScale * 0.5f
                );

            updateEnergySprite(dt);
        }

        private void updateEnergySprite(float dt)
        {
            enball.frame_timer += dt;
            if (enball.frame_timer > (1.0f / enball.fps))
            {
                enball.frame_counter++;
                enball.frame_timer -= (1.0f / enball.fps);

                enball.frame_x += enball.frame_width;
                if (enball.frame_x >= enball.texture.Width || enball.frame_counter >= enball.num_frames)
                {
                    enball.frame_x = 0;

                    if (enball.frame_counter >= enball.num_frames)
                    {
                        enball.frame_counter = 0;
                        enball.frame_y = 0;
                    }
                }
            }
        }

        private void drawScore(Score score, float dt)
        {
            Vector2 scoreposition;
            scoreposition = new Vector2(10, 10);

            SpriteFont spriteFont = Fab5_Game.inst().get_content<SpriteFont>("sector034");

            if(score.score != score.display_score)
                score.current_time_span += dt;

            if (score.current_time_span > 2.0f || score.display_score >= score.score) // When two second has passed or we increased to far we are done.
            {
                score.display_score = score.score;
                score.linear_start_score = (int)score.score;
                score.current_time_span = 0.0f;
            }
            else
            {   
                score.display_score += (2.0f - score.current_time_span) * (score.score - score.linear_start_score) * dt;
            }

            var score_str = "P" + ship_info.pindex + " Score: " + ((int)score.display_score).ToString();
            sprite_batch.DrawString(spriteFont,
                score_str, 
                position: scoreposition, 
                color: Color.Black * 0.75f
            );

            sprite_batch.DrawString(spriteFont,
                score_str,
                position: scoreposition + new Vector2(4.0f, 4.0f), 
                color: Color.White
            );
        }
    }
}
