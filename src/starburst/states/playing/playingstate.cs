namespace Fab5.Starburst.States {

using Fab5.Engine;
using Fab5.Engine.Components;
using Fab5.Engine.Core;
using Fab5.Engine.Subsystems;

using Fab5.Starburst.States.Playing.Entities;
using Microsoft.Xna.Framework.Graphics;

using System;

public class Playing_State : Game_State {

    public static System.Random rand = new System.Random();

    public override void init() {
        add_subsystems(
            new Position_Integrator(),
            new Inputhandler_System(),
            new Sprite_Renderer(new SpriteBatch(Starburst.inst().GraphicsDevice)),
            new Text_Renderer(new SpriteBatch(Starburst.inst().GraphicsDevice)),
            new Window_Title_Writer(),
            new Collision_Solver(),
            new Sound(),
            new Particle_System(),
            new Lifetime_Manager()
        );

        create_entity(new FpsCounter());
        var player = create_entity(Player_Ship.create_components());
        create_entity(new BackgroundMusic("sound/SpaceLoungeLoop", true));

        create_entity(Dummy.create_components());

        var p2 = create_entity(Dummy.create_components()).get_component<Position>();
        p2.x = 800.0f;
        p2.y = 300.0f;

        var playerpos = player.get_component<Position>();
        var playervel = player.get_component<Velocity>();
        var playerrot = player.get_component<Angle>();

        var pemit = create_entity(new Component[] {
            new Particle_Emitter() {
                emit_fn = () => {
                    return new Component[] {
                        new Position() { x = playerpos.x - (float)Math.Cos(playerrot.angle + (float)(rand.NextDouble() - 0.5) * 0.5) * 20.0f,
                                         y = playerpos.y - (float)Math.Sin(playerrot.angle + (float)(rand.NextDouble() - 0.5) * 0.5) * 20.0f },
                        new Velocity() { x = playervel.x - (float)Math.Cos(playerrot.angle + (float)(rand.NextDouble() - 0.5) * 0.5) * 190.0f * (float)(rand.NextDouble()+0.5),
                                         y = playervel.y - (float)Math.Sin(playerrot.angle + (float)(rand.NextDouble() - 0.5) * 0.5) * 190.0f * (float)(rand.NextDouble()+0.5) },
                        new Sprite() {
                            texture = Starburst.inst().get_content<Texture2D>("particle")
                        },
                        new TTL() { time = 0.2f + (float)(rand.NextDouble() * 0.1f) }
//                        new Bounding_Circle() { radius = 1.0f },
//                        new Mass() { mass = 0.0f }

                    };
                },
                interval = 0.05f,
                num_particles_per_emit = 20
            }
        });

    }

    public override void update(float t, float dt) {
        base.update(t, dt);

        if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape)) {
                Starburst.inst().Quit();
        }
    }

}

}
