namespace Fab5.Starburst.States {

using Fab5.Engine;
using Fab5.Engine.Components;
using Fab5.Engine.Core;
using Fab5.Engine.Subsystems;

using Fab5.Starburst.States.Playing;
using Fab5.Starburst.States.Playing.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

    using System.Collections.Generic;

using System;

public class Playing_State : Game_State {

    bool can_pause = false;
    public static System.Random rand = new System.Random();
    private Collision_Handler coll_handler;
    private List<Inputhandler> inputs;
    public Spawn_Util spawner;

    public Playing_State(List<Inputhandler> inputs, Game_Config conf = null) {
        this.inputs = inputs;

        this.game_conf = conf ?? new Game_Config();
        this.spawner = new Spawn_Util(game_conf);
    }

    public readonly Game_Config game_conf = new Game_Config();

    public override void on_message(string msg, dynamic data) {
        if (msg == "collision") {
            coll_handler.on_collision(data.entity1, data.entity2, data);
            return;
        }
    }

    public Tile_Map tile_map;

    private void load_map() {
        var s = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), game_conf.map_name);
        System.Console.WriteLine("loading map " + s);
        using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(s)) {
            for (int x = 0; x < 256; x++) {
                for (int y = 0; y < 256; y++) {
                    int i = x+y*256;

                    tile_map.tiles[i] = 0;

                    var c = bitmap.GetPixel(x, y);

                    if (c == System.Drawing.Color.FromArgb(0, 0, 0)) {
                        tile_map.tiles[i] = 1;
                    }
                    else if (c == System.Drawing.Color.FromArgb(0, 255, 0)) {
                        tile_map.tiles[i] = 2;
                    }
                    else if (c == System.Drawing.Color.FromArgb(255, 0, 0)) {
                        tile_map.tiles[i] = 3;
                    }
                    else if (c == System.Drawing.Color.FromArgb(255, 255, 0)) {
                        tile_map.tiles[i] = 4;
                    }
                    else if (c == System.Drawing.Color.FromArgb(127, 0, 0)) {
                        tile_map.tiles[i] = 5;
                    }
                    else if (c == System.Drawing.Color.FromArgb(127, 63, 127)) {
                        // pepita
                        tile_map.tiles[i] = 6;
                    }
                    else if (c == System.Drawing.Color.FromArgb(127, 127, 0)) {
                        // soccer net team 1 (team 2 scores here)
                        tile_map.tiles[i] = 7;
                    }
                    else if (c == System.Drawing.Color.FromArgb(127, 63, 0)) {
                        // soccer net team 2 (team 1 scores here)
                        tile_map.tiles[i] = 8;
                    }
                    else if (c == System.Drawing.Color.FromArgb(255, 0, 255)) {
                        // soccer spawn
                        tile_map.tiles[i] = 9;
                    }
                    else if (c == System.Drawing.Color.FromArgb(0, 255, 255)) {
                        // powerup spawn
                        tile_map.tiles[i] = 10;
                    }
                    else if (c == System.Drawing.Color.FromArgb(255, 127, 0)) {
                        // team 1 spawn
                        tile_map.tiles[i] = 11;
                    }
                    else if (c == System.Drawing.Color.FromArgb(0, 127, 255)) {
                        // team 2 spawn
                        tile_map.tiles[i] = 12;
                    }
                    else if (c == System.Drawing.Color.FromArgb(127, 0, 255)) {
                        // asteroid spawn
                        tile_map.tiles[i] = 13;
                    }
                }
            }
        }
    }

    public override void init() {
//        Starburst.inst().IsMouseVisible = true;        // @To-do: Load map here.

        tile_map = new Tile_Map();
        coll_handler = new Collision_Handler(this, tile_map, spawner);

        game_conf.powerup_spawn_time = 1.0f;
        game_conf.num_powerups = 50;

        load_map();

        add_subsystems(
            new Async_Multi_Subsystem(
                new Multi_Subsystem(
                    new Position_Integrator(),
                    new Collision_Solver(tile_map)
                ),
                new Async_Multi_Subsystem(
                    new Inputhandler_System(),
                    new Sound(),
                    new Particle_System(),
                    new Lifetime_Manager(),
                    new Weapon_System(this),
                    new AI()
                ),
                new Multi_Subsystem(
                    new Rendering_System(Starburst.inst().GraphicsDevice) {
                        tile_map = tile_map
                    },
                    new Window_Title_Writer()
               )
            )
        );

        create_entity(new FpsCounter());

        for(int i = 0; i < inputs.Count; i++) {
            if (inputs[i] == null)
                continue;
            var player = create_entity(Player_Ship.create_components(inputs[i], game_conf, i < 2 ? 1 : 2));
            var player_spawn = spawner.get_player_spawn_pos(player, tile_map);
            player.get_component<Position>().x = player_spawn.x;
            player.get_component<Position>().y = player_spawn.y;
            player.get_component<Angle>().angle = (float)rand.NextDouble() * 6.28f;

            /*if (i == 0) {
                player.get_component<Position>().x = -1800;
                player.get_component<Position>().y = 1500;
                player.get_component<Velocity>().x = 55.0f;
                player.get_component<Velocity>().y = 0.0f;
            }*/
        }

        create_entity(SoundManager.create_backmusic_component());
        create_entity(SoundManager.create_soundeffects_component());



        for (int i = 0; i < game_conf.num_asteroids; i++) {
            var asteroid = create_entity(Dummy.create_components());
            var r = asteroid.get_component<Bounding_Circle>().radius;

            Position ap;

            int num_fails = 0;
            bool colliding = false;
            do {
                colliding = false;
                ap = asteroid.get_component<Position>();
                var sp = spawner.get_asteroid_spawn_pos(tile_map);
                ap.x = sp.x;
                ap.y = sp.y;

                foreach (var ast in Starburst.inst().get_entities_fast(typeof (Bounding_Circle))) {
                    if (ast == asteroid) {
                        continue;
                    }

                    var dx = ast.get_component<Position>().x - asteroid.get_component<Position>().x;
                    var dy = ast.get_component<Position>().y - asteroid.get_component<Position>().y;

                    var dist = (dx*dx+dy*dy);

                    var min_dist = ast.get_component<Bounding_Circle>().radius + asteroid.get_component<Bounding_Circle>().radius;
                    min_dist *= 1.05f;
                    min_dist *= min_dist;

                    if (dist < min_dist) {
                        colliding = true;
                        num_fails++;
                        break;
                    }
                }

                if (num_fails > 1000) {
                    // failed to spawn this one.
                    asteroid.destroy();
                    break;
                }
            } while (colliding);

            var av = asteroid.get_component<Velocity>();

            av.x = -15 + 30 * (float)rand.NextDouble();
            av.y = -15 + 30 * (float)rand.NextDouble();
        }

        if (game_conf.enable_soccer) {
            var ball = create_entity(Soccer_Ball.create_components());
            var ball_pos = spawner.get_soccerball_spawn_pos(tile_map);
            ball.get_component<Position>().x = ball_pos.x;
            ball.get_component<Position>().y = ball_pos.y;
            ball.get_component<Angle>().ang_vel = 3.141592f * 2.0f * -2.0f;
        }

        //create_entity(Multifire_Powerup.create_components());
        //create_entity(Turbo_Powerup.create_components());


        /*var shield1 = create_entity(Powerup.create(new Shield_Powerup()));
        shield1.get_component<Position>().x = -1800.0f; shield1.get_component<Position>().y = 1500.0f;

        var multi1 = create_entity(Powerup.create(new Multifire_Powerup()));
        multi1.get_component<Position>().x = -1700.0f; multi1.get_component<Position>().y = 1500.0f;

        var freefire1 = create_entity(Powerup.create(new Free_Fire_Powerup()));
        freefire1.get_component<Position>().x = -1600.0f; freefire1.get_component<Position>().y = 1500.0f;

        var turbo1 = create_entity(Powerup.create(new Turbo_Powerup()));
        turbo1.get_component<Position>().x = -1500.0f; turbo1.get_component<Position>().y = 1500.0f;

        var shield2 = create_entity(Powerup.create(new Shield_Powerup()));
        shield2.get_component<Position>().x = 1800.0f; shield2.get_component<Position>().y = -1500.0f;

        var multi2 = create_entity(Powerup.create(new Multifire_Powerup()));
        multi2.get_component<Position>().x = 1700.0f; multi2.get_component<Position>().y = -1500.0f;

        var freefire2 = create_entity(Powerup.create(new Free_Fire_Powerup()));
        freefire2.get_component<Position>().x = 1600.0f; freefire2.get_component<Position>().y = -1500.0f;

        var turbo2 = create_entity(Powerup.create(new Turbo_Powerup()));
        turbo2.get_component<Position>().x = 1500.0f; turbo2.get_component<Position>().y = -1500.0f;*/

        //create_entity(Dummy_Enemy.create_components());

        Starburst.inst().message("play_sound", new { name = "begin_game" });
    }

    private Entity new_random_powerup() {
        var types = new Type[] {
            typeof (Turbo_Powerup),
            typeof (Free_Fire_Powerup),
            typeof (Shield_Powerup),
            typeof (Multifire_Powerup),
            typeof (Bouncy_Bullets_Powerup),
        };

        var i = rand.Next(0, types.Length);
        object impl = Activator.CreateInstance(types[i]);

        return create_entity(Powerup.create((Powerup_Impl)impl));
    }

    private float powerup_spawn_timer;
    public override void draw(float t, float dt) {
        base.draw(t, dt);

        var ships = Starburst.inst().get_entities_fast(typeof(Ship_Info));
        for(int i=0; i < ships.Count; i++)
        {
            Ship_Info ship = ships[i].get_component<Ship_Info>();
            if (ship.energy_value < ship.top_energy)
                ship.energy_value += ship.recharge_rate * dt;
            else if (ship.energy_value > ship.top_energy)
                ship.energy_value = ship.top_energy;

        }

        powerup_spawn_timer -= dt;
        if (powerup_spawn_timer <= 0.0f) {
            powerup_spawn_timer = game_conf.powerup_spawn_time;

            int num_powerups_now = Starburst.inst().get_entities_fast(typeof (Powerup)).Count;
            if (num_powerups_now < game_conf.num_powerups) {
                var powerup = new_random_powerup();
                var powerup_pos = spawner.get_powerup_spawn_pos(tile_map);
                powerup.get_component<Position>().x = powerup_pos.x;
                powerup.get_component<Position>().y = powerup_pos.y;
            }
        }


        if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape)) {
            Starburst.inst().Quit();
        }

        if (!can_pause) {
            bool no_buttons_pressed = true;

            for (int i = 0; i <= 3; i++) {
                if (GamePad.GetState((PlayerIndex)i).IsConnected && GamePad.GetState((PlayerIndex)i).Buttons.Start == ButtonState.Pressed) {
                    no_buttons_pressed = false;
                    break;
                }
            }

            if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.P)) {
                no_buttons_pressed = false;
            }

            if (no_buttons_pressed) {
                can_pause = true;
            }
        }
        else {
            for (int i = 0; i <= 3; i++) {
                if (GamePad.GetState((PlayerIndex)i).IsConnected && GamePad.GetState((PlayerIndex)i).Buttons.Start == ButtonState.Pressed) {
                    can_pause = false;
                    Starburst.inst().enter_state(new Pause_State());
                    return;
                }
            }

            if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.P)) {
                can_pause = false;
                Starburst.inst().enter_state(new Pause_State());
                return;
            }
        }

        if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt) &&
            Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter))
        {
            Starburst.inst().GraphicsMgr.ToggleFullScreen();
            System.Threading.Thread.Sleep(150);
        }
    }

}

}
