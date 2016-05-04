namespace Fab5.Engine.Subsystems {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

// Solves collision between bounding circles.
public class Collision_Solver : Subsystem {
    private readonly Tile_Map tile_map;
    private System.Threading.AutoResetEvent mre = new System.Threading.AutoResetEvent(false);

    public Collision_Solver(Tile_Map tile_map) {
        this.tile_map = tile_map;
    }

    private void get_entities(Position p1, Bounding_Circle c1, Dictionary<uint, HashSet<Entity>> grid, out HashSet<Entity> entities) {
        entities = null;

        var radius = c1.radius;
        uint left   = (uint)(p1.x - radius + 2048.0f) / grid_size;
        uint right  = (uint)(p1.x + radius + 2048.0f) / grid_size;
        uint top    = (uint)(p1.y - radius + 2048.0f) / grid_size;
        uint bottom = (uint)(p1.y + radius + 2048.0f) / grid_size;

        for (uint x = left; x <= right; x++) {
            for (uint y = top; y <= bottom; y++) {
                uint key = (y<<16)|x;
                if (grid.ContainsKey(key)) {
                    if (entities == null) {
                        entities = new HashSet<Entity>();
                    }

                    entities.UnionWith(grid[key]);
                }
            }
        }
    }

    const uint grid_size = 128;
    private Dictionary<uint, HashSet<Entity>> grid = new Dictionary<uint, HashSet<Entity>>();
    public override void update(float t, float dt) {
        // Collisions occur at an instant so who cares about dt?

//        int num_entities;

        /*        var entities = Fab5_Game.inst().get_entities(out num_entities,
            typeof (Bounding_Circle),
            typeof (Position),
            typeof (Velocity)
        );*/

        // Spatial grid ftw!
        //var grid = new Dictionary<uint, HashSet<Entity>>();
//        for (int i = 0; i < num_entities; i++) {
  //          var e1  = entities[i];
        grid.Clear();
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

        int counter = entities.Count;

        if (counter == 0) {
            return;
        }

        foreach (var entity in entities) {
            //System.Threading.ThreadPool.QueueUserWorkItem(o => {
            System.Threading.Tasks.Task.Factory.StartNew(() => {
                var e1  = entity;//entities[(int)o];
                var p1  = e1.get_component<Position>();
                var v1  = e1.get_component<Velocity>();

                /*if (p1 == null || v1 == null) {
                    continue;
                }*/

                if (p1.x < -2048.0f) {
                    var rc1 = Math.Abs(e1.get_component<Mass>()?.restitution_coeff ?? 1.0f);
                    p1.x = -2048.0f;
                    v1.x = -v1.x * rc1;
                }

                if (p1.x > 2048.0f) {
                    var rc1 = Math.Abs(e1.get_component<Mass>()?.restitution_coeff ?? 1.0f);
                    p1.x = 2048.0f;
                    v1.x = -v1.x * rc1;
                }

                if (p1.y < -2048.0f) {
                    var rc1 = Math.Abs(e1.get_component<Mass>()?.restitution_coeff ?? 1.0f);
                    p1.y = -2048.0f;
                    v1.y = -v1.y * rc1;
                }

                if (p1.y > 2048.0f) {
                    var rc1 = Math.Abs(e1.get_component<Mass>()?.restitution_coeff ?? 1.0f);
                    p1.y = 2048.0f;
                    v1.y = -v1.y * rc1;
                }

                resolve_circle_map_collision(e1);

                HashSet<Entity> hash_set;

                get_entities(p1, e1.get_component<Bounding_Circle>(), grid, out hash_set);
                if (hash_set != null) {
                    foreach (Entity e2 in hash_set) {
                        // only test against higher ids. oh lol what a hack if i ever saw one lulz
                        if (e2.id <= e1.id) {
                            continue;
                        }

                        resolve_circle_circle_collision(e1, e2);
                    }
                }

                if (System.Threading.Interlocked.Decrement(ref counter) == 0) {
                    mre.Set();
                }
            });
        }

        mre.WaitOne();
    }

    private void collide(Entity e1, float c_x, float c_y, float n_x, float n_y) {

        var n = (float)Math.Sqrt(n_x*n_x+n_y*n_y);

        n_x /= n;
        n_y /= n;

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
            a1.ang_vel += (w/(r_x*r_x+r_y*r_y) - a1.ang_vel)*friction;
        }

        if (c1.collision_cb != null) {
            c1.collision_cb(e1, null);
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
        var top = y;
        var bottom = y;
        var h = (int)(c.radius / th)+1;

        // @To-do: Do we need to check further out here?
        for (int i = 1; i < h; i++) {
            if (!has_tile(x, y-i)) {
                break;
            }

            top--;
        }

        for (int i = 1; i < h; i++) {
            if (!has_tile(x, y+i)) {
                break;
            }

            bottom++;
        }


        var eps = 0.01f;

        if (check_left && v.x > 0.0f) {

            var c_y = p.y;
            if (c_y < top*th-2048.0f) c_y = top*th-2048.0f;
            if (c_y > (bottom+1)*th-2048.0f) c_y = (bottom+1)*th-2048.0f;

            var dy  = c_y - p.y;
            var dx  = c.radius - dy*dy/c.radius;
            var c_x = x*tw - 2048.0f;

            if ((p.y+c.radius >= (top*th - 2048.0f))
             && (p.y-c.radius < ((bottom+1)*th - 2048.0f))
             && (p.x+dx > c_x)
             && (p.x < c_x+16.0f))
            {
//            Console.WriteLine("left " + x + "," + y);
                p.x = c_x-dx-eps;
                collide(e1, c_x, c_y, -dx, -dy);
                Fab5_Game.inst().message("collision", new { entity1 = e1, entity2 = (Entity)null, c_x = c_x, c_y = c_y });


                return true;
            }
        }

        if (check_right && v.x < 0.0f) {

            var c_y = p.y;
            if (c_y < top*th-2048.0f) c_y = top*th-2048.0f;
            if (c_y > (bottom+1)*th-2048.0f) c_y = (bottom+1)*th-2048.0f;

            var dy  = c_y - p.y;
            var dx  = c.radius - dy*dy/c.radius;
            var c_x = (x+1)*tw - 2048.0f;

            if ((p.y+c.radius >= (top*th - 2048.0f))
             && (p.y-c.radius < ((bottom+1)*th - 2048.0f))
             && (p.x-dx < c_x)
             && (p.x > c_x-16.0f))
            {
//            Console.WriteLine("right " + x + "," + y);
                p.x = c_x+dx+eps;
                collide(e1, c_x, c_y, dx, -dy);
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

        var left = x;
        var right = x;
        int tw = 16;
        int th = 16;
        var w = (int)(c.radius / tw)+1;

        // @To-do: Do we need to check further out here?
        for (int i = 1; i < w; i++) {
            if (!has_tile(x-i, y)) {
                break;
            }

            left--;
        }

        for (int i = 1; i < w; i++) {
            if (!has_tile(x+i, y)) {
                break;
            }

            right++;
        }




        var eps = 0.01f;

        if (check_top && v.y > 0.0f) {

            var c_x = p.x;
            if (c_x < left*tw-2048.0f) c_x = left*tw-2048.0f;
            if (c_x > (right+1)*tw-2048.0f) c_x = (right+1)*tw-2048.0f;

            var dx = c_x - p.x;
            var dy = c.radius - dx*dx/c.radius;
            var c_y = y*th - 2048.0f;

            if ((p.x+c.radius >= (left*tw - 2048.0f))
             && (p.x-c.radius < ((right+1)*tw - 2048.0f))
             && (p.y+dy > c_y)
             && (p.y < c_y+16.0f))
            {
//            Console.WriteLine("top " + x + "," + y);
                p.y = c_y-dy-eps;
                collide(e1, c_x, c_y, -dx, -dy);
                Fab5_Game.inst().message("collision", new { entity1 = e1, entity2 = (Entity)null, c_x = c_x, c_y = c_y });
                return true;
            }
        }

        if (check_bottom && v.y < 0.0f) {
            var c_x = p.x;
            if (c_x < left*tw-2048.0f) c_x = left*tw-2048.0f;
            if (c_x > (right+1)*tw-2048.0f) c_x = (right+1)*tw-2048.0f;

            var dx = c_x - p.x;
            var dy = c.radius - dx*dx/c.radius;
            var c_y = (y+1)*th - 2048.0f;

            if ((p.x+c.radius >= (left*tw - 2048.0f))
             && (p.x-c.radius < ((right+1)*tw - 2048.0f))
             && (p.y-dy < c_y)
             && (p.y > c_y-16.0f))
            {
//            Console.WriteLine("bottom " + x + "," + y);
                p.y = c_y+dy+eps;
                collide(e1, c_x, c_y, -dx, dy);
                Fab5_Game.inst().message("collision", new { entity1 = e1, entity2 = (Entity)null, c_x = c_x, c_y = c_y });
                return true;
            }
        }

        return false;
    }

    private void resolve_circle_tile_collision(Entity e1, int x, int y, ref bool coll_h, ref bool coll_v) {
        if (x < 0 || x > 255 || y < 0 || y > 255) return;
        var k = tile_map.tiles[x+(y<<8)];
        if (k == 0 || k >= 6) { // 6 and up are specials
            return;
        }

        var v = e1.get_component<Velocity>();
        var c = e1.get_component<Bounding_Circle>();
        var p = e1.get_component<Position>();

        var tw = 16;
        var th = 16;

        var rect = Rectangle.Intersect(new Rectangle((int)(p.x-c.radius), (int)(p.y-c.radius),(int)(c.radius*2.0f),(int)(c.radius*2.0f)),
                                       new Rectangle((int)(x*tw-2048.0f), (int)(y*th-2048.0f), tw, th));




        if (Math.Abs(v.x) > Math.Abs(v.y)) {
            if (!coll_h) {
                bool lr = check_left_right(e1, x, y);
                coll_h |= lr;

            }
            if (!coll_v) {
                var tb = check_top_bottom(e1, x, y);
                coll_v |= tb;

            }
        }
        else {

            if (!coll_v) {
                var tb = check_top_bottom(e1, x, y);
                coll_v |= tb;

            }
            if (!coll_h) {
                bool lr = check_left_right(e1, x, y);
                coll_h |= lr;

            }
        }

    }

    private bool resolve_circle_map_collision(Entity e1) {
        var p = e1.get_component<Position>();
        var c = e1.get_component<Bounding_Circle>();

        //int tw     = 16;
        //int th     = 16; shifting instead lewl
        var rad = c.radius;
        int left   = (int)(p.x - rad+2048.0f) >> 4;
        int top    = (int)(p.y - rad+2048.0f) >> 4;
        int right  = (int)(p.x + rad+2048.0f) >> 4;
        int bottom = (int)(p.y + rad+2048.0f) >> 4;

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
            bool coll_v = false;
            bool coll_h = false;
            for (int x = left; (xs > 0) ? x <= right : x >= right; x += xs) {
                for (int y = top; (ys > 0) ? y <= bottom : y >= bottom; y += ys) {
                    if (coll_h && coll_v) {
                        break;
                    }

                    resolve_circle_tile_collision(e1, x, y, ref coll_h, ref coll_v);
                }
            }

            return coll_v|coll_h;
        }
        else {
            bool coll_v = false;
            bool coll_h = false;

            for (int y = top; (ys > 0) ? y <= bottom : y >= bottom; y += ys) {
                for (int x = left; (xs > 0) ? x <= right : x >= right; x += xs) {
                    if (coll_h && coll_v) {
                        break;
                    }

                    resolve_circle_tile_collision(e1, x, y, ref coll_h, ref coll_v);
                }
            }

            return coll_v|coll_h;
        }

    }

    private bool resolve_circle_circle_collision(Entity e1, Entity e2) {
        var c1 = e1.get_component<Bounding_Circle>();
        var c2 = e2.get_component<Bounding_Circle>();
        var p1     = e1.get_component<Position>();
        var p2     = e2.get_component<Position>();
        var d_x    = p1.x - p2.x;
        var d_y    = p1.y - p2.y;
        var r2     = d_x*d_x + d_y*d_y;
        var cs     = (c1.radius+c2.radius);
        var r2_min = cs * cs;

        if (r2 >= r2_min || r2 < 0.0001f) {
            // No penetration or full penetration (which cannot be
            // solved in a sane way).
            return false;
        }

        if (c1.ignore_collisions == c2.ignore_collisions && c1.ignore_collisions > 0) {
            return false;
        }

        if (c1.ignore_collisions2 == c2.ignore_collisions2 && c1.ignore_collisions2 > 0) {
            return false;
        }


        var r    = (float)Math.Sqrt(r2);
        var n_x  = d_x/r;
        var n_y  = d_y/r;
        var c_x  = p2.x + n_x * c2.radius;
        var c_y  = p2.y + n_y * c2.radius;

        Fab5_Game.inst().message("collision", new { entity1 = e1, entity2 = e2, c_x = c_x, c_y = c_y });

        if (c1.collision_cb != null) {
            c1.collision_cb(e1, e2);
        }
        if (c2.collision_cb != null) {
            c2.collision_cb(e2, e1);
        }

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
            a1.ang_vel += (w/(p1_x*p1_x+p1_y*p1_y) - a1.ang_vel)  *friction;
        }

        if (a2 != null) {
            var w = (-(p2_y)*i_x+(p2_x)*i_y);
            a2.ang_vel += (w/(p2_x*p2_x+p2_y*p2_y) - a2.ang_vel) * friction;
        }

        return true;

    }
}

}
