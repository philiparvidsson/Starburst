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

        float display_begins;
        int display_num_entities;
        float  display_tris;

        public override void on_message(string msg, dynamic data) {
            if (msg == "set_w_title" && Fab5_Game.inst().Window != null) {
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
                    display_begins = (float)Rendering_System.num_begins / (float)Rendering_System.num_draws;
                    display_tris = (float)Rendering_System.tri_counter / (float)Rendering_System.tri_frame_counter;

                    Rendering_System.num_begins = 0;
                    Rendering_System.num_draws = 0;

                    Rendering_System.tri_counter = 0;
                    Rendering_System.tri_frame_counter = 0;

                    Fab5_Game.inst().message("set_w_title", new { str = " * Starburst * " + fps.frameRate });
                    display_num_entities = state.get_num_entities();
                }
            }

            sprite_batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            GFX_Util.draw_def_text(sprite_batch, "fps: " + display_fps, 10, 90);
            GFX_Util.draw_def_text(sprite_batch, "begins: " + string.Format("{0:0.00}", display_begins), 10, 126);
            GFX_Util.draw_def_text(sprite_batch, "entities: " + display_num_entities, 10, 162);
            GFX_Util.draw_def_text(sprite_batch, "triangles: " + display_tris, 10, 202);
            sprite_batch.End();
        }

    }
}
