namespace Starburst.Entities {

using Engine;
using Engine.Components;
using Engine.Core;

using Microsoft.Xna.Framework.Graphics;

public class Ball : Base_Entity {

    public Ball() {
        add_components(
             new C_Position(),
             new C_Velocity() { x = 50.0f, y = 92.0f },
             new FpsCounter(),
             new C_Sprite() {
                 texture = Game_Engine.inst().get_content<Texture2D>("ball")
             }
         );
    }

}

}
