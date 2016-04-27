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

using System;

public class Playing_State : Game_State {

    public static System.Random rand = new System.Random();

    private Collision_Handler coll_handler;


    static float last_collision_t;
    public override void on_message(string msg, dynamic data) {
        if (msg == "collision") {
            coll_handler.on_collision(data.entity1, data.entity2, data);
            return;
        }

        /*        if (msg == "collision") {
            Entity player = null;
            Entity bullet = null;

            var is_player_hit = false;

            if (data.entity1 is Entity && data.entity2 is Entity) {
                var n1 = data.entity1.get_component<Sprite>()?.texture.Name;
                var n2 = data.entity2.get_component<Sprite>()?.texture.Name;

                if ((n1.StartsWith("ships/ship1") && n2 == "beams") || (n2.StartsWith("ships/ship1") && n1 == "beams")) {
                    is_player_hit = true;

                    player = (n1 == "beams") ? data.entity2 : data.entity1;
                    bullet = (n1 == "beams") ? data.entity1 : data.entity2;
                }
            }

            var t = Starburst.inst().get_time();
            if (t-last_collision_t < 0.1f && !is_player_hit) {
                return;
            }

            last_collision_t = t;

            var p1 = data.entity1.get_component<Position>();

            var x = data.c_x;
            var y = data.c_y;

            Func<Sprite> fn = () => new Sprite() {
                texture = Starburst.inst().get_content<Texture2D>("particle"),
                color = new Color(0.4f, 0.3f, 0.1f),
                blend_mode = Sprite.BM_ADD,
                scale = 0.4f + (float)rand.NextDouble() * 0.3f,
                layer_depth = 0.9f
            };

            if (is_player_hit) {
                fn = () => new Sprite() {
                    texture = Starburst.inst().get_content<Texture2D>("particle"),
                    color = new Color(1.0f, 0.3f, 0.1f),
                    blend_mode = Sprite.BM_ADD,
                    scale = 0.9f + (float)rand.NextDouble() * 0.9f,
                    layer_depth = 0.3f
                };

                //System.Console.WriteLine(bullet.get_component<Weapon>() + "hej");
                bullet.destroy();
                player.get_component<Ship_Info>().energy_value -= 10;
            }

            create_entity(Particle_System.explosion(x, y, fn));
        }*/
    }

    Tile_Map tile_map;

    Position player1_pos;

    public override void init() {
        Starburst.inst().IsMouseVisible = true;        // @To-do: Load map here.

        coll_handler = new Collision_Handler(this);

        tile_map = new Tile_Map();

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
            },
            new Text_Renderer(new SpriteBatch(Starburst.inst().GraphicsDevice))
        );

        create_entity(new FpsCounter());
        var player = create_entity(Player_Ship.create_components());
        var player2 = create_entity(Player_Ship.create_components());



        player1_pos = player.get_component<Position>();;

        player2.get_component<Position>().x = 400;
        player2.get_component<Position>().y = 400;
        player2.get_component<Ship_Info>().hp_value = 50;

        /*var player3 = create_entity(Player_Ship.create_components());

            player3.get_component<Position>().x = 500;
            player3.get_component<Position>().y = 500;
            player3.get_component<Ship_Info>().hp_value = 50;

            var player4 = create_entity(Player_Ship.create_components());

            player4.get_component<Position>().x = 400;
            player4.get_component<Position>().y = 500;*/

            create_entity(SoundManager.create_backmusic_component());
        create_entity(SoundManager.create_soundeffects_component());


        for (int i = 0; i < 65; i++) {
            var asteroid = create_entity(Dummy.create_components());
            var ap = asteroid.get_component<Position>();
            var av = asteroid.get_component<Velocity>();
            ap.x = -1500 + 3000 * (float)rand.NextDouble();
            ap.y = -1500 + 3000 * (float)rand.NextDouble();
            av.x = -15 + 30 * (float)rand.NextDouble();
            av.y = -15 + 30 * (float)rand.NextDouble();
        }

        var playerpos = player.get_component<Position>();
        var playervel = player.get_component<Velocity>();
        var playerrot = player.get_component<Angle>();


        var ball = create_entity(Soccer_Ball.create_components());
    }

    int edit_tile = 1;
    public override void update(float t, float dt) {
        base.update(t, dt);

        var tw = 16;
        var th = 16;
        var mx = (player1_pos.x + Mouse.GetState().X - 320 + 2048) / tw;
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
        }

        if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape)) {
            Starburst.inst().Quit();
        }

        if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt) &&
            Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter))
        {
            Starburst.inst().GraphicsMgr.ToggleFullScreen();
        }
    }

}

}
