using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fab5.Engine.Components;
using Fab5.Engine.Core;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

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
        Entity[] players;
        Viewport[] viewports;
        Camera[] cameras;
        int currentPlayerNumber, prevPlayerNumber;
        SpriteBatch sprite_batch;
        Viewport defaultViewport;

        public Tile_Map tile_map;

        Hudsystem hudsystem_instance;

        private Texture2D backdrop;
        private Texture2D stardrop;

        public Rendering_System(GraphicsDevice graphicsDevice) {
            sprite_batch = new SpriteBatch(graphicsDevice);
            defaultViewport = graphicsDevice.Viewport;

            backdrop = Fab5_Game.inst().get_content<Texture2D>("backdrops/backdrop4");
            stardrop = Fab5_Game.inst().get_content<Texture2D>("backdrops/stardrop");
        }

        private void draw_backdrop(SpriteBatch sprite_batch, Position playerPosition) {
            sprite_batch.Begin(SpriteSortMode.Immediate,
                BlendState.Additive);

                var fac1 = 0.05f;
                sprite_batch.Draw(backdrop,
                                  Vector2.Zero,
                                  null,
                                  Color.White,
                                  0.0f,
                                  new Vector2(backdrop.Width/2.0f  + playerPosition.x * fac1,
                                              backdrop.Height/2.0f + playerPosition.y * fac1),
                                  new Vector2(1.5f, 1.5f),
                                  SpriteEffects.None,
                                  1.0f);

                var fac2 = 0.25f;
                sprite_batch.Draw(stardrop,
                                  Vector2.Zero,
                                  null,
                                  Color.White,
                                  0.0f,
                                  new Vector2(stardrop.Width/2.0f  + playerPosition.x * fac2,
                                              stardrop.Height/2.0f + playerPosition.y * fac2),
                                  new Vector2(2.0f, 2.0f),
                                  SpriteEffects.None,
                                  0.9f);
            sprite_batch.End();

        }

        private void draw_tile_map(SpriteBatch sprite_batch, Camera camera) {
            sprite_batch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);

            int tw     = 16;
            int th     = 16;
            int left   = (int)((camera.position.x+2048.0f-camera.viewport.Width*0.5f) / tw);
            int top    = (int)((camera.position.y+2048.0f-camera.viewport.Height*0.5f) / th);
            int right  = (int)(left + camera.viewport.Width / tw);
            int bottom = (int)(top + camera.viewport.Height / th);

//            System.Console.WriteLine(left + ", " + right);

            float xfrac = left*tw - (camera.position.x-camera.viewport.Width*0.5f)-2048.0f;
            float yfrac = top*th - (camera.position.y-camera.viewport.Height*0.5f)-2048.0f;

//            System.Console.WriteLine(camera.position.x + ", " + camera.position.x);

            float x = 0.0f;
            for (int i = left; i <= right; i++) {
                float y = 0.0f;
                for (int j = top; j <= bottom; j++) {
                    if (i < 0 || i > 255 || j < 0 || j > 255)
                        continue;

                    int o = i + (j*256);

                    if (tile_map.tile_tex == null) {
                        tile_map.tile_tex = Fab5_Game.inst().get_content<Texture2D>("tile");
                    }

                    if (tile_map.tiles[o] != 0)
                        sprite_batch.Draw(tile_map.tile_tex, new Vector2(x+xfrac, y+yfrac), Color.White);

                    y += th;
                }

                x += tw;
            }
            sprite_batch.End();
        }

        public override void init()
        {
            // kör uppdatering av viewports och kameror
            updatePlayers();
            this.hudsystem_instance = new Hudsystem(sprite_batch);

 	        base.init();
        }




        private void updatePlayers() {
            // ev hantering för om inga spelare hittas?

            if(currentPlayerNumber == 1) {
                // full screen
                viewports = new Viewport[1];
                cameras = new Camera[1];
                viewports[0] = defaultViewport;
                currentPlayerNumber = 1;
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
            }

            // add cameras to each viewport
            for (int i = 0; i < currentPlayerNumber; i++) {
                cameras[i] = new Camera(viewports[i]);
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
        public override void draw(float t, float dt)
        {
            sprite_batch.GraphicsDevice.Clear(Color.Black);

            // måla ut en eventuell bakgrund/border

            // hämta spelare och dess positioner
            players = Fab5_Game.inst().get_entities(out currentPlayerNumber,
                typeof(Inputhandler)
            );

            // är det inte samma antal spelare som förut, räkna om antalet och gör om viewports o kameror (viewports, cameras)
            if (currentPlayerNumber != prevPlayerNumber)
            {
                updatePlayers();
            }

            // rita ut

            int num_components;
            var entities = Fab5_Game.inst().get_entities(out num_components,
                typeof(Position),
                typeof(Sprite)
            );

             /*
             * Loopa igenom kameror
             *
             * För varje kamera,
             * kör den vanliga draw-loopen baserat på kamerans viewport
             */
            for (int i = 0; i < num_components; i++) {
                update_sprite(entities[i], dt);
                var s1 = entities[i].get_component<Sprite>();

                for (int j = (i+1); j < num_components; j++) {
                    var s2 = entities[j].get_component<Sprite>();

                    if (s1.blend_mode > s2.blend_mode) {
                        var tmp = entities[i];
                        entities[i] = entities[j];
                        entities[j] = tmp;
                    }
                }
            }

            for (int i = 0; i < num_components; i++) {
                var s1 = entities[i].get_component<Sprite>();
                for (int j = (i+1); j < num_components; j++) {
                    var s2 = entities[j].get_component<Sprite>();

                    if (s1.layer_depth < s2.layer_depth) {
                        var tmp = entities[i];
                        entities[i] = entities[j];
                        entities[j] = tmp;
                    }
                }
            }

            for (int p = 0; p < currentPlayerNumber; p++)
            {
                Camera current = cameras[p];
                sprite_batch.GraphicsDevice.Viewport = current.viewport;

                var currentPlayer = players[p];
                var currentPlayerPosition = currentPlayer.get_component<Position>();
                cameras[p].position = currentPlayerPosition;

                draw_backdrop(sprite_batch, currentPlayerPosition);


                //drawHUD(sprite_batch, entity, currentPlayerNumber);
                draw_tile_map(sprite_batch, current);
                drawSprites(sprite_batch, current, num_components, entities, 0.0f);
                hudsystem_instance.drawHUD(currentPlayer);
            }
            sprite_batch.GraphicsDevice.Viewport = defaultViewport;
            base.draw(t, dt);
        }

        private void drawSprites(SpriteBatch sprite_batch, Camera camera, int num_components, Entity[] entities, float dt)
        {
            int blend_mode = -1;

            for (int i = 0; i < num_components; i++)
            {
                var entity = entities[i];

                if (entity.get_component<Sprite>().blend_mode != blend_mode) {
                    if (blend_mode != -1) {
                        sprite_batch.End();
                    }

                    blend_mode = entity.get_component<Sprite>().blend_mode;

                    BlendState bs = BlendState.AlphaBlend;

                    if (entity.get_component<Sprite>().blend_mode == Sprite.BM_ALPHA) {
                        bs = BlendState.AlphaBlend;
                    }
                    else if (entity.get_component<Sprite>().blend_mode == Sprite.BM_ADD) {
                        bs = BlendState.Additive;
                    }

                    sprite_batch.Begin(SpriteSortMode.Immediate,
                                       bs, null, null, null, null,
                                       transformMatrix: camera.getViewMatrix(camera.viewport));
                }

                draw_sprite(entity, dt);
            }

            sprite_batch.End();
        }

        private void update_sprite(Entity entity, float dt) {
            var sprite   = entity.get_component<Sprite>();
            var hud = entity.get_component<Hud_Component>();
            if (hud != null)
                return;

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
                frame_width = sprite.texture.Width;
                frame_height = sprite.texture.Height;
            }
            var source_rect = entity.get_component<DrawArea>()?.rectangle ?? new Rectangle(0, 0, frame_width, frame_height);

            if (sprite.num_frames > 1) {
                source_rect = new Rectangle(sprite.frame_x, sprite.frame_y, frame_width, frame_height);
            }

            sprite_batch.Draw(sprite.texture,
                              new Vector2(position.x, position.y),
                              source_rect,
                              sprite.color,
                              angle,
                              new Vector2(source_rect.Width/2.0f, source_rect.Height/2.0f),
                              sprite.scale,
                              SpriteEffects.None,
                              sprite.layer_depth);

    }

    }
}
