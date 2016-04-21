namespace Fab5.Starburst.States {

using Fab5.Engine;
using Fab5.Engine.Components;
using Fab5.Engine.Core;
using Fab5.Engine.Subsystems;

using Fab5.Starburst.States.Falling_Ball.Entities;

using Microsoft.Xna.Framework.Graphics;

public class Falling_Ball_State : Game_State {

    public override void init() {
        add_subsystems(
            new Position_Integrator(),
            new Sprite_Renderer(new SpriteBatch(Starburst.inst().GraphicsDevice)),
            new Window_Title_Writer()
        );

        create_entity(Ball.create_components());
        create_entity(new FpsCounter());
    }

    public override void update(float t, float dt) {
        base.update(t, dt);

        if (t > 2.0f) {
            Starburst.inst().leave_state();
        }
    }

}

}
