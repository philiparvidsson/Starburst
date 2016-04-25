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
                drawSprites(sprite_batch, current, num_components, entities);
            }
            sprite_batch.GraphicsDevice.Viewport = defaultViewport;
            base.draw(t, dt);
        }
        private void drawSprites(SpriteBatch sprite_batch, Camera camera, int num_components, Entity[] entities)
        {
            sprite_batch.Begin(SpriteSortMode.FrontToBack,
                BlendState.AlphaBlend, null, null, null, null,
                transformMatrix: camera.getViewMatrix(camera.viewport));

            for (int i = 0; i < num_components; i++)
            {
                var entity = entities[i];
                var position = entity.get_component<Position>();
                var sprite = entity.get_component<Sprite>();
                var angle = entity.get_component<Angle>();

                if (angle == null)
                    sprite_batch.Draw(sprite.texture, new Vector2(position.x, position.y), Color.White);
                else
                    sprite_batch.Draw(
                        texture: sprite.texture,
                        position: new Vector2(position.x, position.y),
                        sourceRectangle: null,
                        color: Color.White,
                        rotation: angle.angle,
                        origin: new Vector2(sprite.texture.Width * .5f, sprite.texture.Height * .5f),
                        scale: 1f,
                        effects: SpriteEffects.None,
                        layerDepth: sprite.depth
                        );
            }

            sprite_batch.End();
        }
    }
}
