namespace Fab5.Starburst.States.Falling_Ball.Entities {

using Fab5.Engine;
using Fab5.Engine.Components;
using Fab5.Engine.Core;

using Microsoft.Xna.Framework.Graphics;

public static class Ball {

    public static Component[] create_components()  {
        return new Component[] {
             new Position(),
             new Velocity() { x = 5.0f, y = 92.0f },

             new Sprite() {
                 texture = Starburst.inst().get_content<Texture2D>("ball")
             }
        };
    }

}

}
