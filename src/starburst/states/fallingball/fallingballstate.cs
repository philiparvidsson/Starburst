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
            new Text_Renderer(new SpriteBatch(Starburst.inst().GraphicsDevice)),
            new Window_Title_Writer()
        );

        create_entity(Ball.create_components());
        create_entity(new FpsCounter());
        create_entity(
            new Position() { x = 10.0f, y = 10.0f },
            new Text() {
                font = Starburst.inst().get_content<SpriteFont>("arial"),
                format = "this is a bitchin' string!" });
    }

    public override void update(float t, float dt) {
        base.update(t, dt);

        if (t > 2.0f) {
            Starburst.inst().leave_state();
        }
    }

}

}
