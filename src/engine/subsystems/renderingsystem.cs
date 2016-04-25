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

        public Rendering_System(GraphicsDevice graphicsDevice) {
            sprite_batch = new SpriteBatch(graphicsDevice);
            defaultViewport = graphicsDevice.Viewport;
        }

        public override void  init()
        {
            // kör uppdatering av viewports och kameror
            updatePlayers();



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
            else {
                // 1/4 screen, handle sizes and positions
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

            for (int p = 0; p < currentPlayerNumber; p++)
            {
                Camera current = cameras[p];
                sprite_batch.GraphicsDevice.Viewport = current.viewport;

                var currentPlayer = players[p];
                var currentPlayerPosition = currentPlayer.get_component<Position>();
                cameras[p].position = currentPlayerPosition;

                //drawBackground(sprite_batch, currentPlayerPosition);
                //drawHUD(sprite_batch, entity, currentPlayerNumber);
                drawSprites(sprite_batch, current, num_components, entities, dt);
            }
            sprite_batch.GraphicsDevice.Viewport = defaultViewport;
            base.draw(t, dt);
        }

        private void drawSprites(SpriteBatch sprite_batch, Camera camera, int num_components, Entity[] entities, float dt)
        {
            sprite_batch.Begin(SpriteSortMode.FrontToBack,
                BlendState.AlphaBlend, null, null, null, null,
                transformMatrix: camera.getViewMatrix(camera.viewport));

            for (int i = 0; i < num_components; i++)
            {
                var entity = entities[i];

                draw_sprite(entity, dt);
            }

            sprite_batch.End();
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

            var source_rect = new Rectangle(0, 0, frame_width, frame_height);

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

                source_rect = new Rectangle(sprite.frame_x, sprite.frame_y, frame_width, frame_height);
            }

            sprite_batch.Draw(sprite.texture,
                              new Vector2(position.x, position.y),
                              source_rect,
                              sprite.color,
                              angle,
                              new Vector2(frame_width/2.0f, frame_height/2.0f),
                              sprite.scale,
                              SpriteEffects.None,
                              0.5f);

    }

    }
}
