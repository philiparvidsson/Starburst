namespace Fab5.Engine {

using Fab5.Engine.Components;
using Fab5.Engine.Core;
using Fab5.Starburst;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


using System;

public class Multifire_Powerup : Powerup_Impl {
    private Int64 effect_id;

    private static System.Random rand = new System.Random();

    public override Texture2D icon {
        get { return Starburst.inst().get_content<Texture2D>("powerups/turbo"); }
    }

    public override string name {
        get { return "multifire"; }
    }

    public override void end() {
        var e = Fab5_Game.inst().get_entity(effect_id);
        if (e != null) {
            e.destroy();
        }
    }

    private void activate_effect(Entity holder) {
        var pos = holder.get_component<Position>();
        var vel = holder.get_component<Velocity>();

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
                                     color       = new Color(1.0f, 0.5f, 0.1f, 1.0f),
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

    public override void begin(Entity holder) {
        time = 60.0f;
        //activate_effect(holder);
    }

}

}
