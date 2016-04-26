namespace Fab5.Engine.Subsystems {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using System;

using Microsoft.Xna.Framework;

// Solves collision between bounding circles.
public class Collision_Solver : Subsystem {
    private readonly Tile_Map tile_map;

    public Collision_Solver(Tile_Map tile_map) {
        this.tile_map = tile_map;
    }

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
            var e1  = entities[i];
            var p1  = e1.get_component<Position>();
            var v1  = e1.get_component<Velocity>();
            var rc1 = e1.get_component<Mass>()?.restitution_coeff ?? 1.0f;

            if (p1.x < -2048.0f) {
                p1.x = -2048.0f;
                v1.x = -v1.x * rc1;
            }

            if (p1.x > 2048.0f) {
                p1.x = 2048.0f;
                v1.x = -v1.x * rc1;
            }

            if (p1.y < -2048.0f) {
                p1.y = -2048.0f;
                v1.y = -v1.y * rc1;
            }

            if (p1.y > 2048.0f) {
                p1.y = 2048.0f;
                v1.y = -v1.y * rc1;
            }

            resolve_circle_map_collision(e1);

            for (int j = (i+1); j < num_entities; j++) {
                var e2 = entities[j];

                resolve_circle_circle_collision(e1, e2);
            }
        }
    }

    private bool has_tile(int x, int y) {
        if (x < 0) return false;
        if (x > 255) return false;
        if (y < 0) return false;
        if (y > 255) return false;

        return tile_map.tiles[x+y*256] != 0;
    }

    private bool check_left_right(Entity e1, int x, int y) {
        var p = e1.get_component<Position>();
        var c = e1.get_component<Bounding_Circle>();
        var v = e1.get_component<Velocity>();

        bool check_right  = !has_tile(x+1, y);
        bool check_left   = !has_tile(x-1, y);


        int tw = 16;
        int th = 16;

        if (check_left) {
            var c_x = x*tw - 2048.0f;
            var c_y = p.y;

            if ((p.y+c.radius >= (y*th - 2048.0f))
             && (p.y-c.radius < ((y+1)*th - 2048.0f))
             && (p.x+c.radius > c_x)
             && (v.x > 0.0f))
            {
                p.x = c_x - c.radius;
                v.x *= -1.0f;

                Fab5_Game.inst().message("collision", new { entity1 = e1, entity2 = (Entity)null, c_x = c_x, c_y = c_y });
                return true;
            }
        }

        if (check_right) {
            var c_x = (x+1)*tw - 2048.0f;
            var c_y = p.y;

            if ((p.y+c.radius >= (y*th - 2048.0f))
             && (p.y-c.radius < ((y+1)*th - 2048.0f))
             && (p.x-c.radius < c_x)
             && (v.x < 0.0f))
            {
                p.x = c_x + c.radius;
                v.x *= -1.0f;
                Fab5_Game.inst().message("collision", new { entity1 = e1, entity2 = (Entity)null, c_x = c_x, c_y = c_y });
                return true;
            }
        }

        return false;
    }

    private bool check_top_bottom(Entity e1, int x, int y) {
        var p = e1.get_component<Position>();
        var c = e1.get_component<Bounding_Circle>();
        var v = e1.get_component<Velocity>();


        bool check_top    = !has_tile(x, y-1);
        bool check_bottom = !has_tile(x, y+1);


        int tw = 16;
        int th = 16;


        if (check_top) {
            var c_x = p.x;
            var c_y = y*th - 2048.0f;

            if ((p.x+c.radius >= (x*tw - 2048.0f))
             && (p.x-c.radius < ((x+1)*tw - 2048.0f))
             && (p.y+c.radius > c_y)
             && (v.y > 0.0f))
            {
                p.y = c_y - c.radius;
                v.y *= -1.0f;
                Fab5_Game.inst().message("collision", new { entity1 = e1, entity2 = (Entity)null, c_x = c_x, c_y = c_y });
                return true;
            }
        }

        if (check_bottom) {
            var c_x = p.x;
            var c_y = (y+1)*th - 2048.0f;

            if ((p.x+c.radius >= (x*tw - 2048.0f))
             && (p.x-c.radius < ((x+1)*tw - 2048.0f))
             && (p.y-c.radius < c_y)
             && (v.y < 0.0f))
            {
                p.y = c_y + c.radius;
                v.y *= -1.0f;
                Fab5_Game.inst().message("collision", new { entity1 = e1, entity2 = (Entity)null, c_x = c_x, c_y = c_y });
                return true;
            }
        }

        return false;
    }

    private bool resolve_circle_tile_collision(Entity e1, int x, int y) {
        if (x < 0 || x > 255 || y < 0 || y > 255) return false;
        if (tile_map.tiles[x+y*256] == 0) {
            return false;
        }

        var v = e1.get_component<Velocity>();
        var c = e1.get_component<Bounding_Circle>();
        var p = e1.get_component<Position>();

        var tw = 16;
        var th = 16;

        var rect = Rectangle.Intersect(new Rectangle((int)(p.x-c.radius), (int)(p.y-c.radius),(int)(c.radius*2.0f),(int)(c.radius*2.0f)),
                                       new Rectangle((int)(x*tw-2048.0f), (int)(y*th-2048.0f), tw, th));



        if (rect.Width < rect.Height) {
            return check_left_right(e1, x, y) || check_top_bottom(e1, x, y);
        }
        else {
            return check_top_bottom(e1, x, y) || check_left_right(e1, x, y);
        }
    }

    private void resolve_circle_map_collision(Entity e1) {
        var p = e1.get_component<Position>();
        var c = e1.get_component<Bounding_Circle>();

        int tw     = 16;
        int th     = 16;
        int left   = (int)(p.x - c.radius+2048.0f) / tw;
        int top    = (int)(p.y - c.radius+2048.0f) / th;
        int right  = (int)(p.x + c.radius+2048.0f) / tw;
        int bottom = (int)(p.y + c.radius+2048.0f) / th;

        var v = e1.get_component<Velocity>();

        int xs = -Math.Sign(v.x);
        int ys = -Math.Sign(v.y);

        if (xs < 0) {
            var tmp = right;
            right = left;
            left = tmp;
        }

        if (ys < 0) {
            var tmp = bottom;
            bottom = top;
            top = tmp;
        }

        if (Math.Abs(v.x) < Math.Abs(v.y)) {
            for (int x = left; (xs > 0) ? x <= right : x >= right; x += xs) {
                for (int y = top; (ys > 0) ? y <= bottom : y >= bottom; y += ys) {
                    if (resolve_circle_tile_collision(e1, x, y))
                        return;
                }
            }
        }
        else {

            for (int y = top; (ys > 0) ? y <= bottom : y >= bottom; y += ys) {
                for (int x = left; (xs > 0) ? x <= right : x >= right; x += xs) {
                    if (resolve_circle_tile_collision(e1, x, y))
                        return;
                }
            }
        }
    }

    private void resolve_circle_circle_collision(Entity e1, Entity e2) {
        var p1     = e1.get_component<Position>();
        var p2     = e2.get_component<Position>();
        var p_x    = p2.x - p1.x;
        var p_y    = p2.y - p1.y;
        var r2     = p_x*p_x + p_y*p_y;
        var c1     = e1.get_component<Bounding_Circle>();
        var c2     = e2.get_component<Bounding_Circle>();
        var r2_min = (c1.radius+c2.radius) * (c1.radius+c2.radius);

        if (r2 < 0.00001f || r2 >= r2_min) {
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
        var rc1 = e1.get_component<Mass>()?.restitution_coeff ?? 1.0f;
        var rc2 = e2.get_component<Mass>()?.restitution_coeff ?? 1.0f;
        var f1  = 1.0f - m1/(m1+m2);
        var f2  = 1.0f - m2/(m1+m2);

        // Normalize difference in position.
        p_x /= r;
        p_y /= r;

        // Move apart to solve penetration.
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


        var c_x = p1.x + p_x*c1.radius;
        var c_y = p1.y + p_y*c1.radius;
        Fab5_Game.inst().message("collision", new { entity1 = e1, entity2 = e2, c_x = c_x, c_y = c_y });

        // @To-do: Apply restitution factor here.

        // Newton's third law.
        p_x *= d*2.0f;
        p_y *= d*2.0f;

        // Apply restitution coefficients.
        f1 *= rc1*rc2;
        f2 *= rc1*rc2;

        v1.x -= p_x*f1;
        v1.y -= p_y*f1;
        v2.x += p_x*f2;
        v2.y += p_y*f2;
    }
}

}
