namespace Fab5.Engine {

using Fab5.Engine.Components;
using Fab5.Engine.Core;
using Fab5.Starburst;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


using System;

public class Boost_Powerup : Powerup_Impl {

    /*private float old_acc;
    private float old_vel;*/

    private Int64 holder_id;
    //private Int64 effect_id;*/

    private static System.Random rand = new System.Random();

    public override Texture2D icon {
        get { return Starburst.inst().get_content<Texture2D>("powerups/turbo"); }
    }

    public Boost_Powerup()
    {
        time = 0.5f;
    }

    public override void end() {

        var holder = Fab5_Game.inst().get_entity(holder_id);
        //if (holder != null) {
            var v = holder.get_component<Velocity>();

            v.ax = 0.0f;
            v.ay = 0.0f;
        //}
    }

    /* private void activate_effect(Entity holder) {
        var pos = holder.get_component<Position>();
        var vel = holder.get_component<Velocity>();

        effect_id = Fab5_Game.inst().create_entity(new Component [] {
            new Particle_Emitter {
                emit_fn = () => {
                    var theta1 = 2.0f*3.1415f*(float)rand.NextDouble();
                    var theta2 = 2.0f*3.1415f*(float)rand.NextDouble();
                    var radius = 13.0f * (float)rand.NextDouble();
                    var speed  = 10.0f * (float)(0.05f+rand.NextDouble());
                    Color col = new Color(0.0f, 0.5f, 1.0f, 1.0f);

                    if ((float)rand.NextDouble() < 0.2f) {
                        col = new Color(0.7f, 0.8f, 1.0f, 1.0f);
                    }

                    return new Component [] {
                        new Mass     { drag_coeff = 1.9f },
                        new Position { x = pos.x + (float)Math.Cos(theta1) * radius,
                                       y = pos.y + (float)Math.Sin(theta1) * radius },

                        new Velocity { x = vel.x * 0.5f + (float)Math.Cos(theta2) * speed,
                                       y = vel.y * 0.5f + (float)Math.Sin(theta2) * speed },

                        new Sprite { blend_mode  = Sprite.BM_ADD,
                                     color       = col,
                                     layer_depth = 0.3f,
                                     scale       = 0.6f + (float)rand.NextDouble() * 0.4f,
                                     texture     = Fab5_Game.inst().get_content<Texture2D>("particle") },

                        new TTL { alpha_fn = (x, max) => 1.0f - (x*x)/(max*max),
                                  max_time = 0.55f + 1.5f * (float)Math.Pow((rand.NextDouble() * 0.9f), 2.0f) }
                    };
                },

                interval               = 0.02f,
                num_particles_per_emit = 1
            }
        }).id;
    }*/

    public override void begin(Entity holder) {

        //var ship_info = holder.get_component<Ship_Info>();
        holder_id = holder.id;

        //old_acc = ship_info.acceleration;
        //old_vel = ship_info.top_velocity;

        //ship_info.acceleration *= 1.3f;
        //ship_info.top_velocity *= 1.2f;

        var force = 1500.0f;

        var v = holder.get_component<Velocity>();

        var a = holder.get_component<Angle>().angle;

        v.ax += (float)Math.Cos(a)*force;
        v.ay += (float)Math.Sin(a)*force;

        //activate_effect(holder);
    }

}

}
