namespace Fab5.Starburst.States {

    using Fab5.Engine;
    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Fab5.Engine.Subsystems;

    using Fab5.Starburst.States.Playing.Entities;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    using System;

    public class Splash_Screen_State : Game_State {
        float elapsedTime;
        float delay = .5f;
        float duration = 2f;
        float outDelay;
        float displayTime = 2f;
        float splashTime;
        Texture2D splash;
        SpriteFont font;

        bool preloading;
        bool preloaded = false;

        string asset_name;
        int percent_preloaded;

        public override void init() {
            splash = Starburst.inst().get_content<Texture2D>("splash");
            font = Starburst.inst().get_content<SpriteFont>("sector034");
            outDelay = delay + duration + displayTime;
            splashTime = outDelay + duration;
        }

        public override void update(float t, float dt) {
            base.update(t, dt);
            elapsedTime += dt;

            KeyboardState state = Keyboard.GetState();
            var skip_button_pressed = state.IsKeyDown(Keys.Enter) && preloaded;

            for (int i = 0; i <= 3; i++) {
                if (GamePad.GetState((PlayerIndex)i).IsConnected && GamePad.GetState((PlayerIndex)i).Buttons.Start == ButtonState.Pressed) {
                    skip_button_pressed = true;
                    break;
                }
            }

            if(elapsedTime >= splashTime || skip_button_pressed) {
                Starburst.inst().enter_state(new Main_Menu_State());
                return;
            }

            if (state.IsKeyDown(Keys.LeftAlt) &&
                state.IsKeyDown(Keys.Enter))
            {
                Starburst.inst().GraphicsMgr.ToggleFullScreen();
            }

            if (!preloading && t > duration+0.5f) {
                Starburst.inst().begin_preload_content((s, p) => {
                    asset_name = s;
                    percent_preloaded = (int)Math.Round(p*100.0f);
                });
                preloading = true;
            }
            if (preloading && !preloaded) {
                preloaded = !Starburst.inst().preload_next();
                elapsedTime -= dt; // wait until loaded
            }

            System.Threading.Thread.Sleep(1);
        }

        public override void draw(float t, float dt) {
            base.draw(t, dt);

            Starburst.inst().GraphicsDevice.Clear(Color.Black);
            SpriteBatch sprite_batch = new SpriteBatch(Starburst.inst().GraphicsDevice);
            Viewport vp = sprite_batch.GraphicsDevice.Viewport;
            sprite_batch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied);
            Rectangle destRect = new Rectangle((int)(sprite_batch.GraphicsDevice.Viewport.Width * .5 - splash.Width * .5), (int)(sprite_batch.GraphicsDevice.Viewport.Height * .5 - splash.Height * .5), splash.Width, splash.Height);
            if (elapsedTime > delay && elapsedTime < outDelay) {
                sprite_batch.Draw(splash, destRect, new Color(255, 255, 255, quadInOut(delay, 0, 1)));
                //sprite_batch.DrawString(font, "In: " + quadInOut(delay, 0, 1), new Vector2(20, 20), Color.White);
            }
            else if (elapsedTime >= outDelay){
                sprite_batch.Draw(splash, destRect, new Color(255, 255, 255, 1-quadInOut(outDelay, 0, 1)));
                //sprite_batch.DrawString(font, "Out: " + (1-quadInOut(outDelay, 0, 1)), new Vector2(20, 20), Color.White);
            }

            if (preloaded || preloading) {
                String text = preloaded ? "Press enter to skip" : "Loading... " + percent_preloaded + "%";
                Vector2 textSize = font.MeasureString(!preloaded ? "Loading... 100%" : "Press enter to skip");
                sprite_batch.DrawString(font, text, new Vector2(vp.Width * .5f - textSize.X * .5f, vp.Height - textSize.Y - 20), Color.White);
            }

            sprite_batch.End();

            System.Threading.Thread.Sleep(1);
        }

        private float quadInOut(float delayVal, float b, float c) {
            // b - start value
            // c - final value
            float t = elapsedTime-delayVal; // current time in seconds
            float d = duration; // duration of animation

            if (t == 0) {
                return b;
            }

            if (t == d) {
                return b + c;
            }

            if ((t /= d / 2) < 1) {
                return c / 2 * (float)Math.Pow(2, 10 * (t - 1)) + b;
            }

            return c / 2 * (-(float)Math.Pow(2, -10 * --t) + 2) + b;
        }

    }

}
