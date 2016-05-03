namespace Fab5.Engine {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


using System;

public class Turbo_Powerup : Powerup_Impl {
    private float old_acc;
    private float old_vel;

    private Int64 holder_id;
    private Int64 effect_id;

    private static System.Random rand = new System.Random();

    public string name {
        get { return "turbo"; }
    }

    public void end() {
        Fab5_Game.inst().destroy_entity(effect_id);

        var holder = Fab5_Game.inst().get_entity(holder_id);
        if (holder != null) {
            var ship_info = holder.get_component<Ship_Info>();

            ship_info.acceleration = old_acc;
            ship_info.top_velocity = old_vel;
        }
    }

    public static Component[] create_components() {
        Position pos;
        Velocity vel;
        Angle ang;

        var powerup = new Component[] {
            new Bounding_Circle { radius = 14.0f },
      pos = new Position        { x = -1800.0f, y = 1600.0f },
            new Sprite          { blend_mode = Sprite.BM_ADD, texture = Fab5_Game.inst().get_content<Texture2D>("powerup"), color = Color.White * 0.7f },
            new Powerup         { impl = new Turbo_Powerup() },
      vel = new Velocity        { x = 0.0f, y = 0.0f },
      ang = new Angle           { ang_vel = 3.14f*0.7f, drag = 0.0f },

            new Particle_Emitter {
                emit_fn = () => {
                    var theta1 = 2.0f*3.1415f*(float)rand.NextDouble();
                    var theta2 = 2.0f*3.1415f*(float)rand.NextDouble();
                    var radius = 10.0f * (float)rand.NextDouble();
                    var speed  = 60.0f * (float)(0.5f+rand.NextDouble());
                    var i = rand.Next(0, 6);
                    var color = Color.Red;

                    if ((float)rand.NextDouble() > 0.5) {
                        theta2 += 3.141592f;
                    }

                    switch (i) {
                        case 0: color = new Color(1.0f, 0.3f, 0.3f); break;
                        case 1: color = new Color(0.3f, 1.0f, 0.3f); break;
                        case 2: color = new Color(0.3f, 0.3f, 1.0f); break;
                        case 3: color = new Color(1.0f, 1.0f, 0.3f); break;
                        case 4: color = new Color(0.3f, 1.0f, 1.0f); break;
                        case 5: color = new Color(1.0f, 0.3f, 0.3f); break;
                        case 6: color = new Color(1.0f, 0.3f, 1.0f); break;
                    };

                    return new Component [] {
                        new Mass     { drag_coeff = 1.9f },
                        new Position { x = pos.x + (float)Math.Cos(theta1) * radius,
                                       y = pos.y + (float)Math.Sin(theta1) * radius },

                        new Velocity { x = vel.x * 0.5f + (float)Math.Cos(theta2) * speed,
                                       y = vel.y * 0.5f + (float)Math.Sin(theta2) * speed },

                        new Sprite { blend_mode  = Sprite.BM_ADD,
                                     color       = color,
                                     layer_depth = 0.3f,
                                     scale       = 0.6f + (float)rand.NextDouble() * 0.9f,
                                     texture     = Fab5_Game.inst().get_content<Texture2D>("particle") },

                        new TTL { alpha_fn = (x, max) => (1.0f - ((x*x*x)/(max*max*max)))*0.4f,
                                  max_time = 0.55f + (float)Math.Pow((rand.NextDouble() * 1.2f), 2.0f) }
                    };
                },

                interval               = 0.07f,
                num_particles_per_emit = 4
            }
        };

        return powerup;
    }

    private void do_pickup_effect(Entity powerup) {
        var pos = powerup.get_component<Position>();
        var vel = powerup.get_component<Velocity>();

        Fab5_Game.inst().create_entity(new Component [] {
            new Particle_Emitter {
                emit_fn = () => {
                    var theta1 = 2.0f*3.1415f*(float)rand.NextDouble();
                    var theta2 = 2.0f*3.1415f*(float)rand.NextDouble();
                    var radius = 13.0f * (float)rand.NextDouble();
                    var speed  = 400.0f * (float)(0.05f+rand.NextDouble());

                    return new Component [] {
                        new Mass     { drag_coeff = 1.9f },
                        new Position { x = pos.x + (float)Math.Cos(theta1) * radius,
                                       y = pos.y + (float)Math.Sin(theta1) * radius },

                        new Velocity { x = vel.x * 0.5f + (float)Math.Cos(theta2) * speed,
                                       y = vel.y * 0.5f + (float)Math.Sin(theta2) * speed },

                        new Sprite { blend_mode  = Sprite.BM_ADD,
                                     color       = new Color(0.0f, 0.5f, 1.0f, 1.0f),
                                     layer_depth = 0.9f,
                                     scale       = 0.8f + (float)rand.NextDouble() * 0.7f,
                                     texture     = Fab5_Game.inst().get_content<Texture2D>("particle") },

                        new TTL { alpha_fn = (x, max) => 1.0f - (x/max)*(x/max),
                                  max_time = 0.25f + (float)Math.Pow((rand.NextDouble() * 0.9f), 2.0f) }
                    };
                },

                interval               = 0.025f,
                num_particles_per_emit = 90
            },

            new TTL { max_time = 0.1f }
        });
    }

    private void do_persistent_effect(Entity holder) {
        var pos = holder.get_component<Position>();
        var vel = holder.get_component<Velocity>();

        holder_id = holder.id;
        effect_id = Fab5_Game.inst().create_entity(new Component [] {
            new Particle_Emitter {
                emit_fn = () => {
                    var theta1 = 2.0f*3.1415f*(float)rand.NextDouble();
                    var theta2 = 2.0f*3.1415f*(float)rand.NextDouble();
                    var radius = 13.0f * (float)rand.NextDouble();
                    var speed  = 200.0f * (float)(0.05f+rand.NextDouble());

                    return new Component [] {
                        new Mass     { drag_coeff = 1.9f },
                        new Position { x = pos.x + (float)Math.Cos(theta1) * radius,
                                       y = pos.y + (float)Math.Sin(theta1) * radius },

                        new Velocity { x = vel.x * 0.5f + (float)Math.Cos(theta2) * speed,
                                       y = vel.y * 0.5f + (float)Math.Sin(theta2) * speed },

                        new Sprite { blend_mode  = Sprite.BM_ADD,
                                     color       = new Color(0.0f, 0.5f, 1.0f, 1.0f),
                                     layer_depth = 0.3f,
                                     scale       = 0.8f + (float)rand.NextDouble() * 0.7f,
                                     texture     = Fab5_Game.inst().get_content<Texture2D>("particle") },

                        new TTL { alpha_fn = (x, max) => 1.0f - (x/max)*(x/max),
                                  max_time = 0.25f + (float)Math.Pow((rand.NextDouble() * 0.9f), 2.0f) }
                    };
                },

                interval               = 0.05f,
                num_particles_per_emit = 7
            }
        }).id;
    }

    public void on_begin(Entity holder, Entity powerup) {
        var ship_info = holder.get_component<Ship_Info>();

        old_acc = ship_info.acceleration;
        old_vel = ship_info.top_velocity;

        ship_info.acceleration *= 15.0f;
        ship_info.top_velocity *= 2.0f;

        do_pickup_effect(powerup);
        do_persistent_effect(holder);
    }

}

}
