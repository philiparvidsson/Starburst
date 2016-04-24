namespace Fab5.Engine.Subsystems {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using System;

// Solves collision between bounding circles.
public class Collision_Solver : Subsystem {
    public override void update(float t, float dt) {
        // Collisions occur at an instant so who cares about dt?

        int num_entities;

        var entities = Fab5_Game.inst().get_entities(out num_entities,
            typeof (Bounding_Circle),
            typeof (Position),
            typeof (Velocity)
        );

        // @To-do: Implement a quad tree or spatial grid here to reduce the
        //         number of candidates for collision testing.
        for (int i = 0; i < num_entities; i++) {
            var e1 = entities[i];
            var c1 = e1.get_component<Bounding_Circle>();
            var p1 = e1.get_component<Position>();
            var v1 = e1.get_component<Velocity>();
            var m1 = e1.get_component<Mass>()?.mass ?? 1.0f;

            if (p1.x < 0.0f) {
                p1.x = 0.0f;
                v1.x *= -1.0f;
            }

            if (p1.x > 1279.0f) {
                p1.x = 1279.0f;
                v1.x *= -1.0f;
            }

            if (p1.y < 0.0f) {
                p1.y = 0.0f;
                v1.y *= -1.0f;
            }

            if (p1.y > 719.0f) {
                p1.y = 719.0f;
                v1.y *= -1.0f;
            }

            for (int j = (i+1); j < num_entities; j++) {
                var e2 = entities[j];

                resolve_collision(e1, e2);
            }
        }
    }

    private void resolve_collision(Entity e1, Entity e2) {
        var c1     = e1.get_component<Bounding_Circle>();
        var c2     = e2.get_component<Bounding_Circle>();
        var p1     = e1.get_component<Position>();
        var p2     = e2.get_component<Position>();
        var p_x    = p2.x - p1.x;
        var p_y    = p2.y - p1.y;
        var r2     = p_x*p_x + p_y*p_y;
        var r2_max = (c1.radius+c2.radius) * (c1.radius+c2.radius);

        if (r2 < 0.0000001f || r2 > r2_max) {
            // No penetration or full penetration (which cannot be
            // solved in a sane way).
            return;
        }

        var r   = (float)Math.Sqrt(r2);
        var p   = c1.radius+c2.radius - r;
        var v1  = e1.get_component<Velocity>();
        var v2  = e2.get_component<Velocity>();
        var v_x = v1.x - v2.x;
        var v_y = v1.y - v2.y;
        var m1  = e1.get_component<Mass>()?.mass ?? 1.0f;
        var m2  = e2.get_component<Mass>()?.mass ?? 1.0f;
        var f1  = 1.0f - m1/(m1+m2);
        var f2  = 1.0f - m2/(m1+m2);

        // Normalize difference in position.
        p_x /= r;
        p_y /= r;

        // Move apart to fix penetration.
        p1.x -= p_x*p*f1;
        p1.y -= p_y*p*f1;
        p2.x += p_x*p*f2;
        p2.y += p_y*p*f2;

        // Dot product to get cosine of angle between the two vectors
        // times the magnitude of v.
        var d = p_x*v_x + p_y*v_y;

        if (d < 0.0f) {
            // Moving away from each other.
            return;
        }

        // @To-do: Multiply with restitution here.

        // Newton's third law.
        p_x *= d*2.0f;
        p_y *= d*2.0f;

        v1.x -= p_x*f1;
        v1.y -= p_y*f1;
        v2.x += p_x*f2;
        v2.y += p_y*f2;
    }
}

}
