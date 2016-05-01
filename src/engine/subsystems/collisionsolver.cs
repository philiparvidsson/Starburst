namespace Fab5.Engine.Subsystems {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

// Solves collision between bounding circles.
public class Collision_Solver : Subsystem {
    private readonly Tile_Map tile_map;

    public Collision_Solver(Tile_Map tile_map) {
        this.tile_map = tile_map;
    }

    private void get_entities(Position p1, Bounding_Circle c1, Dictionary<uint, HashSet<Entity>> grid, HashSet<Entity> entities) {
        uint left   = (uint)(p1.x - c1.radius + 2048.0f) / grid_size;
        uint right  = (uint)(p1.x + c1.radius + 2048.0f) / grid_size;
        uint top    = (uint)(p1.y - c1.radius + 2048.0f) / grid_size;
        uint bottom = (uint)(p1.y + c1.radius + 2048.0f) / grid_size;

        for (uint x = left; x <= right; x++) {
            for (uint y = top; y <= bottom; y++) {
                uint key = (y<<16)+x;
                if (grid.ContainsKey(key)) entities.UnionWith(grid[key]);
            }
        }
    }

    const uint grid_size = 64;
    public override void update(float t, float dt) {
        // Collisions occur at an instant so who cares about dt?

//        int num_entities;

        /*        var entities = Fab5_Game.inst().get_entities(out num_entities,
            typeof (Bounding_Circle),
            typeof (Position),
            typeof (Velocity)
        );*/

        // Spatial grid ftw!
        var grid = new Dictionary<uint, HashSet<Entity>>();
//        for (int i = 0; i < num_entities; i++) {
  //          var e1  = entities[i];
        var entities = Fab5_Game.inst().get_entities_fast(typeof (Bounding_Circle));
        foreach (var e1 in entities) {
            var p1  = e1.get_component<Position>();
            var c1  = e1.get_component<Bounding_Circle>();

            if (p1 == null || c1 == null) {
                continue;
            }

            uint left   = (uint)(p1.x - c1.radius + 2048.0f) / grid_size;
            uint right  = (uint)(p1.x + c1.radius + 2048.0f) / grid_size;
            uint top    = (uint)(p1.y - c1.radius + 2048.0f) / grid_size;
            uint bottom = (uint)(p1.y + c1.radius + 2048.0f) / grid_size;

            for (uint x = left; x <= right; x++) {
                for (uint y = top; y <= bottom; y++) {
                    uint key = (y<<16)+x;

                    if (!grid.ContainsKey(key)) grid[key] = new HashSet<Entity>();
                    grid[key].Add(e1);
                }
            }
        }



        // @To-do: Implement a quad tree or spatial grid here to reduce the
        //         number of candidates for collision testing.
        System.Threading.ManualResetEvent mre = new System.Threading.ManualResetEvent(false);
        int counter = entities.Count;

        foreach (var entity in entities) {
            System.Threading.ThreadPool.QueueUserWorkItem(o => {
                var e1  = (Entity)o;//entities[(int)o];
                var p1  = e1.get_component<Position>();
                var v1  = e1.get_component<Velocity>();
                var rc1 = Math.Abs(e1.get_component<Mass>()?.restitution_coeff ?? 1.0f);

                if (p1 == null || v1 == null) {
                    return;
                }

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

                if (!resolve_circle_map_collision(e1)) {
                    HashSet<Entity> hash_set = new HashSet<Entity>();

                    get_entities(p1, e1.get_component<Bounding_Circle>(), grid, hash_set);
                    foreach (Entity e2 in hash_set) {
                        if (e2 == e1) continue;
                        // only test against higher ids. oh lol what a hack if i ever saw one lulz
                        if (e2.id < e1.id) continue;

                        if (resolve_circle_circle_collision(e1, e2)) {
                            break;
                        }
                    }

                }

                if (System.Threading.Interlocked.Decrement(ref counter) == 0) {
                    mre.Set();
                }
            }, entity);
        }

        mre.WaitOne();
    }

    private void collide(Entity e1, float c_x, float c_y, float n_x, float n_y) {
        var p1  = e1.get_component<Position>();
        var c1  = e1.get_component<Bounding_Circle>();
        var v1  = e1.get_component<Velocity>();
        var m1  = e1.get_component<Mass>();
        var a1  = e1.get_component<Angle>();
        var r_x = c_x - p1.x;
        var r_y = c_y - p1.y;

        var w_x = 0.0f;
        var w_y = 0.0f;

        if (a1 != null) {
            var w = a1.ang_vel;
            w_x = -w*r_y;
            w_y =  w*r_x;;
        }

        var v_x = v1.x + w_x;
        var v_y = v1.y + w_y;

        var v = v_x*n_x+v_y*n_y;

        if (v > 0.0f) {
            return;
        }

        var e = Math.Abs(m1.restitution_coeff);
        var friction = m1.friction;

        var v_dot_n = (v_x*n_x+v_y*n_y);
        var v_n_x = v_dot_n*n_x;
        var v_n_y = v_dot_n*n_y;
        var v_t_x = v_x - v_n_x;
        var v_t_y = v_y - v_n_y;

        /*var v_t = (float)Math.Sqrt(v_t_x*v_t_x+v_t_y*v_t_y);
        if (v_t < 0.01f) {
            friction = 1.01f;
        }*/

        var i_x = v_t_x * -(friction) + v_n_x * -(1.0f+e);
        var i_y = v_t_y * -(friction) + v_n_y * -(1.0f+e);
        v1.x += i_x;
        v1.y += i_y;

        if (a1 != null) {
            var w = (-r_y*i_x+r_x*i_y);
            a1.ang_vel += w/(r_x*r_x+r_y*r_y) * friction;
        }
    }

    private bool has_tile(int x, int y) {
        if (x < 0) return false;
        if (x > 255) return false;
        if (y < 0) return false;
        if (y > 255) return false;

        var k = tile_map.tiles[x+y*256];
        return k >  0 && k < 6; // 6 and up are specials
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
                collide(e1, c_x, c_y, -1.0f, 0.0f);

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
                collide(e1, c_x, c_y, 1.0f, 0.0f);
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
                collide(e1, c_x, c_y, 0.0f, -1.0f);
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
                collide(e1, c_x, c_y, 0.0f, 1.0f);
                Fab5_Game.inst().message("collision", new { entity1 = e1, entity2 = (Entity)null, c_x = c_x, c_y = c_y });
                return true;
            }
        }

        return false;
    }

    private bool resolve_circle_tile_collision(Entity e1, int x, int y) {
        if (x < 0 || x > 255 || y < 0 || y > 255) return false;
        var k = tile_map.tiles[x+y*256];
        if (k == 0 || k >= 6) { // 6 and up are specials
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

    private bool resolve_circle_map_collision(Entity e1) {
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

        if (xs == 0) xs = 1;
        if (ys == 0) ys = 1;

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
                        return true;
                }
            }
        }
        else {

            for (int y = top; (ys > 0) ? y <= bottom : y >= bottom; y += ys) {
                for (int x = left; (xs > 0) ? x <= right : x >= right; x += xs) {
                    if (resolve_circle_tile_collision(e1, x, y))
                        return true;
                }
            }
        }

        return false;
    }

    private bool resolve_circle_circle_collision(Entity e1, Entity e2) {
        var c1 = e1.get_component<Bounding_Circle>();
        var c2 = e2.get_component<Bounding_Circle>();

        if (c1.ignore_collisions == c2.ignore_collisions && c1.ignore_collisions > 0) {
            return false;
        }

        if (c1.ignore_collisions2 == c2.ignore_collisions2 && c1.ignore_collisions2 > 0) {
            return false;
        }

        var p1     = e1.get_component<Position>();
        var p2     = e2.get_component<Position>();
        var d_x    = p1.x - p2.x;
        var d_y    = p1.y - p2.y;
        var r2     = d_x*d_x + d_y*d_y;
        var r2_min = (c1.radius+c2.radius) * (c1.radius+c2.radius);

        if (r2 < 0.00001f || r2 >= r2_min) {
            // No penetration or full penetration (which cannot be
            // solved in a sane way).
            return false;
        }

        var r    = (float)Math.Sqrt(r2);
        var n_x  = d_x/r;
        var n_y  = d_y/r;
        var c_x  = p2.x + n_x * c2.radius;
        var c_y  = p2.y + n_y * c2.radius;

        Fab5_Game.inst().message("collision", new { entity1 = e1, entity2 = e2, c_x = c_x, c_y = c_y });

        var m1 = e1.get_component<Mass>();
        var m2 = e2.get_component<Mass>();

        if (m1 == null || m2 == null) {
            return false;
        }

        var v1   = e1.get_component<Velocity>();
        var v2   = e2.get_component<Velocity>();
        var v_x  = v1.x - v2.x;
        var v_y  = v1.y - v2.y;

        if (v_x*n_x+v_y*n_y > 0.0f) {
            // moving apart
            return false;
        }

        var a1   = e1.get_component<Angle>();
        var a2   = e2.get_component<Angle>();
        var im1  = 1.0f / m1.mass;
        var im2  = 1.0f / m2.mass;
        var p    = c1.radius+c2.radius - r + 0.001f; // <-- @To-Do: floating point sucks satans ass.
        var p1_x = c_x - p1.x;
        var p1_y = c_y - p1.y;
        var p2_x = c_x - p2.x;
        var p2_y = c_y - p2.y;
        var w1_x = 0.0f;
        var w1_y = 0.0f;
        var w2_x = 0.0f;
        var w2_y = 0.0f;

        if (a1 != null) {
            w1_x = -p1_y*a1.ang_vel;
            w1_y =  p1_x*a1.ang_vel;
        }

        if (a2 != null) {
            w2_x = -p2_y*a2.ang_vel;
            w2_y =  p2_x*a2.ang_vel;
        }

        v_x = (v1.x+w1_x) - (v2.x+w2_x);
        v_y = (v1.y+w1_y) - (v2.y+w2_y);

        var e = Math.Abs(Math.Min(m1.restitution_coeff, m2.restitution_coeff));

        var v_dot_n = v_x*n_x+v_y*n_y;
        var v_n_x = v_dot_n * n_x;
        var v_n_y = v_dot_n * n_y;
        var v_t_x = v_x - v_n_x;
        var v_t_y = v_y - v_n_y;

        var friction = Math.Max(m1.friction, m1.friction);

        /*var v_t = (float)Math.Sqrt(v_t_x*v_t_x+v_t_y*v_t_y);
        if (v_t < 0.01f) {
            friction = 1.01f;
        }*/

        var i_x = v_t_x * -(friction) + v_n_x * -(1.0f + e);
        var i_y = v_t_y * -(friction) + v_n_y * -(1.0f + e);

        var a = (im1 / (im1+im2));
        var b = (im2 / (im1+im2));

        p1.x += p*n_x * a;
        p1.y += p*n_y * a;
        p2.x -= p*n_x * b;
        p2.y -= p*n_y * b;

        // should this shit be multiplied by 0.5? phil phucking wonders
        v1.x += i_x * a;
        v1.y += i_y * a;
        v2.x -= i_x * b;
        v2.y -= i_y * b;

        if (a1 != null) {
            var w = (-p1_y*i_x+p1_x*i_y);
            a1.ang_vel += w/(p1_x*p1_x+p1_y*p1_y) * friction;
        }

        if (a2 != null) {
            var w = (-(p2_y)*i_x+(p2_x)*i_y);
            a2.ang_vel += w/(p2_x*p2_x+p2_y*p2_y) * friction;
        }

        return true;

    }
}

}
