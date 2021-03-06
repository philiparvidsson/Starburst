﻿using Fab5.Engine.Core;
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

        public Background_Renderer(SpriteBatch sb) {
            sprite_batch = sb;
        }
        public override void init() {
            backdrop = Starburst.inst().get_content<Texture2D>("backdrops/menubg");
            stardrop = Starburst.inst().get_content<Texture2D>("backdrops/stardrop");
        }
        public override void draw(float t, float dt) {
            draw_backdrop(sprite_batch, Starburst.inst().get_global_time());
        }
        private void draw_backdrop(SpriteBatch sprite_batch, float t) {
            sprite_batch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            Viewport vp = sprite_batch.GraphicsDevice.Viewport;

            var hw = vp.Width * 0.5f;
            var hh = vp.Height * 0.5f;

            var scale = 1.1f * (vp.Width/1920.0f);
            var x = scale/1.1f * (float)Math.Cos(0.4f * t * 0.07f) * 270.0f;
            var y = scale/1.1f *(float)Math.Sin(0.4f * t * 0.1f) * 200.0f ;


            sprite_batch.Draw(backdrop,
                              new Vector2(hw - (backdrop.Width * 0.5f) * scale + x, hh - (backdrop.Height * 0.5f) * scale + y),
                              null,
                              new Color(0.8f, 0.8f, 0.8f, 0.8f),
                              0.0f,
                              Vector2.Zero,
                              new Vector2(scale, scale),
                              SpriteEffects.None,
                              1.0f);

            sprite_batch.End();


        }
    }
}
