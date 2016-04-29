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

    public static System.Random rand = new System.Random();

    private Collision_Handler coll_handler;


    public override void on_message(string msg, dynamic data) {
        if (msg == "collision") {
            coll_handler.on_collision(data.entity1, data.entity2, data);
            return;
        }
    }

    Tile_Map tile_map;

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
        coll_handler = new Collision_Handler(this, tile_map);


        load_map();

        add_subsystems(
            new Position_Integrator(),
            new Inputhandler_System(),
//            new BG_Renderer(new SpriteBatch(Starburst.inst().GraphicsDevice)),
            //new Sprite_Renderer(new SpriteBatch(Starburst.inst().GraphicsDevice)),

            new Window_Title_Writer(),
            new Collision_Solver(tile_map),
            new Sound(),
            new Particle_System(),
            new Lifetime_Manager(),
            new Weapon_System(this),
            new Rendering_System(Starburst.inst().GraphicsDevice) {
                tile_map = tile_map
            }
            //new Text_Renderer(new SpriteBatch(Starburst.inst().GraphicsDevice))
        );

        create_entity(new FpsCounter());

        var player1 = create_entity(Player_Ship.create_components());
        var player2 = create_entity(Player_Ship.create_components());
//        var player3 = create_entity(Player_Ship.create_components());
  //      var player4 = create_entity(Player_Ship.create_components());

        var player1_spawn = Spawn_Util.get_player_spawn_pos(1, tile_map);
        var player2_spawn = Spawn_Util.get_player_spawn_pos(2, tile_map);
     //   var player3_spawn = Spawn_Util.get_player_spawn_pos(1, tile_map);
    //    var player4_spawn = Spawn_Util.get_player_spawn_pos(2, tile_map);

        player1.get_component<Position>().x = player1_spawn.x;
        player1.get_component<Position>().y = player1_spawn.y;
        player1.get_component<Angle>().angle = (float)rand.NextDouble() * 6.28f;
        player2.get_component<Position>().x = player2_spawn.x;
        player2.get_component<Position>().y = player2_spawn.y;
        player2.get_component<Angle>().angle = (float)rand.NextDouble() * 6.28f;
        // player3.get_component<Position>().x = player3_spawn.x;
        // player3.get_component<Position>().y = player3_spawn.y;
        // player3.get_component<Angle>().angle = (float)rand.NextDouble() * 6.28f;
        // player4.get_component<Position>().x = player4_spawn.x;
        // player4.get_component<Position>().y = player4_spawn.y;
        // player4.get_component<Angle>().angle = (float)rand.NextDouble() * 6.28f;



            //player2.get_component<Score>().score = 100000000;

            /*var player3 = create_entity(Player_Ship.create_components());

                player3.get_component<Position>().x = 500;
                player3.get_component<Position>().y = 500;
                player3.get_component<Ship_Info>().hp_value = 50;

                var player4 = create_entity(Player_Ship.create_components());

                player4.get_component<Position>().x = 400;
                player4.get_component<Position>().y = 500;*/

            create_entity(SoundManager.create_backmusic_component());
        create_entity(SoundManager.create_soundeffects_component());


        for (int i = 0; i < 30; i++) {
            var asteroid = create_entity(Dummy.create_components());
            var ap = asteroid.get_component<Position>();
            var av = asteroid.get_component<Velocity>();
            var sp = Spawn_Util.get_asteroid_spawn_pos(tile_map);
            ap.x = sp.x;
            ap.y = sp.y;
            av.x = -15 + 30 * (float)rand.NextDouble();
            av.y = -15 + 30 * (float)rand.NextDouble();
        }

        var ball = create_entity(Soccer_Ball.create_components());
        var ball_pos = new Position() { x = -1800, y = 1800 };//Spawn_Util.get_soccerball_spawn_pos(tile_map);
        ball.get_component<Position>().x = ball_pos.x;
        ball.get_component<Position>().y = ball_pos.y;
        ball.get_component<Angle>().ang_vel = 3.141592f * 2.0f * -9.0f;
        ball.get_component<Velocity>().x = -15.0f;

        player1.get_component<Position>().x = -1800;
        player1.get_component<Position>().y = 1700;
    }

    int edit_tile = 1;
    public override void update(float t, float dt) {
        base.update(t, dt);

            var ships = Starburst.inst().get_entities_fast(typeof(Ship_Info));
            for(int i=0; i < ships.Count; i++)
            {
                Ship_Info ship = ships[i].get_component<Ship_Info>();
                if (ship.energy_value < ship.top_energy)
                    ship.energy_value += ship.recharge_rate * dt;
                else if (ship.energy_value > ship.top_energy)
                    ship.energy_value = ship.top_energy;

            }


        /*        var tw = 16;
        var th = 16;
        var mx = (player1_pos.x + Mouse.GetState().X - 640 + 2048) / tw;
        var my = (player1_pos.y + Mouse.GetState().Y - 360 + 2048) / th;

        if (mx >= 0 && mx <= 255 && my >= 0 && my <= 255) {
            int o = (int)my*256+(int)mx;

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                tile_map.tiles[o] = edit_tile;

            if (Mouse.GetState().RightButton == ButtonState.Pressed)
                tile_map.tiles[o] = 0;
        }

        if (Keyboard.GetState().IsKeyDown(Keys.D1)) {
            edit_tile = 1;
        }

        if (Keyboard.GetState().IsKeyDown(Keys.D2)) {
            edit_tile = 2;
        }

        if (Keyboard.GetState().IsKeyDown(Keys.D3)) {
            edit_tile = 3;
        }

        if (Keyboard.GetState().IsKeyDown(Keys.S)) {
            using (var f = new System.IO.StreamWriter("map.txt")) {
                for (int i = 0; i < 256; i++) {
                    for (int j = 0; j < 256; j++) {
                        var o = j + i *256;
                        f.Write(tile_map.tiles[o].ToString());
                    }

                    f.WriteLine();
                }
            }
        }

        if (Keyboard.GetState().IsKeyDown(Keys.L)) {
            using (var f = new System.IO.StreamReader("map.txt")) {
                for (int i = 0; i < 256; i++) {
                    var s = f.ReadLine();
                    for (int j = 0; j < 256; j++) {
                        tile_map.tiles[i*256+j] = int.Parse(s[j].ToString());
                    }
                }
            }
        }*/

        if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape)) {
            Starburst.inst().Quit();
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
