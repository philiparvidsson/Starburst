namespace Fab5.Engine {

using Fab5.Engine.Components;
using Fab5.Engine.Core;
using Fab5.Starburst;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


using System;

public class Bouncy_Bullets_Powerup : Powerup_Impl {
    public override Texture2D icon {
        get { return Starburst.inst().get_content<Texture2D>("powerups/bouncybullets"); }
    }

    public Bouncy_Bullets_Powerup()
    {
        time = 60.0f;
    }

    public override void end() {
    }

    public override void begin(Entity holder) {

    }

}

}
