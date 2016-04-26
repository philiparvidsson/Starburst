namespace Fab5.Engine.Subsystems {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

public class Position_Integrator : Subsystem {
    public override void update(float t, float dt) {
        int num_components;

        var entities = Fab5_Game.inst().get_entities(out num_components,
            typeof (Position),
            typeof (Velocity)
        );

        for (int i = 0; i < num_components; i++) {
            var entity   = entities[i];
            var position = entity.get_component<Position>();
            var velocity = entity.get_component<Velocity>();
            var angle    = entity.get_component<Angle>();

            position.x += velocity.x * dt;
            position.y += velocity.y * dt;

            if (angle != null) {
                angle.angle += angle.ang_vel * dt;
            }
        }
    }
}

}
