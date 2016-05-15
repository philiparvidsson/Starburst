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
                var dtm = dt*mass.drag_coeff;
                velocity.x -= velocity.x * dtm;
                velocity.y -= velocity.y * dtm;
            }

            velocity.x += velocity.ax * dt;
            velocity.y += velocity.ay * dt;

            position.x += velocity.x * dt;
            position.y += velocity.y * dt;

            if (angle != null) {
                angle.angle += angle.ang_vel * dt;

                while (angle.angle <= 0.0f) {
                    angle.angle += 2.0f*3.141592653f;
                }

                while (angle.angle >= 2.0f*3.141592653f) {
                    angle.angle -= 2.0f*3.141592653f;
                }

                angle.ang_vel -= angle.drag * angle.ang_vel * dt;
            }
        }
    }
}

}
