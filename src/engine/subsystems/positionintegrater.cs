namespace Fab5.Engine.Subsystems {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

public class Position_Integrator : Subsystem {
    public override void update(float t, float dt) {

        var entities = Fab5_Game.inst().get_entities_fast(typeof (Velocity));
        int num_entities = entities.Count;

        for (int i = 0; i < num_entities; i++) {
            var entity   = entities[i];
            var position = entity.get_component<Position>();
            var velocity = entity.get_component<Velocity>();
            var angle    = entity.get_component<Angle>();
            var mass     = entity.get_component<Mass>();

            if (mass != null) {
                velocity.x -= velocity.x * dt * mass.drag_coeff;
                velocity.y -= velocity.y * dt * mass.drag_coeff;
            }

            position.x += velocity.x * dt;
            position.y += velocity.y * dt;



            if (angle != null) {
                angle.angle += angle.ang_vel * dt;

                angle.ang_vel -= angle.drag * angle.ang_vel * dt;
            }
        }
    }
}

}
