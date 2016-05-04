namespace Fab5.Starburst {

using Fab5.Engine;
using Fab5.Engine.Components;
using Fab5.Engine.Core;
using Fab5.Engine.Subsystems;

using Fab5.Starburst.States;

using Microsoft.Xna.Framework.Graphics;

// Starburst game implementation.
public class Starburst : Fab5_Game {
        public Starburst() {
            GraphicsMgr.HardwareModeSwitch = false;
        }
    protected override void init() {
            if (false && GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height > 800 && GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width > 1400) {
                GraphicsMgr.PreferredBackBufferWidth = 1280;
                GraphicsMgr.PreferredBackBufferHeight = 680;
            }
            else {
                GraphicsMgr.PreferredBackBufferWidth = 1920;
                GraphicsMgr.PreferredBackBufferHeight = 1080;
            }
       // GraphicsMgr.PreferMultiSampling = true;

            //        GraphicsMgr.GraphicsDevice.RasterizerState = new RasterizerState { MultiSampleAntiAlias = true };



        GraphicsMgr.SynchronizeWithVerticalRetrace = false;
        IsFixedTimeStep = false;



            GraphicsMgr.ApplyChanges();
            GraphicsMgr.ToggleFullScreen();
        Microsoft.Xna.Framework.Media.MediaPlayer.Volume = 0.7f;

        enter_state(new Splash_Screen_State());

        var form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(this.Window.Handle);
        form.Location = new System.Drawing.Point(300, 200);

    }

    protected override void cleanup() {
    }

    protected override void update(float t, float dt) {
    }

    protected override void draw(float t, float dt) {
    }

    static void Main() {
        using (var game = new Starburst()) {
            game.run();
        }
    }
}

}
