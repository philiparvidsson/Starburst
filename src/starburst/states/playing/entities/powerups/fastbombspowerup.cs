namespace Fab5.Engine {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Fab5.Starburst;

using System;

public class Fast_Bombs_Powerup : Powerup_Impl {
    public override Texture2D icon {
        get { return Starburst.inst().get_content<Texture2D>("powerups/fastbombs"); }
    }

    public override void end() {
    }

    public override void begin(Entity holder) {
        time = 5.0f;
    }

}

}
