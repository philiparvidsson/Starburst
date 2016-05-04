using Fab5.Engine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fab5.Starburst.States.Menus.Subsystems {
    public class Background_Renderer : Subsystem {
        SpriteBatch sprite_batch;
        private Texture2D backdrop;
        private Texture2D stardrop;

        private float elapsedTime;
        private float animationTime = 5f; //tid att komma till slut av animation (lika lång tid för att komma tillbaka)
        private float halfwayTime;

        private float posX = 200; // ursprungsposition
        private float posY = 200;
        private float goalX = 500;
        private float goalY = 500;

        public Background_Renderer(SpriteBatch sb) {
            sprite_batch = sb;
        }
        public override void init() {
            backdrop = Starburst.inst().get_content<Texture2D>("backdrops/menubg");
            stardrop = Starburst.inst().get_content<Texture2D>("backdrops/stardrop");
        }
        public override void draw(float t, float dt) {
            draw_backdrop(sprite_batch, t);
        }
        private void draw_backdrop(SpriteBatch sprite_batch, float t) {
            sprite_batch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            Viewport vp = sprite_batch.GraphicsDevice.Viewport;

            var hw = vp.Width * 0.5f;
            var hh = vp.Height * 0.5f;

            var scale = 1.5f;
            var x = (float)Math.Cos(0.4f * t * 0.07f) * 270.0f - scale * backdrop.Width * 0.5f + hw;
            var y = (float)Math.Sin(0.4f * t * 0.1f) * 200.0f - scale * backdrop.Height * 0.5f + hh;

            var fac1 = 0.05f;
            sprite_batch.Draw(backdrop,
                              new Vector2(hw - (backdrop.Width * 0.5f + x * fac1) * 1.5f, hh - (backdrop.Height * 0.5f + y * fac1) * 1.5f),
                              null,
                              new Color(0.8f, 0.8f, 0.8f, 0.8f),
                              0.0f,
                              Vector2.Zero,
                              new Vector2(1.5f, 1.5f),
                              SpriteEffects.None,
                              1.0f);


            var fac2 = 0.25f;
            sprite_batch.Draw(stardrop,
                              new Vector2(hw - (stardrop.Width * 0.5f + x * fac2) * 2.0f, hh - (stardrop.Height * 0.5f + y * fac2) * 2.0f),
                              null,
                              Color.White,
                              0.0f,
                              Vector2.Zero,
                              new Vector2(2.0f, 2.0f),
                              SpriteEffects.None,
                              0.9f);

            sprite_batch.End();


        }
    }
}
