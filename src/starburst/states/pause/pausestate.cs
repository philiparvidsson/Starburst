namespace Fab5.Starburst.States {

using Fab5.Engine;
using Fab5.Engine.Components;
using Fab5.Engine.Core;
using Fab5.Engine.Subsystems;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using System;
using System.Threading;

public class Pause_State : Game_State {
    private bool can_unpause = false;
    private float restore_vol;
        private Game_State last_state;


        public Pause_State(Game_State last_state)
        {
            this.last_state = last_state;
        }
    public override void init() {
        /*Gamepad_Util.vibrate(0, 0.0f, 0.0f);
        Gamepad_Util.vibrate(1, 0.0f, 0.0f);
        Gamepad_Util.vibrate(2, 0.0f, 0.0f);
        Gamepad_Util.vibrate(3, 0.0f, 0.0f);*/

        var sprite_batch = new SpriteBatch(Starburst.inst().GraphicsDevice);

        var w = Starburst.inst().GraphicsMgr.PreferredBackBufferWidth;
        var h = Starburst.inst().GraphicsMgr.PreferredBackBufferHeight;

        sprite_batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

        GFX_Util.fill_rect(sprite_batch, new Rectangle(0, 0, w, h), Color.Black * 0.5f);

            var s = new[] { "one", "two", "three", "four" };

            var text_size = GFX_Util.measure_string("Paused");
        var tx = (w-text_size.X)*0.5f;
        var ty = (h-text_size.Y)*0.5f;

            var pList = last_state.get_entities_fast(typeof(Ship_Info));

            int i = 30;

            foreach(var player in pList){
                Score player_score = player.get_component<Score>();
                Ship_Info player_shipinfo = player.get_component<Ship_Info>();

                if (player_score == null || player_shipinfo.pindex >= 5) continue;

                var scoretext = string.Format("Player {0} stats;    Kills: {1}  Death: {2}  Score: {3}", s[player_shipinfo.pindex - 1], player_score.num_kills, player_score.num_deaths, player_score.display_score);

                var scoretext_size = GFX_Util.measure_string(scoretext);
                var xx = (w - scoretext_size.X) * 0.5f;
                GFX_Util.draw_def_text(sprite_batch, scoretext, xx, i);

                int pad_size = 6;

                GFX_Util.fill_rect(sprite_batch, new Rectangle((int)xx - pad_size, i - pad_size, (int)scoretext_size.X + (pad_size * 2), (int)scoretext_size.Y + (pad_size * 2)), Color.AliceBlue * 0.2f);

                i *= 3;

            }


            /*
                            var s     = new [] { "one", "two", "three", "four" };
                            var text2 = string.Format("Killed by player {0}!", s[bulletInfo.sender.get_component<Ship_Info>().pindex-1]);
                            var ts1   = GFX_Util.measure_string("Respawning in 0.00");
                            var ts2   = GFX_Util.measure_string(text2);

                            var text3 = string.Format("Kills: {0}", player_kills);
                            var text4 = string.Format("Deaths: {0}", player_deaths);

                            var a = 0.5f*(float)Math.Min(Math.Max(0.0f, (10.0f-t*2.0f)), 1.0f);
                            var a2 = (float)Math.Min(Math.Max(0.0f, (3.0f*t*(1.0f/5.0f)-1.0f)), 1.0f);
                            GFX_Util.fill_rect(sprite_batch, new Rectangle(0, 0, camera.viewport.Width, camera.viewport.Height), Color.Black * a);
                            GFX_Util.draw_def_text(sprite_batch, text2, (camera.viewport.Width-ts2.X)*0.5f, 90.0f, a2);
                            GFX_Util.draw_def_text(sprite_batch, text3, (camera.viewport.Width-ts2.X)*0.5f, (camera.viewport.Height-ts2.Y)*0.5f-90.0f);
                            GFX_Util.draw_def_text(sprite_batch, text4, (camera.viewport.Width-ts2.X)*0.5f, (camera.viewport.Height-ts2.Y)*0.5f-130.0f);
                            GFX_Util.draw_def_text(sprite_batch, text1, (camera.viewport.Width-ts1.X)*0.5f, (camera.viewport.Height-ts1.Y)*0.5f);
                            
             
        */
        GFX_Util.draw_def_text(sprite_batch, "Paused", tx, ty);

        sprite_batch.End();

        restore_vol = MediaPlayer.Volume;
        MediaPlayer.Volume *= 0.4f;
    }

    public override void draw(float t, float dt) {
        if (can_unpause) {
            for (int i = 0; i <= 3; i++) {
                if (GamePad.GetState((PlayerIndex)i).IsConnected && GamePad.GetState((PlayerIndex)i).Buttons.Start == ButtonState.Pressed) {
                    MediaPlayer.Volume = restore_vol;
                    Starburst.inst().leave_state();
                    return;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                MediaPlayer.Volume = restore_vol;
                Starburst.inst().leave_state();
                return;
            }
        }
        else {
            bool no_buttons_pressed = true;

            for (int i = 0; i <= 3; i++) {
                if (GamePad.GetState((PlayerIndex)i).IsConnected && GamePad.GetState((PlayerIndex)i).Buttons.Start == ButtonState.Pressed) {
                    no_buttons_pressed = false;
                    break;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                no_buttons_pressed = false;
            }

            if (no_buttons_pressed) {
                can_unpause = true;
            }
        }

        Thread.Sleep(10);
    }
}

}
