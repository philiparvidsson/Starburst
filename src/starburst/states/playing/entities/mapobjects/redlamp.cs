namespace Fab5.Starburst.States.Playing.Entities {

using Fab5.Engine.Components;
using Fab5.Engine.Core;
using Fab5.Engine;

using System;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

public static class Red_Lamp {
    private static System.Random rand = new System.Random();

    public static Component[] create_components() {
        var pos  = new Position { x = 0.0f, y = 0.0f };

        return new Component[] {
            new Light_Source { color = new Color(1.0f, 0.1f, 0.1f), intensity = 0.55f, size = 3.0f },
            pos,
        };
    }

}

}
