namespace Game.Entities {

using Engine.Components;
using Engine.Core;

public class Ball : Base_Entity {

    public Ball() {
        add_components(
             new C_Position(),
             new C_Velocity() { x = 100.0f, y = 12.0f },
             new C_Sprite(),
             new FpsCounter()
         );
    }

}

}
