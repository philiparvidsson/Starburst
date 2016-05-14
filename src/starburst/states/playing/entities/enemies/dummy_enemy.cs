namespace Fab5.Engine.Subsystems {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using Fab5.Starburst.Components;
using Fab5.Starburst.States;

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public static class Dummy_Enemy {
    private const float THINK_INTERVAL = 1.0f/30.0f; // think 5 times per sec

    enum Behavior {
        Chase_Player
    }


    private static bool has_tile2(Tile_Map tile_map, int x, int y) {
        if (x < 0 || x > 255 || y < 0 || y > 255) return false;
        var k = tile_map.tiles[x+y*256];
        return k >  0 && k < 7; // 7 and up are specials
    }

    private static bool has_tile(Tile_Map tile_map, int x, int y) {
        int n = 2;
        for (int i = -n; i <= n; i++) {
            for (int j = -n; j <= n; j++) {
                if (has_tile2(tile_map, x+i, y+j)) {
                    return true;
                }
            }
        }

        return false;
    }

    private static Component[] create_waypoint(float x, float y) {
        return new Component[] {
            new Position { x = x,
                           y = y },

            new Sprite { color = Color.Yellow,
                         texture = Fab5_Game.inst().get_content<Texture2D>("particle") }
        };
    }

    private static int heuristic(int ax, int ay, int bx, int by) {
        return dist(ax, ay, bx, by);//*dist(ax, ay, bx, by);
    }

    private static int dist(int ax, int ay, int bx, int by) {
        return (int)((new Vector2(bx-ax,by-ay)*1000).Length());
            //return (int)Math.Abs(bx - ax)+(int)Math.Abs(by-ay);
    }

    private static int pos_to_node_index(int x, int y) {
        return (y<<16)|x;
    }

    private static int get_x(int node) {
        return node&0xffff;
    }

    private static int get_y(int node) {
        return (node>>16)&0xffff;
    }

    private static List<int> find_path(int x, int y, int tx, int ty, Tile_Map tile_map) {
        //if (open == null) {
            var open = new HashSet<int>();
            open.Add(pos_to_node_index(x, y));
        //}

        //if (closed == null) {
            var closed = new HashSet<int>();
       // }

        var came_from = new Dictionary<int, int>();
        var g_score = new Dictionary<int, int>();
        var f_score = new Dictionary<int, int>();

        g_score[pos_to_node_index(x, y)] = 0;
        f_score[pos_to_node_index(x, y)] = heuristic(x, y, tx, ty);

        while (open.Count > 0) {
            var best_node = -1;
            var lowest_cost = 999999999;
            foreach (var node in open) {
                var cost = f_score.ContainsKey(node) ? f_score[node] : 999999999;
                if (cost < lowest_cost) {
                    lowest_cost = cost;
                    best_node = node;
                }
            }

            var current = best_node;
            if (current == pos_to_node_index(tx, ty)) {
                List<int> path = new List<int>();
                while (came_from.ContainsKey(current)) {
                    current = came_from[current];
                    path.Insert(0, current);
                }

                return path;
            }

            open.Remove(current);
            closed.Add(current);

            // up
            var upx = get_x(current);
            var upy = get_y(current)-1;
            if (upy >= 0 && !has_tile(tile_map, upx, upy)) {
                var up = pos_to_node_index(upx, upy);
                if (!closed.Contains(up)) {
                    var score = (g_score.ContainsKey(current) ? g_score[current] : 999999999) + dist(get_x(current), get_y(current), upx, upy);
                    if (!open.Contains(up)) {
                        open.Add(up);
                    }
                    if (score < (g_score.ContainsKey(up) ? g_score[up] : 999999999)) {
                        came_from[up] = current;
                        g_score[up] = score;
                        f_score[up] = g_score[up] + heuristic(upx, upy, tx, ty);
                    }
                }
            }

            // right
            var rightx = get_x(current)+1;
            var righty = get_y(current);
            if (righty <= 255 && !has_tile(tile_map, rightx, righty)) {
                var right = pos_to_node_index(rightx, righty);
                if (!closed.Contains(right)) {
                    var score = (g_score.ContainsKey(current) ? g_score[current] : 999999999) + dist(get_x(current), get_y(current), rightx, righty);
                    if (!open.Contains(right)) {
                        open.Add(right);
                    }
                    if (score < (g_score.ContainsKey(right) ? g_score[right] : 999999999)) {
                        came_from[right] = current;
                        g_score[right] = score;
                        f_score[right] = g_score[right] + heuristic(rightx, righty, tx, ty);
                    }
                }
            }

            // down
            var downx = get_x(current);
            var downy = get_y(current)+1;
            if (downy <= 255 && !has_tile(tile_map, downx, downy)) {
                var down = pos_to_node_index(downx, downy);
                if (!closed.Contains(down)) {
                    var score = (g_score.ContainsKey(current) ? g_score[current] : 999999999) + dist(get_x(current), get_y(current), downx, downy);
                    if (!open.Contains(down)) {
                        open.Add(down);
                    }
                    if (score < (g_score.ContainsKey(down) ? g_score[down] : 999999999)) {
                        came_from[down] = current;
                        g_score[down] = score;
                        f_score[down] = g_score[down] + heuristic(downx, downy, tx, ty);
                    }
                }
            }

            // left
            var leftx = get_x(current)-1;
            var lefty = get_y(current);
            if (lefty >= 0 && !has_tile(tile_map, leftx, lefty)) {
                var left = pos_to_node_index(leftx, lefty);
                if (!closed.Contains(left)) {
                    var score = (g_score.ContainsKey(current) ? g_score[current] : 999999999) + dist(get_x(current), get_y(current), leftx, lefty);
                    if (!open.Contains(left)) {
                        open.Add(left);
                    }
                    if (score < (g_score.ContainsKey(left) ? g_score[left] : 999999999)) {
                        came_from[left] = current;
                        g_score[left] = score;
                        f_score[left] = g_score[left] + heuristic(leftx, lefty, tx, ty);
                    }
                }
            }
        }

        return null;
    }

    private static void calc_path(Data data, int x, int y, int tx, int ty, Tile_Map tile_map, float speed) {
        List<Component[]> waypoints = new List<Component[]>();

        var path = find_path(x, y, tx, ty, tile_map);
        if (path != null) {
            int counter = -(int)speed+1;
            foreach (var node in path) {
                if (counter++ == 4) {
                    waypoints.Add(create_waypoint(get_x(node)*16.0f-2048.0f+8.0f, get_y(node)*16.0f-2048.0f+8.0f));
                    counter = 0;
                }
            }
        }


        // optimize path
        while (waypoints.Count > 2) {
            bool all_checked = true;

            for (int i = 0; i < waypoints.Count-1; i++) {
                var wp0 = (Position)waypoints[i][0];
                var wp1 = (Position)waypoints[i+1][0];

                var v0 = new Vector2(wp1.x-wp0.x, wp1.y-wp0.y);

                var sum_angle = 0.0f;
                int last_ok = -1;
                for (int j = (i+1); j < waypoints.Count-1; j++) {
                    var wp2 = (Position)waypoints[j][0];
                    var wp3 = (Position)waypoints[j+1][0];

                    var v1 = new Vector2(wp3.x-wp2.x, wp3.y-wp2.y);
                    v1 = new Vector2(-v1.Y, v1.X);

                    var dot = Vector2.Dot(v0, v1);
                    sum_angle += (float)Math.Abs(dot);
                    if (sum_angle < (float)Math.Cos(15.0f*3.141592f/180.0f)) {
                        last_ok = j;
                    }
                    else {
                        break;
                    }
                }

                if (last_ok != -1) {
                    all_checked = false;

                    int num = last_ok - (i+1) + 1;
                    for (int n = 0; n < num; n++) {
                        waypoints.RemoveAt(i+1);
                    }

                    break;

                }
            }

            if (all_checked) {
                break;
            }
        }


        data.data["path_recalc_time"] = Fab5_Game.inst().get_time();
        data.data["path_calc"] = 2;
        data.data["waypoints2"] = waypoints;
    }

    private static void swap<T>(ref T lhs, ref T rhs) { T temp; temp = lhs; lhs = rhs; rhs = temp; }
    public static bool is_open_path(int x0, int y0, int x1, int y1, Tile_Map tile_map)
    {
        bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
        if (steep) { swap<int>(ref x0, ref y0); swap<int>(ref x1, ref y1); }
        if (x0 > x1) { swap<int>(ref x0, ref x1); swap<int>(ref y0, ref y1); }
        int dX = (x1 - x0), dY = Math.Abs(y1 - y0), err = (dX / 2), ystep = (y0 < y1 ? 1 : -1), y = y0;

        for (int x = x0; x <= x1; ++x)
        {
            if (steep) {
                if (has_tile2(tile_map, y, x)) return false;
            }
            else {
                if (has_tile2(tile_map, x, y)) return false;
            }

            err = err - dY;
            if (err < 0) { y += ystep;  err += dX; }
        }

        return true;
    }

    private static void think(Entity self) {
        var data = self.get_component<Data>();

        var waypoints = (List<Entity>)data.get_data("waypoints", null);
        if (waypoints == null) {
            waypoints = new List<Entity>();
        }

        var w  = self.get_component<Angle>();
        var p  = self.get_component<Position>();
        var v  = self.get_component<Velocity>();
        var x = (int)((p.x + 2048.0f) / 16.0f);
        var y = (int)((p.y + 2048.0f) / 16.0f);
        var calc_state = (int)data.get_data("path_calc", 0);

        w.ang_vel = 0.0f;
        v.ax = 0.0f;
        v.ay = 0.0f;

        float path_recalc_time = (float)data.get_data("path_recalc_time", 0.0f);
        var tile_map = ((Playing_State)self.state).tile_map;
        if (calc_state == 0 && Fab5_Game.inst().get_time() - path_recalc_time > 0.5f) {


            data.data["path_calc"] = 1;


            var player = Fab5_Game.inst().get_entities_fast(typeof (Input))[0];
            var playerpos=player.get_component<Position>();


            var tx = (int)((playerpos.x + 2048.0f) / 16.0f);
            var ty = (int)((playerpos.y + 2048.0f) / 16.0f);

            var vx = v.x/16.0f;
            var vy = v.y/16.0f;
            var speed = (float)Math.Sqrt(vx*vx+vy*vy);

            System.Threading.Tasks.Task.Factory.StartNew(() => {
                calc_path(data, x, y, tx, ty, tile_map, speed);
            });
        }
        else if (calc_state == 2) {
            foreach (var e in waypoints) {
                e.destroy();
            }
            var wps = new List<Entity>();
            data.data["waypoints"] = wps;
            foreach (var comps in (List<Component[]>)data.data["waypoints2"]) {
                wps.Add(Fab5_Game.inst().create_entity(comps));
            }
            data.data["path_calc"] = 0;
        }

        if (waypoints.Count == 0) {
            return;
        }


        Position wp = waypoints[0].get_component<Position>();
        Entity wpe = null;

        var min_dist = 99999999999.0f;
        foreach (var waypoint in waypoints) {
            var waypointpos = waypoint.get_component<Position>();
            var wpx = (int)((waypointpos.x + 2048.0f) / 16.0f);
            var wpy = (int)((waypointpos.y + 2048.0f) / 16.0f);

            if (!is_open_path(x, y, wpx, wpy, tile_map))
                continue;

            var dxwp = waypointpos.x - p.x;
            var dywp = waypointpos.y - p.y;

            var wpdist = new Vector2(dxwp, dywp).Length();
            if (wpdist < min_dist) {
                min_dist = wpdist;
                wp = waypointpos;
                wpe = waypoint;
            }
        }

        var dx = wp.x - p.x;
        var dy = wp.y - p.y;

        var hx = (float)Math.Cos(w.angle);
        var hy = (float)Math.Sin(w.angle);

        var a = new Vector2(-dy, dx);
        var b = new Vector2(hx, hy);

        var dist = a.Length();

        a.Normalize();
        b.Normalize();

        var dot = Vector2.Dot(a, b);


        // 5 degrees off per 1000 pixels distance
        var angle_error_fac = 1.0f + dist / 1000.0f;
        var threshold = (float)Math.Cos((90.0f - 5.0f*angle_error_fac) * 3.141592f / 180.0f);

        //Console.WriteLine(dot + ", " + threshold);

        if (dot < -threshold) {
            w.ang_vel = 8.0f * Math.Abs(dot-0.2f);
        }
        else if (dot > threshold) {
            w.ang_vel = -8.0f * Math.Abs(dot+0.2f);
        }
        else if (dist > 22.624f * 5.0f) {
            v.ax = 380.0f * (float)Math.Cos(w.angle) - v.x;
            v.ay = 380.0f * (float)Math.Sin(w.angle) - v.y;
        }
        else {
            Entity old_wp;
            do {
                old_wp = waypoints[0];
                waypoints[0].destroy();
                waypoints.RemoveAt(0);
            } while (old_wp != wpe && waypoints.Count > 0);

            // think again!
            think(self);
                // shoot
        }
    }

    public static Component[] create_components() {
        return new Component[] {
            new Angle           { },

            new Bounding_Circle { radius = 16.0f },

            new Brain           { think_fn       = think,
                                  think_interval = THINK_INTERVAL },

            new Data            { },

            new Mass            { mass = 15.0f, friction = 0.0f, restitution_coeff = 0.4f },

            new Position        { x = -1700.0f,
                                  y = 1700.0f },

            new Ship_Info(100.0f, 100.0f, 100.0f, 100.0f) { },

            new Sprite          { texture = Fab5_Game.inst().get_content<Texture2D>("ships/ship14") },

            new Velocity        { x = 0.0f,
                                  y = 0.0f },
        };
    }

    public static void spawn() { // @To-do: for testing only!
        var entity = Fab5_Game.inst().create_entity(create_components());

        var pos = entity.get_component<Position>();

        pos.x = -200.0f;
        pos.y = 200.0f;
    }

}

}
