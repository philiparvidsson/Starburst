namespace Fab5.Engine.Subsystems {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using System;

public class Collision_Solver : Subsystem {
    public override void update(float t, float dt) {
        int num_components;

        var entities = Fab5_Game.inst().get_entities(out num_components,
            typeof (Bounding_Circle),
            typeof (Position),
            typeof (Velocity)
        );

        // @To-do: Implement quad tree or spatial  grid here.
        for (int i = 0; i < num_components; i++) {
            var entity1          = entities[i];
            var bounding_circle1 = entity1.get_component<Bounding_Circle>();
            var position1        = entity1.get_component<Position>();
            var velocity1        = entity1.get_component<Velocity>();
            var mass1            = entity1.get_component<Mass>();

            if (position1.x < 0.0f) {
                position1.x = 0.0f;
                velocity1.x *= -1.0f;
            }

            if (position1.x > 1279.0f) {
                position1.x = 1279.0f;
                velocity1.x *= -1.0f;
            }

            if (position1.y < 0.0f) {
                position1.y = 0.0f;
                velocity1.y *= -1.0f;
            }

            if (position1.y > 719.0f) {
                position1.y = 719.0f;
                velocity1.y *= -1.0f;
            }


            for (int j = (i+1); j < num_components; j++) {
                var entity2          = entities[j];
                var bounding_circle2 = entity2.get_component<Bounding_Circle>();
                var position2        = entity2.get_component<Position>();
                var velocity2        = entity2.get_component<Velocity>();
                var mass2            = entity2.get_component<Mass>();

                var r_sum = (bounding_circle1.radius + bounding_circle2.radius);
                var r2    = r_sum * r_sum;// Square sum of radii to avoid roots!
                var d_x   = position2.x - position1.x;
                var d_y   = position2.y - position1.y;
                var d2_x  = d_x * d_x;
                var d2_y  = d_y * d_y;
                var d2    = d2_x + d2_y;

                if (d2 > r2 || d2 <= 0.000001f) {
                    // Squared distance is greater than squared radii, no collision!
                    continue;
                }

                var d = (float)Math.Sqrt(d2);

                d_x /= d2;
                d_y /= d2;

                position1.x -= r_sum * d_x;
                position1.y -= r_sum * d_y;
                position2.x += r_sum * d_x;
                position2.y += r_sum * d_y;

                // Impulse and impact vectors ftw
                var impact_x  = velocity2.x - velocity1.x;
                var impact_y  = velocity2.y - velocity1.y;
                var impulse_x = position2.x - position1.x;
                var impulse_y = position2.y - position1.y;
                var r         = (float)Math.Sqrt((impulse_x * impulse_x) + (impulse_y * impulse_y));

                impulse_x /= r;
                impulse_y /= r;

                var m1 = 1.0f;
                var m2 = 1.0f;

                if (mass1 != null) m1 = mass1.mass;
                if (mass2 != null) m2 = mass2.mass;

                // Dot product to get impulse factor lewl
                var imp = (impulse_x * impact_x + impulse_y * impact_y);
                var fac = (float)(imp * Math.Sqrt(m1 * m2));

                impulse_x *= fac;
                impulse_y *= fac;

                if (m1 > 0.0f) {
                    velocity1.x += impulse_x / m1;
                    velocity1.y += impulse_y / m1;
                }

                if (m2 > 0.0f) {
                    velocity2.x -= impulse_x / m2;
                    velocity2.y -= impulse_y / m2;
                }

            }
        }
    }
}

}
