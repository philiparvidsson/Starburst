namespace Fab5.Engine.Subsystems {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using Fab5.Starburst.Components;
using Fab5.Starburst.States;

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

    using Fab5.Starburst.States.Playing;

public static class Dummy_Enemy {
    public static int ai_index = 1;

    private const float THINK_INTERVAL = 1.0f/30.0f; // think 5 times per sec

    private static Random rand = new Random();

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
                           y = y }

            // new Sprite { color = Color.Yellow,
            //              texture = Fab5_Game.inst().get_content<Texture2D>("particle") }
        };
    }

    private static int heuristic(int ax, int ay, int bx, int by) {
        return dist(ax, ay, bx, by);
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

        int num_its = 0;
        int k = 3;
        while (open.Count > 0) {
            num_its++;
            /*if (num_its > 20000) {
                // too slow or something
                break;
            }*/
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
            var current_x = get_x(current);
            var current_y = get_y(current);
            if (Math.Abs(current_x-tx) <= k && Math.Abs(current_y-ty) <= k) {
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
            var upy = get_y(current)-k;
            if (upy >= 0 && !has_tile(tile_map, upx, upy)) {
                var up = pos_to_node_index(upx, upy);
                if (!closed.Contains(up)) {
                    var score = (g_score.ContainsKey(current) ? g_score[current] : 999999999) + dist(get_x(current), get_y(current), upx, upy);
                    open.Add(up);
                    if (score < (g_score.ContainsKey(up) ? g_score[up] : 999999999)) {
                        came_from[up] = current;
                        g_score[up] = score;
                        f_score[up] = g_score[up] + heuristic(upx, upy, tx, ty);
                    }
                }
            }

            // right
            var rightx = get_x(current)+k;
            var righty = get_y(current);
            if (righty <= 255 && !has_tile(tile_map, rightx, righty)) {
                var right = pos_to_node_index(rightx, righty);
                if (!closed.Contains(right)) {
                    var score = (g_score.ContainsKey(current) ? g_score[current] : 999999999) + dist(get_x(current), get_y(current), rightx, righty);
                    open.Add(right);
                    if (score < (g_score.ContainsKey(right) ? g_score[right] : 999999999)) {
                        came_from[right] = current;
                        g_score[right] = score;
                        f_score[right] = g_score[right] + heuristic(rightx, righty, tx, ty);
                    }
                }
            }

            // down
            var downx = get_x(current);
            var downy = get_y(current)+k;
            if (downy <= 255 && !has_tile(tile_map, downx, downy)) {
                var down = pos_to_node_index(downx, downy);
                if (!closed.Contains(down)) {
                    var score = (g_score.ContainsKey(current) ? g_score[current] : 999999999) + dist(get_x(current), get_y(current), downx, downy);
                    open.Add(down);
                    if (score < (g_score.ContainsKey(down) ? g_score[down] : 999999999)) {
                        came_from[down] = current;
                        g_score[down] = score;
                        f_score[down] = g_score[down] + heuristic(downx, downy, tx, ty);
                    }
                }
            }

            // left
            var leftx = get_x(current)-k;
            var lefty = get_y(current);
            if (lefty >= 0 && !has_tile(tile_map, leftx, lefty)) {
                var left = pos_to_node_index(leftx, lefty);
                if (!closed.Contains(left)) {
                    var score = (g_score.ContainsKey(current) ? g_score[current] : 999999999) + dist(get_x(current), get_y(current), leftx, lefty);
                    open.Add(left);
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
            int counter = (int)(-1*speed*0.25f);
            bool first = true;
            foreach (var node in path) {
                if (first && path.Count < 3) {
                    first = false;
                    var dx = ((float)rand.NextDouble()-0.5f)*12.0f;
                    var dy = ((float)rand.NextDouble()-0.5f)*12.0f;
                    waypoints.Add(create_waypoint(get_x(node)*16.0f-2048.0f+8.0f+dx, get_y(node)*16.0f-2048.0f+8.0f+dy));
                }
                else if (counter++ == 2) {
                    var dx = ((float)rand.NextDouble()-0.5f)*12.0f;
                    var dy = ((float)rand.NextDouble()-0.5f)*12.0f;
                    waypoints.Add(create_waypoint(get_x(node)*16.0f-2048.0f+8.0f+dx, get_y(node)*16.0f-2048.0f+8.0f+dy));
                    if (waypoints.Count > 10) {
                        break;
                    }
                    counter = 0;
                }
            }

            var dxx = ((float)rand.NextDouble()-0.5f)*8.0f;
            var dyy = ((float)rand.NextDouble()-0.5f)*8.0f;
            waypoints.Add(create_waypoint(tx*16.0f-2048.0f+8.0f+dxx, ty*16.0f-2048.0f+8.0f+dyy));
        }
        else {
            Console.WriteLine("could not solve path from {0}:{1} to {2}:{3}", x, y, tx, ty);
        }


        // optimize path
        while (waypoints.Count > -2) {
            bool all_checked = true;

            for (int i = 0; i < waypoints.Count-1; i++) {
                var wp0 = (Position)waypoints[i][0];
                var wp1 = (Position)waypoints[i+1][0];

                var v0 = new Vector2(wp1.x-wp0.x, wp1.y-wp0.y);
                v0.Normalize();

                var sum_angle = 0.0f;
                int last_ok = -1;
                for (int j = (i+1); j < waypoints.Count-1; j++) {
                    var wp2 = (Position)waypoints[j][0];
                    var wp3 = (Position)waypoints[j+1][0];

                    var v1 = new Vector2(wp3.x-wp2.x, wp3.y-wp2.y);
                    v1 = new Vector2(-v1.Y, v1.X);
                    v1.Normalize();

                    var dot = Vector2.Dot(v0, v1);
                    sum_angle += (float)Math.Abs(dot);
                    if (sum_angle < (float)Math.Cos(65.0f*3.141592f/180.0f)) {
                        last_ok = j;
                    }
                    else {
                        break;
                    }

                    v0 = new Vector2(v1.Y, -v1.X);
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

    private static Position closest_pos(Position p, out Entity target, params List<Entity>[] entities) {
        var min_dist = 99999999.0f;
        Position closest = null;
        Entity targ = null;

        foreach (var ents in entities) {
            foreach (var e in ents) {
                var p2 = e.get_component<Position>();

                if (p2 == null) {
                    continue;
                }

                var dx = p2.x-p.x;
                var dy = p2.y-p.y;
                var dist = dx*dx+dy*dy;
                if (e.has_component<Powerup>()) {
                    dist *= 4; // double distance for powerups
                }

                if (dist < min_dist) {
                    min_dist = dist;
                    closest = p2;
                    targ = e;
                }
            }
        }

        target = targ;
        return closest;
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

        Input input = (Input)data.get_data("input", null);

        w.ang_vel = 0.0f;
        v.ax = 0.0f;
        v.ay = 0.0f;
        input.throttle = 0.0f;

        var si = self.get_component<Ship_Info>();

        if (si.is_dead) {
            data.data.Remove("escape_point");
            return;
        }
        else {
            if (!data.data.ContainsKey("escape_point")) {
                data.data["escape_point"] = new Position { x = p.x, y = p.y };
            }
        }

        for (int i = 0; i < si.max_powerups_inv; i++) {
            if (si.powerup_inv[i] != null) {
                Fab5_Game.inst().message("ai_use_powerup", new { self = self, index = i });
                break;
            }
        }

        float path_recalc_time = (float)data.get_data("path_recalc_time", 0.0f);
        var tile_map = ((Playing_State)self.state).tile_map;
        if (calc_state == 0 && Fab5_Game.inst().get_time() - path_recalc_time > 0.5f) {


            data.data["path_calc"] = 1;
            Position targetpos = null;
            var escape = false;
            if (si.energy_value > si.top_energy * 0.18f) {
                var players = new List<Entity>();
                var powerups = Fab5_Game.inst().get_entities_fast(typeof (Powerup));


                int num_friends_nearby = 0;
                int num_enemies_nearby = 0;
                foreach (var player in Fab5_Game.inst().get_entities_fast(typeof (Ship_Info))) {
                    var other_si = player.get_component<Ship_Info>();
                    if (player == self) {
                        continue;
                    }

                    if (other_si.is_dead) {
                        continue;
                    }

                    if (((Playing_State)self.state).game_conf.mode == Game_Config.GM_TEAM_DEATHMATCH && other_si.team == si.team) {
                        // following team mate behavior?
                        var other_p = player.get_component<Position>();
                        var distd = new Vector2(p.x-other_p.x, p.y-other_p.y).Length();
                        if (distd < 600.0f) {
                            num_friends_nearby++;
                        }
                    }
                    else {
                        var other_p = player.get_component<Position>();
                        var distd = new Vector2(p.x-other_p.x, p.y-other_p.y).Length();
                        if (distd < 900.0f) {
                            num_enemies_nearby++;
                        }

                        if (!player.has_component<Velocity>()) {
                            // probably a turret
                            continue;
                        }

                        players.Add(player);
                    }
                }

                data.data["num_enemies"] = num_enemies_nearby;
                data.data["num_friends"] = num_friends_nearby;

                if (((Playing_State)self.state).game_conf.mode == Game_Config.GM_TEAM_DEATHMATCH && num_enemies_nearby > 1+2*num_friends_nearby) {
                    escape = true;
                }
                else {
                    Entity etarget;
                    targetpos = closest_pos(p, out etarget, players, powerups);
                    data.data["target"] = etarget;
                }
            }
            else {
                escape = true;
            }

            if (escape) {
                targetpos = (Position)data.data["escape_point"];
            }

            if (targetpos == null)
            {
                targetpos = new Position { x = 0.0f, y = 0.0f };
            }


            var tx = (int)((targetpos.x + 2048.0f) / 16.0f);
            var ty = (int)((targetpos.y + 2048.0f) / 16.0f);

            var vx = v.x/16.0f;
            var vy = v.y/16.0f;
            var speed = (float)Math.Sqrt(vx*vx+vy*vy);

            System.Threading.Tasks.Task.Factory.StartNew(() => {
                var t = Fab5_Game.inst().get_time();
                calc_path(data, x, y, tx, ty, tile_map, speed);
                //Console.WriteLine("calc_path took {0:0.000} s", Fab5_Game.inst().get_time() - t);
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

        bool target_is_friend = true;
        float fac = 1.0f;
        Entity target = (Entity)data.get_data("target", null);
        if (target == null) {
            fac = 1.0f; // shouldnt really happen much
            return;
        }
        else if (target.has_component<Ship_Info>()) {
            // player or other ai, move close but not pin-point
            fac = 5.0f;
            var tsi = target.get_component<Ship_Info>();
            target_is_friend = ((Playing_State)self.state).game_conf.mode == Game_Config.GM_TEAM_DEATHMATCH && tsi.team == si.team;
        }
        else if (target.has_component<Powerup>()) {
            // powerup, pin-point
            if (waypoints.Count > 2) {
                fac = 5.0f;
            }
            else {
                fac = 0.9f;
            }
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

            var wpdist = dxwp*dxwp+dywp*dywp;
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
        var c = new Vector2(dx, dy);
        c.Normalize();
        var dot2 = 0.25 + 0.75f * Vector2.Dot(c, b);
        if (dot2 < 0.0f) {
            dot2 = -dot2 * 0.5f;
        }


        // 5 degrees off per 1000 pixels distance
        var angle_error_fac = 1.0f + dist / 1000.0f;
        var threshold = (float)Math.Cos((90.0f - 5.0f*angle_error_fac) * 3.141592f / 180.0f);

        //Console.WriteLine(dot + ", " + threshold);

        var target_energy = target.get_component<Ship_Info>()?.energy_value ?? 1.0f;
        var target_max_energy = target.get_component<Ship_Info>()?.top_energy ?? 1.0f;
        var inv_aggression = 0.1f + (target_energy/target_max_energy)*(float)((int)data.get_data("num_enemies", 0));
        if (inv_aggression > 1.0f) inv_aggression = 1.0f;
        if (si.energy_value > si.top_energy*0.7f*inv_aggression) {
            data.data["shoot"] = true;
        }
        else if (si.energy_value < si.top_energy*0.25f) {
            data.data["shoot"] = false;
        }

        if (waypoints.Count < 4 && !target_is_friend) {
            var tpos = target.get_component<Position>();
            var tvel = target.get_component<Velocity>() ?? new Velocity();

            var tx = (int)((tpos.x + 2048.0f) / 16.0f);
            var ty = (int)((tpos.y + 2048.0f) / 16.0f);
            var d = new Vector2(-(tpos.y-p.y), tpos.x-p.x);
            var dist_to_target = d.Length();

            bool shoot_bomb = false;
            var vfac = 1.0f;

            if (si.energy_value > si.top_energy * 0.8f) {
                shoot_bomb = true;
                vfac = 2.0f;
            }

            var tpos_x = tpos.x+(tvel.x-v.x)*(dist_to_target*0.0015f*vfac);
            var tpos_y = tpos.y+(tvel.y-v.y)*(dist_to_target*0.0015f*vfac);
            var e = new Vector2(-(tpos_y-p.y), tpos_x-p.x);
            e.Normalize();
            var dot3 = Vector2.Dot(b, e);
            var aim_threshold = (float)Math.Cos((90.0f - 1.5f) * 3.141592f / 180.0f);
            var shoot_threshold = (float)Math.Cos((90.0f - 5f) * 3.141592f / 180.0f);
            if (dot3 < -aim_threshold) {
                w.ang_vel = 6.0f * Math.Abs(dot3);
            }
            else if (dot3 > aim_threshold) {
                w.ang_vel = -6.0f * Math.Abs(dot3);
            }
            if(dot3 > -shoot_threshold && dot3 < shoot_threshold && is_open_path(x, y, tx, ty, tile_map)) {
                //Console.WriteLine("pew");
                if ((bool)data.get_data("shoot", false)) {
                    if (shoot_bomb) {
                        fire(self, si, self.get_component<Secondary_Weapon>());
                    }
                    else {
                        fire(self, si, self.get_component<Primary_Weapon>());
                    }
                }
            }

            var dist_mult = 1.0f/(si.energy_value/si.top_energy);
            if (dist_to_target < 180.0f*dist_mult) {
                v.ax = -si.top_velocity * (float)Math.Cos(w.angle) - v.x;
                v.ay = -si.top_velocity * (float)Math.Sin(w.angle) - v.y;
                input.throttle = -1.0f;
            }
            else if (dist_to_target > 220.0f) {
                v.ax = si.top_velocity * (float)Math.Cos(w.angle) - v.x;
                v.ay = si.top_velocity * (float)Math.Sin(w.angle) - v.y;
                input.throttle = 1.0f;
            }
        }
        else if (dist < 22.624f * fac*dot2) {
            Entity old_wp;
            do {
                old_wp = waypoints[0];
                waypoints[0].destroy();
                waypoints.RemoveAt(0);
            } while (old_wp != wpe && waypoints.Count > 0);

            // think again!

            think(self);
        }
        else if (dot < -threshold) {
            w.ang_vel = 8.0f * Math.Abs(dot-0.2f);
        }
        else if (dot > threshold) {
            w.ang_vel = -8.0f * Math.Abs(dot+0.2f);
        }
        else {
            v.ax = si.top_velocity * (float)Math.Cos(w.angle) - v.x;
            v.ay = si.top_velocity * (float)Math.Sin(w.angle) - v.y;
            input.throttle = 1.0f;
        }
    }

    private static void fire(Entity entity, Ship_Info ship, Weapon weapon) {
        if (weapon.timeSinceLastShot >= weapon.fire_rate && ship.energy_value >= weapon.energy_cost) {
            var message = new { Origin = entity, Weapon = weapon, Ship = ship };
            Fab5_Game.inst().message("fireInput", message);
        }
    }

    public static Component[] create_components(Fab5.Starburst.States.Playing.Game_Config conf, int team) {
        List<Component> components = new List<Component>(Fab5.Starburst.States.Playing.Entities.Player_Ship.create_components(new Input(), conf, team));

        Input input = (Input)components[0];
        components.RemoveAt(0);

        /*return new Component[] {
            new Angle           { },

            new Bounding_Circle { radius = 16.0f },

            new Brain           { think_fn       = think,
                                  think_interval = THINK_INTERVAL },

            new Data            { },

            new Mass            { mass = 15.0f, friction = 0.0f, restitution_coeff = 0.4f },

            new Position        { x = -1700.0f,
                                  y = 1700.0f },

            new Ship_Info(100.0f, 100.0f, 100.0f, 100.0f) { },

            new Sprite          { texture = Fab5_Game.inst().get_content<Texture2D>("ships/ship1" + ai_counter) },

            new Velocity        { x = 0.0f,
                                  y = 0.0f },
        };*/

        components.Add(
            new Brain { think_fn         = think,
                        think_interval   = THINK_INTERVAL,
                        time_since_think = (float)rand.NextDouble() * THINK_INTERVAL }
        );

        var data = new Data{};
        data.data["input"] = input;
        data.data["ai_index"] = ai_index++;
        data.data["path_recalc_time"] = Fab5_Game.inst().get_time() + 0.5f - (float)rand.NextDouble() * 2.0f;
        components.Add(data);

        return components.ToArray();
    }



}

}
