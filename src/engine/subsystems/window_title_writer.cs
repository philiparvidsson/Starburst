namespace Fab5.Engine.Subsystems {

    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using Fab5.Starburst;


    public class Window_Title_Writer : Subsystem
    {
        float elapsedTime = 0;

        public override void on_message(string msg, dynamic data) {
            if (msg == "set_w_title") {
                Fab5_Game.inst().Window.Title = data.str;
            }
        }

        int display_fps = 0;
        SpriteBatch sprite_batch;

        public override void init() {
            sprite_batch = new SpriteBatch(Starburst.inst().GraphicsDevice);
        }
        public override void draw(float t, float dt)
        {
            var entities = Fab5_Game.inst().get_entities_fast(typeof(FpsCounter));
            int num_entities = entities.Count;
            for (int i = 0; i < num_entities; i++)
            {
                var entity = entities[i];
                var fps = entity.get_component<FpsCounter>();
                fps.frameCounter += 1;
                elapsedTime += dt;

                if (elapsedTime > 1)
                {
                    elapsedTime -= 1;
                    fps.frameRate = fps.frameCounter;
                    fps.frameCounter = 0;

                    display_fps = (int)fps.frameRate;

                    Fab5_Game.inst().message("set_w_title", new { str = " * Starburst * " + fps.frameRate });
                }
            }

            sprite_batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            GFX_Util.draw_def_text(sprite_batch, "FPS: " + display_fps, 10, 90);
            sprite_batch.End();
        }

    }
}
