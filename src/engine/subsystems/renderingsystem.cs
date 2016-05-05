using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fab5.Engine.Components;
using Fab5.Engine.Core;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Fab5.Starburst.States;
using Fab5.Starburst.States.Playing;
using Fab5.Starburst;

namespace Fab5.Engine.Subsystems {
    class Rendering_System : Subsystem {
        /**
        * Camera, Viewport, Entity (med InputHandler)
        *
        * initialize:
        * loopa igenom alla spelare (alla entiteter med inputhandlers)
        * ställ in viewports och kameror baserat på hur många inputs
        *
        * update:
        * uppdatera kameran för att centreras på kopplade spelarens position
        *
        * draw:
        * kör spriterenderer och textrenderer på varje viewport
        **/
        List<Entity> players;
        Viewport[] viewports;
        Camera[] cameras;
        int currentPlayerNumber, prevPlayerNumber;
        SpriteBatch sprite_batch;
        Viewport defaultViewport;

        public Tile_Map tile_map;

        Hudsystem hudsystem_instance;

        private Texture2D backdrop;
        private Texture2D stardrop;

        private Texture2D timer_tex;

        private Texture2D player_indicator_tex;
        private Texture2D player_indicator2_tex;
        private bool team_play;
        private GraphicsDevice graphicsDevice;

        public Rendering_System(GraphicsDevice graphicsDevice) {
            this.graphicsDevice = graphicsDevice;
        }

        private void draw_backdrop(SpriteBatch sprite_batch, Camera camera) {
            sprite_batch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

            var hw = camera.viewport.Width  * 0.5f;
            var hh = camera.viewport.Height * 0.5f;

            var x = camera.position.x;
            var y = camera.position.y;

            var fac1 = 0.05f;
            var scale1 = 1.0f*camera.zoom;
            sprite_batch.Draw(backdrop,
                              new Vector2(hw - (backdrop.Width*0.5f  + x * fac1) * scale1, hh - (backdrop.Height*0.5f + y * fac1) * scale1),
                              null,
                              new Color(0.65f, 0.65f, 0.65f, 0.65f),
                              0.0f,
                              Vector2.Zero,
                              new Vector2(scale1, scale1),
                              SpriteEffects.None,
                              1.0f);


            var fac2 = 0.1f;
            var scale2 = 2.0f*camera.zoom;
            sprite_batch.Draw(stardrop,
                              new Vector2(hw - (stardrop.Width*0.5f  + x * fac2) * scale2, hh - (stardrop.Height*0.5f + y * fac2) * scale2),
                              null,
                              Color.White,
                              0.0f,
                              Vector2.Zero,
                              new Vector2(scale2, scale2),
                              SpriteEffects.None,
                              0.9f);

            var fac3 = 0.1f;
            var scale3 = 4.0f*camera.zoom;
            sprite_batch.Draw(stardrop,
                              new Vector2(hw - (stardrop.Width*0.5f  + x * fac3) * scale3, hh - (stardrop.Height*0.5f + y * fac3) * scale3),
                              null,
                              Color.White * 0.8f,
                              0.0f,
                              Vector2.Zero,
                              new Vector2(scale3, scale3),
                              SpriteEffects.FlipHorizontally,
                              0.9f);

            sprite_batch.End();


        }


        //Texture2D grid_tex;
        private void draw_tile_map(SpriteBatch sprite_batch, Camera camera) {


            float tw     = 16.0f;
            float th     = 16.0f;
            float w      = camera.viewport.Width  / camera.zoom;
            float h      = camera.viewport.Height / camera.zoom;
            int left   = (int)((camera.position.x+2048.0f-w*0.5f) / tw);
            int top    = (int)((camera.position.y+2048.0f-h*0.5f) / th);
            int right  = (int)(left + w/tw)+1;
            int bottom = (int)(top  + h/th)+1;

            float xfrac = left*tw - (camera.position.x+2048.0f-w*0.5f);
            float yfrac = top *th - (camera.position.y+2048.0f-h*0.5f);
//            System.Console.WriteLine(yfrac);

            xfrac *= camera.zoom;
            yfrac *= camera.zoom;

            //if (grid_tex == null) {
              //  grid_tex = Fab5_Game.inst().get_content<Texture2D>("tgrid");
            //}

            sprite_batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            var tile_tex = tile_map.tex;
            var x = 0.0f;
            th *= camera.zoom;
            tw *= camera.zoom;
            var tiles = tile_map.tiles;
            for (int i = left; i <= right; i++) {
                var y = 0.0f;
                var sx = x+xfrac;

                for (int j = top; j <= bottom; j++) {
                    if (i < 0 || i > 255 || j < 0 || j > 255) {
                        y += th;
                        continue;
                    }

                    int o = i + (j<<8);

//                    sprite_batch.Draw(grid_tex, new Vector2(x+xfrac, y+yfrac), Color.White * 0.14f);

                    int k = tiles[o];
                    if (k != 0 && k < 9) {// 9 and up are not visible walls
                        var v = k-1;

                        var sy = y+yfrac;
                        sprite_batch.Draw(tile_tex,
                                          new Vector2(sx, sy),
                                          new Rectangle(18*v+1, 0, 16, 16),
                                          Color.White,
                                          0.0f,
                                          Vector2.Zero,
                                          camera.zoom,
                                          SpriteEffects.None,
                                          0.5f);
                    }

                    y += th;
                }

                x += tw;
            }
            sprite_batch.End();
        }

        public override void init()
        {
 	        base.init();

            timer_tex = Fab5_Game.inst().get_content<Texture2D>("clock");

            team_play = ((Playing_State)state).game_conf.mode == Game_Config.GM_TEAM_DEATHMATCH;

            sprite_batch = new SpriteBatch(graphicsDevice);
            defaultViewport = graphicsDevice.Viewport;

            backdrop = Fab5_Game.inst().get_content<Texture2D>("backdrops/backdrop4");
            stardrop = Fab5_Game.inst().get_content<Texture2D>("backdrops/stardrop");

            player_indicator_tex = Fab5_Game.inst().get_content<Texture2D>("indicator");
            player_indicator2_tex = Fab5_Game.inst().get_content<Texture2D>("indicator2");

            // kör uppdatering av viewports och kameror
            updatePlayers();
            this.hudsystem_instance = new Hudsystem(sprite_batch, tile_map);



        }

        private void updatePlayers() {
            // ev hantering för om inga spelare hittas?
            var x = (float)Math.Max(Fab5_Game.inst().GraphicsMgr.PreferredBackBufferWidth, Fab5_Game.inst().GraphicsMgr.PreferredBackBufferHeight);
            float zoom = 1.0f * (x/1920.0f);
            if(currentPlayerNumber == 1) {
                // full screen
                viewports = new Viewport[1];
                cameras = new Camera[1];
                viewports[0] = defaultViewport;
                currentPlayerNumber = 1;
                zoom *= 1.3f;
            }
            else if(currentPlayerNumber <= 2) {
                // 1/2 screen, handle heights and y position
                viewports = new Viewport[2];
                cameras = new Camera[2];
                Viewport top = defaultViewport;
                top.Width = (int)(defaultViewport.Width * .5);

                Viewport bottom = defaultViewport;
                bottom.Width = (int)(defaultViewport.Width * .5);
                bottom.X = top.Width;

                viewports[0] = top;
                viewports[1] = bottom;
                zoom *= 1.0f;
            }
            else if(currentPlayerNumber == 3){
                // 1/4 screen, handle sizes and positions
                viewports = new Viewport[3];
                cameras = new Camera[3];
                Viewport topLeft = defaultViewport;
                topLeft.Width = (int)(defaultViewport.Width * .5);
                topLeft.Height = (int)(defaultViewport.Height * .5);

                Viewport topRight = defaultViewport;
                topRight.Width = (int)(defaultViewport.Width * .5);
                topRight.Height = (int)(defaultViewport.Height * .5);
                topRight.X = topLeft.Width;

                Viewport bottom = defaultViewport;
                bottom.Width = (int)(defaultViewport.Width * .5);
                bottom.Height = (int)(defaultViewport.Height * .5);
                bottom.Y = topLeft.Height;

                viewports[0] = topLeft;
                viewports[1] = topRight;
                viewports[2] = bottom;

                zoom *= .9f;
            }
            else {
                // 1/4 screen, handle sizes and positions
                viewports = new Viewport[4];
                cameras = new Camera[4];
                Viewport topLeft = defaultViewport;
                topLeft.Width = (int)(defaultViewport.Width * .5);
                topLeft.Height = (int)(defaultViewport.Height * .5);

                Viewport topRight = defaultViewport;
                topRight.Width = (int)(defaultViewport.Width * .5);
                topRight.Height = (int)(defaultViewport.Height * .5);
                topRight.X = topLeft.Width;

                Viewport bottomLeft = defaultViewport;
                bottomLeft.Width = (int)(defaultViewport.Width * .5);
                bottomLeft.Height = (int)(defaultViewport.Height * .5);
                bottomLeft.Y = topLeft.Height;

                Viewport bottomRight = defaultViewport;
                bottomRight.Width = (int)(defaultViewport.Width * .5);
                bottomRight.Height = (int)(defaultViewport.Height * .5);
                bottomRight.Y = topLeft.Height;
                bottomRight.X = topLeft.Width;

                viewports[0] = topLeft;
                viewports[1] = topRight;
                viewports[2] = bottomLeft;
                viewports[3] = bottomRight;

                zoom *= .9f;
            }

            // add cameras to each viewport
            for (int i = 0; i < currentPlayerNumber; i++) {
                cameras[i] = new Camera(viewports[i]) { zoom = zoom };
                cameras[i].index = i+1;
            }

            prevPlayerNumber = currentPlayerNumber;
        }

        public override void update(float t, float dt) {
            base.update(t, dt);

            /**
             * uppdatera kameran för att centreras på kopplade spelarens position
             * (görs nu i draw för att onödigt att ändra kamera när det ändå inte renderas)
             **/
            /*
            for (int i = 0; i < currentPlayerNumber; i++)
            {
                cameras[i].position = players[i].get_component<Position>();
            }*/
        }

        private void draw_match_time() {
            sprite_batch.Draw(timer_tex, new Vector2(760.0f, 40.0f), null, Color.White, 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.5f);
            GFX_Util.draw_def_text(sprite_batch, "0.00", 800.0f, 40.0f);
        }


        private void draw_indicators(Camera current, int currentPlayerNumber, Entity player) {
            var currentPlayerPosition = player.get_component<Position>();
            for (int p2 = 0; p2 < currentPlayerNumber; p2++) {
                var player2 = players[p2];

                var ih = player2.get_component<Input>();
                if (ih != null && !ih.enabled) {
                    continue;
                }

                var tex = player_indicator_tex;
                if (team_play && player2.get_component<Ship_Info>().team == 2) {
                    tex = player_indicator2_tex;
                }

                var player2_pos = player2.get_component<Position>();
                var d_x = player2_pos.x - current.position.x;
                var d_y = player2_pos.y - current.position.y;

                var d = (float)Math.Sqrt(d_x*d_x + d_y*d_y);

                if (Math.Abs(d_x) < current.viewport.Width*0.5f/current.zoom && Math.Abs(d_y) < current.viewport.Height*0.5f/current.zoom) {
                    // other player is on same screen
                    continue;
                }

                d_x /= d;
                d_y /= d;

                d_x *= 36.0f * current.zoom;
                d_y *= 36.0f * current.zoom;

                var r = (float)Math.Atan2(d_y, d_x);

                var p_x = current.zoom*(currentPlayerPosition.x - current.position.x) + current.viewport.Width  * 0.5f + d_x;
                var p_y = current.zoom*(currentPlayerPosition.y - current.position.y) + current.viewport.Height * 0.5f + d_y;

                sprite_batch.Draw(tex,
                                  new Vector2(p_x, p_y),
                                  null,
                                  Color.White * 0.9f,
                                  r,
                                  new Vector2(player_indicator_tex.Width/2.0f, player_indicator_tex.Height/2.0f),
                                  current.zoom,
                                  SpriteEffects.None,
                                  0.5f);
            }

        }


        List<Entity> temp_ = new List<Entity>(256);
        public override void draw(float t, float dt)
        {
            sprite_batch.GraphicsDevice.Clear(Color.Black);


            players              = Fab5_Game.inst().get_entities_fast(typeof(Input));
            currentPlayerNumber = players.Count;

            // är det inte samma antal spelare som förut, räkna om antalet och gör om viewports o kameror (viewports, cameras)
            if (currentPlayerNumber != prevPlayerNumber) {
                updatePlayers();
            }

            var entities = Fab5_Game.inst().get_entities_fast(typeof(Sprite));
            var num_entities = entities.Count;

            var temp = temp_;
            temp.Clear();

            for (int i = 0; i < num_entities; i++) {
                var e = entities[i];
                if (e == null) continue; // thread safety
                var pos = e.get_component<Position>();
                if (pos == null) continue; // thread safety
                var px = pos.x;
                var py = pos.y;
                var tex = e.get_component<Sprite >().texture;
                var texw = tex.Width*0.5f;
                var texh = tex.Height*0.5f;
                var left = px+texw;
                var right = px-texw;
                var top = py+texh;
                var bottom = py-texh;

                var in_any_view = false;

                foreach (Camera cam in cameras) {
                    var cx = cam.position.x;
                    var cy = cam.position.y;
                    var hw = cam.viewport.Width/cam.zoom * 0.5f;
                    var hh = cam.viewport.Height/cam.zoom * 0.5f;

                    if ((left   > cx - hw)
                     && (right  < cx + hw)
                     && (top    > cy - hh)
                     && (bottom < cy + hh))
                    {
                        in_any_view = true;
                        break;
                    }
                }

                if (in_any_view) {
                    temp.Add(e);
                }
            }


            entities     = temp;
            num_entities = temp.Count;

            for (int i = 0; i < num_entities; i++) {
                update_sprite(temp[i], dt);
            }

            var hooks = Fab5_Game.inst().get_entities_fast(typeof (Post_Render_Hook));

            for (int p = 0; p < currentPlayerNumber; p++) {
                Camera current = cameras[p];

                sprite_batch.GraphicsDevice.Viewport = current.viewport;

                var currentPlayer         = players[p];
                var currentPlayerPosition = currentPlayer.get_component<Position>();

                current.position.x += ((currentPlayerPosition.x-current.position.x) * 10.0f) * dt;
                current.position.y += ((currentPlayerPosition.y-current.position.y) * 10.0f) * dt;

                var inv_zoom = 1.0f/current.zoom;

                if (current.position.x + 0.5f*current.viewport.Width*inv_zoom  >  2048.0f) current.position.x = 2048.0f - 0.5f*current.viewport.Width*inv_zoom;
                if (current.position.x - 0.5f*current.viewport.Width*inv_zoom  < -2048.0f) current.position.x = -2048.0f + 0.5f*current.viewport.Width*inv_zoom;
                if (current.position.y + 0.5f*current.viewport.Height*inv_zoom >  2048.0f) current.position.y = 2048.0f - 0.5f*current.viewport.Height/current.zoom;
                if (current.position.y - 0.5f*current.viewport.Height*inv_zoom < -2048.0f) current.position.y = -2048.0f + 0.5f*current.viewport.Height*inv_zoom;

                draw_backdrop(sprite_batch, current);
                draw_tile_map(sprite_batch, current);

                drawSprites(sprite_batch, current, num_entities, entities, 0.0f);

                sprite_batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                hudsystem_instance.drawHUD(currentPlayer, dt, current);
                draw_indicators(current, currentPlayerNumber, currentPlayer);

                for (int i = 0; i < hooks.Count; i++) {
                    var hook = hooks[i];
                    hook.get_component<Post_Render_Hook>().render_fn(current, sprite_batch);
                }

                sprite_batch.End();
            }
            sprite_batch.GraphicsDevice.Viewport = defaultViewport;

            /*sprite_batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            draw_match_time();
            sprite_batch.End();*/

            base.draw(t, dt);
        }

        public static int num_begins = 0;
        public static int num_draws = 0;

        private void drawSprites(SpriteBatch sprite_batch, Camera camera, int num_entities, List<Entity> entities, float dt)
        {
            int blend_mode = -1;

            num_draws++;

            var begins = 0;
            for (int i = 0; i < num_entities; i++)
            {
                var entity = entities[i];

                var bm = entity.get_component<Sprite>().blend_mode;
                if (bm != blend_mode) {
                    if (blend_mode != -1) {
                        sprite_batch.End();
                    }

                    blend_mode = bm;

                    BlendState bs = BlendState.AlphaBlend;

                    if (entity.get_component<Sprite>().blend_mode == Sprite.BM_ALPHA) {
                        bs = BlendState.AlphaBlend;
                    }
                    else if (entity.get_component<Sprite>().blend_mode == Sprite.BM_ADD) {
                        bs = BlendState.Additive;
                    }

                    begins++;
                    sprite_batch.Begin(SpriteSortMode.Deferred,
                                       bs, null, null, null, null,
                                       transformMatrix: camera.getViewMatrix(camera.viewport));
                }

                draw_sprite(entity, dt);
            }

            num_begins += begins;

            sprite_batch.End();
        }

        private void update_sprite(Entity entity, float dt) {
            var sprite   = entity.get_component<Sprite>();

            if (sprite.num_frames > 1) {
                sprite.frame_timer += dt;
                if (sprite.frame_timer > (1.0f/sprite.fps)) {
                    sprite.frame_counter++;
                    sprite.frame_timer -= (1.0f/sprite.fps);

                    sprite.frame_x += sprite.frame_width;
                    if (sprite.frame_x >= sprite.texture.Width || sprite.frame_counter >= sprite.num_frames) {
                        sprite.frame_x = 0;

                        if (sprite.frame_counter >= sprite.num_frames) {
                            sprite.frame_counter = 0;
                            sprite.frame_y = 0;
                        }
                        else {
                            sprite.frame_y += sprite.frame_height;
                            if (sprite.frame_y >= sprite.texture.Height || sprite.frame_counter >= sprite.num_frames) {
                                sprite.frame_y = 0;
                                sprite.frame_counter = 0;
                            }
                        }
                    }
                }
            }
        }

        public void draw_sprite(Entity entity, float dt) {
            var position = entity.get_component<Position>();
            var sprite   = entity.get_component<Sprite>();
            var angle    = entity.get_component<Angle>()?.angle ?? 0.0f;

            int frame_width  = sprite.frame_width;
            int frame_height = sprite.frame_height;

            int frame_x = sprite.frame_x;
            int frame_y = sprite.frame_y;

            if (frame_width == 0.0f) {
                frame_width  = sprite.texture.Width;
                frame_height = sprite.texture.Height;
            }

            Rectangle source_rect;

            if (sprite.num_frames > 1) {
                source_rect = new Rectangle(sprite.frame_x, sprite.frame_y, frame_width, frame_height);
            }
            else {
                source_rect = entity.get_component<DrawArea>()?.rectangle ?? new Rectangle(0, 0, frame_width, frame_height);
            }

            sprite_batch.Draw(sprite.texture,
                              new Vector2(position.x, position.y),
                              source_rect,
                              sprite.color,
                              angle,
                              new Vector2(source_rect.Width*0.5f, source_rect.Height*0.5f),
                              new Vector2(sprite.scale_x, sprite.scale_y),
                              SpriteEffects.None,
                              sprite.layer_depth);

    }

    }
}
