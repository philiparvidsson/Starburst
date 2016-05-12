namespace Fab5.Engine.Subsystems
{
    using Microsoft.Xna.Framework;

    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Microsoft.Xna.Framework.Graphics;
    using Engine;
    using System;
    using Fab5.Starburst;
    public class Hudsystem
    {
        SpriteBatch sprite_batch;
        Ship_Info ship_info;
        Sprite enball;

        private Texture2D minimap_tex;
        private Texture2D white_pixel_tex;
        //private int number_of_players;

        public Hudsystem(SpriteBatch sprite_batch, Tile_Map tile_map)
        {
            this.sprite_batch = sprite_batch;

            enball = new Sprite()
            {
                texture = Fab5_Game.inst().get_content<Texture2D>("EnergiAtlas"),
                //color = new Color(0.70f, 0.70f, 0.70f)
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
                }
            }

            for (int i = 0; i < 256; i++) {
                for (int j = 0; j < 256; j++) {
                    var x = i>>1;
                    var y = j>>1;

                    var k = i+256*j;
                    if (tile_map.tiles[k] != 0 && tile_map.tiles[k] < 7) {
                        minimap_tex.SetData(0, new Rectangle(x, y, 1, 1), new [] { Color.White }, 0, 1);
                    }
                }
            }

            white_pixel_tex = new Texture2D(Fab5_Game.inst().GraphicsDevice, 1, 1);
            white_pixel_tex.SetData(new [] { Color.White });

        }

        private void draw_powerup_inv(Camera cam, Entity player) {
            int size = Math.Min((int)(cam.viewport.Width / 15), 92);
            //int size = 64;
            int smallSize = (int)(size * .75);
            int spacing = 8;
            var si = player.get_component<Ship_Info>();
            int num_inv = si.max_powerups_inv;

            int x = (int)(cam.viewport.Width * 0.5f - 0.5f*((num_inv-1)*smallSize+size+(num_inv-1)*spacing));
            int y = 40;

            for (int i = 0; i < num_inv; i++) {
                int mySize = (i == si.powerup_inv_index ? size : smallSize);
                int myY = (int)(i == si.powerup_inv_index ? y - (size - smallSize) * .5f : y);
                GFX_Util.fill_rect(sprite_batch, new Rectangle(x, myY, mySize, mySize), Color.Black * 0.5f);

                if (si.powerup_inv[i] != null) {
                    sprite_batch.Draw(si.powerup_inv[i].icon, new Rectangle(x, myY, mySize, mySize), Color.White);
                    //GFX_Util.fill_rect(sprite_batch, new Rectangle(x, y, 15, 15), Color.White);
                }

                //if (i == si.powerup_inv_index) {
                    var col = (i == si.powerup_inv_index ? Color.White : Color.Gray);
                    var border = 2;
                    GFX_Util.fill_rect(sprite_batch, new Rectangle(x, myY, border, mySize), col);
                    GFX_Util.fill_rect(sprite_batch, new Rectangle(x+ mySize - border, myY, border, mySize), col);
                    GFX_Util.fill_rect(sprite_batch, new Rectangle(x, myY, mySize, border), col);
                    GFX_Util.fill_rect(sprite_batch, new Rectangle(x, myY + mySize - border, mySize, border), col);
                //}

                x += mySize + spacing;
            }


            var vp_size = Fab5_Game.inst().GraphicsDevice.Viewport.Width;
            var scale = 1.0f*(float)System.Math.Sqrt(1.0f+vp_size/1920.0f);
            var xx  = Fab5_Game.inst().GraphicsDevice.Viewport.Width - 32.0f - 12.0f;
            var yy = Fab5_Game.inst().GraphicsDevice.Viewport.Height  - 128.0f * scale - 15.0f - 32.0f - 16.0f;

            foreach (var e in si.powerups) {
                var textsize = GFX_Util.measure_string(e.Value.time >= 100.0f ? "100.9" : "99.9");
                var textx = xx - textsize.X - 2.0f;
                var texty = 16.0f-textsize.Y*0.5f;
                sprite_batch.Draw(e.Value.icon, new Vector2(xx, yy), null, Color.White, 0.0f, Vector2.Zero, new Vector2(0.5f, 0.5f), SpriteEffects.None, 0.5f);
                GFX_Util.draw_def_text(sprite_batch, string.Format("{0:0.0}", e.Value.time), textx, yy+texty);
                yy -= 36.0f;
            }
        }

        private void draw_minimap(Entity player) {
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

            var team = player.get_component<Ship_Info>().team;

            foreach (var team_mate in Fab5_Game.inst().get_entities_fast(typeof (Ship_Info))) {
                var tm_team = team_mate.get_component<Ship_Info>().team;
                if (tm_team != team) {
                    continue;
                }

                var color = Color.White;
                if (team_mate != player) {
                    color = (tm_team == 1) ? Color.Red : Color.Blue;
                }

                var position = team_mate.get_component<Position>();

                var tw = 16.0f;
                var th = 16.0f;
                var map_pos_x = minimap_left + scale*0.5f*(position.x+2048.0f) / tw;
                var map_pos_y = minimap_top + scale*0.5f*(position.y+2048.0f)  / th;

                sprite_batch.Draw(white_pixel_tex,
                                  new Vector2(map_pos_x, map_pos_y),
                                  null,
                                  color,
                                  0.0f,
                                  new Vector2(0.5f, 0.5f),
                                  new Vector2(4.0f, 4.0f),
                                  SpriteEffects.None,
                                  1.0f);
            }
        }

        public void drawHUD(Entity player, float dt, Camera camera)
        {

            //sprite_batch.Begin(SpriteSortMode.Deferred);
            if (!player.get_component<Input>().enabled)
                return;

            Position playerPos = player.get_component<Position>();

            this.ship_info = player.get_component<Ship_Info>();
            //drawHP();
            drawEnergy(playerPos, camera, dt);
            drawScore(player, dt);
            draw_minimap(player);
            draw_powerup_inv(camera, player);

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

        float energy_blink_timer;
        private void drawEnergy(Position shipPos, Camera camera, float dt)
        {
            float energyScale = (ship_info.energy_value / ship_info.top_energy);
            float energyScaleSqrt = (float)Math.Sqrt(energyScale);
            Color color = Color.White;

            energy_blink_timer += dt;
            if (energyScale > 0.5f) {
                energy_blink_timer = 0.0f;
            }
            else {
                //color = new Color(1.0f, 0.1f, 0.1f);
            }
            if (energy_blink_timer > 1.0f/5.0f) {
                energy_blink_timer -= 1.0f/5.0f;
            }
            
            //var source_rect = new Rectangle(enball.frame_x, enball.frame_y, enball.frame_width, enball.frame_height);

            
           // Console.WriteLine(                new Vector2((shipPos.x - camera.position.x - camera.displacement.X) * camera.zoom, (shipPos.y - camera.position.y - camera.displacement.Y) * camera.zoom));
            if (energy_blink_timer < 1.0f/10.0f) {
            sprite_batch.Draw(enball.texture,
                new Vector2((shipPos.x - camera.position.x - camera.displacement.X) * camera.zoom + camera.viewport.Width * 0.5f, (shipPos.y - camera.position.y - camera.displacement.Y) * camera.zoom + camera.viewport.Height * 0.5f),
                scale: new Vector2(energyScaleSqrt, energyScaleSqrt)*camera.zoom,
                sourceRectangle: null,
                origin: new Vector2(enball.texture.Width * 0.5f, enball.texture.Height * 0.5f),
                              color: color/*new Color(1.0f, energyScale*energyScale, energyScale*energyScale, 1.0f)*/
                );
            }

            //updateEnergySprite(dt);
        }

        // private void updateEnergySprite(float dt)
//         {
//             var players = Fab5_Game.inst().get_entities_fast(typeof(Input));
// 
// 
//             this.number_of_players = 0;
// 
//             foreach(Entity p in players)
//             {
//                 if(p.get_component<Input>().enabled)
//                     this.number_of_players++;
//             }
//             
// 
//             enball.fps = 20.0f / this.number_of_players;
// 
//             enball.frame_timer += dt;
//             if (enball.frame_timer > (1.0f / enball.fps))
//             {
//                 enball.frame_counter++;
//                 enball.frame_timer -= (1.0f / enball.fps);
// 
//                 enball.frame_x += enball.frame_width;
//                 if (enball.frame_x >= enball.texture.Width || enball.frame_counter >= enball.num_frames)
//                 {
//                     enball.frame_x = 0;
// 
//                     if (enball.frame_counter >= enball.num_frames)
//                     {
//                         enball.frame_counter = 0;
//                         enball.frame_y = 0;
//                     }
//                 }
//             }
//         }

        private void drawScore(Entity player, float dt)
        {
            var score = player.get_component<Score>();
            var si = player.get_component<Ship_Info>();

            bool left_align = (si.pindex%2)==1;
            Vector2 scoreposition;

            if (left_align) {
                scoreposition = new Vector2(10, 10);
            }
            else {
                //var str_w = GFX_Util.measure_string(score.display_score.ToString()).X + 10;
                scoreposition = new Vector2(Fab5_Game.inst().GraphicsDevice.Viewport.Width-10, 10);
            }

            SpriteFont spriteFont = Fab5_Game.inst().get_content<SpriteFont>("sector034");

            float time;

            if (score.score > 200)
                time = 3.0f;
            else
                time = 1.0f;

            if(score.score != score.display_score)
                score.current_time_span += dt;

            if (score.current_time_span > time || score.display_score >= score.score) // Otherwise shit will go high
            {
                score.display_score = score.score;
                score.linear_start_score = (int)score.score;
                score.current_time_span = 0.0f;
            }
            else
            {
                score.display_score = (float)Easing.ExpoEaseInOut(score.current_time_span, score.linear_start_score, score.score, time);
            }



            var y = GFX_Util.measure_string("0").Y;
            var score_str = string.Format("{0}", (int)score.display_score);
            var w1 = GFX_Util.measure_string(score_str).X;
            var w2 = GFX_Util.measure_string("P. " + si.pindex).X;

            if (left_align) {
                GFX_Util.draw_def_text(sprite_batch, score_str, scoreposition.X, scoreposition.Y+y,Color.White);
                GFX_Util.draw_def_text(sprite_batch, "P. " + si.pindex, scoreposition.X, scoreposition.Y);
            }
            else {
                GFX_Util.draw_def_text(sprite_batch, score_str, scoreposition.X-w1, scoreposition.Y+y,Color.White);
                GFX_Util.draw_def_text(sprite_batch, "P. " + si.pindex, scoreposition.X-w2, scoreposition.Y);
            }
        }
    }
}
