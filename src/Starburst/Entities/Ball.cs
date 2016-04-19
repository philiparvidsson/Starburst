namespace Starburst.Entities {

using Engine;
using Engine.Components;
using Engine.Core;

using Microsoft.Xna.Framework.Graphics;

public static class Ball {

    public static Component[] create_components()  {
        return new Component[] {
             new C_Position(),
             new C_Velocity() { x = 5.0f, y = 92.0f },

             new C_Sprite() {
                 texture = Game_Engine.inst().get_content<Texture2D>("ball")
             }
        };
    }

}

}
