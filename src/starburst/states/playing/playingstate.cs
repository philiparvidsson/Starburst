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
        using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap("map.png")) {
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


        load_map();

        add_subsystems(
            new Position_Integrator(),
            new Inputhandler_System(),
            new Window_Title_Writer(),
            new Collision_Solver(tile_map),
            new Sound(),
            new Particle_System(),
            new Lifetime_Manager(),
            new Weapon_System(this),
            new AI(),
            new Rendering_System(Starburst.inst().GraphicsDevice) {
                tile_map = tile_map
            }
        );

        create_entity(new FpsCounter());

        for(int i = 0; i < inputs.Count; i++) {
            var player = create_entity(Player_Ship.create_components(inputs[i], game_conf));
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


        for (int i = 0; i < 30; i++) {
            var asteroid = create_entity(Dummy.create_components());
            var ap = asteroid.get_component<Position>();
            var av = asteroid.get_component<Velocity>();
            var sp = spawner.get_asteroid_spawn_pos(tile_map);
            ap.x = sp.x;
            ap.y = sp.y;
            av.x = -15 + 30 * (float)rand.NextDouble();
            av.y = -15 + 30 * (float)rand.NextDouble();
        }

        var ball = create_entity(Soccer_Ball.create_components());
        var ball_pos = spawner.get_soccerball_spawn_pos(tile_map);
        ball.get_component<Position>().x = ball_pos.x;
        ball.get_component<Position>().y = ball_pos.y;
        ball.get_component<Angle>().ang_vel = 3.141592f * 2.0f * -2.0f;

        create_entity(Turbo_Powerup.create_components());

        //create_entity(Dummy_Enemy.create_components());

        Starburst.inst().message("play_sound", new { name = "begin_game" });
    }

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
