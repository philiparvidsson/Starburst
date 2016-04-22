namespace Fab5.Starburst.States {

using Fab5.Engine;
using Fab5.Engine.Components;
using Fab5.Engine.Core;
using Fab5.Engine.Subsystems;

using Fab5.Starburst.States.Playing.Entities;
using Microsoft.Xna.Framework.Graphics;

public class Playing_State : Game_State {

    public override void init() {
        add_subsystems(
            new Position_Integrator(),
            new Inputhandler_System(),
            new Sprite_Renderer(new SpriteBatch(Starburst.inst().GraphicsDevice)),
            new Text_Renderer(new SpriteBatch(Starburst.inst().GraphicsDevice)),
            new Window_Title_Writer(),
            new Collision_Solver(),
            new Sound()
        );

        create_entity(new FpsCounter());
        create_entity(Player_Ship.create_components());
        create_entity(new BackgroundMusic("sound/SpaceLoungeLoop", true));

        create_entity(Dummy.create_components());

        var p2 = create_entity(Dummy.create_components()).get_component<Position>();
        p2.x = 800.0f;
        p2.y = 300.0f;

    }

}

}
