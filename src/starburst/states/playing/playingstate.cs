namespace Fab5.Starburst.States {

using Fab5.Engine;
using Fab5.Engine.Components;
using Fab5.Engine.Core;
using Fab5.Engine.Subsystems;

using Fab5.Starburst.States.Playing.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

public class Playing_State : Game_State {

    public static System.Random rand = new System.Random();

        public override void on_message(string msg, dynamic data) { 
            if (msg == "collision") { 
                var p1 = data.entity1.get_component<Position>();

                var x = data.c_x;
                var y = data.c_y;

                Func<Sprite> fn = () => new Sprite() {
                    texture = Starburst.inst().get_content<Texture2D>("particle"),
                    color = new Color(0.4f, 0.3f, 0.1f),
                    blend_mode = Sprite.BM_ADD,
                    scale = 0.5f + (float)rand.NextDouble()
                };
                create_entity(Particle_System.explosion(x, y, fn));
            }
        }

    public override void init() {
        // @To-do: Load map here.

        var tile_map = new Tile_Map();

        add_subsystems(
            new Position_Integrator(),
            new Inputhandler_System(),
//            new BG_Renderer(new SpriteBatch(Starburst.inst().GraphicsDevice)),
            //new Sprite_Renderer(new SpriteBatch(Starburst.inst().GraphicsDevice)),
            new Rendering_System(Starburst.inst().GraphicsDevice) {
                tile_map = tile_map
            },
            new Text_Renderer(new SpriteBatch(Starburst.inst().GraphicsDevice)),
            new Window_Title_Writer(),
            new Collision_Solver(tile_map),
            new Sound(),
            new Particle_System(),
            new Lifetime_Manager(),
            new Weapon_System(this)
        );
        create_entity(Back_drop.create_components()).get_component<Backdrop>();

        create_entity(new FpsCounter());
        var player = create_entity(Player_Ship.create_components());
        var player2 = create_entity(Player_Ship.create_components());

        player2.get_component<Position>().x = 400;
        player2.get_component<Position>().y = 400;
            player2.get_component<Ship_Info>().hp_value = 50;

        create_entity(SoundManager.create_components());

        for (int i = 0; i < 75; i++) {

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

        var pemit1 = create_entity(new Component[] {
            new Particle_Emitter() {
                emit_fn = () => {
                    return new Component[] {
                        new Position() { x = playerpos.x - (float)Math.Cos(playerrot.angle + (float)(rand.NextDouble() - 0.5) * 0.5) * 20.0f,
                                         y = playerpos.y - (float)Math.Sin(playerrot.angle + (float)(rand.NextDouble() - 0.5) * 0.5) * 20.0f },
                        new Velocity() { x = playervel.x - (float)Math.Cos(playerrot.angle + (float)(rand.NextDouble() - 0.5) * 0.5) * 250.0f * (float)(rand.NextDouble()+0.5),
                                         y = playervel.y - (float)Math.Sin(playerrot.angle + (float)(rand.NextDouble() - 0.5) * 0.5) * 250.0f * (float)(rand.NextDouble()+0.5) },
                        new Sprite() {
                            texture = Starburst.inst().get_content<Texture2D>("particle"),
                            color = Color.White * 0.3f,
                            scale = 0.9f + (float)rand.NextDouble() * 1.3f,
                            blend_mode = Sprite.BM_ADD
                        },
                        new TTL() { time = 0.05f + (float)(rand.NextDouble() * 0.05f) }
//                        new Bounding_Circle() { radius = 1.0f },
//                        new Mass() { mass = 0.0f }

                    };
                },
                interval = 0.02f,
                num_particles_per_emit = 10
            }
        });

        var playerpos2 = player2.get_component<Position>();
        var playervel2 = player2.get_component<Velocity>();
        var playerrot2 = player2.get_component<Angle>();


        var pemit2 = create_entity(new Component[] {
            new Particle_Emitter() {
                emit_fn = () => {
                    return new Component[] {
                        new Position() { x = playerpos2.x - (float)Math.Cos(playerrot2.angle + (float)(rand.NextDouble() - 0.5) * 0.5) * 20.0f,
                                         y = playerpos2.y - (float)Math.Sin(playerrot2.angle + (float)(rand.NextDouble() - 0.5) * 0.5) * 20.0f },
                        new Velocity() { x = playervel2.x - (float)Math.Cos(playerrot2.angle + (float)(rand.NextDouble() - 0.5) * 0.5) * 250.0f * (float)(rand.NextDouble()+0.5),
                                         y = playervel2.y - (float)Math.Sin(playerrot2.angle + (float)(rand.NextDouble() - 0.5) * 0.5) * 250.0f * (float)(rand.NextDouble()+0.5) },
                        new Sprite() {
                            texture = Starburst.inst().get_content<Texture2D>("particle"),
                            color = Color.White * 0.3f,
                            scale = 0.9f + (float)rand.NextDouble() * 1.3f,
                            blend_mode = Sprite.BM_ADD
                        },
                        new TTL() { time = 0.05f + (float)(rand.NextDouble() * 0.05f) }
//                        new Bounding_Circle() { radius = 1.0f },
//                        new Mass() { mass = 0.0f }

                    };
                },
                interval = 0.02f,
                num_particles_per_emit = 10
            }
        });
    }

    public override void update(float t, float dt) {
        base.update(t, dt);

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
